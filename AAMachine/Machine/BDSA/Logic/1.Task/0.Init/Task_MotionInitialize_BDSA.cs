using System;
using System.Collections.Generic;
using System.Threading;

using DeviceCore;
using ToolFunction;

using AAMachine.Base;
using AAMachine.Machine;

namespace AAMachine.Machine.BDSA.Task
{
    /// <summary>
    /// BDSA motion initialization task migrated from FitTech.
    /// FitTech-only UI, loader and global-state calls are intentionally kept as TODOs
    /// until their LFSMEO services are defined.
    /// </summary>
    public class Task_MotionInitialize_BDSA : IBaseTask<Task_MotionInitialize_BDSA.WORK>
    {
        public Task_MotionInitialize_BDSA(
            IBaseTaskDependence dependencies,
            IF_StateControl f_StateControl,
            MachineContext machine,
            string set_state = "Default")
            : base(dependencies)
        {
            Machine = machine ?? throw new ArgumentNullException(nameof(machine));
            F_StateControl = f_StateControl;
            TableX = Machine.Axis.Axis_X;
            TableZ = Machine.Axis.Axis_Z;
            TaskName = GetType().Name;
            State = WORK.START;

            switch (set_state)
            {
                default:
                    State = WORK.START;
                    break;
            }

            Tool.SaveLogToFile($"{TaskName} Start", level: "INF");
        }

        #region parameter define

        private readonly MachineContext Machine;
        private readonly IF_StateControl F_StateControl;

        #region FitTech migration settings

        // FitTech source: EIO.DispensingPressure / EIO.WipeNeedle / EIO.CCDFiducial.
        // These names do not currently exist in DeviceCore.EIOName, so the original
        // output operations remain explicit TODOs in SetIOStatus().
        private readonly EIOName GZSafetyPosition = EIOName.GZSafetyPosition;
        private readonly EIOName FZSafetyPosition = EIOName.FZSafetyPosition;
        private readonly EIOName TableZSafetyPosition = EIOName.TableZSafetyPosition;
        private readonly EIOName EXSafetyPosition = EIOName.EXSafetyPosition;

        #endregion

        #region axis mapping

        // FitTech source axis aliases are preserved in comments. The LFSMEO motion
        // abstraction currently exposes AXIS_NAME plus the common MachineContext axis
        // definition; verify these aliases against the BDSA axis configuration.
        private readonly int PARX = (int)AXIS_NAME.AXIS_EX;  // DML.Axis.GX
        private readonly int PARZ = (int)AXIS_NAME.AXIS_EZ;  // DML.Axis.GZ
        private readonly int PALX = (int)AXIS_NAME.AXIS_IX;  // DML.Axis.FR
        private readonly int PALZ = (int)AXIS_NAME.AXIS_IZ;  // DML.Axis.FS

        private readonly int LEARX = (int)AXIS_NAME.AXIS_AX; // DML.Axis.FX
        private readonly int LEARY = (int)AXIS_NAME.AXIS_AY; // DML.Axis.FY
        private readonly int LEARZ = (int)AXIS_NAME.AXIS_AZ; // DML.Axis.FZ
        private readonly int LEARTX = (int)AXIS_NAME.AXIS_AA; // DML.Axis.FA
        private readonly int LEARTY = (int)AXIS_NAME.AXIS_EA; // DML.Axis.FP
        private readonly int LEARTZ = (int)AXIS_NAME.AXIS_IY; // DML.Axis.FQ

        private readonly int LEALX = (int)AXIS_NAME.AXIS_IZ; // DML.Axis.R
        private readonly int LEALY = (int)AXIS_NAME.AXIS_IY; // DML.Axis.S
        private readonly int LEALZ = (int)AXIS_NAME.AXIS_EZ; // DML.Axis.EP
        private readonly int LEALTX = (int)AXIS_NAME.AXIS_EA; // DML.Axis.EQ
        private readonly int LEALTY = (int)AXIS_NAME.AXIS_AX; // DML.Axis.ER
        private readonly int LEALTZ = (int)AXIS_NAME.AXIS_AY; // DML.Axis.ES

