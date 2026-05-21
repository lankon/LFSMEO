using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ToolFunction;
using DeviceCore;
using RGBTester.Base;
using RGBTester.Base.FunctionTesterItem;

namespace RGBTester.Logic
{
    public class SubTaskMoveToOptical : IBaseTask<SubTaskMoveToOptical.WORK>
    {
        public SubTaskMoveToOptical(IBaseTaskDependence dependencies, 
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
        private int TestSide = ApplicationSetting.Get_Int_Recipe<eF_FunctionTester>((int)eF_FunctionTester.Cmbx_TestMode);
        private EIOName SphereWork_Input;
        private IF_BaseTask SubTask;                  //子流程
        private IF_StateControl F_StateControl;
        //private F_StateControl TaskForm;
        public enum WORK
        {
            NONE,
            INITIAL,
            IDLE,

            CHECK_POSITION_READY,
            CHUCK_DOWN,
            CHUCK_LEFT,
            SPHERE_LR_SIDE,
            CHUCK_UP,
            SPHERE_DOWN,
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
            if(TestSide == (int)eTestMode.LEFT)
                SphereWork_Input = EIOName.SphereLeftSensor;
            else
                SphereWork_Input = EIOName.SphereRightSensor;
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
                    {   //要分辨積分球左右
                        if(Deps.DIOL.GetInputStatus(EIOName.SphereDownSensor) == true &&
                           Deps.DIOL.GetInputStatus(SphereWork_Input) == true &&
                           Deps.DIOL.GetInputStatus(EIOName.ChuckLeftSensor) == true)
                        {
                            Transition(WORK.SUCCESS);
                        }
                        else
                        {
                            Transition(WORK.CHUCK_DOWN);
                        }
                    }
                    break;
                case WORK.CHUCK_DOWN:
                    {
                        Deps.DIOL.SetOutputStatus(EIOName.ChuckDown, true);
                        Deps.DIOL.SetOutputStatus(EIOName.ChuckUp, false);
                        Transition(WORK.CHUCK_LEFT);
                    }
                    break;
                case WORK.CHUCK_LEFT:
                    if (Deps.DIOL.GetInputStatus(EIOName.ChuckDownSensor))
                    {
                        Deps.DIOL.SetOutputStatus(EIOName.Chuck_LR, true);
                        ResetTimeCount(out task_delay);
                        Transition(WORK.SPHERE_LR_SIDE);
                    }
                    else if (CheckTimeOverSec(task_delay, 5)) // Position Fail
                    {
                        Transition(WORK.ABORT);
                    }
                    break;
                case WORK.SPHERE_LR_SIDE:
                    if (Deps.DIOL.GetInputStatus(EIOName.ChuckLeftSensor))
                    {
                        if (TestSide == (int)eTestMode.LEFT)
                            Deps.DIOL.SetOutputStatus(EIOName.Sphere_LR, true);
                        else
                            Deps.DIOL.SetOutputStatus(EIOName.Sphere_LR, false);

                        ResetTimeCount(out task_delay);
                        Transition(WORK.CHUCK_UP);
                    }
                    else if (CheckTimeOverSec(task_delay, 5))
                    {
                        Transition(WORK.ABORT);
                    }
                    break;
                case WORK.CHUCK_UP:
                    if (Deps.DIOL.GetInputStatus(SphereWork_Input))
                    {
                        Deps.DIOL.SetOutputStatus(EIOName.ChuckUp, true);
                        Deps.DIOL.SetOutputStatus(EIOName.ChuckDown, false);
                        ResetTimeCount(out task_delay);
                        Transition(WORK.SPHERE_DOWN);
                    }
                    else if (CheckTimeOverSec(task_delay, 5))
                    {
                        Transition(WORK.ABORT);
                    }
                    break;
                case WORK.SPHERE_DOWN:
                    if (Deps.DIOL.GetInputStatus(EIOName.ChuckUpSensor))
                    {
                        Deps.DIOL.SetOutputStatus(EIOName.SphereDown, true);
                        Deps.DIOL.SetOutputStatus(EIOName.SphereUp, false);
                        ResetTimeCount(out task_delay);
                        Transition(WORK.CHECK_ACTION_FINISH); 
                    }
                    else if (CheckTimeOverSec(task_delay, 5))
                    {
                        Transition(WORK.ABORT);
                    }
                    break;
                case WORK.CHECK_ACTION_FINISH:
                    {
                        if (Deps.DIOL.GetInputStatus(EIOName.SphereDownSensor) == true &&
                            Deps.DIOL.GetInputStatus(EIOName.ChuckUpSensor) == true)
                        {
                            Transition(WORK.SUCCESS);
                        }
                        else
                        {
                            Transition(WORK.ABORT);
                        }
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
