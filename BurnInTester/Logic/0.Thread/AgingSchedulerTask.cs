using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using BurnInTester.Base;
using BurnInTester.Logic.TestSetting;

namespace BurnInTester.Logic
{
    public sealed class AgingSchedulerTask : IBaseMainTask
    {
        // 老化流程主要排程器
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
        private bool InitialComplete = false;
        private bool[] AbortAfterPause;
        private WORK state = WORK.INITIAL;
        private TASK_STATUS[] status_commad;
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
            if(InitialComplete == false)
            {
                Tool.SaveLogToFile("Aging scheduler task not initialized yet", level:"WRN");
                return;
            }
            
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

            int boxIndex = _AgingInformation.CurSelectBoxNo;
            BaseTask[boxIndex].GoToPause();
        }
        /// <summary>
        /// 放棄Task執行
        /// </summary>
        public void GoToAbort()
        {
            //SetStatusCommand(TASK_STATUS.ABORT);
            //F_StateControl.SetPauseAbortContinue(TASK_STATUS.NONE);
            //Transition(pre_state);

            //int boxIndex = _AgingInformation.CurSelectBoxNo;
            //SetStatusCommand(boxIndex, TASK_STATUS.ABORT);

            // 老化機流程沒有人員按暫停,所以特殊處理
            int boxIndex = _AgingInformation.CurSelectBoxNo;
            AbortAfterPause[boxIndex] = true;
            BaseTask[boxIndex].GoToPause();
        }
        /// <summary>
        /// 繼續Task執行
        /// </summary>
        public void GoToContinue()
        {
            //if (GetResultSatus() == TASK_STATUS.PAUSE)   //如果為流程暫停,傳入繼續指令
            //    SetStatusCommand(TASK_STATUS.CONTINUE);

            //F_StateControl.SetPauseAbortContinue(TASK_STATUS.PAUSE);   //設定使用者只能按暫停鍵

            //Transition(pre_state);
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
            // 此執行緒無法從外部呼叫Task, 因為此執行緒是針對多個溫控箱的流程排程器, 需要依照溫控箱編號來設定Task
            throw new NotImplementedException();
        }
        #endregion

        #region private function
        private void Preset()
        {
            Tool.SaveLogToFile("Aging scheduler thread started", level:"INF");

            BaseTask = new Task_AgingBox[_AgingInformation.AgingProcessSetting.Length];
            status_commad = new TASK_STATUS[_AgingInformation.AgingProcessSetting.Length];
            AbortAfterPause = new bool[_AgingInformation.AgingProcessSetting.Length];

            for (int i = 0; i < BaseTask.Length; i++)
            {
                SetTask<Task_AgingBox>(i, method:$"{i.ToString()}");
                status_commad[i] = TASK_STATUS.NONE;
                AbortAfterPause[i] = false;
            }

            InitialComplete = true;
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

            // 對應複數溫控箱流程
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
        private void SetStatusCommand(int box_num, TASK_STATUS task_status)
        {
            status_commad[box_num] = task_status;
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
        private TASK_STATUS GetStatusCommand(int box_num)
        {
            TASK_STATUS temp_status;
            temp_status = status_commad[box_num];
            status_commad[box_num] = TASK_STATUS.NONE;

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
                            Transition(WORK.IDLE);
                        }
                        break;
                    case WORK.IDLE: //待命
                        {
                            Thread.Sleep(100); // 沒使用時Delay,避免CPU過度使用
                        }
                        break;
                    case WORK.TASK:
                        {
                            //F_StateControl.SetPauseAbortContinue(TASK_STATUS.PAUSE);
                            int boxIndex = _AgingInformation.CurSelectBoxNo;
                            SetStatusCommand(boxIndex, TASK_STATUS.NONE);
                            Transition(WORK.WAIT_TASK);
                        }
                        break;
                    case WORK.WAIT_TASK:    //等待Task執行完畢確認結果
                        {
                            for (int boxIndex = 0; boxIndex < BaseTask.Length; boxIndex++)
                            {
                                TASK_STATUS check = BaseTask[boxIndex].Run(GetStatusCommand(boxIndex));

                                if (check == TASK_STATUS.PAUSE && AbortAfterPause[boxIndex])
                                {
                                    AbortAfterPause[boxIndex] = false;
                                    SetStatusCommand(boxIndex, TASK_STATUS.ABORT);
                                }
                            }

                            Thread.Sleep(50);
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
