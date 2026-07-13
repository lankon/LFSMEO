using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using ProbeTester.Base;
using ToolFunction;

namespace ProbeTester.Logic
{
    #region Task
    public class SubTaskGotoMLOPos : IBaseTask<SubTaskGotoMLOPos.WORK>
    {
        public SubTaskGotoMLOPos(IBaseTaskDependence dependencies,
            IF_StateControl f_StateControl,
            string set_state = "Default")
            : base(dependencies)
        {
            TaskName = this.GetType().Name;
            State = WORK.INITIAL;

            switch (set_state)
            {
                default:
                    State = WORK.INITIAL;
                    break;
            }
            Tool.SaveLogToFile($"{TaskName} Start", level: "INF");

            F_StateControl = f_StateControl;
        }

        #region parameter
        // External FitTech settings are centralized here.
        private bool IsLeft = true;

        private int AxisNestX = 0;
        private int AxisNestY = 0;
        private int AxisNestZ = 0;
        private int AxisNestA = 0;
        private int AxisNestTX = 0;
        private int AxisNestTY = 0;

        private double NestSafePosX = 0.0;

        private double NestYFeaturePos = 0.0;
        private double NestYSidePAMLOOffset = 0.0;
        private double NestZFeaturePos = 0.0;
        private double NestZSidePAMLOOffset = 0.0;
        private double NestAMLOPos = 0.0;
        private double NestTxMLOPos = 0.0;
        private double NestTyMLOPos = 0.0;

        private double LeftNestYCenterFeatureOffset = 0.0;
        private double LeftNestZCenterFeatureOffset = 0.0;
        private double RightNestYCenterFeatureOffset = 0.0;
        private double RightNestZCenterFeatureOffset = 0.0;

        private IF_StateControl F_StateControl;
        ProbeTesterFunction.AxisHardwareParam Axis;
        ProbeTesterFunction ProbeTesterFunc;

        public enum WORK
        {
            NONE,
            INITIAL,

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
            if (target != State) //狀態有變化時紀錄
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
                //case WORK.RUNNING_2: target_work = WORK.RUNNING_ABORT; break;
                default: target_work = WORK.ABORT; break;
            }

            return target_work;
        }
        /// <summary>
        /// 確認Task狀態
        /// </summary>
        /// <param name="check"></param>
        private void CheckResult(TASK_STATUS check, WORK SUCCESS = WORK.SUCCESS,
                                                    WORK PAUSE = WORK.PAUSE,
                                                    WORK ABORT = WORK.ABORT,
                                                    WORK FAIL = WORK.FAIL)
        {
            switch (check)
            {
                case TASK_STATUS.SUCCESS:
                    {
                        Transition(SUCCESS);
                    }
                    break;
                case TASK_STATUS.PAUSE:
                    {
                        SetStatus(TASK_STATUS.PAUSE);   //設定目前Task狀態,讓MainTask知道Task狀態
                        SetPauseState(State);           //記錄目前暫停的case
                        SetNextState(State);            //設定使用者按繼續後要回到的case
                        Transition(PAUSE);
                    }
                    break;
                case TASK_STATUS.ABORT:
                    {
                        Transition(ABORT);
                    }
                    break;
                case TASK_STATUS.CONTINUE:
                    {

                    }
                    break;
                case TASK_STATUS.FAIL:
                    {
                        Transition(FAIL);
                    }
                    break;
            }
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
            AxisNestA = Axis.AxisA;
            AxisNestTX = Axis.AxisX;
            AxisNestTY = Axis.AxisY;
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
            //執行暫停後可根據State選擇操作者使用狀態
            //ABORT_CONTINUE
            //ABORT
            //CONTINUE
            //讓使用者選擇
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
            if (task_command == TASK_STATUS.ABORT)   //人員傳入ABORT命令
                GoToCaseAbortState(GetPauseState());
            else if (task_command == TASK_STATUS.CONTINUE)    //人員傳入CONTINUE命令
                GoToCaseConitinueState();

            switch (State)
            {
                case WORK.INITIAL:
                    {
                        Preset();
                        Transition(WORK.GOTO_SAFE_NEST_X);
                    }
                    break;

                case WORK.GOTO_SAFE_NEST_X:
                    {
                        if (Deps.DML.Get_Motion_Complete(AxisNestX))
                        {
                            Deps.DML.PTP_Move(AxisNestX, NestSafePosX, MOVE_MODE.ABS);
                            Transition(WORK.WAIT_GOTO_SAFE_NEST_X);
                        }
                    }
                    break;
                case WORK.WAIT_GOTO_SAFE_NEST_X:
                    {
                        if (Deps.DML.Get_Motion_Complete(AxisNestX))
                            Transition(WORK.GOTO_NEST_POS);
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
                                center_feature_offset_y = LeftNestYCenterFeatureOffset;
                                center_feature_offset_z = LeftNestZCenterFeatureOffset;
                            }
                            else
                            {
                                center_feature_offset_y = RightNestYCenterFeatureOffset;
                                center_feature_offset_z = RightNestZCenterFeatureOffset;
                            }

                            double nest_y = NestYFeaturePos + center_feature_offset_y + NestYSidePAMLOOffset;
                            double nest_z = NestZFeaturePos + center_feature_offset_z + NestZSidePAMLOOffset;

                            Deps.DML.PTP_Move(AxisNestY, nest_y, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(AxisNestZ, nest_z, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(AxisNestA, NestAMLOPos, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(AxisNestTX, NestTxMLOPos, MOVE_MODE.ABS);
                            Deps.DML.PTP_Move(AxisNestTY, NestTyMLOPos, MOVE_MODE.ABS);

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
                            Transition(WORK.SUCCESS);
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
