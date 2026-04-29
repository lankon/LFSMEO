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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

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
        ResultData ResultData;
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

            DISP_TEST,
            WAIT_DISP_TEST,

            LED_R_TEST,
            LED_G_TEST,
            LED_B_TEST,
            LED_B2_TEST,

            WAIT_LED_R_TEST,
            WAIT_LED_G_TEST,
            WAIT_LED_B_TEST,
            WAIT_LED_B2_TEST,

            CHECK_SLOPE_OFFSET,

            BURN_IN_TEST,
            WAIT_BURN_IN_TEST,

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

                        if (Scope.TaskRGBTest.IsSingleTest == true)
                        {
                            int select = ApplicationSetting.Get_Int_Recipe<eF_StartForm>((int)eF_StartForm.Cmbx_PartTest);

                            if (select == (int)ePartTestItem.BurinIn)
                                Transition(WORK.BURN_IN_TEST);
                            else
                                Transition(WORK.LED_R_TEST);
                        }
                        else
                        {
                            if(RGBfunc.GetModuleType() == eModuleType.IV_Calibration)
                                Transition(WORK.LED_R_TEST);
                            else if(RGBfunc.GetModuleType() == eModuleType.Function_Test)
                                Transition(WORK.DISP_TEST);
                        }
                    }
                    break;

                #region PP_DISP
                case WORK.DISP_TEST:
                    {
                        Tool.SaveLogToFile("PP DISP Test", level: "INF");
                        SubTask = new SubTask_DISP_Test(Deps, F_StateControl, Type);
                        SetSubTaskProcessing(true);
                        Transition(WORK.WAIT_DISP_TEST);
                    }
                    break;
                case WORK.WAIT_DISP_TEST:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());
                        CheckResult(check, SUCCESS: WORK.LED_R_TEST);
                    }
                    break;
                #endregion
                #region RED
                case WORK.LED_R_TEST:
                    {
                        Tool.SaveLogToFile("LED_R_Test", level: "INF");
                        
                        if(RGBfunc.GetModuleType() == eModuleType.IV_Calibration)
                            SubTask = new SubTaskRGB_H_L_Test(Deps, F_StateControl, Type + "_R");
                        else
                            SubTask = new SubTaskRGB_H_L_Test_FunctionTester(Deps, F_StateControl, Type + "_R");

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

                        if (RGBfunc.GetModuleType() == eModuleType.IV_Calibration)
                            SubTask = new SubTaskRGB_H_L_Test(Deps, F_StateControl, Type + "_G");
                        else
                            SubTask = new SubTaskRGB_H_L_Test_FunctionTester(Deps, F_StateControl, Type + "_G");

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

                        if (RGBfunc.GetModuleType() == eModuleType.IV_Calibration)
                            SubTask = new SubTaskRGB_H_L_Test(Deps, F_StateControl, Type + "_B");
                        else
                            SubTask = new SubTaskRGB_H_L_Test_FunctionTester(Deps, F_StateControl, Type + "_B");

                        SetSubTaskProcessing(true);
                        Transition(WORK.WAIT_LED_B_TEST);
                    }
                    break;
                case WORK.WAIT_LED_B_TEST:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());

                        if (RGBfunc.GetModuleType() == eModuleType.IV_Calibration)
                            CheckResult(check, SUCCESS: WORK.CHECK_SLOPE_OFFSET);
                        else
                            CheckResult(check, SUCCESS: WORK.LED_B2_TEST);
                    }
                    break;
                #endregion
                #region BLUE2
                case WORK.LED_B2_TEST:
                    {
                        Tool.SaveLogToFile("LED_B2_TEST", level: "INF");

                        if (RGBfunc.GetModuleType() == eModuleType.IV_Calibration)
                            SubTask = new SubTaskRGB_H_L_Test(Deps, F_StateControl, Type + "_B2");
                        else
                            SubTask = new SubTaskRGB_H_L_Test_FunctionTester(Deps, F_StateControl, Type + "_B2");

                        SetSubTaskProcessing(true);
                        Transition(WORK.WAIT_LED_B_TEST);
                    }
                    break;
                case WORK.WAIT_LED_B2_TEST:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());
                        CheckResult(check, SUCCESS: WORK.CHECK_SLOPE_OFFSET);
                    }
                    break;
                #endregion

                case WORK.CHECK_SLOPE_OFFSET:
                    {
                        bool res = ResultData.CheckSlopeData.CheckSlopeCorrect();
                        var para_set = Deps.ServiceProvider.GetRequiredService<IF_ParameterSetting>();
                        para_set.ShowSlopeCheckDataInvoke(ResultData.CheckSlopeData.LCM_R_Calculate, ResultData.CheckSlopeData.LCM_R_Dev,
                                                            ResultData.CheckSlopeData.LCM_G_Calculate, ResultData.CheckSlopeData.LCM_G_Dev,
                                                            ResultData.CheckSlopeData.LCM_B_Calculate, ResultData.CheckSlopeData.LCM_B_Dev,
                                                            ResultData.CheckSlopeData.HCM_R_Calculate, ResultData.CheckSlopeData.HCM_R_Dev,
                                                            ResultData.CheckSlopeData.HCM_G_Calculate, ResultData.CheckSlopeData.HCM_G_Dev,
                                                            ResultData.CheckSlopeData.HCM_B_Calculate, ResultData.CheckSlopeData.HCM_B_Dev);

                        if (!res)
                        {
                            Scope.TestFail = true;
                            RGBfunc.FailReasonFlag.IsSlopeCalculateCurrentErr = true;
                        }

                        string copy_path = ApplicationSetting.Get_String_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_TestFileCopyPath);
                        string copy_path1 = ApplicationSetting.Get_String_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_TestFileCopyPath1);
                        string side = (Type == "Left") ? "L" : "R";
                        ResultData.CheckSlopeData.OutputResult(SN, side, copy_path, copy_path1);

                        if (Scope.TaskRGBTest.IsSingleTest == true)
                        {
                            int select = ApplicationSetting.Get_Int_Recipe<eF_StartForm>((int)eF_StartForm.Cmbx_PartTest);

                            if (select == (int)ePartTestItem.IV_Test_LCM || select == (int)ePartTestItem.IV_Test ||
                                select == (int)ePartTestItem.IV_Test_HCM)
                                Transition(WORK.SUCCESS);
                            else if (select == (int)ePartTestItem.BurinIn)
                                Transition(WORK.BURN_IN_TEST);
                        }
                        else
                        {
                            // 燒測時間設定為0時不進行燒測
                            if (ApplicationSetting.Get_Int_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_BurnInTime) == 0)
                                Transition(WORK.SUCCESS);
                            else
                                Transition(WORK.BURN_IN_TEST);
                        }
                    }
                    break;

                #region BURN_IN
                case WORK.BURN_IN_TEST:
                    {
                        Tool.SaveLogToFile("BURN_IN_TEST", level: "INF");
                        SubTask = new SubTaskBurnInTest(Deps, F_StateControl, Type+"_BurnIn");
                        SetSubTaskProcessing(true);
                        Transition(WORK.WAIT_BURN_IN_TEST);
                    }
                    break;
                case WORK.WAIT_BURN_IN_TEST:
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
                            string description = RGBfunc.FailReasonFlag.GetFailDescription();
                            StatusBox.ShowMessage(description);
                            RGBfunc.YieldStatistics(false, SN, description);
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
