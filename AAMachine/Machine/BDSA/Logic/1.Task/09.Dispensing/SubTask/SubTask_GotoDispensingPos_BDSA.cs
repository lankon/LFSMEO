using System;

using DeviceCore;
using ToolFunction;

using AAMachine.Base;
using AAMachine.Machine;
using Microsoft.Extensions.DependencyInjection;
using AAMachine.Machine.BDSA.Base;

namespace AAMachine.Machine.BDSA.Task
{
    #region Task
    public class SubTask_GotoDispensingPos_BDSA : IBaseTask<SubTask_GotoDispensingPos_BDSA.WORK>
    {
        public SubTask_GotoDispensingPos_BDSA(
            IBaseTaskDependence dependencies,
            IF_StateControl f_StateControl,
            string set_state = "Default")
            : base(dependencies)
        {
            F_StateControl = f_StateControl;
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
        private double Tool_X_DispLeftPos = ApplicationSetting.Get_Double_Recipe<eF_BackSetting>((int)eF_BackSetting.TxtBx_CrtLeft_WG_X);
        private double Tool_X_DispRightPos = ApplicationSetting.Get_Double_Recipe<eF_BackSetting>((int)eF_BackSetting.TxtBx_CrtRight_WG_X);
        private double Tool_Z_DispMovePos = ApplicationSetting.Get_Double_Recipe<eF_BackSetting>((int)eF_BackSetting.TxtBx_DispMoveZ);
        private double Nest_Y_DispLeftPos = ApplicationSetting.Get_Double_Recipe<eF_BackSetting>((int)eF_BackSetting.TxtBx_CrtLeft_WG_Y);
        private double Nest_Y_DispRightPos = ApplicationSetting.Get_Double_Recipe<eF_BackSetting>((int)eF_BackSetting.TxtBx_CrtRight_WG_Y);
        private double ToolModel_Z_SafePos = ApplicationSetting.Get_Double_Recipe<eF_BackSetting>((int)eF_BackSetting.Txt_ToolModelZSafePos);
        private long wait_timer = 0;
        private MachineContext Machine;
        private readonly IF_StateControl F_StateControl;
        private WG_SIDE WG_Side = WG_SIDE.LEFT;

        #region FitTech migration settings

        // FitTech source: FN.GetEquipmentOffset("CCD2NeedleX")
        // TODO: bind to the LFSMEO equipment-offset service when available.
        private double CCDToNiddleOffset_X = 0.0;

        // FitTech source: FN.GetEquipmentOffset("CCD2NeedleY")
        // TODO: bind to the LFSMEO equipment-offset service when available.
        private double CCDToNiddelOffset_Y = 0.0;

        #endregion
        private enum WG_SIDE
        {
            LEFT = 0,
            RIGHT = 1,
        }

        public enum WORK
        {
            NONE,
            START,

            GOTO_TOOL_Z_SAFE_POS,
            WAIT_GOTO_TOOL_Z_SAFE_POS,

            GOTO_DISP_X_POS,
            WAIT_GOTO_DISP_X_POS,
            GOTO_DISP_Y_POS,
            WAIT_GOTO_DISP_Y_POS,

            GOTO_TOOL_Z_DISP_POS,
            WAIT_GOTO_TOOL_Z_DISP_POS,

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
            SetPosition();

            try
            {
                Machine = Deps.ServiceProvider.GetRequiredService<MachineContext>();
            }
            catch (Exception ex)
            {
                Tool.SaveLogToFile(
                    $"Failed to get MachineContext from service provider: {ex.Message}", level: "ERR");
                Transition(WORK.FAIL);
            }
        }

        private void SetPosition()
        {
            // FitTech source: FN.GetEquipmentOffset("CCD2NeedleX/Y")
            // TODO: load the two equipment offsets from the BDSA setting service.
            CCDToNiddleOffset_X = 0.0;
            CCDToNiddelOffset_Y = 0.0;

            // FitTech source: EProbePosSetting.TxtBx_CrtLeft_WG_X/Y
            // FitTech source: EProbePosSetting.TxtBx_CrtRight_WG_X/Y
            // TODO: load left/right dispensing positions from BDSA recipe settings.
            if (WG_Side == WG_SIDE.LEFT)
            {
                Tool_X_DispLeftPos -= CCDToNiddleOffset_X;
                Nest_Y_DispLeftPos -= CCDToNiddelOffset_Y;
            }
            else
            {
                Tool_X_DispRightPos -= CCDToNiddleOffset_X;
                Nest_Y_DispRightPos -= CCDToNiddelOffset_Y;
            }
        }

        private bool CheckWorkPos()
        {
            double targetPosition = WG_Side == WG_SIDE.LEFT? Tool_X_DispLeftPos : Tool_X_DispRightPos;
            return Math.Abs(Deps.DML.GetPosition(Machine.Axis.Tool_X) - targetPosition) < 0.01;
        }

        private double GetDispensingToolXPosition()
        {
            return WG_Side == WG_SIDE.LEFT
                ? Tool_X_DispLeftPos
                : Tool_X_DispRightPos;
        }

        private double GetDispensingNestYPosition()
        {
            return WG_Side == WG_SIDE.LEFT
                ? Nest_Y_DispLeftPos
                : Nest_Y_DispRightPos;
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
                default:
                    target_work = WORK.ABORT;
                    break;
            }

            return target_work;
        }

