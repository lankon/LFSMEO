using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using RGBTester.Base;


namespace RGBTester.Logic
{
    public class MainTask : IBaseMainTask
    {
        public MainTask(IServiceProvider serviceProvider, IRGBTesterMachine rGBTesterMachine, IF_StateControl f_StateControl)
        {
            Machine = rGBTesterMachine;
            F_StateControl = f_StateControl;
            ServiceProvider = serviceProvider;

            Task task = Task.Run(() => Process());
        }

        #region parameter
        private bool Terminate = false;
        private IServiceProvider ServiceProvider;
        public IF_BaseTask BaseTask;
        private IRGBTesterMachine Machine;
        private IF_StateControl F_StateControl;
        private TASK_STATUS status_commad = TASK_STATUS.NONE;
        private TASK_STATUS result_status = TASK_STATUS.NONE;
        private WORK state = WORK.INITIAL;
        private WORK pre_state = WORK.INITIAL;
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

        #region private function
        /// <summary>
        /// 變換狀態
        /// </summary>
        private void Transition(WORK target)
        {
            if (target != state) //狀態有變化時紀錄
            {
                Tool.SaveLogToFile("[Process]" + target.ToString());
                F_StateControl.UpdateTask("[Process]" + target.ToString());
            }

            state = target;
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
        /// 設定狀態命令
        /// </summary>
        /// <param name="task_status"></param>
        private void SetStatusCommand(TASK_STATUS task_status)
        {
            status_commad = task_status;
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

        #region public function
        /// <summary>
        /// 設定欲執行Task物件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        public void SetTask<T>(string method = "Default") where T : IF_BaseTask
        {
            if (state != WORK.IDLE)
                return;

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

            BaseTask = (IF_BaseTask)ctor.Invoke(constructorArgs);
        }

        /// <summary>
        /// 執行Task
        /// </summary>
        public void Run()
        {
            F_StateControl.SetMainTask(this);
            F_StateControl.ShowForm(0);
            //BaseTask.SetForm(f_StateControl);

            Transition(WORK.TASK);
        }
        /// <summary>
        /// 暫停Task執行
        /// </summary>
        public void GoToPause()
        {
            //操作人員主動按下暫停
            pre_state = state;
            BaseTask.GoToPause();
            Transition(WORK.PAUSE);
        }
        /// <summary>
        /// 放棄Task執行
        /// </summary>
        public void GoToAbort()
        {
            SetStatusCommand(TASK_STATUS.ABORT);
            F_StateControl.SetPauseAbortContinue(TASK_STATUS.NONE);
            Transition(pre_state);
        }
        /// <summary>
        /// 繼續Task執行
        /// </summary>
        public void GoToContinue()
        {
            if(GetResultSatus() == TASK_STATUS.PAUSE)   //如果為流程暫停,傳入繼續指令
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
        #endregion

        private void Process()
        {
            while (!Terminate)
            {
                Thread.Sleep(1000);  //測試用

                switch (state)
                {
                    case WORK.INITIAL:
                        {
                            state = WORK.IDLE;
                        }
                        break;
                    case WORK.IDLE: //待命
                        {
                            
                        }
                        break;
                    case WORK.TASK:
                        {
                            F_StateControl.SetPauseAbortContinue(TASK_STATUS.PAUSE);
                            SetStatusCommand(TASK_STATUS.NONE);
                            Transition(WORK.WAIT_TASK);
                        }
                        break;
                    case WORK.WAIT_TASK:    //等待Task執行完畢確認結果
                        {
                            TASK_STATUS check = BaseTask.Run(GetStatusCommand());
                            CheckResult(check);
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

                Thread.Sleep(1);
            }
        }
    } 
}