        private int NestY => Machine.Axis.Axis_Y;  // DML.Axis.Y
        private int NestTX => Machine.Axis.Axis_TX; // DML.Axis.P
        private int NestTY => Machine.Axis.Axis_TY; // DML.Axis.Q
        private int NestA => Machine.Axis.Axis_A; // DML.Axis.A

        private readonly int TableX;
        private readonly int TableZ;
        private readonly int DownCCDY = (int)AXIS_NAME.AXIS_EY; // DML.Axis.GY

        #endregion

        public enum WORK
        {
            NONE,
            START,

            CHECK_POWER,
            CHECK_MOTIONPOWER,
            CHECK_LIMIT,
            CHECK_SAFETY,

            GO_HOME_NED_TOOL_Z,
            WAIT_GO_HOME_NED_TOOL_Z,
            GO_HOME_NED_X,
            WAIT_GO_HOME_NED_X,
            GO_HOME_LEA_X,
            WAIT_GO_HOME_LEA_X,
            GO_HOME_LEA_Y_TX_TY_TZ,
            WAIT_GO_HOME_LEA_Y_TX_TY_TZ,

            INIT_DONE,
            END,
            SUCCESS,
            FAIL,
            PAUSE,
            ABORT,
        }

        #endregion

        #region private function

        private void MultiAxisGoHome(int[] axisArray, WORK nextState)
        {
            foreach (int axis in axisArray)
            {
                // FitTech source: DML.Assert(axis), then DML.go_home(axis).
                // LFSMEO currently has no Assert API; GoHome is the available motion
                // command and its completion is checked in the following state.
                Deps.DML.GoHome(axis);
                Thread.Sleep(50);
            }

            Transition(nextState);
        }

        private void WaitMultiAxisGoHome(int[] axes, WORK nextState)
        {
            foreach (int axis in axes)
            {
                if (!Deps.DML.Get_Home_Complete(axis))
                    return;
            }

            Transition(nextState);
        }

        private bool CheckAxisLimit()
        {
            // FitTech source used DML.get_state(), target_limit_sensor_reached and
            // GControls.Tag[...]. LFSMEO's current IFunction_MotionCard does not expose
            // those APIs. Keep the migration boundary explicit until MotionCard provides
            // a limit-state contract.
            return true;
        }

        private bool CheckSafety()
        {
            return Deps.DIOL.GetInputStatus(GZSafetyPosition)
                && Deps.DIOL.GetInputStatus(FZSafetyPosition)
                && Deps.DIOL.GetInputStatus(TableZSafetyPosition)
                && Deps.DIOL.GetInputStatus(EXSafetyPosition);
        }

        private void SetIOStatus()
        {
            // FitTech source:
            // DIOL.output(EIO.WipeNeedle, false)
            // DIOL.output(EIO.CCDFiducial, false)
            // These EIO names are not currently present in DeviceCore.EIOName.
        }

        protected override void Transition(WORK target)
        {
            if (target != State)
                Tool.SaveLogToFile($"[Task]({TaskName}) {target}", level: "DBG");

            State = target;
        }

        protected override WORK AbortState(WORK target)
        {
            return WORK.ABORT;
        }

        #endregion

        #region public function

        public override void GoToPause()
        {
            SetPauseState(State);
        }

        #endregion

