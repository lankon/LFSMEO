using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using ProbeTester.Base;
using ToolFunction;

namespace ProbeTester.Logic
{
    #region Task
    public class Task_UnLoadNest : IBaseTask<Task_UnLoadNest.WORK>
    {
        public Task_UnLoadNest(IBaseTaskDependence dependencies,
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
        private IF_BaseTask SubTask;
        private IF_StateControl F_StateControl;
        ProbeTesterFunction.AxisHardwareParam Axis;
        ProbeTesterFunction ProbeTesterFunc;

        public enum WORK
        {
            NONE,
            START,
            PRESET,

            UNLOAD_NEST,
            WAIT_UNLOAD_NEST,

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
            ProbeTesterFunc = Deps.ServiceProvider.GetRequiredService<ProbeTesterFunction>();
            Axis = ProbeTesterFunc.Axis_HardwareParam;

            DoReStretchIO();

            return true;
        }

        private void DoReStretchIO()
        {
            Deps.DIOL.SetOutputStatus(EIOName.SideCCDLightStretch, false);
            Deps.DIOL.SetOutputStatus(EIOName.SideCCDLightReStretch, true);
        }

        protected override void Transition(WORK target)
        {
            if (target != State)
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
                default: target_work = WORK.ABORT; break;
            }

            return target_work;
        }

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
                        SetStatus(TASK_STATUS.PAUSE);
                        SetPauseState(State);
                        SetNextState(State);
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
            SetPauseState(State);

            switch (State)
            {
                default:
                    {
                        F_StateControl.SetPauseAbortContinue(TASK_STATUS.ABORT_CONTINUE);
                    }
                    break;
            }

            if (GetSubTaskProcessing())
                SubTask.GoToPause();
        }
        #endregion

        protected override void RunLoop(TASK_STATUS task_command)
        {
            if (task_command == TASK_STATUS.ABORT)
                GoToCaseAbortState(GetPauseState());
            else if (task_command == TASK_STATUS.CONTINUE)
                GoToCaseConitinueState();

            switch (State)
            {
                case WORK.START:
                    {
                        Transition(WORK.PRESET);
                    }
                    break;

                case WORK.PRESET:
                    {
                        PresetLoop();
                        Tool.ResetTimeCount(out wait_timer);
                        Transition(WORK.UNLOAD_NEST);
                    }
                    break;

                #region UNLOAD_NEST
                case WORK.UNLOAD_NEST:
                    {
                        if (Deps.DIOL.GetInputStatus(EIOName.SideCCDLightInSensor))
                        {
                            SubTask = new SubTask_UnLoadNest_DETester(Deps, F_StateControl);
                            SetSubTaskProcessing(true);
                            Transition(WORK.WAIT_UNLOAD_NEST);
                        }
                        else if (!Deps.DIOL.GetInputStatus(EIOName.SideCCDLightInSensor) && Tool.GetTime(wait_timer, "s") > 5)
                        {
                            Tool.SaveLogToFile("Cylinder is not retracted.", level: "ERR");
                            Transition(WORK.END);
                            break;
                        }
                    }
                    break;
                case WORK.WAIT_UNLOAD_NEST:
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
