using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using DeviceCore;
using RGBTester.Base;

namespace RGBTester.Logic
{
    public class SubTaskOpticalTest : IBaseTask<SubTaskOpticalTest.WORK>
    {
        public SubTaskOpticalTest(IBaseTaskDependence dependencies, 
                          IF_StateControl f_StateControl,  
                          string set_state = "Default") : base(dependencies)
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
        private long cycletime = 0;
        private int delay_time = 1;
        private int TotalStep = 0;
        private int IntgTimeSetting = 0;
        private int _searchMinTime = 0;     //二分法搜尋用
        private int _searchMaxTime = 0;     //二分法搜尋用
        private int TestDAC = 0;
        private int TestCurrent = 0;
        private byte Side;
        private byte Color;
        private string Type;
        private string TestSide;
        private string TestColor;
        private string[] TestColors = { "R", "G", "B", "B2"};
        private string CurrentModeStatus = "";
        private float[] fSpectrumRawData;
        private double[] SpectrumRawData;
        private double LED_Slope = 0;
        private double LED_Offset = 0;
        private double PowerGain = 1.0;
        private double WavelengthGain = 0;
        private IF_BaseTask SubTask;                  //子流程
        private IF_StateControl F_StateControl;
        private IF_ProgressBar ProgressBar;
        private IWriteFile WriteFile;
        private Dictionary<string, Dictionary<string, CurrentCondition>> CurrentConfig; //雙層字典：[Side (Left/Right)][Color (R/G/B/B2)]
        private LuminousFlux LF = new LuminousFlux();
        private Queue<int> qCurrent = new Queue<int>();
        private Queue<int>[] qWPC_Current;
        private RGBTesterData TesterData = new RGBTesterData();
        private RGBTesterFunction RGBFunc;
        private Wavelength WL = new Wavelength();

        public class CurrentCondition
        {
            public int Start { get; set; }
            public int Step { get; set; }
            public int End { get; set; }
            public int IntegralTimeStart { get; set; }
            public int IntegralTimeStep { get; set; }
            public int IntegralTimeEnd { get; set; }
        }
        public enum WORK
        {
            NONE,
            INITIAL,
            IDLE,

            SET_LED_DAC,

            AUTO_INTEGRAL,
            SET_DEFAULT_VALUE,

            REST,
            WAIT_REST,
            GET_SPECTRUM,
            CALCULATE_LUMEN,
            CALCULATE_WAVELENGTH,

