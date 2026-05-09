using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using ToolFunction;
using DeviceCore;
using RGBTester.Base;
using Microsoft.Extensions.DependencyInjection;

namespace RGBTester.Logic
{
    public class SubTaskUnLoad : IBaseTask<SubTaskUnLoad.WORK>
    {
        public SubTaskUnLoad(IBaseTaskDependence dependencies, 
                          IF_StateControl f_StateControl,  
                          string set_state = "Default") : base(dependencies)
        {
            TaskName = this.GetType().Name;
            State = WORK.INITIAL;

            switch (set_state)
            {
                default:
                    State = WORK.INITIAL;
                    break;
            }
            ResetTimeCount(out task_delay);
            Tool.SaveLogToFile($"{TaskName} Start", level: "INF");

            F_StateControl = f_StateControl;
        }

        #region parameter
        private int task_delay = 0;
        private int delay_time = 1;
        private IF_BaseTask SubTask;                  //子流程
        private IF_StateControl F_StateControl;
        private IF_StatusBox StatusBox;
        //private F_StateControl TaskForm;
        public enum WORK
        {
            NONE,
            INITIAL,
            IDLE,

            CHECK_POSITION_READY,
            SPHERE_UP,
            CHUCK_DOWN,
            CHUCK_LEFT,
            CHECK_ACTION_FINISH,

            END, 

            SUCCESS,
            FAIL,
             
            PAUSE,
            ABORT,
            CONTINUE,
        }
        #endregion

        #region private function
        protected override void Transition(WORK target)
        {
            if (target != State) //狀態有變化時紀錄
            {
                Tool.SaveLogToFile($"[Task]({TaskName})" + target.ToString());
                F_StateControl.UpdateTask($"({TaskName})\n" + target.ToString());
            }

            State = target;
        }
        protected override WORK AbortState(WORK target)
        {
            WORK target_work = WORK.NONE;

            switch (target)
            {
                //case WORK.RUNNING_2: target_work = WORK.RUNNING_ABORT; break;
                default: target_work = WORK.ABORT; break;
            }

            return target_work;
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
                        SetStatus(TASK_STATUS.PAUSE);   //設定目前Task狀態,讓MainTask知道Task狀態
                        SetPauseState(State);           //記錄目前暫停的case
                        SetNextState(State);            //設定使用者按繼續後要回到的case
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
        }
        
        private void ResetTimeCount(out int tick)
        {
            tick = Environment.TickCount;
        }
        private bool CheckTimeOverSec(int tick, int time)
        {
            var time_count = Environment.TickCount - tick;
            bool res = time_count > time * 1000;

            return res;
        }
        private void Preset()
        {
            StatusBox = Deps.ServiceProvider.GetRequiredService<IF_StatusBox>();
        }
        #endregion

        #region public function
        public override TASK_STATUS Run(TASK_STATUS command = TASK_STATUS.NONE)
        {
            if (GetStatus() == TASK_STATUS.CONTINUE || command != TASK_STATUS.NONE)
            {
                RunLoop(command);
            }

            TASK_STATUS res = GetStatus();

            return res;
        }
        public override void GoToPause()
        {
            //執行暫停後可根據State選擇操作者使用狀態
            //ABORT_CONTINUE
            //ABORT
            //CONTINUE
            //讓使用者選擇
            SetPauseState(State);

            switch (State)
            {
                default:
                    {
                        F_StateControl.SetPauseAbortContinue(TASK_STATUS.ABORT_CONTINUE);
                    }
                    break;
            }

            if (GetSubTaskProcessing()) //判斷是否有SubTask執行
                SubTask.GoToPause();

        }
        #endregion

        protected override void RunLoop(TASK_STATUS task_command)
        {
            if (task_command == TASK_STATUS.ABORT)   //人員傳入ABORT命令
                GoToCaseAbortState(GetPauseState());
            else if(task_command == TASK_STATUS.CONTINUE)    //人員傳入CONTINUE命令
                GoToCaseConitinueState();
                
            switch (State)
            {
                case WORK.INITIAL:
                    {
                        Preset();
                        Transition(WORK.CHECK_POSITION_READY);
                    }
                    break;

                case WORK.CHECK_POSITION_READY:
                    {
                        if(Deps.DIOL.GetInputStatus(EIOName.SphereUpSensor) == true &&
                           Deps.DIOL.GetInputStatus(EIOName.ChuckDownSensor) == true &&
                           Deps.DIOL.GetInputStatus(EIOName.ChuckLeftSensor) == true)
                        {
                            Transition(WORK.SUCCESS);
                        }
                        else
                        {
                            Transition(WORK.SPHERE_UP);
                        }
                    }
                    break;

                case WORK.SPHERE_UP:
                    {
                        Deps.DIOL.SetOutputStatus(EIOName.SphereUp, true);
                        Deps.DIOL.SetOutputStatus(EIOName.SphereDown, false);
                        Transition(WORK.CHUCK_DOWN);
                    }
                    break;

                case WORK.CHUCK_DOWN:
                    if (Deps.DIOL.GetInputStatus(EIOName.SphereUpSensor))
                    {
                        Deps.DIOL.SetOutputStatus(EIOName.ChuckDown, true);
                        Deps.DIOL.SetOutputStatus(EIOName.ChuckUp, false);
                        ResetTimeCount(out task_delay);
                        Transition(WORK.CHUCK_LEFT);
                    }
                    else if (CheckTimeOverSec(task_delay, 5)) // Position Fail
                    {
                        StatusBox.ShowMessage("Sphere Pos Error");
                        Transition(WORK.ABORT);
                    }
                    break;

                case WORK.CHUCK_LEFT:
                    if (Deps.DIOL.GetInputStatus(EIOName.ChuckDownSensor))
                    {
                        Deps.DIOL.SetOutputStatus(EIOName.Chuck_LR, true);
                        ResetTimeCount(out task_delay);
                        Transition(WORK.CHECK_ACTION_FINISH);
                    }
                    else if (CheckTimeOverSec(task_delay, 5))
                    {
                        
                        Transition(WORK.ABORT);
                    }
                    break;

                case WORK.CHECK_ACTION_FINISH:
                    if (Deps.DIOL.GetInputStatus(EIOName.ChuckLeftSensor))
                        Transition(WORK.SUCCESS);
                    else if (CheckTimeOverSec(task_delay, 5))
                    {
                        StatusBox.ShowMessage("Chcuk Pos Error");
                        Transition(WORK.ABORT);
                    }
                        
                    break;

                case WORK.SUCCESS:
                    {
                        SetStatus(TASK_STATUS.SUCCESS);
                        Tool.SaveLogToFile($"{TaskName} End", level: "INF");
                    }
                    break;
                case WORK.FAIL:
                    {
                        if (CheckTimeOverSec(task_delay, delay_time))
                        {
                            SetStatus(TASK_STATUS.FAIL);
                        }
                    }
                    break;
                case WORK.PAUSE:
                    {
                        SetStatus(TASK_STATUS.PAUSE);
                    }
                    break;
                case WORK.ABORT:
                    {
                        SetStatus(TASK_STATUS.ABORT);
                    }
                    break;
                case WORK.CONTINUE:
                    {
                        if (GetNextState() != WORK.NONE)
                            GoToNextState();

                        SetStatus(TASK_STATUS.CONTINUE);
                        SetStatusCommand(TASK_STATUS.CONTINUE);
                    }
                    break;
                case WORK.END:
                    {
                        if (CheckTimeOverSec(task_delay, delay_time))
                        {
                            SetStatus(TASK_STATUS.SUCCESS);
                        }
                    }
                    break;
            }
        }
    }
}
