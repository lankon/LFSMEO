using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using RGBTester.Device;
using RGBTester.UI;
using SampleCode.Logic;
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
    public class SubTaskOpticalTestRGB : IBaseTask<SubTaskOpticalTestRGB.WORK>
    {
        public SubTaskOpticalTestRGB(IBaseTaskDependence dependencies,
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
        ResultData ResultData;
        private Queue<string> TestQueue = new Queue<string>(new[] { "R", "G", "B" ,"B1"});
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

            TEST_COLOR,
            WAIT_TEST_COLOR,

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
        private void SetCheckSlopeDAC()
        {
            ResultData.CheckSlopeData.ResetParameter();
            ResultData.CheckSlopeData.SetDeviationLimit(ApplicationSetting.Get_Double_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_DeviationLimit));
            ResultData.CheckSlopeData.SetCheck_LCM_DAC(ApplicationSetting.Get_Int_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_LCM_Check_DAC1),
                                                        ApplicationSetting.Get_Int_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_LCM_Check_DAC2),
                                                        ApplicationSetting.Get_Int_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_LCM_Check_DAC3),
                                                        ApplicationSetting.Get_Int_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_LCM_Check_DAC4),
                                                        ApplicationSetting.Get_Int_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_LCM_Check_DAC5));

            ResultData.CheckSlopeData.SetCheck_HCM_DAC(ApplicationSetting.Get_Int_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_HCM_Check_DAC1),
                                                        ApplicationSetting.Get_Int_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_HCM_Check_DAC2),
                                                        ApplicationSetting.Get_Int_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_HCM_Check_DAC3),
                                                        ApplicationSetting.Get_Int_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_HCM_Check_DAC4),
                                                        ApplicationSetting.Get_Int_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_HCM_Check_DAC5));
        }

        private void Preset()
        {
            StatusBox = Deps.ServiceProvider.GetRequiredService<IF_StatusBox>();
            RGBfunc = Deps.ServiceProvider.GetRequiredService<RGBTesterFunction>();
            ResultData = Deps.ServiceProvider.GetRequiredService<ResultData>();

            Scope.TestFail = false;
            RGBfunc.FailReasonFlag.ResetAllFlag();
            SetCheckSlopeDAC();

            SN = RGBfunc.SerialNumber;
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
                        Transition(WORK.TEST_COLOR);
                    }
                    break;

                #region TEST_COLOR
                case WORK.TEST_COLOR:
                    {
                        if(TestQueue.Count > 0)
                        {
                            string color = TestQueue.Dequeue();
                            Tool.SaveLogToFile($"LED_{color}_Test", level: "INF");

                            SubTask = new SubTaskOpticalTest(Deps, F_StateControl, Type + $"_{color}");
                            SetSubTaskProcessing(true);

                            Transition(WORK.WAIT_TEST_COLOR);
                        }
                        else
                        {
                            Transition(WORK.SUCCESS);
                        }
                    }
                    break;
                case WORK.WAIT_TEST_COLOR:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());
                        CheckResult(check, SUCCESS: WORK.TEST_COLOR);
                    }
                    break;
                #endregion

                case WORK.SUCCESS:
                    {
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
