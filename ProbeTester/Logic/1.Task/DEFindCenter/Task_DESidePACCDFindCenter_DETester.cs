using Microsoft.Extensions.DependencyInjection;
using ProbeTester.Base;
using ToolFunction;

namespace ProbeTester.Logic
{
    #region Task
    public class Task_DESidePACCDFindCenter_DETester : IBaseTask<Task_DESidePACCDFindCenter_DETester.WORK>
    {
        public Task_DESidePACCDFindCenter_DETester(IBaseTaskDependence dependencies,
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
        #region FitTech migration settings
        // FitTech source: AutoProcessingParam_DETester param
        private bool ErrorStatus = false;
        #endregion

        private IF_BaseTask SubTask;
        private IF_StateControl F_StateControl;
        ProbeTesterFunction.AxisHardwareParam Axis;
        ProbeTesterFunction ProbeTesterFunc;

        public enum WORK
        {
            NONE,
            START,

            GO_DE_SIDEPA_CCD_POS,
            WAIT_GO_DE_SIDEPA_CCD_POS,

            DO_DE_SIDEPA_CCD_FINDCENTER,
            WAIT_DO_DE_SIDEPA_CCD_FINDCENTER,

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

            ErrorStatus = false;

            return true;
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
                        ErrorStatus = true;
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
                        ErrorStatus = true;
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
                        if (!PresetLoop())
                        {
                            Transition(WORK.FAIL);
                            break;
                        }

                        Transition(WORK.GO_DE_SIDEPA_CCD_POS);
                    }
                    break;

                case WORK.GO_DE_SIDEPA_CCD_POS:
                    {
                        SubTask = new SubTask_GotoDESidePACCDPos_DETester(Deps, F_StateControl);
                        SetSubTaskProcessing(true);
                        Transition(WORK.WAIT_GO_DE_SIDEPA_CCD_POS);
                    }
                    break;
                case WORK.WAIT_GO_DE_SIDEPA_CCD_POS:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());

                        CheckResult(check, SUCCESS: WORK.DO_DE_SIDEPA_CCD_FINDCENTER);
                    }
                    break;

                case WORK.DO_DE_SIDEPA_CCD_FINDCENTER:
                    {
                        SubTask = new SubTask_DESidePACCDFindCenter_DETester(Deps, F_StateControl);
                        SetSubTaskProcessing(true);
                        Transition(WORK.WAIT_DO_DE_SIDEPA_CCD_FINDCENTER);
                    }
                    break;
                case WORK.WAIT_DO_DE_SIDEPA_CCD_FINDCENTER:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());

                        CheckResult(check, SUCCESS: WORK.SUCCESS);
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
                        SetStatus(ErrorStatus ? TASK_STATUS.FAIL : TASK_STATUS.SUCCESS);
                    }
                    break;
            }
        }
    }
    #endregion
}
