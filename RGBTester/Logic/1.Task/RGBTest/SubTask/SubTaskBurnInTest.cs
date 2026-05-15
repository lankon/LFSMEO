using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using RGBTester.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Windows.Forms;

using ToolFunction;

namespace RGBTester.Logic
{
    public class SubTaskBurnInTest : IBaseTask<SubTaskBurnInTest.WORK>
    {
        public SubTaskBurnInTest(IBaseTaskDependence dependencies, 
                          IF_StateControl f_StateControl,  
                          string set_state = "Default") : base(dependencies)
        {
            TaskName = this.GetType().Name;
            State = WORK.INITIAL;

            TestType = set_state;

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
        private IF_BaseTask SubTask;                            //子流程
        private IF_StateControl F_StateControl;
        private IF_StatusBox StatusBox;
        private string TestType = "";
        private int DelayTime = 0;
        private int BurnInDuration = 0;                         //Burn In時間(單位:秒)
        private int RepeatTimes = 0;                            //重複次數
        private byte Side;                                      //LED測邊
        private Queue<int> Test_R_DAC = new Queue<int>();       //Red測試用DAC值
        private Queue<int> Test_G_DAC = new Queue<int>();       //Green測試用DAC值
        private Queue<int> Test_B_DAC = new Queue<int>();       //Blue測試用DAC值
        private Queue<int> Test_B2_DAC = new Queue<int>();       //Blue2測試用DAC值
        private Queue<string> Test_CurrentMode = new Queue<string>();   //測試用電流模式
        RGBTesterFunction RGBfunc;
        public enum WORK
        {
            NONE,
            INITIAL,

            INITIAL_SETTING,

            SET_BURN_IN_CURRENT,
            WAIT_BURN_IN_TIME,
            SET_CURRENT_OFF,
            WAIT_COOLING_TIME,

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
            RGBfunc = Deps.ServiceProvider.GetRequiredService<RGBTesterFunction>();
            StatusBox = Deps.ServiceProvider.GetRequiredService<IF_StatusBox>();

            BurnInDuration = ApplicationSetting.Get_Int_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_BurnInTime);

            bool isLeft = false;
            if(ApplicationSetting.Get_Int_Recipe<eF_StartForm>((int)eF_StartForm.Cmbx_TestMode) == 0)
                isLeft = true;

            Side = isLeft ? Deps.LightEngine.LED_LeftSide : Deps.LightEngine.LED_RightSide;
        }

