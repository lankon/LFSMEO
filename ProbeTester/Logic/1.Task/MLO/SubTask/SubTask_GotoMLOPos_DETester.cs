using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using ProbeTester.Base;
using ToolFunction;

namespace ProbeTester.Logic
{
    #region Task
    public class SubTask_GotoMLOPos_DETester : IBaseTask<SubTask_GotoMLOPos_DETester.WORK>
    {
        public SubTask_GotoMLOPos_DETester(IBaseTaskDependence dependencies,
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
        // FitTech source: EPosSetting.TxtBx_DETilt_NestY_Result
        private double NestY_FeaturePos = 0.0;
        // FitTech source: EPosSetting.TxtBx_PA_MLO_OffsetY
        private double NestY_SidePA_MLO_Offset = 0.0;
        // FitTech source: EPosSetting.TxtBx_DETilt_NestZ_Result
        private double NestZ_FeaturePos = 0.0;
        // FitTech source: EPosSetting.TxtBx_PA_MLO_OffsetZ
        private double NestZ_SidePA_MLO_Offset = 0.0;
        // FitTech source: EPosSetting.TxtBx_3DProfile_A_Result
        private double NestA_MLO_Pos = 0.0;
        // FitTech source: EPosSetting.TxtBx_DETilt_NestTX_Result
        private double NestTx_MLO_Pos = 0.0;
        // FitTech source: EPosSetting.TxtBx_3DProfile_Ty_Result
        private double NestTy_MLO_Pos = 0.0;

        // FitTech source: Side_PA_Left_DE_Center_Pos_NestY - Side_PA_Left_DE_Feature_Pos_NestY
        private double Left_NestY_CenterFeatureOffset = 0.0;
        // FitTech source: Side_PA_Left_DE_Center_Pos_NestZ - Side_PA_Left_DE_Feature_Pos_NestZ
        private double Left_NestZ_CenterFeatureOffset = 0.0;
        // FitTech source: Side_PA_Right_DE_Center_Pos_NestY - Side_PA_Right_DE_Feature_Pos_NestY
        private double Right_NestY_CenterFeatureOffset = 0.0;
        // FitTech source: Side_PA_Right_DE_Center_Pos_NestZ - Side_PA_Right_DE_Feature_Pos_NestZ
        private double Right_NestZ_CenterFeatureOffset = 0.0;

        // FitTech source: GControls.Tag[EWorkFlow_DETester.Cmbx_MakeSide].Ivalue == 0
        private bool IsLeft = true;

        // FitTech source: EPosSetting.TxtBx_NestXSafePos
        private double NestSafePos_X = 0.0;

        // FitTech source: DIOL.input(EIO.SafePosition)
        private EIOName SafePositionSensor = EIOName.SafePos_Sensor;
        private int SafePositionTimeoutSec = 5;
        #endregion

        #region axis mapping
        private int AxisNestX = 0;
        private int AxisNestY = 0;
        private int AxisNestZ = 0;
        private int AxisNestA = 0;
        private int AxisNestTX = 0;
        private int AxisNestTY = 0;
        #endregion

        private long delay = 0;
        private IF_StateControl F_StateControl;
        ProbeTesterFunction.AxisHardwareParam Axis;
        ProbeTesterFunction ProbeTesterFunc;

        public enum WORK
        {
            NONE,
            START,

            GOTO_SAFE_NEST_X,
            WAIT_GOTO_SAFE_NEST_X,

            GOTO_NEST_POS,
            WAIT_GOTO_NEST_POS,

            GOTO_NEST_X_WORK_POS,
            WAIT_GOTO_NEST_X_WORK_POS,

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

        private void Preset()
        {
            ProbeTesterFunc = Deps.ServiceProvider.GetRequiredService<ProbeTesterFunction>();
            Axis = ProbeTesterFunc.Axis_HardwareParam;

            SetDefaultAxis();
        }

        private void SetDefaultAxis()
        {
            AxisNestX = Axis.AxisX;
            AxisNestY = Axis.AxisY;
            AxisNestZ = Axis.AxisZ;
            AxisNestA = Axis.AxisRX;
            AxisNestTX = Axis.AxisRY;
            AxisNestTY = Axis.AxisRZ;
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
                        Transition(WORK.GOTO_SAFE_NEST_X);
                    }
                    break;

                case WORK.GOTO_SAFE_NEST_X:
                    {
                        if (Deps.DML.Get_Motion_Complete(AxisNestX))
                        {
                            Deps.DML.PTP_Move(AxisNestX, NestSafePos_X, MOVE_MODE.ABS);
                            Tool.ResetTimeCount(out delay);
                            Transition(WORK.WAIT_GOTO_SAFE_NEST_X);
                        }
                    }
                    break;

                case WORK.WAIT_GOTO_SAFE_NEST_X:
                    {
                        if (Deps.DML.Get_Motion_Complete(AxisNestX) && Deps.DIOL.GetInputStatus(SafePositionSensor))
                            Transition(WORK.GOTO_NEST_POS);
                        else if (Tool.GetTime(delay, "s") > SafePositionTimeoutSec && !Deps.DIOL.GetInputStatus(SafePositionSensor))
                        {
                            Tool.SaveLogToFile("X axis did not reach safe position.", level: "ERR");
                            Transition(WORK.END);
                            break;
                        }
                    }
                    break;

                case WORK.GOTO_NEST_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(AxisNestY) && Deps.DML.Get_Motion_Complete(AxisNestZ) &&
                            Deps.DML.Get_Motion_Complete(AxisNestA) && Deps.DML.Get_Motion_Complete(AxisNestTX) &&
                            Deps.DML.Get_Motion_Complete(AxisNestTY))
                        {
                            double center_feature_offset_y, center_feature_offset_z;
                            if (IsLeft)
                            {
                                center_feature_offset_y = Left_NestY_CenterFeatureOffset;
                                center_feature_offset_z = Left_NestZ_CenterFeatureOffset;
                            }
                            else
                            {
                                center_feature_offset_y = Right_NestY_CenterFeatureOffset;
                                center_feature_offset_z = Right_NestZ_CenterFeatureOffset;
                            }

                            double nest_y = NestY_FeaturePos + center_feature_offset_y + NestY_SidePA_MLO_Offset;
                            double nest_z = NestZ_FeaturePos + center_feature_offset_z + NestZ_SidePA_MLO_Offset;

                            Deps.DML.PTP_Move(AxisNestY, nest_y, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(AxisNestZ, nest_z, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(AxisNestA, NestA_MLO_Pos, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(AxisNestTX, NestTx_MLO_Pos, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(AxisNestTY, NestTy_MLO_Pos, MOVE_MODE.ABS);

                            Transition(WORK.WAIT_GOTO_NEST_POS);
                        }
                    }
                    break;
                case WORK.WAIT_GOTO_NEST_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(AxisNestY) && Deps.DML.Get_Motion_Complete(AxisNestZ) &&
                            Deps.DML.Get_Motion_Complete(AxisNestA) && Deps.DML.Get_Motion_Complete(AxisNestTX) &&
                            Deps.DML.Get_Motion_Complete(AxisNestTY))
                        {
                            Transition(WORK.GOTO_NEST_X_WORK_POS);
                        }
                    }
                    break;

                case WORK.GOTO_NEST_X_WORK_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(AxisNestX))
                        {
                            // FitTech source keeps the NestX work-position move commented out.

                            Transition(WORK.WAIT_GOTO_NEST_X_WORK_POS);
                        }
                    }
                    break;
                case WORK.WAIT_GOTO_NEST_X_WORK_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(AxisNestX))
                        {
                            Transition(WORK.SUCCESS);
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
