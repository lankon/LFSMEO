using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

using ToolFunction;
using BurnInTester.Base;
using BurnInTester.Logic.TestSetting;
using Microsoft.Extensions.DependencyInjection;


namespace BurnInTester.Logic
{
    public sealed class AgingSchedulerTask : IBaseMainTask
    {
        public AgingSchedulerTask(IServiceProvider serviceProvider, IBurnInTesterMachine burnInTesterMachine, 
                                  IF_StateControl f_StateControl, AgingInformation agingInformation)
        {
            _AgingInformation = agingInformation;
            ServiceProvider = serviceProvider;
            F_StateControl = f_StateControl;

            Task task = Task.Run(() => Process());
        }

        #region parameter


        private bool Terminate = false;
        private WORK state = WORK.INITIAL;
        private TASK_STATUS status_commad = TASK_STATUS.NONE;
        private TASK_STATUS result_status = TASK_STATUS.NONE;
        private WORK pre_state = WORK.INITIAL;
        private IServiceProvider ServiceProvider;
        private IF_StateControl F_StateControl;
        private IF_BaseTask[] BaseTask;
        private AgingInformation _AgingInformation;

        enum WORK
        {
            INITIAL,
            IDLE,

            TASK,
            WAIT_TASK,

            COMPLETE_STAGE,

            SUCCESS,
            FAIL,

            PAUSE,
            ABORT,
            CONTINUE,

            TASK_END,

            END,
        }
        #endregion

        #region public function
        /// <summary>
        /// 執行Task
        /// </summary>
        public void Run()
        {
            //F_StateControl.SetMainTask(this);
            //F_StateControl.ShowForm(0);
            //BaseTask.SetForm(f_StateControl);

            int boxIndex = _AgingInformation.CurSelectBoxNo; 
            bool success = false;
            if(BaseTask[boxIndex] is Task_AgingBox aging_box)
            {
                TestSettingData setting = _AgingInformation.AgingProcessSetting[boxIndex];
                success = aging_box.Start(setting);
            }

            Transition(WORK.TASK);
        }
        /// <summary>
        /// 暫停Task執行
        /// </summary>
        public void GoToPause()
        {
            //操作人員主動按下暫停
            //pre_state = state;
            //BaseTask.GoToPause();
            //Transition(WORK.PAUSE);
        }
        /// <summary>
        /// 放棄Task執行
        /// </summary>
        public void GoToAbort()
        {
            //SetStatusCommand(TASK_STATUS.ABORT);
            //F_StateControl.SetPauseAbortContinue(TASK_STATUS.NONE);
            //Transition(pre_state);

            int boxIndex = _AgingInformation.CurSelectBoxNo;

        }
        /// <summary>
        /// 繼續Task執行
        /// </summary>
        public void GoToContinue()
        {
            if (GetResultSatus() == TASK_STATUS.PAUSE)   //如果為流程暫停,傳入繼續指令
                SetStatusCommand(TASK_STATUS.CONTINUE);

            F_StateControl.SetPauseAbortContinue(TASK_STATUS.PAUSE);   //設定使用者只能按暫停鍵

            Transition(pre_state);
        }
        /// <summary>
        /// 強制MainTask至Idle,讓其他Task執行
        /// </summary>
        public void ForceAction()
        {
            Transition(WORK.IDLE);
        }
        /// <summary>
        /// 結束MainTask迴圈
        /// </summary>
        public void StopTask()
        {
            Terminate = true;
        }
        public void SetTask<T>(string method = "Default") where T : IF_BaseTask
        {
            throw new NotImplementedException();
        }
        #endregion

        #region private function
        private void Preset()
        {
            Tool.SaveLogToFile("Aging scheduler thread started", level:"INF");

            BaseTask = new Task_AgingBox[_AgingInformation.AgingProcessSetting.Length];

            for (int i = 0; i < BaseTask.Length; i++)
            {
                SetTask<Task_AgingBox>(i, method:$"{i.ToString()}");
            }
        }