        #endregion

        #region public function
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
                #region START
                case WORK.START:
                    {
                        Preset();

                        if (CheckWorkPos())
                            Transition(WORK.GOTO_TOOL_Z_DISP_POS);
                        else
                            Transition(WORK.GOTO_TOOL_Z_SAFE_POS);
                    }
                    break;
                #endregion

                #region GOTO_TOOL_Z_SAFE_POS
                case WORK.GOTO_TOOL_Z_SAFE_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(Machine.Axis.Tool_Z))
                        {
                            Deps.DML.PTP_Move(Machine.Axis.Tool_Z, ToolModel_Z_SafePos, MOVE_MODE.ABS);

                            Tool.ResetTimeCount(out wait_timer);
                            Transition(WORK.WAIT_GOTO_TOOL_Z_SAFE_POS);
                        }
                    }
                    break;

                case WORK.WAIT_GOTO_TOOL_Z_SAFE_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(Machine.Axis.Tool_Z) && Deps.DIOL.GetInputStatus(EIOName.ToolZSafePosition))
                        {
                            Transition(WORK.GOTO_DISP_X_POS);
                        }
                        else if (Tool.GetTime(wait_timer, "s") > 10)
                        {
                            Tool.SaveLogToFile("ToolModel Z did not reach dispensing safe position.", level: "ERR");
                            Transition(WORK.END);
                            break;
                        }
                    }
                    break;
                #endregion

                #region GOTO_DISP_X_POS
                case WORK.GOTO_DISP_X_POS:
                case WORK.GOTO_DISP_Y_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(Machine.Axis.Tool_X) && Deps.DML.Get_Motion_Complete(Machine.Axis.Nest_Y))
                        {
                            double posX = GetDispensingToolXPosition();
                            double posY = GetDispensingNestYPosition();

                            Deps.DML.PTP_Move(Machine.Axis.Tool_X, posX, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(Machine.Axis.Nest_Y, posY, MOVE_MODE.ABS);

                            Transition(WORK.WAIT_GOTO_DISP_X_POS);
                        }
                    }
                    break;

                case WORK.WAIT_GOTO_DISP_X_POS:
                case WORK.WAIT_GOTO_DISP_Y_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(Machine.Axis.Tool_X) && Deps.DML.Get_Motion_Complete(Machine.Axis.Nest_Y))
                        {
                            Transition(WORK.GOTO_TOOL_Z_DISP_POS);
                        }
                    }
                    break;
                #endregion

                #region GOTO_TOOL_Z_DISP_POS
                case WORK.GOTO_TOOL_Z_DISP_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(Machine.Axis.Tool_Z))
                        {
                            Deps.DML.PTP_Move(Machine.Axis.Tool_Z, Tool_Z_DispMovePos, MOVE_MODE.ABS);
                            Transition(WORK.WAIT_GOTO_TOOL_Z_DISP_POS);
                        }
                    }
                    break;

                case WORK.WAIT_GOTO_TOOL_Z_DISP_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(Machine.Axis.Tool_Z))
                            Transition(WORK.SUCCESS);
                    }
                    break;
                #endregion

                #region SUCCESS
                case WORK.SUCCESS:
                    {
                        SetStatus(TASK_STATUS.SUCCESS);
                        Tool.SaveLogToFile($"{TaskName} End", level: "INF");
                    }
                    break;
                #endregion

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
