using AAMachine.Base;
using DeviceCore;
using System.Threading;
using ToolFunction;

namespace AAMachine.Logic
{
    #region Task
    public class Task_MotionInitialize_MirrorAA : IBaseTask<Task_MotionInitialize_MirrorAA.WORK>
    {
        public Task_MotionInitialize_MirrorAA(IBaseTaskDependence dependencies,
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
        }

        #region parameter
        #region FitTech migration settings
        // FitTech source: EIO.*
        private EIOName UVLightStretch = EIOName.UVLightStretch;
        private EIOName SideCCDLightStretch = EIOName.SideCCDLightStretch;
        private EIOName PAMaskStretch = EIOName.PAMaskStretch;
        private EIOName DispensingMaskStretch = EIOName.DispensingMaskStretch;
        private EIOName GlueCleanReStretch = EIOName.GlueCleanReStretch;
        private EIOName UVLightReStretch = EIOName.UVLightReStretch;
        private EIOName SideCCDLightReStretch = EIOName.SideCCDLightReStretch;
        private EIOName PAMaskReStretch = EIOName.PAMaskReStretch;
        private EIOName DispensingMaskReStretch = EIOName.DispensingMaskReStretch;
        private EIOName GlueCleanStretch = EIOName.GlueCleanStretch;

        private EIOName UVLightInSensor = EIOName.UVLightInSensor;
        private EIOName SideCCDLightInSensor = EIOName.SideCCDLightInSensor;
        private EIOName PAMaskInSensor = EIOName.PAMaskInSensor;
        private EIOName DispensingMaskInSensor = EIOName.DispensingMaskInSensor;
        private EIOName GZSafetyPosition = EIOName.GZSafetyPosition;
        private EIOName FZSafetyPosition = EIOName.FZSafetyPosition;
        private EIOName TableZSafetyPosition = EIOName.TableZSafetyPosition;
        private EIOName EXSafetyPosition = EIOName.EXSafetyPosition;
        #endregion

        #region axis mapping
        // FitTech source: DML.Axis.EA
        private int NestX = 0;
        // FitTech source: DML.Axis.Y
        private int NestY = 1;
        // FitTech source: DML.Axis.EZ
        private int NestZ = 2;
        // FitTech source: DML.Axis.P
        private int NestTX = 3;
        // FitTech source: DML.Axis.Q
        private int NestTY = 4;
        // FitTech source: DML.Axis.A
        private int NestA = 5;

        // FitTech source: DML.Axis.GX
        private int FrontX = 6;
        // FitTech source: DML.Axis.GZ
        private int UpPA_Z = 7;
        // FitTech source: DML.Axis.FX
        private int MirrorAA_X = 8;
        // FitTech source: DML.Axis.FY
        private int MirrorAA_Y = 9;
        // FitTech source: DML.Axis.FZ
        private int MirrorAA_Z = 10;
        // FitTech source: DML.Axis.FP
        private int MirrorAA_TX = 11;
        // FitTech source: DML.Axis.FQ
        private int MirrorAA_TY = 12;
        // FitTech source: DML.Axis.EY
        private int MirrorAA_TY2 = 13;
        // FitTech source: DML.Axis.FA
        private int MirrorAA_TZ = 14;

        // FitTech source: DML.Axis.X
        private int TableX = 15;
        // FitTech source: DML.Axis.Z
        private int TableZ = 16;

        // FitTech source: DML.Axis.EX
        private int FrontCCD = 17;
        // FitTech source: DML.Axis.S
        private int DownPA = 18;
        // FitTech source: DML.Axis.R
        private int DownCCDY = 19;
        #endregion

        public enum WORK
        {
            NONE,
            START,

            CHECK_POWER,

            CHECK_MOTIONPOWER,
            MOTIONPOWER_WAIT,

            CHECK_LIMIT,
            CHECK_SAFETY,

            GO_HOME_NEST_Mirror_TOOL_Z,
            WAIT_GO_HOME_NEST_Mirror_TOOL_Z,

            GO_HOME_Nest_X,
            WAIT_GO_HOME_Nest_X,

