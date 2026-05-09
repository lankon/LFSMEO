using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

using ToolFunction;
using RGBTester.Base;

namespace RGBTester.Logic
{
    #region Task
    public class TaskOpticalTest : IBaseTask<TaskOpticalTest.WORK>
    {
        public TaskOpticalTest(IBaseTaskDependence dependencies,
            IF_StateControl f_StateControl,
            string set_state = "Default")
            : base(dependencies)
        {
            TaskName = this.GetType().Name;
            State = WORK.INITIAL;

            if (set_state == "Left")
                OnlyLeftTest = true;
            else if (set_state == "Right")
                OnlyRightTest = true;

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
        private IF_BaseTask SubTask;                  //子流程
        private IF_StateControl F_StateControl;
        private bool OnlyLeftTest = false;
        private bool OnlyRightTest = false;
        public enum WORK
        {
            NONE,
            INITIAL,
            IDLE,

            GOTO_OPTICAL_TEST_POSITION,
            WAIT_GOTO_OPTICAL_TEST_POSITION,

            LEFT_GLASSES_TEST,
            WAIT_LEFT_GLASSES_TEST,

            RIGHT_GLASSES_TEST,
            WAIT_RIGHT_GLASSES_TEST,

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
        
        private void ResetTimeCount(out int tick)
        {
            tick = Environment.TickCount;
        }
        private bool CheckTimeOverSec(int tick, int time)
        {
            var time_count = Environment.TickCount - tick;
            bool res = time_count > time * 1000;

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
            else if(task_command == TASK_STATUS.CONTINUE)    //人員傳入CONTINUE命令
                GoToCaseConitinueState();
                
            switch (State)
            {
                case WORK.INITIAL:
                    {
                        Transition(WORK.GOTO_OPTICAL_TEST_POSITION);
                    }
                    break;

                #region GoTo Test Position
                case WORK.GOTO_OPTICAL_TEST_POSITION:
                    {
                        //建立SubTask
                        SubTask = new SubTaskMoveToOptical(Deps, F_StateControl);
                        //設定是否有SubTask執行
                        SetSubTaskProcessing(true);

                        Transition(WORK.WAIT_GOTO_OPTICAL_TEST_POSITION);
                    }
                    break;
                case WORK.WAIT_GOTO_OPTICAL_TEST_POSITION:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());

                        if (OnlyRightTest)
                            CheckResult(check, SUCCESS: WORK.RIGHT_GLASSES_TEST);
                        else
                            CheckResult(check, SUCCESS: WORK.LEFT_GLASSES_TEST);
                    }
                    break;
                #endregion

                #region Left
                case WORK.LEFT_GLASSES_TEST:
                    {
                        Tool.SaveLogToFile("LEFT_GLASSES_TEST", level: "INF");
                        SubTask = new SubTaskOpticalTestRGB(Deps, F_StateControl, "Left");
                        SetSubTaskProcessing(true);

                        Transition(WORK.WAIT_LEFT_GLASSES_TEST);
                    }
                    break;
                case WORK.WAIT_LEFT_GLASSES_TEST:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());

                        if (OnlyLeftTest)
                            CheckResult(check, SUCCESS: WORK.SUCCESS);
                        else
                            CheckResult(check, SUCCESS: WORK.RIGHT_GLASSES_TEST);
                    }
                    break;
                #endregion
                #region Right
                case WORK.RIGHT_GLASSES_TEST:
                    {
                        Tool.SaveLogToFile("RIGHT_GLASSES_TEST", level: "INF");
                        SubTask = new SubTaskOpticalTestRGB(Deps, F_StateControl, "Right");
                        SetSubTaskProcessing(true);
                        Transition(WORK.WAIT_RIGHT_GLASSES_TEST);
                    }
                    break;
                case WORK.WAIT_RIGHT_GLASSES_TEST:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());
                        CheckResult(check, SUCCESS: WORK.SUCCESS);
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
                        //SaveHistoryCurrentState(WORK.ABORT);
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
