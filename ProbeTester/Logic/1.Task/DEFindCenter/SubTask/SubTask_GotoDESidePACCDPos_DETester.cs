using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using ProbeTester.Base;
using System;
using System.Threading;
using ToolFunction;

namespace ProbeTester.Logic
{
    #region Task
    public class SubTask_GotoDESidePACCDPos_DETester : IBaseTask<SubTask_GotoDESidePACCDPos_DETester.WORK>
    {
        public SubTask_GotoDESidePACCDPos_DETester(IBaseTaskDependence dependencies,
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
        // FitTech source: EPosSetting_Recipe.Side_PA_Left_DE_Feature_Pos_NestX
        private double NestXPos_Left = 0.0;
        // FitTech source: EPosSetting_Recipe.Side_PA_Left_DE_Feature_Pos_NestY
        private double NestYPos_Left = 0.0;
        // FitTech source: EPosSetting_Recipe.Side_PA_Left_DE_Feature_Pos_NestZ
        private double NestZPos_Left = 0.0;
        // FitTech source: EPosSetting_Recipe.Side_PA_Left_DE_Feature_Pos_NestTX
        private double NestTXPos_Left = 0.0;

        // FitTech source: EPosSetting_Recipe.Side_PA_Right_DE_Feature_Pos_NestX
        private double NestXPos_Right = 0.0;
        // FitTech source: EPosSetting_Recipe.Side_PA_Right_DE_Feature_Pos_NestY
        private double NestYPos_Right = 0.0;
        // FitTech source: EPosSetting_Recipe.Side_PA_Right_DE_Feature_Pos_NestZ
        private double NestZPos_Right = 0.0;
        // FitTech source: EPosSetting_Recipe.Side_PA_Right_DE_Feature_Pos_NestTX
        private double NestTXPos_Right = 0.0;

        // FitTech source: EPosSetting.TxtBx_NestXSafePos
        private double NestXSafePos = 0.0;

        // FitTech source: GControls.Tag[EWorkFlow_DETester.Cmbx_MakeSide].Ivalue
        private int MakeSide = 0;

        // FitTech source: EIO.SideCCDLightStretch / EIO.SideCCDLightReStretch / EIO.SideCCDLightInSensor
        private EIOName SideCCDLightStretch = EIOName.SideCCDLightStretch;
        private EIOName SideCCDLightReStretch = EIOName.SideCCDLightReStretch;
        private EIOName SideCCDLightInSensor = EIOName.SideCCDLightInSensor;
        private int SideCCDLightTimeoutSec = 5;
        #endregion

        #region axis mapping
        private int NestX = 0;
        private int NestY = 0;
        private int NestZ = 0;
        private int NestTX = 0;
        #endregion

        private long wait_timer = 0;
        private IF_StateControl F_StateControl;
        ProbeTesterFunction.AxisHardwareParam Axis;
        ProbeTesterFunction ProbeTesterFunc;

        public enum WORK
        {
            NONE,
            START,

            GOTO_SAFE_POS,
            WAIT_GOTO_SAFE_POS,

            GOTO_SidePACCD_NEST_POS,
            WAIT_GOTO_SidePACCD_NEST_POS,

            GOTO_SidePACCD_NESTX_POS,
            WAIT_GOTO_SidePACCD_NESTX_POS,

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
        }

        private void SetDefaultAxis()
        {
            NestX = Axis.AxisX;
            NestY = Axis.AxisY;
            NestZ = Axis.AxisZ;
            NestTX = Axis.AxisRY;
        }

        private bool CheckWorkPos()
        {
            if (Math.Abs(Deps.DML.GetPosition(NestY) - NestYPos_Left) < 10)
                return true;
            else
                return false;
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

                        if (CheckWorkPos() == true)
                            Transition(WORK.GOTO_SidePACCD_NEST_POS);
                        else
                            Transition(WORK.GOTO_SAFE_POS);
                    }
                    break;

                case WORK.GOTO_SAFE_POS:
                    {
                        Deps.DIOL.SetOutputStatus(SideCCDLightStretch, false);
                        Thread.Sleep(50);
                        Deps.DIOL.SetOutputStatus(SideCCDLightReStretch, true);

                        Deps.DML.PTP_Move(NestX, NestXSafePos, MOVE_MODE.ABS);

                        Tool.ResetTimeCount(out wait_timer);

                        Transition(WORK.WAIT_GOTO_SAFE_POS);
                    }
                    break;
                case WORK.WAIT_GOTO_SAFE_POS:
                    {
                        if (Deps.DIOL.GetInputStatus(SideCCDLightInSensor) && Deps.DML.Get_Motion_Complete(NestX))
                        {
                            Transition(WORK.GOTO_SidePACCD_NEST_POS);
                        }
                        else if (Tool.GetTime(wait_timer, "s") > SideCCDLightTimeoutSec)
                        {
                            Tool.SaveLogToFile("Cylinder is not retracted.", level: "ERR");
                            Transition(WORK.END);
                            break;
                        }
                    }
                    break;

                case WORK.GOTO_SidePACCD_NEST_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(NestY) &&
                            Deps.DML.Get_Motion_Complete(NestZ) &&
                            Deps.DML.Get_Motion_Complete(NestTX))
                        {
                            if (MakeSide == 0)
                            {
                                Deps.DML.PTP_Move(NestY, NestYPos_Left, MOVE_MODE.ABS);
                                Deps.DML.PTP_Move(NestZ, NestZPos_Left, MOVE_MODE.ABS);
                                Deps.DML.PTP_Move(NestTX, NestTXPos_Left, MOVE_MODE.ABS);
                            }
                            else if (MakeSide == 1)
                            {
                                Deps.DML.PTP_Move(NestY, NestYPos_Right, MOVE_MODE.ABS);
                                Deps.DML.PTP_Move(NestZ, NestZPos_Right, MOVE_MODE.ABS);
                                Deps.DML.PTP_Move(NestTX, NestTXPos_Right, MOVE_MODE.ABS);
                            }

                            Transition(WORK.WAIT_GOTO_SidePACCD_NEST_POS);
                        }
                    }
                    break;
                case WORK.WAIT_GOTO_SidePACCD_NEST_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(NestY) &&
                            Deps.DML.Get_Motion_Complete(NestZ) &&
                            Deps.DML.Get_Motion_Complete(NestTX))
                        {
                            Transition(WORK.GOTO_SidePACCD_NESTX_POS);
                        }
                    }
                    break;

                case WORK.GOTO_SidePACCD_NESTX_POS:
                    {
                        if (MakeSide == 0)
                            Deps.DML.PTP_Move(NestX, NestXPos_Left, MOVE_MODE.ABS);
                        else
                            Deps.DML.PTP_Move(NestX, NestXPos_Right, MOVE_MODE.ABS);

                        Transition(WORK.WAIT_GOTO_SidePACCD_NESTX_POS);
                    }
                    break;
                case WORK.WAIT_GOTO_SidePACCD_NESTX_POS:
                    {
                        if (Deps.DML.Get_Motion_Complete(NestX))
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