        private void SetDAC(int current_element, Queue<int> dac, double slope_h, double slope_l, double offset_h, double offset_l)
        {
            double current = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>(current_element);
            if (current < 0)
            {
                current = 10;
                Tool.SaveLogToFile($"Burn In Test Current is set to default value: {current}mA, because the input value is invalid.", level: "WRN");
            }

            if (Scope.TaskRGBTest.IsSingleTest == false)
            {
                bool isHigh = current > ApplicationSetting.Get_Double_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_LCM_MaxCurrent);
                double slope = isHigh ? slope_h : slope_l;
                double offset = isHigh ? offset_h : offset_l;

                int R_DAC = (slope < 0.0000001) ? 0 : (int)((current - offset) / slope);

                if (R_DAC < 0 || R_DAC > 1023)
                    R_DAC = 0;

                dac.Enqueue(R_DAC);
            }
            else
            {
                bool isHigh = current > ApplicationSetting.Get_Double_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_LCM_MaxCurrent);
                double R_sense = isHigh ? ApplicationSetting.Get_Double_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Rfb_HCM) : 
                                          ApplicationSetting.Get_Double_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Rfb_LCM);

                int R_DAC = (int)(current * 1023 * R_sense / 160);

                if (R_DAC < 0 || R_DAC > 1023)
                    R_DAC = 0;

                dac.Enqueue(R_DAC);
            }
        }
        private void SetCurrent()
        {
            SetDAC((int)eF_ParameterSettingRecipe.TxtBx_BurnInCurrent_R, Test_R_DAC,
                    RGBfunc.SlopeOffsetResult.R_Slope_HCM, RGBfunc.SlopeOffsetResult.R_Slope_LCM,
                    RGBfunc.SlopeOffsetResult.R_Offset_HCM, RGBfunc.SlopeOffsetResult.R_Offset_LCM);
            SetDAC((int)eF_ParameterSettingRecipe.TxtBx_BurnInCurrent_G, Test_G_DAC,
                    RGBfunc.SlopeOffsetResult.G_Slope_HCM, RGBfunc.SlopeOffsetResult.G_Slope_LCM,
                    RGBfunc.SlopeOffsetResult.G_Offset_HCM, RGBfunc.SlopeOffsetResult.G_Offset_LCM);
            SetDAC((int)eF_ParameterSettingRecipe.TxtBx_BurnInCurrent_B, Test_B_DAC,
                    RGBfunc.SlopeOffsetResult.B_Slope_HCM, RGBfunc.SlopeOffsetResult.B_Slope_LCM,
                    RGBfunc.SlopeOffsetResult.B_Offset_HCM, RGBfunc.SlopeOffsetResult.B_Offset_LCM);
            
            if(RGBfunc.GetModuleType() ==  eModuleType.Function_Test)
            {
                SetDAC((int)eF_ParameterSettingRecipe.TxtBx_BurnInCurrent_B, Test_B2_DAC,               //先暫時用Blue的電流設定
                    RGBfunc.SlopeOffsetResult.B2_Slope_HCM, RGBfunc.SlopeOffsetResult.B2_Slope_LCM,
                    RGBfunc.SlopeOffsetResult.B2_Offset_HCM, RGBfunc.SlopeOffsetResult.B2_Offset_LCM);
            }
            
            double current_R = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_BurnInCurrent_R);
            double current_G = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_BurnInCurrent_G);
            double current_B = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_BurnInCurrent_B);
            double lowlimit = ApplicationSetting.Get_Double_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_LCM_MaxCurrent);

            if (current_R < lowlimit || current_G < lowlimit || current_B < lowlimit)
                Test_CurrentMode.Enqueue("LCM");
            else
                Test_CurrentMode.Enqueue("HCM");
        }
        private bool CheckTestTemperature(double temperature)
        {
            double limit = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_FailOverTemp);
            if (temperature > limit)
            {
                Scope.TestFail = true;
                RGBfunc.FailReasonFlag.IsTemperatureErr = true;
                Tool.SaveLogToFile($"Temperature = {temperature}°C,溫度過高", level: "WRN");
                return false;
            }

            return true;
        }
        private bool TurnOffLed()
        {
            bool res = false;
            
            if (RGBfunc.GetModuleType() == eModuleType.IV_Calibration)
                res = Deps.LightEngine.SetLed_AllColorDAC(Side, 0, 0, 0);
            else
                res = Deps.LightEngine.SetLed_AllColorDAC(Side, 0, 0, 0, 0);

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
                        Preset();

                        string sn = RGBfunc.SerialNumber;
                        Deps.File.WriteFile($"SN,{sn}", TestType);
                        Deps.File.WriteFile($"Temperature,TestTime", TestType);

                        TurnOffLed();                      

                        Transition(WORK.INITIAL_SETTING);
                    }
                    break;
                case WORK.INITIAL_SETTING:
                    {
                        SetCurrent();
                        Transition(WORK.SET_BURN_IN_CURRENT);
                    }
                    break;
                case WORK.SET_BURN_IN_CURRENT:
                    {
                        if (!Deps.LightEngine.SetLed_CurrentMode(Test_CurrentMode.Dequeue()))
                        {
                            StatusBox.ShowMessage("HDMI Board Set Current Mode Fail");
                            Transition(WORK.ABORT);
                            break;
                        }

                        if(RGBfunc.GetModuleType() == eModuleType.Function_Test)
                        {
                            Deps.LightEngine.SetLed_AllColorDAC(Side, Test_R_DAC.Dequeue(),
                                                                        Test_G_DAC.Dequeue(),
                                                                        Test_B_DAC.Dequeue(),
                                                                        Test_B2_DAC.Dequeue());
                        }
                        else
                        {
                            Deps.LightEngine.SetLed_AllColorDAC(Side, Test_R_DAC.Dequeue(),
                                                                        Test_G_DAC.Dequeue(),
                                                                        Test_B_DAC.Dequeue());
                        }

                        DelayTime = Tool.GetCurrentTickCount();
                        Transition(WORK.WAIT_BURN_IN_TIME);
                    }
                    break;
                case WORK.WAIT_BURN_IN_TIME:
                    {
                        if(Tool.CheckTimeOverSec(DelayTime, BurnInDuration))
                        {
                            double temperature = double.Parse(Deps.LightEngine.GetTemperature());
                            Deps.File.WriteFile($"{temperature},{DateTime.Now.ToString("HH:mm:ss")}", TestType);
                            if (!CheckTestTemperature(temperature))
                            {
                                TurnOffLed();
                                Transition(WORK.SUCCESS);
                                break;
                            }

                            Transition(WORK.SET_CURRENT_OFF);
                        }
                    }
                    break;
                case WORK.SET_CURRENT_OFF:
                    {
                        if(!TurnOffLed())
                        {
                            StatusBox.ShowMessage("HDMI Board Set DAC Fail");
                            Transition(WORK.ABORT);
                            break;
                        }

                        DelayTime = Tool.GetCurrentTickCount();
                        Transition(WORK.WAIT_COOLING_TIME);
                    }
                    break;
                case WORK.WAIT_COOLING_TIME:
                    {
                        if(Tool.CheckTimeOverSec(DelayTime, BurnInDuration))
                        {
                            if (Test_R_DAC.Count != 0)
                            {
                                Transition(WORK.SET_BURN_IN_CURRENT);
                            }
                            else
                            {
                                int AllTime = ApplicationSetting.Get_Int_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_BurnInRepeat);
                                RepeatTimes++;
                                if (RepeatTimes >= AllTime)
                                    Transition(WORK.SUCCESS);
                                else
                                    Transition(WORK.INITIAL_SETTING);
                            }
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
}