            GO_HOME_Mirror_Nest,
            WAIT_GO_HOME_Mirror_Nest,

            GO_HOME_NEST_Z_A_TX_TY,
            WAIT_GO_HOME_NEST_Z_A_TX_TY,

            GO_HOME_FRONTX_TABLEX,
            WAIT_GO_HOME_FRONTX_TABLEX,

            GO_HOME_NESTY_NEDY_DOWNCCDY_DOWNPA,
            WAIT_GO_HOME_NESTY_NEDY_DOWNCCDY_DOWNPA,

            INIT_DONE,
            END,

            SUCCESS,
            FAIL,

            PAUSE,
            ABORT,
            CONTINUE,
        }
        #endregion

        #region private function
        private void MultiAxisGoHome(int[] axisArray, WORK next_state)
        {
            for (int i = 0; i < axisArray.Length; i++)
            {
                Deps.DML.GoHome(axisArray[i]);
                Thread.Sleep(50);
            }

            Transition(next_state);
        }

        private void WaitMultiAxisGoHome(int[] axis, WORK next_state)
        {
            bool finish = true;
            for (int i = 0; i < axis.Length; i++)
            {
                if (!Deps.DML.Get_Home_Complete(axis[i]))
                {
                    finish = false;
                }
            }

            if (finish)
                Transition(next_state);
        }

        private bool CheckAxisLimit()
        {
            var axesToCheck = new (int axis, string name)[]
            {
                (NestX, "Nest X"),
                (NestY, "Nest Y"),
                (NestZ, "Nest Z"),
                (NestTX, "Nest TX"),
                (NestTY, "Nest TY"),
                (NestA, "Nest A"),

                (FrontX, "Front X"),
                (UpPA_Z, "UpPA Z"),
                (MirrorAA_X, "MirrorAA X"),
                (MirrorAA_Y, "MirrorAA Y"),
                (MirrorAA_Z, "MirrorAA Z"),
                (MirrorAA_TX, "MirrorAA TX"),
                (MirrorAA_TY, "MirrorAA TY"),
                (MirrorAA_TZ, "MirrorAA TZ"),
                (MirrorAA_TY2, "MirrorAA TY"),

                (TableX, "Tool Model X"),
                (TableZ, "Tool Model Z"),

                (FrontCCD, "Front X"),

                (DownPA, "Down PA"),

                (DownCCDY, "Down CCD Y"),
            };

            string ss = "";
            bool anyAtLimit = false;

            foreach (var (axis, name) in axesToCheck)
            {
                if (false)
                {
                    ss += " " + name + " ";

                    Deps.DML.SetServo(axis, false);
                    anyAtLimit = true;
                }
            }

            if (anyAtLimit)
            {
                Tool.SaveLogToFile($"{ss}Axis on limit, remove to continue.", level: "ERR");
                return false;
            }

            return true;
        }

        private void SetIOStatus()
        {
            Deps.DIOL.SetOutputStatus(UVLightStretch, false);
            Deps.DIOL.SetOutputStatus(SideCCDLightStretch, false);
            Deps.DIOL.SetOutputStatus(PAMaskStretch, false);
            Deps.DIOL.SetOutputStatus(DispensingMaskStretch, false);
            Deps.DIOL.SetOutputStatus(GlueCleanReStretch, false);

            Thread.Sleep(50);

            Deps.DIOL.SetOutputStatus(UVLightReStretch, true);
            Deps.DIOL.SetOutputStatus(SideCCDLightReStretch, true);
            Deps.DIOL.SetOutputStatus(PAMaskReStretch, true);
            Deps.DIOL.SetOutputStatus(DispensingMaskReStretch, true);
            Deps.DIOL.SetOutputStatus(GlueCleanStretch, true);
        }

