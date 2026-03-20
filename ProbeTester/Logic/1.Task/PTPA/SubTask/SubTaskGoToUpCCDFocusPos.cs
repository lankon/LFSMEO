using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using ProbeTester.Base;
using DeviceCore;

namespace ProbeTester.Logic
{
    #region Task
    public class SubTaskGoToUpCCDFocusPos : IBaseTask<SubTaskGoToUpCCDFocusPos.WORK>
    {
        public SubTaskGoToUpCCDFocusPos(IBaseTaskDependence dependencies,
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
        private double UpCCDFocusPosX = 166.546;
        private double UpCCDFocusPosY = 457.302;
        private double UpCCDFocusPosZ = 9.8;
        private double ChuckSafePosZ = 7.0;
        private long Delay = 0;
        private IF_BaseTask SubTask;                  //子流程
        private IF_StateControl F_StateControl;
        ProbeTesterFunction.AxisHardwareParam Axis;
        ProbeTesterFunction ProbeTesterFunc;
        public enum WORK
        {
            NONE,
            INITIAL,

            CHUCK_Z_GOTO_SAFE,
            WAIT_CHUCK_Z_GOTO_SAFE,

            GOTO_UP_DOWN_CCD_FOCUS_POS,
            WAIT_GOTO_UP_DOWN_CCD_FOCUS_POS,

            GOTO_FOCUS_Z_POS,
            WAIT_GOTO_FOCUS_Z_POS,

            WAIT_AXIS_STABE,
            CAPTURE_IMAGE,
            CALCULATE_OFFSET,



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

        private void Preset()
        {
            ProbeTesterFunc = Deps.ServiceProvider.GetRequiredService<ProbeTesterFunction>();
            Axis = ProbeTesterFunc.Axis_HardwareParam;
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
                        Preset();
                        Transition(WORK.CHUCK_Z_GOTO_SAFE);
                    }
                    break;

                case WORK.CHUCK_Z_GOTO_SAFE:
                    {
                        Deps.DML.PTP_Move(Axis.AxisZ, ChuckSafePosZ);
                        Transition(WORK.WAIT_CHUCK_Z_GOTO_SAFE);
                    }
                    break;
                case WORK.WAIT_CHUCK_Z_GOTO_SAFE:
                    {
                        if(Deps.DML.Get_Motion_Complete(Axis.AxisZ))
                        {
                            Transition(WORK.GOTO_UP_DOWN_CCD_FOCUS_POS);
                        }
                    }
                    break;

                case WORK.GOTO_UP_DOWN_CCD_FOCUS_POS:
                    {
                        Deps.DML.PTP_Move(Axis.AxisX, UpCCDFocusPosX);
                        Deps.DML.PTP_Move(Axis.AxisY, UpCCDFocusPosY);
                        Transition(WORK.WAIT_GOTO_UP_DOWN_CCD_FOCUS_POS);
                    }
                    break;
                case WORK.WAIT_GOTO_UP_DOWN_CCD_FOCUS_POS:
                    {
                        if(Deps.DML.Get_Motion_Complete(Axis.AxisX) && Deps.DML.Get_Motion_Complete(Axis.AxisY))
                        {
                            Tool.ResetTimeCount(out Delay);
                            Transition(WORK.GOTO_FOCUS_Z_POS);
                        }
                    }
                    break;

                case WORK.GOTO_FOCUS_Z_POS:
                    {
                        Deps.DML.PTP_Move(Axis.AxisZ, UpCCDFocusPosZ);
                        Transition(WORK.WAIT_GOTO_FOCUS_Z_POS);
                    }
                    break;
                case WORK.WAIT_GOTO_FOCUS_Z_POS:
                    {
                        if(Deps.DML.Get_Motion_Complete(Axis.AxisZ))
                        {
                            Deps.DIOL.SetOutputStatus(EIOName.CCD_FiducialMaskWork, true);
                            Deps.DIOL.SetOutputStatus(EIOName.CCD_FiducialMaskIdle, false);

                            Tool.ResetTimeCount(out Delay);
                            Transition(WORK.WAIT_AXIS_STABE);
                        }
                    }
                    break;

                case WORK.WAIT_AXIS_STABE:
                    {
                        if(Tool.GetTime(Delay) > 200 && 
                           Deps.DIOL.GetInputStatus(EIOName.CCD_FiducialMaskWork_Sensor) == true &&
                           Deps.DIOL.GetInputStatus(EIOName.CCD_FiducialMaskIdle_Sensor) == false)
                        {
                            Transition(WORK.SUCCESS);
                        }
                        else if(Tool.GetTime(Delay) > 5000)
                        {
                            Transition(WORK.ABORT);
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
