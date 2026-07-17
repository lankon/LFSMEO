using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using ProbeTester.Base;
using System.Threading;
using ToolFunction;

namespace ProbeTester.Logic
{
    #region Task
    public class SubTask_DESidePACCDFindCenter_DETester : IBaseTask<SubTask_DESidePACCDFindCenter_DETester.WORK>
    {
        public SubTask_DESidePACCDFindCenter_DETester(IBaseTaskDependence dependencies,
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
        // FitTech source: string PatternPath
        private string PatternPath = string.Empty;
        // FitTech source: bool FindCenterSuccess
        private bool FindCenterSuccess = false;
        // FitTech source: GControls.Tag[EWorkFlow_DETester.Cmbx_MakeSide].Ivalue
        private int MakeSide = 0;

        private int correctionRetryCount = 0;
        private const int MaxCorrectionRetry = 3;
        private const double PositionTolerance = 0.05;
        private const double AngleTolerance = 0.05;

        // FitTech source: EPosSetting.Side_PA_Left_DE_Feature_Pos_Y1/Z1/Y2/Z2/Y3/Z3
        private double Left_TargetY1 = 0.0;
        private double Left_TargetZ1 = 0.0;
        private double Left_TargetY2 = 0.0;
        private double Left_TargetZ2 = 0.0;
        private double Left_TargetY3 = 0.0;
        private double Left_TargetZ3 = 0.0;

        // FitTech source: EPosSetting.Side_PA_Right_DE_Feature_Pos_Y1/Z1/Y2/Z2/Y3/Z3
        private double Right_TargetY1 = 0.0;
        private double Right_TargetZ1 = 0.0;
        private double Right_TargetY2 = 0.0;
        private double Right_TargetZ2 = 0.0;
        private double Right_TargetY3 = 0.0;
        private double Right_TargetZ3 = 0.0;

        // FitTech source: EIO.SideCCDLightReStretch
        private EIOName SideCCDLightReStretch = EIOName.SideCCDLightReStretch;
        #endregion

        #region axis mapping
        private int NestY = 0;
        private int NestZ = 0;
        private int NestTX = 0;
        #endregion

        private IF_StateControl F_StateControl;
        ProbeTesterFunction.AxisHardwareParam Axis;
        ProbeTesterFunction ProbeTesterFunc;

        public enum WORK
        {
            NONE,
            START,

            DO_CCD_RINGLIGHT_STRETCH,
            WAIT_DO_CCD_RINGLIGHT_STRETCH,

            DO_DE_SIDEPA_CCD_ANGLE_CORRECTION,
            WAIT_DO_DE_SIDEPA_CCD_ANGLE_CORRECTION,

            CHECK_CORRECTION_RESULT,
            WAIT_CORRECTION_MOVE,

            END,

            SUCCESS,
            FAIL,

            PAUSE,
            ABORT,
            CONTINUE,
        }
        #endregion

        #region private function
        private void Preset()
        {
            ProbeTesterFunc = Deps.ServiceProvider.GetRequiredService<ProbeTesterFunction>();
            Axis = ProbeTesterFunc.Axis_HardwareParam;

            SetDefaultAxis();
            FindCenterSuccess = false;
            correctionRetryCount = 0;
        }

        private void SetDefaultAxis()
        {
            NestY = Axis.AxisY;
            NestZ = Axis.AxisZ;
            NestTX = Axis.AxisRY;
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
                        Preset();
                        Transition(WORK.DO_CCD_RINGLIGHT_STRETCH);
                    }
                    break;

                case WORK.DO_CCD_RINGLIGHT_STRETCH:
                    {
                        Deps.DIOL.SetOutputStatus(SideCCDLightReStretch, false);
                        Thread.Sleep(200);

                        Transition(WORK.WAIT_DO_CCD_RINGLIGHT_STRETCH);
                    }
                    break;
                case WORK.WAIT_DO_CCD_RINGLIGHT_STRETCH:
                    {
                        Transition(WORK.DO_DE_SIDEPA_CCD_ANGLE_CORRECTION);
                    }
                    break;

                case WORK.DO_DE_SIDEPA_CCD_ANGLE_CORRECTION:
                    {
                        Tool.SaveLogToFile("DE side PA CCD find center needs LFSMEO vision/MIL API mapping before it can run.", level: "ERR");
                        Transition(WORK.FAIL);
                    }
                    break;
                case WORK.WAIT_DO_DE_SIDEPA_CCD_ANGLE_CORRECTION:
                    {
                        Transition(WORK.CHECK_CORRECTION_RESULT);
                    }
                    break;

                case WORK.CHECK_CORRECTION_RESULT:
                    {
                        if (FindCenterSuccess)
                            Transition(WORK.SUCCESS);
                        else if (correctionRetryCount < MaxCorrectionRetry)
                            Transition(WORK.DO_DE_SIDEPA_CCD_ANGLE_CORRECTION);
                        else
                            Transition(WORK.FAIL);
                    }
                    break;
                case WORK.WAIT_CORRECTION_MOVE:
                    {
                        if (Deps.DML.Get_Motion_Complete(NestY) &&
                            Deps.DML.Get_Motion_Complete(NestZ) &&
                            Deps.DML.Get_Motion_Complete(NestTX))
                        {
                            Transition(WORK.CHECK_CORRECTION_RESULT);
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
