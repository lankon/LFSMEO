using AAMachine.Base;
using DeviceCore;
using ToolFunction;

namespace AAMachine.Logic
{
    #region Task
    public class SubTask_NestMoveSafe_MirrorAA : IBaseTask<SubTask_NestMoveSafe_MirrorAA.WORK>
    {
        public SubTask_NestMoveSafe_MirrorAA(IBaseTaskDependence dependencies,
            string processing = "default")
            : base(dependencies)
        {
            TaskName = this.GetType().Name;
            State = WORK.START;

            switch (processing)
            {
                default:
                    State = WORK.START;
                    break;
            }

            Tool.SaveLogToFile($"{TaskName} Start", level: "INF");
        }

        #region parameter
        #region FitTech migration settings
        // FitTech source: EFrontSetting2.LCOSSafetyZ
        private double MirrorZSafePos = 0.0;
        // FitTech source: EFrontSetting.UpPA_Safe_Pos_UpPA_Z
        private double UpPA_ZSafePos = 0.0;
        // FitTech source: EBackSetting.Txt_ToolModelZSafePos
        private double Table_ZSafePos = 0.0;
        // FitTech source: EFrontSetting.NEDSafetyX
        private double CCDXSafePos = 0.0;

        // FitTech source: EIO.CCDX_SafePosition / UpPA_SafePosition / MirrorZ_SafePosition / TableZSafetyPosition
        private EIOName CCDX_SafePosition = EIOName.CCDX_SafePosition;
        private EIOName UpPA_SafePosition = EIOName.UpPA_SafePosition;
        private EIOName MirrorZ_SafePosition = EIOName.MirrorZ_SafePosition;
        private EIOName TableZSafetyPosition = EIOName.TableZSafetyPosition;
        private int SafePositionTimeoutSec = 10;
        #endregion

        #region axis mapping
        // FitTech source: CCDX
        private int CCDX = 17;
        // FitTech source: LCOSAA_Z
        private int LCOSAA_Z = 10;
        // FitTech source: UpPA_Z
        private int UpPA_Z = 7;
        // FitTech source: TableZ
        private int TableZ = 16;
        #endregion

        private long delay = 0;

        public enum WORK
        {
            NONE,
            START,

            PRESET,

            GOTO_Z_SAFE_POS,
            WAIT_GOTO_Z_SAFE_POS,

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
                        Transition(WORK.PRESET);
                    }
                    break;
                #endregion

                #region PRESET
                case WORK.PRESET:
                    {
                        Transition(WORK.GOTO_Z_SAFE_POS);
                    }
                    break;
                #endregion

                #region GOTO_Z_SAFE_POS
                case WORK.GOTO_Z_SAFE_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(CCDX) && Deps.DML.Get_Motion_Complete(LCOSAA_Z) &&
                            Deps.DML.Get_Motion_Complete(UpPA_Z) && Deps.DML.Get_Motion_Complete(TableZ))
                        {
                            Deps.DML.PTP_Move(CCDX, CCDXSafePos, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(LCOSAA_Z, MirrorZSafePos, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(UpPA_Z, UpPA_ZSafePos, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(TableZ, Table_ZSafePos, MOVE_MODE.ABS);

                            Tool.ResetTimeCount(out delay);
                            Transition(WORK.WAIT_GOTO_Z_SAFE_POS);
                        }
                    }
                    break;
                case WORK.WAIT_GOTO_Z_SAFE_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(CCDX) && Deps.DML.Get_Motion_Complete(LCOSAA_Z) &&
                            Deps.DML.Get_Motion_Complete(UpPA_Z) && Deps.DML.Get_Motion_Complete(TableZ))
                        {
                            if (Deps.DIOL.GetInputStatus(CCDX_SafePosition) && Deps.DIOL.GetInputStatus(UpPA_SafePosition) &&
                                Deps.DIOL.GetInputStatus(MirrorZ_SafePosition) && Deps.DIOL.GetInputStatus(TableZSafetyPosition))
                            {
                                Transition(WORK.SUCCESS);
                            }
                            else if (Tool.GetTime(delay, "s") > SafePositionTimeoutSec)
                            {
                                Tool.SaveLogToFile("Safe position no reached.", level: "ERR");
                                Transition(WORK.FAIL);
                            }
                        }
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
