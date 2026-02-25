using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using RGBTester.Device;
using RGBTester.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolFunction;

namespace RGBTester.Logic
{
    public class SubTaskRGBTest: IBaseTask<SubTaskRGBTest.WORK>
    {
        public SubTaskRGBTest(IBaseTaskDependence dependencies,
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

            Type = set_state;
        }

        #region parameter
        RGBTesterFunction RGBfunc;
        private IF_BaseTask SubTask;
        private IF_StateControl F_StateControl;
        private IF_StatusBox StatusBox;
        private string Type;
        private string SN;
        public enum WORK
        {
            NONE,
            INITIAL,
            IDLE,

            LED_R_TEST,
            LED_G_TEST,
            LED_B_TEST,

            WAIT_LED_R_TEST,
            WAIT_LED_G_TEST,
            WAIT_LED_B_TEST,

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
            Scope.TestFail = false;
            StatusBox = Deps.ServiceProvider.GetRequiredService<IF_StatusBox>();
            RGBfunc = Deps.ServiceProvider.GetRequiredService<RGBTesterFunction>();

            if(Type == "Left")
            {
                SN = ApplicationSetting.Get_String_Recipe<eF_StartForm>((int)eF_StartForm.TxtBx_Left_SN);
            }
            else if(Type == "Right")
            {
                SN = ApplicationSetting.Get_String_Recipe<eF_StartForm>((int)eF_StartForm.TxtBx_Right_SN);
            }
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
                        Transition(WORK.LED_R_TEST);
                    }
                    break;

                #region RED
                case WORK.LED_R_TEST:
                    {
                        Tool.SaveLogToFile("LED_R_Test", level: "INF");
                        SubTask = new SubTaskRGB_H_L_Test(Deps, F_StateControl, Type+"_R");
                        SetSubTaskProcessing(true);
                        Transition(WORK.WAIT_LED_R_TEST);
                    }
                    break;
                case WORK.WAIT_LED_R_TEST:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());
                        CheckResult(check, SUCCESS: WORK.LED_G_TEST);
                    }
                    break;
                #endregion
                #region GREEN
                case WORK.LED_G_TEST:
                    {
                        Tool.SaveLogToFile("LED_G_TEST", level: "INF");
                        SubTask = new SubTaskRGB_H_L_Test(Deps, F_StateControl, Type + "_G");
                        SetSubTaskProcessing(true);
                        Transition(WORK.WAIT_LED_G_TEST);
                    }
                    break;
                case WORK.WAIT_LED_G_TEST:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());
                        CheckResult(check, SUCCESS: WORK.LED_B_TEST);
                    }
                    break;
                #endregion
                #region BLUE
                case WORK.LED_B_TEST:
                    {
                        Tool.SaveLogToFile("LED_B_TEST", level: "INF");
                        SubTask = new SubTaskRGB_H_L_Test(Deps, F_StateControl, Type + "_B");
                        SetSubTaskProcessing(true);
                        Transition(WORK.WAIT_LED_B_TEST);
                    }
                    break;
                case WORK.WAIT_LED_B_TEST:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());
                        CheckResult(check);
                    }
                    break;
                #endregion

                case WORK.SUCCESS:
                    {
                        if(Scope.TestFail == true)
                        {
                            StatusBox.ShowMessage("GRR Fail");
                            RGBfunc.YieldStatistics(false, SN);
                        }
                        else
                        {
                            RGBfunc.YieldStatistics(true, SN);
                        }

                        SetStatus(TASK_STATUS.SUCCESS);
                        Tool.SaveLogToFile($"{TaskName} End", level:"INF");
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
}