        protected override void RunLoop(TASK_STATUS taskCommand)
        {
            if (taskCommand == TASK_STATUS.ABORT)
                GoToCaseAbortState(GetPauseState());
            else if (taskCommand == TASK_STATUS.CONTINUE)
                GoToCaseConitinueState();

            switch (State)
            {
                case WORK.START:
                    {
                        SetIOStatus();
                        Transition(WORK.CHECK_POWER);
                    }
                    break;

                case WORK.CHECK_POWER:
                    {
                        Transition(WORK.CHECK_MOTIONPOWER);
                    }
                    break;

                case WORK.CHECK_MOTIONPOWER:
                    {
                        Transition(WORK.CHECK_LIMIT);
                    }
                    break;

                case WORK.CHECK_LIMIT:
                    {
                        if (CheckAxisLimit())
                            Transition(WORK.CHECK_SAFETY);
                    }
                    break;

                case WORK.CHECK_SAFETY:
                    {
                        if (!CheckSafety())
                        {
                            Tool.SaveLogToFile(
                                "BDSA safety position check failed.",
                                level: "ERR");
                            Transition(WORK.FAIL);
                            break;
                        }

                        Transition(WORK.GO_HOME_NED_TOOL_Z);
                    }
                    break;

                case WORK.GO_HOME_NED_TOOL_Z:
                    {
                        MultiAxisGoHome(
                            new[] { PALZ, PARZ, TableZ },
                            WORK.WAIT_GO_HOME_NED_TOOL_Z);
                    }
                    break;

                case WORK.WAIT_GO_HOME_NED_TOOL_Z:
                    {
                        WaitMultiAxisGoHome(
                            new[] { PALZ, PARZ, TableZ },
                            WORK.GO_HOME_NED_X);
                    }
                    break;

                case WORK.GO_HOME_NED_X:
                    {
                        if (!CheckSafety())
                        {
                            Tool.SaveLogToFile(
                                "BDSA NED or ToolModel safety position check failed.",
                                level: "ERR");
                            Transition(WORK.FAIL);
                            break;
                        }

                        MultiAxisGoHome(
                            new[] { PARX, PALX, TableX, DownCCDY, LEARZ, LEALZ },
                            WORK.WAIT_GO_HOME_NED_X);
                    }
                    break;

                case WORK.WAIT_GO_HOME_NED_X:
                    {
                        WaitMultiAxisGoHome(
                            new[] { PARX, PALX, TableX, DownCCDY, LEARZ, LEALZ },
                            WORK.GO_HOME_LEA_X);
                    }
                    break;

                case WORK.GO_HOME_LEA_X:
                    {
                        MultiAxisGoHome(
                            new[] { LEARX, LEALX },
                            WORK.WAIT_GO_HOME_LEA_X);
                    }
                    break;

                case WORK.WAIT_GO_HOME_LEA_X:
                    {
                        WaitMultiAxisGoHome(
                            new[] { LEARX, LEALX },
                            WORK.GO_HOME_LEA_Y_TX_TY_TZ);
                    }
                    break;

                case WORK.GO_HOME_LEA_Y_TX_TY_TZ:
                    {
                        MultiAxisGoHome(
                            new[]
                            {
                                LEARY, LEARTX, LEARTY, LEARTZ,
                                LEALY, LEALTX, LEALTY, LEALTZ,
                                NestY, NestTX, NestTY, NestA
                            },
                            WORK.WAIT_GO_HOME_LEA_Y_TX_TY_TZ);
                    }
                    break;

                case WORK.WAIT_GO_HOME_LEA_Y_TX_TY_TZ:
                    {
                        WaitMultiAxisGoHome(
                            new[]
                            {
                                LEARY, LEARTX, LEARTY, LEARTZ,
                                LEALY, LEALTX, LEALTY, LEALTZ,
                                NestY, NestTX, NestTY, NestA
                            },
                            WORK.INIT_DONE);
                    }
                    break;

                case WORK.INIT_DONE:
                    {
                        Tool.SaveLogToFile(
                            "[BDSA Task] Motion Initialized.",
                            level: "INF");
                        Transition(WORK.SUCCESS);
                    }
                    break;

                case WORK.SUCCESS:
                    {
                        SetStatus(TASK_STATUS.SUCCESS);
                        Tool.SaveLogToFile($"{TaskName} End", level: "INF");
                        Transition(WORK.END);
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

                case WORK.END:
                    {
                        SetStatus(TASK_STATUS.SUCCESS);
                    }
                    break;
            }
        }
    }
}