            WRITE_TEST_DATA,

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
        private bool CheckTimeOverMilSec(int tick, int time)
        {
            var time_count = Environment.TickCount - tick;
            bool res = time_count > time;

            return res;
        }
        private void Preset()
        {
            ProgressBar = Deps.ServiceProvider.GetRequiredService<IF_ProgressBar>();
            RGBFunc = Deps.ServiceProvider.GetRequiredService<RGBTesterFunction>();
            WriteFile = Deps.ServiceProvider.GetRequiredService<IWriteFile>();

            string[] res = Type.Split('_');

            TestSide = res[0];
            TestColor = res[1];

            TesterData.TestColor = TestColor;
            TesterData.TestSide = TestSide;
            TesterData.SN = RGBFunc.SerialNumber;

            bool isLeft = (TestSide == "Left");
            Side = isLeft ? Deps.LightEngine.LED_LeftSide : Deps.LightEngine.LED_RightSide;
            if (TestColor == "R")
            {
                Color = Deps.LightEngine.LED_R;
                PowerGain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_RedPowerGain);
                WavelengthGain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_RedWavelengthGain);
            }
            else if(TestColor == "G")
            {
                Color = Deps.LightEngine.LED_G;
                PowerGain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_GreenPowerGain);
                WavelengthGain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_GreenWavelengthGain);
            }
            else if(TestColor == "B")
            {
                Color = Deps.LightEngine.LED_B;
                PowerGain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_BluePowerGain);
                WavelengthGain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_BlueWavelengthGain);
            }
            else if(TestColor == "B2")
            {
                Color = Deps.LightEngine.LED_B2;
                PowerGain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_BluePowerGain);
                WavelengthGain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_BlueWavelengthGain);
            }
            else if (TestColor == "WPC")
            {

            }

            //測試藍光時為了避免紅光漏光,Voltage要調低
            if (TestColor == "B" || TestColor == "B2")
                Deps.LightEngine.SetLed_AllColorVoltage(Side, 0.5, 0.5, 3.6);
            else if (TestColor == "R")
                Deps.LightEngine.SetLed_AllColorVoltage(Side, 5.5, 0.5, 0.5);
            else if(TestColor == "G")
                Deps.LightEngine.SetLed_AllColorVoltage(Side, 0.5, 5.5, 0.5);

            InitialCurrentConfig();
            SetCurrentCondition();
        }
        private void InitialCurrentConfig()
        {
            string[] sides = { "Left", "Right"};

            qWPC_Current = new Queue<int>[TestColors.Length];
            CurrentConfig = new Dictionary<string, Dictionary<string, CurrentCondition>>();

            foreach (var side in sides)
            {
                CurrentConfig[side] = new Dictionary<string, CurrentCondition>();
                foreach (var color in TestColors)
                {
                    CurrentConfig[side][color] = new CurrentCondition
                    {
                        Start = GetCurrentRecipe(side, color, "Start"),
                        Step = GetCurrentRecipe(side, color, "Step"),
                        End = GetCurrentRecipe(side, color, "End"),
                        IntegralTimeStart = GetCurrentRecipe(side, color, "IntgTimeStart"),
                        IntegralTimeStep = GetCurrentRecipe(side, color, "IntgTimeStep"),
                        IntegralTimeEnd = GetCurrentRecipe(side, color, "IntgTimeEnd"),
                    };
                }
            }
        }
        private int GetCurrentRecipe(string side, string color, string type)
        {
            string enumName = "";
            string wpc_mode = "_WPC";

            if (TestColor != "WPC")
                wpc_mode = "";

            if (type.Contains("IntgTime"))
                enumName = $"TxtBx_{side}_{color}_{type}" + wpc_mode;
            else
                enumName = $"TxtBx_{side}_{color}_I_{type}" + wpc_mode;    //"TxtBx_Left_R_I_Start"

            //將字串轉換為 Enum 型別
            if (Enum.TryParse<eF_OpticalTestRecipe>(enumName, out eF_OpticalTestRecipe result))
            {
                return ApplicationSetting.Get_Int_Recipe<eF_OpticalTestRecipe>((int)result);
            }

            Tool.SaveLogToFile($"Recipe Enum not found: {enumName}", level: "ERR");
            return 0;
        }
        private void SetCurrentCondition()
        {
            if(TestColor == "WPC")
            {
                string test_color = "";

                for (int i = 0; i < qWPC_Current.Count(); i++)
                {
                    test_color = TestColors[i];
                    qWPC_Current[i] = new Queue<int>();

                    PopulateQueue(TestSide, TestColors[i], qWPC_Current[i]);
                    TotalStep = qWPC_Current[i].Count;
                }
            }
            else
            {
                PopulateQueue(TestSide, TestColor, qCurrent);
                TotalStep = qCurrent.Count;
            }
        }
        private void PopulateQueue(string side, string color, Queue<int> targetQueue)
        {
            if (CurrentConfig.TryGetValue(side, out var sideDict) && sideDict.TryGetValue(color, out var set))
            {
                Tool.SaveLogToFile($"Current Condition Set:{side}_{color} [Start:{set.Start}/Step:{set.Step}/End:{set.End}]");

                for (int j = set.Start; j <= set.End; j += set.Step)
                {
                    targetQueue.Enqueue(j);
                }

                return;
            }

            // 找不到設定時紀錄錯誤
            Tool.SaveLogToFile($"Condition Missing: {side}_{color}", level: "ERR");
        }
        private int CalculateDACfromCurrent(int current)
        {
            double LCM_I = ApplicationSetting.Get_Double_Recipe<eF_OpticalTestRecipe>((int)eF_OpticalTestRecipe.TxtBx_LCM_MaxCurrent);

            if(RGBFunc.GetFunctionTestProcess() == true && TestColor != "WPC")
            {
                try
                {
                    if (current > LCM_I)
                    {
                        if (TestColor != "WPC")
                        {
                            LED_Slope = WriteFile.LED_Slope[$"{TestColor}_Slope_HCM"];
                            LED_Offset = WriteFile.LED_Offset[$"{TestColor}_Offset_HCM"];
                        }

                        SetLedCurrentMode("HCM");
                    }
                    else
                    {
                        if (TestColor != "WPC")
                        {
                            LED_Slope = WriteFile.LED_Slope[$"{TestColor}_Slope_LCM"];
                            LED_Offset = WriteFile.LED_Offset[$"{TestColor}_Offset_LCM"];
                        }

                        SetLedCurrentMode("LCM");
                    }
                }
                catch(Exception ex)
                {
                    Tool.SaveLogToFile($"CalculateDACfromCurrent對應變數失敗,{ex}", level:"ERR");
                }
            }
            else
            {
                if (current > LCM_I)
                {
                    LED_Slope = ApplicationSetting.Get_Double_Recipe<eF_OpticalTestRecipe>((int)eF_OpticalTestRecipe.TxtBx_HCM_TestSlope);
                    LED_Offset = ApplicationSetting.Get_Double_Recipe<eF_OpticalTestRecipe>((int)eF_OpticalTestRecipe.TxtBx_HCM_TestOffset);
                    SetLedCurrentMode("HCM");
                }
                else
                {
                    LED_Slope = ApplicationSetting.Get_Double_Recipe<eF_OpticalTestRecipe>((int)eF_OpticalTestRecipe.TxtBx_LCM_TestSlope);
                    LED_Offset = ApplicationSetting.Get_Double_Recipe<eF_OpticalTestRecipe>((int)eF_OpticalTestRecipe.TxtBx_LCM_TestOffset);
                    SetLedCurrentMode("LCM");
                }
            }

            if (LED_Slope < 0.0001)
            {
                Tool.SaveLogToFile("斜率異常,電流推算DAC失敗", level: "WRN");
                return 0;
            }

            int dac = (int)((current - LED_Offset) / LED_Slope);

            if (dac > 1022)
                dac = 1022;

            return dac;
        }
        private void UpdateProgressBar(Queue<int> state, int total_state)
        {
            if (state.Count % 1 == 0)   //每幾步更新一次UI
            {
                double res = (double)state.Count / (double)total_state * 100;
                int progress = 100 - (int)res;
                ProgressBar.UpateProgress(progress);
            }
        }
        private void SetLedCurrentMode(string mode)
        {
            if(mode != CurrentModeStatus)
            {
                Deps.LightEngine.SetLed_AllColorDAC(Side, 0, 0, 0, 0);
                Deps.LightEngine.SetLed_CurrentMode(mode);
                CurrentModeStatus = mode;
            }
        }
        private void CheckLumCondition(double test_current, double lum)
        {
            if(TestColor == "R" && Math.Abs(test_current - 50) < 0.001 && 
              (lum < ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_RLight_LL) || 
               lum > ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_RLight_UL)))
            {
                RGBFunc.FailReasonFlag.IsRedLuminousErr = true;
                Tool.SaveLogToFile($"{TestColor} Luminous Out of Range");
            }
            else if (TestColor == "G" && Math.Abs(test_current - 80) < 0.001 &&
                    (lum < ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_GLight_LL) ||
                     lum > ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_GLight_UL)))
            {
                RGBFunc.FailReasonFlag.IsGreenLuminousErr = true;
                Tool.SaveLogToFile($"{TestColor} Luminous Out of Range");
            }
            else if ((TestColor == "B" || TestColor == "B2") && Math.Abs(test_current - 50) < 0.001 &&
                     (lum < ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_BLight_LL) ||
                      lum > ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_BLight_UL)))
            {
                if(TestColor == "B")
                    RGBFunc.FailReasonFlag.IsBlueLuminousErr = true;
                else if(TestColor == "B2")
                    RGBFunc.FailReasonFlag.IsBlue2LuminousErr = true;

                Tool.SaveLogToFile($"{TestColor} Luminous Out of Range");
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

                        Deps.LightEngine.SetLed_CurrentMode("LCM"); //先轉成LCM避免Clamping

                        if (TestColor == "WPC")
                            IntgTimeSetting = ApplicationSetting.Get_Int_Recipe<eF_OpticalTestRecipe>((int)eF_OpticalTestRecipe.TxtBx_Left_IntgTimeStart_WPC);
                        else
                            IntgTimeSetting = CurrentConfig[TestSide][TestColor].IntegralTimeStart;

                        Transition(WORK.SET_LED_DAC);
                    }
                    break;
                case WORK.SET_LED_DAC:
                    {
                        Tool.ResetTimeCount(out cycletime);

                        if (TestColor == "WPC")
                        {
                            UpdateProgressBar(qWPC_Current[0], TotalStep);

                            int dac_r = CalculateDACfromCurrent(qWPC_Current[0].Dequeue());
                            int dac_g = CalculateDACfromCurrent(qWPC_Current[1].Dequeue());
                            int dac_b = CalculateDACfromCurrent(qWPC_Current[2].Dequeue());
                            int dac_b2 = CalculateDACfromCurrent(qWPC_Current[3].Dequeue());

                            Deps.LightEngine.SetLed_AllColorDAC(Side, dac_r, dac_g, dac_b, dac_b2);
                        }
                        else
                        {
                            UpdateProgressBar(qCurrent, TotalStep);

                            TestCurrent = qCurrent.Dequeue();
                            TesterData.Currentpoint.Add(TestCurrent);
                            TestDAC = CalculateDACfromCurrent(TestCurrent);
                            Deps.LightEngine.SetLed_DAC(Color, Side, TestDAC);

                            _searchMaxTime = CurrentConfig[TestSide][TestColor].IntegralTimeStart;
                            _searchMinTime = CurrentConfig[TestSide][TestColor].IntegralTimeEnd;
                        }

                        ResetTimeCount(out delay_time);

                        State = WORK.AUTO_INTEGRAL;
                        goto case WORK.AUTO_INTEGRAL;
                    }
                    break;
                case WORK.AUTO_INTEGRAL:
                    {
                        //if (!CheckTimeOverMilSec(delay_time, 500))
                        //    break;
                        
                        int Intg_interval_start = 30;
                        int Intg_interval_end = 80;

                        //=====================================
                        // 取得光譜與強度
                        fSpectrumRawData = Deps.Spectrometer.GetSpectrumOneShot(ESpectrumName.SPECTRUM_1, (uint)IntgTimeSetting);
                        double percent = Deps.Spectrometer.GetIntensityPercent(ESpectrumName.SPECTRUM_1);

                        // 判斷是否達標
                        if (percent > Intg_interval_start && percent < Intg_interval_end)
                        {
                            double index = IntgTimeSetting / 11.11111;
                            IntgTimeSetting = 11 * (int)index;

                            Tool.SaveLogToFile($"測試積分時間:{IntgTimeSetting}ms");

                            TesterData.IntegralTime.Add(IntgTimeSetting);
                            TesterData.Temperature.Add(double.Parse(Deps.LightEngine.GetTemperature()));

                            State = WORK.REST;
                            goto case WORK.REST;
                        }

                        // 更新二分法搜尋邊界
                        if (percent >= Intg_interval_end)    
                        {
                            // 過曝 / 太亮
                            _searchMaxTime = IntgTimeSetting - 1;
                        }
                        else if (percent <= Intg_interval_start)
                        {
                            // 光強不足 / 太暗
                            _searchMinTime = IntgTimeSetting + 1;
                        }

                        // 異常處理：當最小邊界大於最大邊界，代表在此區間內找不到符合值
                        if (_searchMinTime > _searchMaxTime)
                        {
                            if (percent <= Intg_interval_start)
                            {
                                Tool.SaveLogToFile("光強不足 (已達搜尋極限)", level: "WRN");
                                Transition(WORK.SET_DEFAULT_VALUE);
                            }
                            else
                            {
                                Tool.SaveLogToFile("分光卡過曝請調整積分時間 (已達搜尋極限)", level: "ERR");
                                Transition(WORK.ABORT);
                            }
                            break;
                        }

                        // 計算下一次的積分時間 (取上下限的中間值)
                        int nextTime = (_searchMinTime + _searchMaxTime) / 2;

                        // 套用新時間，等待下一次 state machine 迴圈再次讀取
                        IntgTimeSetting = nextTime;
                    }
                    break;
                case WORK.SET_DEFAULT_VALUE:
                    {
                        TesterData.Lumens.Add(0);
                        TesterData.OpticalPower.Add(0);
                        TesterData.WLD.Add(0);
                        TesterData.CycleTime.Add(Tool.GetTime(cycletime));
                        TesterData.Temperature.Add(0);
                        TesterData.IntegralTime.Add(0);

                        CheckLumCondition(TestCurrent, 0);

                        if ((qCurrent.Count == 0 && TestSide != "WPC")/* || (qWPC_Current[0].Count == 0 && TestSide == "WPC")*/)
                        {
                            Transition(WORK.WRITE_TEST_DATA);
                        }
                        else
                        {
                            State = WORK.SET_LED_DAC;
                            goto case WORK.SET_LED_DAC;
                        }
                    }
                    break;
                case WORK.REST:
                    {
                        Deps.LightEngine.SetLed_DAC(Color, Side, 0);
                        ResetTimeCount(out delay_time);

                        State = WORK.WAIT_REST;
                        goto case WORK.WAIT_REST;
                    }
                    break;
                case WORK.WAIT_REST:
                    {
                        if (CheckTimeOverMilSec(delay_time, 2000)) //硬體轉換時間,需依實際狀況調整
                        {
                            Deps.LightEngine.SetLed_DAC(Color, Side, TestDAC);
                            ResetTimeCount(out delay_time);

                            State = WORK.GET_SPECTRUM;
                            goto case WORK.GET_SPECTRUM;
                        }
                    }
                    break;
                case WORK.GET_SPECTRUM:
                    {
                        if(CheckTimeOverMilSec(delay_time, 50)) //硬體轉換時間,需依實際狀況調整
                        {
                            fSpectrumRawData = Deps.Spectrometer.GetSpectrumOneShot(ESpectrumName.SPECTRUM_1, (uint)IntgTimeSetting);

                            if(fSpectrumRawData == null)
                            {
                                Tool.SaveLogToFile($"Get Spectrum Failed", level: "ERR");
                                Transition(WORK.ABORT);
                                break;
                            }

                            SpectrumRawData = fSpectrumRawData.Select(x => (double)x).ToArray();

                            State = WORK.CALCULATE_LUMEN;
                            goto case WORK.CALCULATE_LUMEN;
                        }
                    }
                    break;
                case WORK.CALCULATE_LUMEN:
                    {
                        float[] fwavelength = Deps.Spectrometer.GetWavelengthSpan(ESpectrumName.SPECTRUM_1);
                        double[] wavelength = fwavelength.Select(x => (double)x).ToArray();

                        double[] intensity = SpectrumRawData.Select(x => x).ToArray();
                        double k_value = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_OpticalKValue);
                        double gain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_PowerGain);
                        double offset = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_PowerOffset);

                        double lum_result = LF.CalculateTotalLumens(wavelength, intensity, IntgTimeSetting, k_value) * gain * PowerGain + offset;
                        TesterData.Lumens.Add(lum_result);

                        CheckLumCondition(TestCurrent, lum_result);

                        State = WORK.CALCULATE_WAVELENGTH;
                        goto case WORK.CALCULATE_WAVELENGTH;
                    }
                    //break;
                case WORK.CALCULATE_WAVELENGTH:
                    {
                        float[] fwavelength = Deps.Spectrometer.GetWavelengthSpan(ESpectrumName.SPECTRUM_1);
                        double[] wavelength = fwavelength.Select(x => (double)x).ToArray();

                        //強度單位必須為W/nm
                        double[] W_Intensity = SpectrumRawData.Select(x => x).ToArray();

                        TesterData.WLD.Add(WL.Calculate_WLD(wavelength, SpectrumRawData) + WavelengthGain);
                        double k_value = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_OpticalKValue);
                        double org_power = WL.Calculate_Power(wavelength, W_Intensity, IntgTimeSetting, k_value);
                        double gain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_PowerGain);
                        double offset = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_PowerOffset);
                        double calibration_power = org_power * gain * PowerGain + offset;

                        TesterData.OpticalPower.Add(calibration_power);
                        TesterData.CycleTime.Add(Tool.GetTime(cycletime));

                        if (TestColor == "WPC")
                        {
                            if (qWPC_Current[0].Count == 0)
                                Transition(WORK.WRITE_TEST_DATA);
                            else
                            {
                                State = WORK.SET_LED_DAC;
                                goto case WORK.SET_LED_DAC;
                            }
                        }
                        else
                        {
                            if (qCurrent.Count == 0)
                                Transition(WORK.WRITE_TEST_DATA);
                            else
                            {
                                State = WORK.SET_LED_DAC;
                                goto case WORK.SET_LED_DAC;
                            }
                        }
                    }
                    break;

                case WORK.WRITE_TEST_DATA:
                    {
                        string copy_path = ApplicationSetting.Get_String_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_TestFileCopyPath);
                        string copy_path1 = ApplicationSetting.Get_String_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_TestFileCopyPath1);

                        WriteFile.OpticalResult.WriteTestData(TesterData, copy_path, copy_path1);
                        Transition(WORK.SUCCESS);
                    }
                    break;

                case WORK.SUCCESS:
                    {
                        ProgressBar.HideForm();
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
                        ProgressBar.HideForm();
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