        protected override void Transition(WORK target)
        {
            if (target != State)
            {
                Tool.SaveLogToFile($"[Task]({TaskName})" + target.ToString());
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
                #region START
                case WORK.START:
                    {
                        SetIOStatus();

                        if (!Deps.DIOL.GetInputStatus(UVLightInSensor) ||
                           !Deps.DIOL.GetInputStatus(SideCCDLightInSensor) ||
                           !Deps.DIOL.GetInputStatus(PAMaskInSensor) ||
                           !Deps.DIOL.GetInputStatus(DispensingMaskInSensor))
                        {
                            Tool.SaveLogToFile("Cylinder is not retracted.", level: "ERR");
                            Transition(WORK.END);
                            break;
                        }

                        Transition(WORK.CHECK_POWER);
                    }
                    break;
                #endregion

                #region CHECK
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
                        bool res = CheckAxisLimit();

                        if (res == true)
                            Transition(WORK.CHECK_SAFETY);
                    }
                    break;

                case WORK.CHECK_SAFETY:
                    {
                        bool IsSafe = true;

                        if (!IsSafe)
                        {
                            Transition(WORK.FAIL);

                            break;
                        }

                        Transition(WORK.GO_HOME_NEST_Mirror_TOOL_Z);
                    }
                    break;
                #endregion

                #region GO_HOME_NEST_Mirror_TOOL_Z
                case WORK.GO_HOME_NEST_Mirror_TOOL_Z:
                    {
                        int[] axis = { MirrorAA_Z, UpPA_Z, TableZ, FrontCCD, DownPA, DownCCDY };

                        MultiAxisGoHome(axis, WORK.WAIT_GO_HOME_NEST_Mirror_TOOL_Z);
                    }
                    break;
                case WORK.WAIT_GO_HOME_NEST_Mirror_TOOL_Z:
                    {
                        int[] axis = { MirrorAA_Z, UpPA_Z, TableZ, FrontCCD, DownPA, DownCCDY };

                        WaitMultiAxisGoHome(axis, WORK.GO_HOME_Nest_X);
                    }
                    break;
                #endregion

                #region GO_HOME_Nest_X
                case WORK.GO_HOME_Nest_X:
                    {
                        if (!Deps.DIOL.GetInputStatus(GZSafetyPosition) ||
                           !Deps.DIOL.GetInputStatus(FZSafetyPosition) ||
                           !Deps.DIOL.GetInputStatus(TableZSafetyPosition))
                        {
                            Tool.SaveLogToFile("MirrorAA_Z, Up PA or ToolModel Safety Position Error", level: "ERR");
                            Transition(WORK.END);
                            break;
                        }

                        int[] axis = { NestX };

                        MultiAxisGoHome(axis, WORK.WAIT_GO_HOME_Nest_X);
                    }
                    break;
                case WORK.WAIT_GO_HOME_Nest_X:
                    {
                        int[] axis = { NestX };

                        WaitMultiAxisGoHome(axis, WORK.GO_HOME_Mirror_Nest);
                    }
                    break;
                #endregion

                #region GO_HOME_Mirror_Nest
                case WORK.GO_HOME_Mirror_Nest:
                    {
                        if (!Deps.DIOL.GetInputStatus(EXSafetyPosition))
                        {
                            Tool.SaveLogToFile("FrontCCD X Safety Position Error", level: "ERR");
                            Transition(WORK.END);
                            break;
                        }

                        int[] axis = { FrontX, TableX, MirrorAA_TY2,
                                       MirrorAA_X, MirrorAA_Y, MirrorAA_TX, MirrorAA_TY, MirrorAA_TZ,
                                       NestY, NestZ, NestA, NestTX, NestTY};

                        MultiAxisGoHome(axis, WORK.WAIT_GO_HOME_Mirror_Nest);
                    }
                    break;
                case WORK.WAIT_GO_HOME_Mirror_Nest:
                    {
                        int[] axis = { FrontX, TableX, MirrorAA_TY2,
                                       MirrorAA_X, MirrorAA_Y, MirrorAA_TX, MirrorAA_TY, MirrorAA_TZ,
                                       NestY, NestZ, NestA, NestTX, NestTY };

                        WaitMultiAxisGoHome(axis, WORK.INIT_DONE);
                    }
                    break;
                #endregion

                #region INIT_DONE
                case WORK.INIT_DONE:
                    {
                        Tool.SaveLogToFile("[TASK] Motion Initialized.", level: "INF");

                        Transition(WORK.SUCCESS);
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
