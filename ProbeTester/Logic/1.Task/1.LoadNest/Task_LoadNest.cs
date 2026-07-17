using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using ProbeTester.Base;
using ToolFunction;

namespace ProbeTester.Logic
{
    #region Task
    public class Task_LoadNest : IBaseTask<Task_LoadNest.WORK>
    {
        public Task_LoadNest(IBaseTaskDependence dependencies,
            IF_StateControl f_StateControl,
            string set_state = "Default")
            : base(dependencies)
        {
            TaskName = this.GetType().Name;
            State = WORK.START;

            switch (set_state)
            {
                default:
                    State = WORK.START;
                    break;
            }
            Tool.SaveLogToFile($"{TaskName} Start", level: "INF");

            F_StateControl = f_StateControl;
        }

        #region parameter
        private long wait_timer = 0;
        private IF_BaseTask SubTask;                  //子流程
        private IF_StateControl F_StateControl;
        ProbeTesterFunction.AxisHardwareParam Axis;
        ProbeTesterFunction ProbeTesterFunc;


        public enum WORK
        {
            NONE,
            START,
            PRESET,

            LOAD_NEST,
            WAIT_LOAD_NEST,

            END,

            SUCCESS,
            FAIL,

            PAUSE,
            ABORT,
            CONTINUE,
        }
        #endregion

        #region private function
        bool PresetLoop()
        {
            return true;
        }
        private void DoReStretchIO()
        {
            Deps.DIOL.SetOutputStatus(EIOName.SideCCDLightStretch, false);
            Deps.DIOL.SetOutputStatus(EIOName.SideCCDLightReStretch, true);
        }
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
                        SetSubTaskProcessing(false);
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
                        SetSubTaskProcessing(false);
                        Transition(ABORT);
                    }
                    break;
                case TASK_STATUS.CONTINUE:
                    {

                    }
                    break;
                case TASK_STATUS.FAIL:
                    {
                        SetSubTaskProcessing(false);
                        Transition(FAIL);
                    }
                    break;
            }
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
            else if (task_command == TASK_STATUS.CONTINUE)    //人員傳入CONTINUE命令
                GoToCaseConitinueState();

            switch (State)
            {
                case WORK.START:
                    {
                        Transition(WORK.PRESET);
                        DoReStretchIO();
                        Tool.ResetTimeCount(out wait_timer);
                    }
                    break;

                case WORK.PRESET:
                    {
                        if (Deps.DIOL.GetInputStatus(EIOName.SideCCDLightInSensor))
                        {
                            PresetLoop();

                            Transition(WORK.LOAD_NEST);
                        }
                        else if (!Deps.DIOL.GetInputStatus(EIOName.SideCCDLightInSensor) && Tool.GetTime(wait_timer, "s") > 5)
                        {
                            Tool.SaveLogToFile("汽缸尚未縮回", level: "ERR");
                            Transition(WORK.END);
                            break;
                        }
                    }
                    break;

                #region LOAD_NEST
                case WORK.LOAD_NEST:
                    {
                        if (Deps.DIOL.GetInputStatus(EIOName.SideCCDLightInSensor))
                        {
                            SubTask = new SubTask_LoadNest_DETester(Deps, F_StateControl);
                            SetSubTaskProcessing(true);
                            Transition(WORK.WAIT_LOAD_NEST);
                        }
                        else if (!Deps.DIOL.GetInputStatus(EIOName.SideCCDLightInSensor) && Tool.GetTime(wait_timer, "s") > 5)
                        {
                            Tool.SaveLogToFile("汽缸尚未縮回", level: "ERR");
                            Transition(WORK.END);
                            break;
                        }
                    }
                    break;
                case WORK.WAIT_LOAD_NEST:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());
                        CheckResult(check, SUCCESS: WORK.SUCCESS);
                    }
                    break;
                #endregion

                case WORK.SUCCESS:
                    {
                        SetStatus(TASK_STATUS.SUCCESS);
                        Tool.SaveLogToFile($"{TaskName} End", level: "INF");
                    }
                    break;
                case WORK.FAIL:
                    {
                        SetStatus(TASK_STATUS.FAIL);
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
                        SetStatus(TASK_STATUS.SUCCESS);
                    }
                    break;
            }
        }
    }
    #endregion
}