        /// <summary>
        /// 設定欲執行Task物件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        private void SetTask<T>(int box_no, string method = "Default") where T : IF_BaseTask
        {
            Type[] constructorSignature = new Type[] {
                typeof(IBaseTaskDependence),
                typeof(IF_StateControl),
                typeof(string)
            };

            ConstructorInfo ctor = typeof(T).GetConstructor(constructorSignature);

            if (ctor == null)
            {
                throw new InvalidOperationException($"在 {typeof(T).Name} 上找不到符合 (IBaseTaskDependence, IF_StateControl, string) 的建構函式。");
            }

            var dep = ServiceProvider.GetRequiredService<IBaseTaskDependence>();

            object[] constructorArgs = new object[] {
                dep,
                F_StateControl,
                method
            };

            BaseTask[box_no] = (IF_BaseTask)ctor.Invoke(constructorArgs);
        }

        private void Transition(WORK target)
        {
            if (target != state) //狀態有變化時紀錄
            {
                Tool.SaveLogToFile("[Process]" + target.ToString());
                //F_StateControl.UpdateTask("[Process]" + target.ToString());
            }

            state = target;
        }

        /// <summary>
        /// 設定狀態命令
        /// </summary>
        /// <param name="task_status"></param>
        private void SetStatusCommand(TASK_STATUS task_status)
        {
            status_commad = task_status;
        }

        /// <summary>
        /// 確認Task狀態
        /// </summary>
        /// <param name="check"></param>
        private void CheckResult(TASK_STATUS check, WORK SUCCESS = WORK.SUCCESS,
                                                    WORK PAUSE = WORK.PAUSE,
                                                    WORK ABORT = WORK.ABORT,
                                                    WORK FAIL = WORK.FAIL)
        {
            switch (check)
            {
                case TASK_STATUS.SUCCESS:
                    {
                        Transition(SUCCESS);
                    }
                    break;
                case TASK_STATUS.PAUSE:
                    {
                        pre_state = state;
                        Transition(PAUSE);
                    }
                    break;
                case TASK_STATUS.ABORT:
                    {
                        Transition(ABORT);
                    }
                    break;
                case TASK_STATUS.CONTINUE:
                    {

                    }
                    break;
                case TASK_STATUS.FAIL:
                    {
                        Transition(FAIL);
                    }
                    break;
            }

            SetResultStatus(check);
        }

        /// <summary>
        /// 取得狀態命令
        /// </summary>
        /// <returns></returns>
        private TASK_STATUS GetStatusCommand()
        {
            TASK_STATUS temp_status;
            temp_status = status_commad;
            status_commad = TASK_STATUS.NONE;

            return temp_status;
        }

        private void SetResultStatus(TASK_STATUS state)
        {
            result_status = state;
        }
        
        private TASK_STATUS GetResultSatus()
        {
            return result_status;
        }
        #endregion

        private void Process()
        {
            while (!Terminate)
            {
                switch (state)
                {
                    case WORK.INITIAL:
                        {
                            Preset();
                            state = WORK.IDLE;
                        }
                        break;
                    case WORK.IDLE: //待命
                        {

                        }
                        break;
                    case WORK.TASK:
                        {
                            //F_StateControl.SetPauseAbortContinue(TASK_STATUS.PAUSE);
                            SetStatusCommand(TASK_STATUS.NONE);
                            Transition(WORK.WAIT_TASK);
                        }
                        break;
                    case WORK.WAIT_TASK:    //等待Task執行完畢確認結果
                        {
                            for (int boxIndex = 0; boxIndex < BaseTask.Length; boxIndex++)
                            {
                                TASK_STATUS check = BaseTask[boxIndex].Run(GetStatusCommand());
                            }

                            Thread.Sleep(50);
                        }
                        break;

                    case WORK.COMPLETE_STAGE:
                        {
                        }
                        break;

                    case WORK.SUCCESS:
                        {
                            Transition(WORK.TASK_END);
                        }
                        break;
                    case WORK.FAIL:
                        {
                            Transition(WORK.TASK_END);
                        }
                        break;
                    case WORK.PAUSE:
                        {

                        }
                        break;
                    case WORK.ABORT:
                        {
                            Transition(WORK.TASK_END);
                        }
                        break;
                    case WORK.TASK_END:
                        {
                            F_StateControl.HideForm();
                            Transition(WORK.IDLE);
                        }
                        break;

                    case WORK.END:
                        {

                        }
                        break;
                }
            }
        }
    }
}
