using Microsoft.Extensions.DependencyInjection;
using ProbeTester.Base;
using ToolFunction;

namespace ProbeTester.Logic
{
    #region Task
    public class Task_MLO : IBaseTask<Task_MLO.WORK>
    {
        public Task_MLO(IBaseTaskDependence dependencies,
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
        // FitTech SingleTestType is centralized here as a local MLO test selector.
        private MLO_TEST_TYPE TestType = MLO_TEST_TYPE.GotoMLOPos;

        private IF_BaseTask SubTask;                  //子流程
        private IF_StateControl F_StateControl;
        ProbeTesterFunction.AxisHardwareParam Axis;
        ProbeTesterFunction ProbeTesterFunc;

        private enum MLO_TEST_TYPE
        {
            None,
            GotoMLOPos,
            Goto3DTiltPos,
            GotoDESidePACCDPos,
            SidePAFindFeature_Left,
            SidePAFindFeature_Right,
            DE3DTiltProcessing,
            DESidePACCDFindCenterProcessing,
        }

        public enum WORK
        {
            NONE,
            INITIAL,

            DO_SUBTASK,
            WAIT_SUBTASK,

            DO_PROCESSINGTASK,
            WAIT_PROCESSINGTASK,

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
                        SetSubTaskProcessing(false);
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
                        SetSubTaskProcessing(false);
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
                        Transition(FAIL);
                    }
                    break;
            }
        }

        private void Preset()
        {
            ProbeTesterFunc = Deps.ServiceProvider.GetRequiredService<ProbeTesterFunction>();
            Axis = ProbeTesterFunc.Axis_HardwareParam;
        }

        private bool IsProcessingTaskAction()
        {
            bool res = false;

            switch (TestType)
            {
                case MLO_TEST_TYPE.DESidePACCDFindCenterProcessing:
                case MLO_TEST_TYPE.DE3DTiltProcessing:
                    res = true;
                    break;
            }

            return res;
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

            if (GetSubTaskProcessing()) //判斷是否有SubTask執行
                SubTask.GoToPause();
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

                        if (IsProcessingTaskAction())
                            Transition(WORK.DO_PROCESSINGTASK);
                        else
                            Transition(WORK.DO_SUBTASK);
                    }
                    break;

                case WORK.DO_SUBTASK:
                    {
                        SubTask = null;
                        switch (TestType)
                        {
                            case MLO_TEST_TYPE.GotoMLOPos:
                                SubTask = new SubTaskGotoMLOPos(Deps, F_StateControl);
                                break;

                            default:
                                Tool.SaveLogToFile($"{TaskName} unsupported MLO test type: {TestType}", level: "INF");
                                Transition(WORK.SUCCESS);
                                break;
                        }

                        if (SubTask != null)
                        {
                            SetSubTaskProcessing(true);
                            Transition(WORK.WAIT_SUBTASK);
                        }
                    }
                    break;
                case WORK.WAIT_SUBTASK:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());
                        CheckResult(check);
                    }
                    break;

                case WORK.DO_PROCESSINGTASK:
                    {
                        Tool.SaveLogToFile($"{TaskName} processing task is not migrated in 10.MLO scope: {TestType}", level: "INF");
                        Transition(WORK.SUCCESS);
                    }
                    break;
                case WORK.WAIT_PROCESSINGTASK:
                    {
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
