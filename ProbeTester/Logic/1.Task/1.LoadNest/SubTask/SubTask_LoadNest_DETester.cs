using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using ProbeTester.Base;
using ToolFunction;

namespace ProbeTester.Logic
{
    #region Task
    public class SubTask_LoadNest_DETester : IBaseTask<SubTask_LoadNest_DETester.WORK>
    {
        public SubTask_LoadNest_DETester(IBaseTaskDependence dependencies,
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
        // FitTech source: EPosSetting.LoadPos_NestX
        private double XLoadPos = 0.0;
        // FitTech source: EPosSetting.LoadPos_NestY
        private double YLoadPos = 0.0;
        // FitTech source: EPosSetting.LoadPos_NestZ
        private double ZLoadPos = 0.0;
        // FitTech source: EPosSetting.LoadPos_NestA
        private double ALoadPos = 0.0;
        // FitTech source: EPosSetting.LoadPos_NestTX
        private double TXLoadPos = 0.0;
        // FitTech source: EPosSetting.LoadPos_NestTY
        private double TYLoadPos = 0.0;
        // FitTech source: EPosSetting.TxtBx_NestXSafePos
        private double XSafePos = 0.0;

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
            PRESET,

            GOTO_NESTX_SAFE_POS,
            WAIT_GOTO_NESTX_SAFE_POS,

            GOTO_NESTY_Z_A_TX_TY_LOAD_POS,
            WAIT_NESTY_Z_A_TX_TY_LOAD_POS,

            GOTO_NESTX_LOAD_POS,
            WAIT_NESTX_LOAD_POS,

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
                        Transition(WORK.PRESET);
                    }
                    break;

                case WORK.PRESET:
                    {
                        Transition(WORK.GOTO_NESTX_SAFE_POS);
                    }
                    break;

                case WORK.GOTO_NESTX_SAFE_POS:
                    {
                        Deps.DML.PTP_Move(AxisNestX, XSafePos, MOVE_MODE.ABS);
                        Tool.ResetTimeCount(out delay);
                        Transition(WORK.WAIT_GOTO_NESTX_SAFE_POS);
                    }
                    break;
                case WORK.WAIT_GOTO_NESTX_SAFE_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(AxisNestX) && Deps.DIOL.GetInputStatus(SafePositionSensor))
                        {
                            Transition(WORK.GOTO_NESTY_Z_A_TX_TY_LOAD_POS);
                        }
                        else if (Tool.GetTime(delay, "s") > SafePositionTimeoutSec && !Deps.DIOL.GetInputStatus(SafePositionSensor))
                        {
                            Tool.SaveLogToFile($"{TaskName} X axis did not reach safe position.", level: "ERR");
                            Transition(WORK.FAIL);
                        }
                    }
                    break;

                case WORK.GOTO_NESTY_Z_A_TX_TY_LOAD_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(AxisNestY) &&
                            Deps.DML.Get_Motion_Complete(AxisNestZ) && Deps.DML.Get_Motion_Complete(AxisNestA) &&
                            Deps.DML.Get_Motion_Complete(AxisNestTX) && Deps.DML.Get_Motion_Complete(AxisNestTY))
                        {
                            Deps.DML.PTP_Move(AxisNestY, YLoadPos, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(AxisNestZ, ZLoadPos, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(AxisNestA, ALoadPos, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(AxisNestTX, TXLoadPos, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(AxisNestTY, TYLoadPos, MOVE_MODE.ABS);

                            Transition(WORK.WAIT_NESTY_Z_A_TX_TY_LOAD_POS);
                        }
                    }
                    break;
                case WORK.WAIT_NESTY_Z_A_TX_TY_LOAD_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(AxisNestY) &&
                            Deps.DML.Get_Motion_Complete(AxisNestZ) && Deps.DML.Get_Motion_Complete(AxisNestA) &&
                            Deps.DML.Get_Motion_Complete(AxisNestTX) && Deps.DML.Get_Motion_Complete(AxisNestTY))
                        {
                            Transition(WORK.GOTO_NESTX_LOAD_POS);
                        }
                    }
                    break;

                case WORK.GOTO_NESTX_LOAD_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(AxisNestX))
                        {
                            Deps.DML.PTP_Move(AxisNestX, XLoadPos, MOVE_MODE.ABS);

                            Transition(WORK.WAIT_NESTX_LOAD_POS);
                        }
                    }
                    break;
                case WORK.WAIT_NESTX_LOAD_POS:
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
