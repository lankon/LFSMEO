using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
//using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

using DeviceCore;
using ToolFunction;
using RGBTester.Base;
using System.Runtime.CompilerServices;

namespace RGBTester.Logic
{
    public class SubTaskRGB_H_L_Test : IBaseTask<SubTaskRGB_H_L_Test.WORK>
    {
        public SubTaskRGB_H_L_Test(IBaseTaskDependence dependencies, 
                          IF_StateControl f_StateControl,  
                          string set_state = "Default") : base(dependencies)
        {
            TaskName = this.GetType().Name;
            State = WORK.INITIAL;

            string[] splits = set_state.Split('_');

            if(Scope.TaskRGBTest.IsSingleTest == true)
            {
                int select = ApplicationSetting.Get_Int_Recipe<eF_StartForm>((int)eF_StartForm.Cmbx_PartTest);
                
                if (select == (int)ePartTestItem.IV_Test_HCM)
                    OnlyHeighMode = true;
                else if (select == (int)ePartTestItem.IV_Test_LCM)
                    OnlyLowMode = true;
            }

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
        private string Type;
        private string TestSide;                        //流程測試方向(Left/Right)
        private string TestColor;                       //流程測試顏色(R/G/B)
        private Queue<int> qDAC_L = new Queue<int>();
        private Queue<int> qDAC_H = new Queue<int>();
        private int TotalState_L = 0, TotalState_H = 0; //計算進度用
        private int StartClampingCount = 0;             //記錄開始Clamping後的計數
        private int Current_DAC = 0;                    //目前測試DAC
        private int DAC_Start = 500, DAC_End_HCM = 600, DAC_End_LCM = 600, DAC_Step = 10;
        //[Left DAC Setting]
        private int Left_R_DAC_Start = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_R_DAC_Start);
        private int Left_G_DAC_Start = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_G_DAC_Start);
        private int Left_B_DAC_Start = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_B_DAC_Start);
        private int Left_R_DAC_Step = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_R_DAC_Step);
        private int Left_G_DAC_Step = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_G_DAC_Step);
        private int Left_B_DAC_Step = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_B_DAC_Step);
        private int Left_R_HCM_DAC_End = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_R_HCM_DAC_End);
        private int Left_G_HCM_DAC_End = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_G_HCM_DAC_End);
        private int Left_B_HCM_DAC_End = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_B_HCM_DAC_End);
        private int Left_R_LCM_DAC_End = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_R_LCM_DAC_End);
        private int Left_G_LCM_DAC_End = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_G_LCM_DAC_End);
        private int Left_B_LCM_DAC_End = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_B_LCM_DAC_End);
        //[Right DAC Setting]
        private int Right_R_DAC_Start = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_R_DAC_Start);
        private int Right_G_DAC_Start = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_G_DAC_Start);
        private int Right_B_DAC_Start = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_B_DAC_Start);
        private int Right_R_DAC_Step = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_R_DAC_Step);
        private int Right_G_DAC_Step = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_G_DAC_Step);
        private int Right_B_DAC_Step = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_B_DAC_Step);
        private int Right_R_HCM_DAC_End = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_R_HCM_DAC_End);
        private int Right_G_HCM_DAC_End = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_G_HCM_DAC_End);
        private int Right_B_HCM_DAC_End = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_B_HCM_DAC_End);
        private int Right_R_LCM_DAC_End = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_R_LCM_DAC_End);
        private int Right_G_LCM_DAC_End = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_G_LCM_DAC_End);
        private int Right_B_LCM_DAC_End = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_B_LCM_DAC_End);
        private int LeftRepeatTime = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_AvgCount);
        private int RightRepeatTime = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_AvgCount);
        private int RepeatTime = 1;                     //取樣平均次數
        private int Period_DAQ_Count = 0;               //一個週期內DAQ取樣次數
        private double LED_Duty = 1;                    //LED Duty(硬體)
        private byte Side;                              //LED Board通訊指令(硬體)_Side
        private byte Color;                             //LED Board通訊指令(硬體)_Color
        private long CycleTime;                         //每筆DAC花費時間
        private bool OnlyHeighMode = false;             //只跑Heigh Current Mode
        private bool OnlyLowMode = false;               //只跑Low Current Mode
        private bool IsClamping = false;                //是否發生Clamping
        private IF_BaseTask SubTask;                    
        private IF_StateControl F_StateControl;
        private IF_StatusBox StatusBox;
        private IF_ProgressBar ProgressBar;
        private IWriteFile ResultData;
        private EIOName DAQ_Vf, DAQ_Iin_LCM, DAQ_Iin_HCM;
        private EIOName DAQ_Vin, DAQ_ILED, DAQ_VLED;
        private RGBTesterData TesterData_H = new RGBTesterData();
        private RGBTesterData TesterData_L = new RGBTesterData();
        private LinearCurveFitting LinearCurveFitting_L;
        private LinearCurveFitting LinearCurveFitting_H;
        RGBTesterFunction RGBfunc;

        public enum WORK
        {
            NONE,
            INITIAL,

            SET_DAC_LOW,
            GET_ADC_LOW,
            CALCULATE_LOW,
            RESET_LED_BOARD_LOW,

            SET_DAC_HIGH,
            GET_ADC_HIGH,
            CALCULATE_HIGH,
            RESET_LED_BOARD,

            WRITE_TEST_DATA,

            SUCCESS,
            END,
            FAIL,

            PAUSE,
            ABORT,
            CONTINUE,
        }
        #endregion

        #region private function
        private void Preset()
        {
            StatusBox = Deps.ServiceProvider.GetRequiredService<IF_StatusBox>();
            ProgressBar = Deps.ServiceProvider.GetRequiredService<IF_ProgressBar>();
            RGBfunc = Deps.ServiceProvider.GetRequiredService<RGBTesterFunction>();
            ResultData = Deps.ServiceProvider.GetRequiredService<IWriteFile>();

            Period_DAQ_Count = RGBfunc.HardwareParam.Period_DAQ_Count * 3;   //抓三個週期的資料

            string[] res = Type.Split('_');

            TestSide = res[0];
            TestColor = res[1];

            bool isLeft = (TestSide == "Left");
            Side = isLeft ? Deps.LightEngine.LED_LeftSide : Deps.LightEngine.LED_RightSide;
            RepeatTime = isLeft ? LeftRepeatTime : RightRepeatTime;
            DAQ_Vin = isLeft ? EIOName.Left_Vin : EIOName.Right_Vin;
            DAQ_ILED = isLeft ? EIOName.Left_ILED : EIOName.Right_ILED;
            DAQ_VLED = isLeft ? EIOName.Left_VLED : EIOName.Right_VLED;
            DAQ_Iin_HCM = isLeft ? EIOName.Left_Iin_HCM : EIOName.Right_Iin_HCM;
            DAQ_Iin_LCM = isLeft ? EIOName.Left_Iin_LCM : EIOName.Right_Iin_LCM;

            if (TestColor == "R")
            {
                Color = Deps.LightEngine.LED_R;
                LED_Duty = RGBfunc.HardwareParam.LED_R_Duty;
                DAQ_Vf = isLeft ? EIOName.Left_VLED_R : EIOName.Right_VLED_R;

                DAC_Start = isLeft ? Left_R_DAC_Start : Right_R_DAC_Start;
                DAC_Step = isLeft ? Left_R_DAC_Step : Right_R_DAC_Step;
                DAC_End_HCM = isLeft ? Left_R_HCM_DAC_End : Right_R_HCM_DAC_End;
                DAC_End_LCM = isLeft ? Left_R_LCM_DAC_End : Right_R_LCM_DAC_End;
            }
            else if(TestColor == "G")
            {
                Color = Deps.LightEngine.LED_G;
                LED_Duty = RGBfunc.HardwareParam.LED_G_Duty;
                DAQ_Vf = isLeft ? EIOName.Left_VLED_G : EIOName.Right_VLED_G;

                DAC_Start = isLeft ? Left_G_DAC_Start : Right_G_DAC_Start;
                DAC_Step = isLeft ? Left_G_DAC_Step : Right_G_DAC_Step;
                DAC_End_HCM = isLeft ? Left_G_HCM_DAC_End : Right_G_HCM_DAC_End;
                DAC_End_LCM = isLeft ? Left_G_LCM_DAC_End : Right_G_LCM_DAC_End;
            }
            else if(TestColor == "B")
            {
                Color = Deps.LightEngine.LED_B;
                LED_Duty = RGBfunc.HardwareParam.LED_B_Duty;
                DAQ_Vf = isLeft ? EIOName.Left_VLED_B : EIOName.Right_VLED_B;

                DAC_Start = isLeft ? Left_B_DAC_Start : Right_B_DAC_Start;
                DAC_Step = isLeft ? Left_B_DAC_Step : Right_B_DAC_Step;
                DAC_End_HCM = isLeft ? Left_B_HCM_DAC_End : Right_B_HCM_DAC_End;
                DAC_End_LCM = isLeft ? Left_B_LCM_DAC_End : Right_B_LCM_DAC_End;
            }

            for (int i = DAC_Start; i <= DAC_End_HCM; i += DAC_Step)
                qDAC_H.Enqueue(i);

            for (int i = DAC_Start; i <= DAC_End_LCM; i += DAC_Step)
                qDAC_L.Enqueue(i);

            TotalState_H = qDAC_H.Count;
            TotalState_L = qDAC_L.Count;
            TesterData_H.CurrentMode = "HCM";
            TesterData_L.CurrentMode = "LCM";

            double R_LCM = ApplicationSetting.Get_Double_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Rfb_LCM);
            double R_HCM = ApplicationSetting.Get_Double_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Rfb_HCM);
            double MaxI_LCM = ApplicationSetting.Get_Double_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_LCM_MaxCurrent);
            double MaxI_HCM = ApplicationSetting.Get_Double_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_HCM_MaxCurrent);
            RGBfunc.Set_LED_Rigester();
            RGBfunc.SetRfb(R_LCM, R_HCM);
            RGBfunc.SetMaxCurrent(MaxI_LCM, MaxI_HCM);
        }

        private RGBTesterFunction.AvgData PeriodAvgValueCalculate(string current_mode)
        {
            double[] Vin = new double[Period_DAQ_Count];
            double[] Iin = new double[Period_DAQ_Count];
            double[] Vled = new double[Period_DAQ_Count];
            double[] Vf = new double[Period_DAQ_Count];
            double[] Iled = new double[Period_DAQ_Count];

            for (int i = 0; i < Period_DAQ_Count; i++)
            {
                //一次取5個通道有增加或減少的話會影響Period_DAQ_Count
                Vin[i] = Deps.DIOL.GetAInputStatus(DAQ_Vin);

                if(current_mode == "LCM")
                    Iin[i] = (Deps.DIOL.GetAInputStatus(DAQ_Iin_LCM) - RGBfunc.HardwareParam.CurrentMeasureBias);
                else
                    Iin[i] = (Deps.DIOL.GetAInputStatus(DAQ_Iin_HCM) - RGBfunc.HardwareParam.CurrentMeasureBias);

                Vled[i] = Deps.DIOL.GetAInputStatus(DAQ_VLED);
                Vf[i] = Vled[i] - Deps.DIOL.GetAInputStatus(DAQ_Vf);
                Iled[i] = (Deps.DIOL.GetAInputStatus(DAQ_ILED) - RGBfunc.HardwareParam.CurrentMeasureBias);
            }

            double threshold = 0.95;
            DataFilter dataFilter = new DataFilter();
            RGBTesterFunction.AvgData avgData = new RGBTesterFunction.AvgData();
            avgData.Avg_Vin = Vin.Average();//dataFilter.GetPreciseHighLevel(Vin.ToList(), threshold, 0.005);
            avgData.Avg_Iin = Iin.Average();//dataFilter.GetPreciseHighLevel(Iin.ToList(), threshold,0.005);
            avgData.Avg_Vled = 0.0;//目前用不到 dataFilter.GetPreciseHighLevel(Vled.ToList(), threshold);
            avgData.Avg_Vf = dataFilter.GetPreciseHighLevel(Vf.ToList(), threshold,0.005);
            avgData.Avg_Iled = dataFilter.GetPreciseHighLevel(Iled.ToList(), threshold,0.005);

            return avgData;
        }
        
        protected override void Transition(WORK target)
        {
            if (target != State && !(target == WORK.GET_ADC_LOW || target == WORK.SET_DAC_LOW || 
                                     target == WORK.GET_ADC_HIGH || target == WORK.SET_DAC_HIGH)) //狀態有變化時紀錄
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
        private void UpdateProgressBar(Queue<int> state, int total_state)
        {
            if(state.Count % 10 == 0)   //每幾步更新一次UI
            {
                double res = (double)state.Count / (double)total_state * 100;
                int progress = 100 - (int)res;
                ProgressBar.UpateProgress(progress);
            }
        }
        private void Write_LCM_Result(int index)
        {
            double milli = 1000;
            Deps.File.WriteTestResult(TesterData_L, index, Type);
            CheckIledResult(TesterData_L.Iled[index] * milli, "LCM");
        }
        private void Write_NonData_Result()
        {
            RGBTesterData non_data = new RGBTesterData();
            non_data = Deps.File.SetNonData(non_data);
            Deps.File.WriteTestResult(non_data, 0, Type);
        }
        private void Write_HCM_Result(int index)
        {
            double milli = 1000;

            //double Vfb = TesterData_H.Iled[index] * RGBfunc.HardwareParam.Rfb_HCM * RGBfunc.HardwareParam.LED_SigMag;

            //Deps.File.WriteTestResult(TesterData_H.DACpoint[index], TesterData_H.Vin[index], TesterData_H.Iin[index] * milli,
            //                                                TesterData_H.Pin[index] * milli, TesterData_H.Vf[index], Vfb, TesterData_H.Iled[index] * milli,
            //                                                TesterData_H.Pled[index] * milli, TesterData_H.Eff[index] * 100, TesterData_H.Temperature[index],
            //                                                LinearCurveFitting_H.mDAC, LinearCurveFitting_H.mCurrent,
            //                                                LinearCurveFitting_H.Slope, LinearCurveFitting_H.Offset, Type);

            Deps.File.WriteTestResult(TesterData_H, index, Type);

            CheckIledResult(TesterData_H.Iled[index] * milli, "HCM");
        }
        private bool CheckContact(double measure_current)
        {
            int type = ApplicationSetting.Get_Int_Recipe<eF_StartForm>((int)eF_StartForm.Cmbx_ProductType);
            eLEAType select_type = eLEAType.VIRTUAL;

            if (type == (int)eLEAType.VIRTUAL)
                return true;

            double current;
            if (Current_DAC >= 200 && Current_DAC <= 300 && TestColor == "R")
            {
                current = RGBfunc.HardwareParam.LCM_MaxCurrent * Current_DAC / 1023 / 1000;  //A

                if ((measure_current - current) / current * 100 < -15)
                {
                    Tool.SaveLogToFile($"Contact Fail,DAC:{Current_DAC},Iled:{measure_current*1000}mA,Target I:{current*1000}mA", level: "ERR");
                    return false;
                }
            }

            return true;
        }
        private void CheckTestResult(double res, string mode)
        {
            double current;

            double HCM_LL = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_HCM_Slope_LL);
            double HCM_UL = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_HCM_Slope_UL);
            double LCM_LL = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_LCM_Slope_LL);
            double LCM_UL = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_LCM_Slope_UL);

            if (mode == "HCM")
            {
                if(res > HCM_UL || res < HCM_LL)
                {
                    Scope.TestFail = true;
                    Tool.SaveLogToFile($"Slope = {res:F3},斜率超出上下限範圍:{HCM_LL:F3}~{HCM_UL:F3}", level: "WRN");
                    RGBfunc.FailReasonFlag.IsSlopeErr = true;
                }
            }
            else
            {
                if (res > LCM_UL || res < LCM_LL)
                {
                    Scope.TestFail = true;
                    Tool.SaveLogToFile($"Slope = {res:F3},斜率超出上下限範圍:{LCM_LL:F3}~{LCM_UL:F3}", level: "WRN");
                    RGBfunc.FailReasonFlag.IsSlopeErr = true;
                }
            }
        }
        private void CheckIledResult(double res, string mode)
        {
            //double current;

            //if (mode == "HCM")
            //{
            //    current = RGBfunc.HardwareParam.HCM_MaxCurrent;

            //    if (res > current+50)
            //        Scope.TestFail = true;
            //}
            //else
            //{
            //    current = RGBfunc.HardwareParam.LCM_MaxCurrent;

            //    if (res > current + 5)
            //        Scope.TestFail= true;
            //}
        }
        private void CheckClamping(double Vled, List<int> DAC, List<double> ILed, string CM)
        {
            if (ILed.Count < 50 || IsClamping)
            {
                if (ILed.Count < 50) 
                    IsClamping = false;
                
                return;
            }

            for(int i=1; i< ILed.Count; i++)
            {
                if((ILed[i] - ILed[i-1])*1000 < -100)
                {
                    int startIdx = i;
                    int startClampingDAC = DAC[startIdx];
                    int failLimit = ApplicationSetting.Get_Int_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_ClampingFailDAC);

                    if (startClampingDAC < failLimit)
                    {
                        Scope.TestFail = true;
                        RGBfunc.FailReasonFlag.IsClampingErr = true;
                    }

                    StartClampingCount = startIdx;
                    IsClamping = true;
                    Tool.SaveLogToFile($"Clamping Detected! Start DAC:{startClampingDAC}", level: "WRN");
                }
            }

            //[移動平均+微分]
            //double CalculateDerivative(int backShift, int takeCount = 10)
            //{
            //    var segmentILed = ILed.Skip(ILed.Count - backShift).Take(takeCount).Select(x => x * 1000).ToArray();
            //    var segmentDAC = DAC.Skip(DAC.Count - backShift).Take(takeCount).ToArray();

            //    double sum_Iled = segmentILed.Sum();
            //    double sum_DAC = segmentDAC.Sum();

            //    return sum_Iled / sum_DAC;
            //}

            //double f1 = CalculateDerivative(10);
            //double slope_low = CM == "HCM" ? ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_HCM_Slope_LL) : 
            //                                 ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_LCM_Slope_LL);
            //double slope_up = CM == "HCM" ? ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_HCM_Slope_UL) :
            //                                ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_LCM_Slope_UL);
            //double slope_limit = (slope_up + slope_low) / 2;
            //bool isSlopeFlat = f1 < slope_limit/2;  //斜率過小代表有可能發生Clamping

            //if (Vled < 3 && isSlopeFlat)
            //{
            //    int startIdx = DAC.Count - 7;   //因為有移動平均會Shift
            //    int startClampingDAC = DAC[startIdx];
            //    int failLimit = ApplicationSetting.Get_Int_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_ClampingFailDAC);

            //    if (startClampingDAC < failLimit)
            //    {
            //        Scope.TestFail = true;
            //        RGBfunc.FailReasonFlag.IsClampingErr = true;
            //    }

            //    StartClampingCount = startIdx;
            //    IsClamping = true;
            //    Tool.SaveLogToFile($"Clamping Detected! Start DAC:{startClampingDAC}", level: "WRN");
            //}
        }
        private void CheckTestTemperature(double temperature)
        {
            double limit = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_FailOverTemp);
            if (temperature > limit)
            {
                Scope.TestFail = true;
                RGBfunc.FailReasonFlag.IsTemperatureErr = true;
                Tool.SaveLogToFile($"Temperature = {temperature}°C,溫度過高", level: "WRN");
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

                        if(OnlyHeighMode)
                        {
                            if(!Deps.LightEngine.SetLed_CurrentMode("HCM"))
                            {
                                StatusBox.ShowMessage("HDMI Board Set HCM Fail");
                                Transition(WORK.ABORT);
                            }
                            else
                                Transition(WORK.SET_DAC_HIGH);

                            Tool.SaveLogToFile($"[Task]({TaskName})" + WORK.SET_DAC_HIGH.ToString());
                        }
                        else
                        {
                            if (!Deps.LightEngine.SetLed_CurrentMode("LCM"))
                            {
                                StatusBox.ShowMessage("HDMI Board Set LCM Fail");
                                Transition(WORK.ABORT);
                            }
                            else
                                Transition(WORK.SET_DAC_LOW);

                            Tool.SaveLogToFile($"[Task]({TaskName})" + WORK.SET_DAC_LOW.ToString());
                        }
                    }
                    break;

                #region Low
                case WORK.SET_DAC_LOW:
                    {
                        UpdateProgressBar(qDAC_L, TotalState_L);

                        Tool.ResetTimeCount(out CycleTime);

                        //傳送廣達指令
                        int val = qDAC_L.Dequeue();
                        Current_DAC = val;
                        TesterData_L.DACpoint.Add(val);

                        if (!Deps.LightEngine.SetLed_DAC(Color, Side, val))
                        {
                            StatusBox.ShowMessage("HDMI Board Cmd Fail");
                            Transition(WORK.ABORT);
                        }
                        else
                            Transition(WORK.GET_ADC_LOW);
                    }
                    break;
                case WORK.GET_ADC_LOW:
                    {
                        double sum_Vin = 0, sum_Iin = 0, sum_Vled = 0, sum_Vrgb = 0, sum_Iled = 0;

                        //取得溫度
                        double temperature = double.Parse(Deps.LightEngine.GetTemperature());
                        CheckTestTemperature(temperature);
                        TesterData_L.Temperature.Add(temperature);

                        RGBTesterFunction.AvgData LowAvgData = new RGBTesterFunction.AvgData();

                        //取得AI訊號
                        for (int i=0; i<RepeatTime; i++)
                        {
                            LowAvgData = PeriodAvgValueCalculate("LCM");

                            sum_Vin += LowAvgData.Avg_Vin;
                            sum_Iin += LowAvgData.Avg_Iin;
                            sum_Vled += LowAvgData.Avg_Vled;
                            sum_Vrgb += LowAvgData.Avg_Vf;
                            sum_Iled += LowAvgData.Avg_Iled;
                        }

                        TesterData_L.CycleTime.Add(Tool.GetTime(CycleTime, "us"));

                        TesterData_L.Vin.Add(sum_Vin / RepeatTime);
                        TesterData_L.Iin.Add(sum_Iin / RepeatTime / RGBfunc.HardwareParam.Rin / RGBfunc.HardwareParam.L_SigMag);
                        TesterData_L.Vled.Add(sum_Vled / RepeatTime);
                        TesterData_L.Vf.Add(sum_Vrgb / RepeatTime); //(sum_Vled - sum_Vrgb)
                        TesterData_L.Iled.Add(sum_Iled / RepeatTime / RGBfunc.HardwareParam.Rfb_LCM / RGBfunc.HardwareParam.LED_SigMag);

                        int count = TesterData_L.Iled.Count;
                        if (!CheckContact(TesterData_L.Iled[count - 1] - TesterData_L.Iled[0]))
                        {
                            StatusBox.ShowMessage("Contact Fail Please Check!");
                            Transition(WORK.ABORT);
                            break;
                        }

                        //Transition(WORK.CALCULATE_LOW);
                        State = WORK.CALCULATE_LOW;
                        goto case WORK.CALCULATE_LOW;
                    }
                    //break;
                case WORK.CALCULATE_LOW:
                    {
                        if (qDAC_L.Count == 0)
                        {
                            CheckClamping(TesterData_L.Vled.Last(), TesterData_L.DACpoint, TesterData_L.Iled, "LCM");

                            for (int i = 0; i < TesterData_L.Vin.Count; i++)
                            {
                                TesterData_L.Pin.Add(TesterData_L.Vin[i] * TesterData_L.Iin[i]);
                                TesterData_L.Pled.Add(TesterData_L.Vf[i] * TesterData_L.Iled[i] * LED_Duty);

                                if (TesterData_L.Pin[i] == 0)
                                    TesterData_L.Eff.Add(-99);
                                else
                                    TesterData_L.Eff.Add(TesterData_L.Pled[i] / TesterData_L.Pin[i]);
                            }

                            if(IsClamping == true)
                            {
                                //因為Clamping的關係會有幾筆異常資料，所以斷點前面再往前移動5筆，確保線性擬合的資料都是正常的
                                LinearCurveFitting_L = new LinearCurveFitting(TesterData_L.DACpoint.Take(StartClampingCount-5).ToArray(),
                                                                              TesterData_L.Iled.Take(StartClampingCount-5).Select(x => x * 1000).ToArray());
                            }
                            else
                            {
                                LinearCurveFitting_L = new LinearCurveFitting(TesterData_L.DACpoint.ToArray(),
                                                                              TesterData_L.Iled.Select(x => x * 1000).ToArray());
                            }

                            //畫面顯示測試結果
                            var IF_StFm = Deps.ServiceProvider.GetRequiredService<IF_StartForm>();
                            if(IsClamping == false)
                                IF_StFm.ShowSlopeOffsetResult(TestSide, TestColor, "LCM", LinearCurveFitting_L.Slope, LinearCurveFitting_L.Offset, false);
                            else
                                IF_StFm.ShowSlopeOffsetResult(TestSide, TestColor, "LCM", LinearCurveFitting_L.Slope, LinearCurveFitting_L.Offset, true);
                            
                            //確認斜率,Offset推算結果
                            RGBfunc.SlopeOffsetResult.SetResult(TestColor, "LCM", LinearCurveFitting_L.Slope, LinearCurveFitting_L.Offset);
                            
                            //測試結果報表
                            Deps.File.SetCalibrationData(TestColor, "LCM", LinearCurveFitting_L.Slope, LinearCurveFitting_L.Offset);
                            TesterData_L.DAC_Avg = LinearCurveFitting_L.mDAC;
                            TesterData_L.Current_Avg = LinearCurveFitting_L.mCurrent;
                            TesterData_L.Slope = LinearCurveFitting_L.Slope;
                            TesterData_L.Offset = LinearCurveFitting_L.Offset;

                            CheckTestResult(LinearCurveFitting_L.Slope, "LCM");

                            double[] current = new double[ResultData.CheckSlope.Check_LCM_DAC.Length];
                            for (int i=0; i< ResultData.CheckSlope.Check_LCM_DAC.Length; i++)
                            {
                                int index = TesterData_L.DACpoint.IndexOf(ResultData.CheckSlope.Check_LCM_DAC[i]);

                                if (index == -1)
                                {
                                    Tool.SaveLogToFile($"Check DAC {ResultData.CheckSlope.Check_LCM_DAC[i]} Not Found!", level: "WRN");
                                    current[i] = 0;
                                }
                                else
                                {
                                    current[i] = TesterData_L.Iled[index] * 1000;
                                }
                            }
                            ResultData.CheckSlope.SetCurrentData(TestColor, "LCM", current, LinearCurveFitting_L.Slope, LinearCurveFitting_L.Offset);

                            Tool.SaveLogToFile($"[Task]({TaskName})" + WORK.GET_ADC_LOW.ToString());
                            Tool.SaveLogToFile($"[Task]({TaskName})" + WORK.CALCULATE_LOW.ToString());
                            Tool.SaveLogToFile($"[Task]({TaskName})" + WORK.SET_DAC_HIGH.ToString());

                            if (!Deps.LightEngine.SetLed_DAC(Color, Side, 0))
                                StatusBox.ShowMessage("HCM Set DAC 0 Fail");

                            Transition(WORK.RESET_LED_BOARD_LOW);
                        }
                        else
                        {
                            //CheckClamping(TesterData_L.Vled.Last(), TesterData_L.DACpoint, TesterData_L.Iled, "LCM");

                            //Transition(WORK.SET_DAC_LOW);
                            State = WORK.SET_DAC_LOW;
                            goto case WORK.SET_DAC_LOW;
                        }
                    }
                    break;
                case WORK.RESET_LED_BOARD_LOW:
                    {
                        if (!Deps.LightEngine.ResetLED())
                        {
                            StatusBox.ShowMessage("Reset LED Fail");
                            Transition(WORK.ABORT);
                            break;
                        }

                        if (OnlyLowMode)
                            Transition(WORK.WRITE_TEST_DATA);
                        else
                        {
                            if (!Deps.LightEngine.SetLed_CurrentMode("HCM"))
                            {
                                StatusBox.ShowMessage("HDMI Board Set HCM Fail");
                                Transition(WORK.ABORT);
                            }
                            else
                                Transition(WORK.SET_DAC_HIGH);
                        }
                    }
                    break;
                #endregion
                #region High
                case WORK.SET_DAC_HIGH:
                    {
                        UpdateProgressBar(qDAC_H, TotalState_H);

                        int val = qDAC_H.Dequeue();
                        Current_DAC = val;
                        TesterData_H.DACpoint.Add(val);

                        Tool.ResetTimeCount(out CycleTime);

                        //傳送廣達指令
                        if (!Deps.LightEngine.SetLed_DAC(Color, Side, val))
                        {
                            StatusBox.ShowMessage("HDMI Board Cmd Fail");
                            Transition(WORK.ABORT);
                        }
                        else
                            Transition(WORK.GET_ADC_HIGH);
                    }
                    break;
                case WORK.GET_ADC_HIGH:
                    {
                        double sum_Vin = 0, sum_Iin = 0, sum_Vled = 0, sum_Vrgb = 0, sum_Iled = 0;

                        //取得溫度
                        double temperature = double.Parse(Deps.LightEngine.GetTemperature());
                        CheckTestTemperature(temperature);
                        TesterData_H.Temperature.Add(temperature);

                        RGBTesterFunction.AvgData HighAvgData = new RGBTesterFunction.AvgData();
                        //取得AI訊號
                        for (int i = 0; i < RepeatTime; i++)
                        {
                            HighAvgData = PeriodAvgValueCalculate("HCM");

                            sum_Vin += HighAvgData.Avg_Vin;
                            sum_Iin += HighAvgData.Avg_Iin;
                            sum_Vled += HighAvgData.Avg_Vled;
                            sum_Vrgb += HighAvgData.Avg_Vf;
                            sum_Iled += HighAvgData.Avg_Iled;
                        }

                        TesterData_H.CycleTime.Add(Tool.GetTime(CycleTime, "us"));

                        TesterData_H.Vin.Add(sum_Vin / RepeatTime);
                        TesterData_H.Iin.Add(sum_Iin / RepeatTime / RGBfunc.HardwareParam.Rin / RGBfunc.HardwareParam.H_SigMag);
                        TesterData_H.Vled.Add(sum_Vled / RepeatTime);
                        TesterData_H.Vf.Add(sum_Vrgb / RepeatTime);
                        TesterData_H.Iled.Add(sum_Iled / RepeatTime / RGBfunc.HardwareParam.Rfb_HCM / RGBfunc.HardwareParam.LED_SigMag);

                        //Transition(WORK.CALCULATE_HIGH);
                        State = WORK.CALCULATE_HIGH;
                        goto case WORK.CALCULATE_HIGH;
                    }
                    //break;
                case WORK.CALCULATE_HIGH:
                    {
                        if (qDAC_H.Count == 0)
                        {
                            CheckClamping(TesterData_H.Vled.Last(), TesterData_H.DACpoint, TesterData_H.Iled, "HCM");

                            for (int i = 0; i < TesterData_H.Vin.Count; i++)
                            {
                                TesterData_H.Pin.Add(TesterData_H.Vin[i] * TesterData_H.Iin[i]);
                                TesterData_H.Pled.Add(TesterData_H.Vf[i] * TesterData_H.Iled[i] * LED_Duty);

                                if (TesterData_H.Pin[i] == 0)
                                    TesterData_H.Eff.Add(-99);
                                else
                                    TesterData_H.Eff.Add(TesterData_H.Pled[i] / TesterData_H.Pin[i]);
                            }

                            if (IsClamping == true)
                            {
                                //因為Clamping的關係會有幾筆異常資料，所以斷點前面再往前移動5筆，確保線性擬合的資料都是正常的
                                LinearCurveFitting_H = new LinearCurveFitting(TesterData_H.DACpoint.Take(StartClampingCount-5).ToArray(),
                                                                              TesterData_H.Iled.Take(StartClampingCount-5).Select(x => x * 1000).ToArray());
                            }
                            else
                            {
                                LinearCurveFitting_H = new LinearCurveFitting(TesterData_H.DACpoint.ToArray(),
                                                                              TesterData_H.Iled.Select(x => x * 1000).ToArray());
                            } 

                            var IF_Ser = Deps.ServiceProvider.GetRequiredService<IF_StartForm>();
                            if (IsClamping == false)
                                IF_Ser.ShowSlopeOffsetResult(TestSide, TestColor, "HCM", LinearCurveFitting_H.Slope, LinearCurveFitting_H.Offset, false);
                            else
                                IF_Ser.ShowSlopeOffsetResult(TestSide, TestColor, "HCM", LinearCurveFitting_H.Slope, LinearCurveFitting_H.Offset, true);
                            RGBfunc.SlopeOffsetResult.SetResult(TestColor, "HCM", LinearCurveFitting_H.Slope, LinearCurveFitting_H.Offset);

                            TesterData_H.DAC_Avg = LinearCurveFitting_H.mDAC;
                            TesterData_H.Current_Avg = LinearCurveFitting_H.mCurrent;
                            TesterData_H.Slope = LinearCurveFitting_H.Slope;
                            TesterData_H.Offset = LinearCurveFitting_H.Offset;

                            Deps.File.SetCalibrationData(TestColor, "HCM", LinearCurveFitting_H.Slope, LinearCurveFitting_H.Offset);
                            CheckTestResult(LinearCurveFitting_H.Slope, "HCM");

                            double[] current = new double[TesterData_H.DACpoint.Count];
                            for (int i = 0; i < ResultData.CheckSlope.Check_HCM_DAC.Length; i++)
                            {
                                int index = TesterData_H.DACpoint.IndexOf(ResultData.CheckSlope.Check_HCM_DAC[i]);

                                if (index == -1)
                                {
                                    Tool.SaveLogToFile($"Check DAC {ResultData.CheckSlope.Check_HCM_DAC[i]} Not Found!", level: "WRN");
                                    current[i] = 0;
                                }
                                else
                                {
                                    current[i] = TesterData_H.Iled[index] * 1000;
                                }
                            }
                            ResultData.CheckSlope.SetCurrentData(TestColor, "HCM", current, LinearCurveFitting_H.Slope, LinearCurveFitting_H.Offset);


                            if (!Deps.LightEngine.SetLed_DAC(Color, Side, 0))
                                StatusBox.ShowMessage("HCM Set DAC 0 Fail");

                            Tool.SaveLogToFile($"[Task]({TaskName})" + WORK.GET_ADC_HIGH.ToString());
                            Tool.SaveLogToFile($"[Task]({TaskName})" + WORK.CALCULATE_HIGH.ToString());
                            Transition(WORK.RESET_LED_BOARD);
                        }
                        else
                        {
                            //CheckClamping(TesterData_H.Vled.Last(), TesterData_H.DACpoint, TesterData_H.Iled, "HCM");

                            State = WORK.SET_DAC_HIGH;
                            goto case WORK.SET_DAC_HIGH;
                        }
                    }
                    break;
                case WORK.RESET_LED_BOARD:
                    {
                        if (!Deps.LightEngine.ResetLED())
                        {
                            StatusBox.ShowMessage("Reset LED Fail");
                            Transition(WORK.ABORT);
                            break;
                        }
                        else
                            Transition(WORK.WRITE_TEST_DATA);
                    }
                    break;
                #endregion
                #region WRITE_TEST_DATA
                case WORK.WRITE_TEST_DATA:
                    {
                        int data_count = Math.Max(TesterData_L.Vin.Count, TesterData_H.Vin.Count);
                        string SN = RGBfunc.SerialNumber;
                        string log_name = "";
                        DateTime now = DateTime.Now;

                        if (TestSide == "Left")
                            log_name = $"Z23A_LEDIV L{SN}_Summary{now.ToString("yyyyMMdd")}()";
                        else if (TestSide == "Right")
                            log_name = $"Z23A_LEDIV R{SN}_Summary{now.ToString("yyyyMMdd")}()";

                        for (int i = 0; i < data_count; i++)
                        {
                            Deps.File.WriteFile($"BFT,{SN},{now.ToString("yyyyMMdd")},{now.ToString("HH: mm:ss")}", Type, false);

                            if(i < TesterData_L.Vin.Count && i < TesterData_H.Vin.Count)
                            {
                                Deps.File.WriteFile($",{TesterData_L.CycleTime[i] + TesterData_H.CycleTime[i]},8888,{log_name},", Type, false);
                                Write_HCM_Result(i);
                                Deps.File.WriteFile(",", Type, false);
                                Write_LCM_Result(i);
                            }
                            else if (i < TesterData_H.Vin.Count)
                            {
                                Deps.File.WriteFile($",{TesterData_H.CycleTime[i]},8888,{log_name},", Type, false);
                                Write_HCM_Result(i);
                                Deps.File.WriteFile(",", Type, false);
                                Write_NonData_Result();
                            }
                            else if (i < TesterData_L.Vin.Count)
                            {
                                Deps.File.WriteFile($",{TesterData_L.CycleTime[i]},8888,{log_name},", Type, false);
                                Write_NonData_Result();
                                Deps.File.WriteFile(",", Type, false);
                                Write_LCM_Result(i);
                            }

                            Deps.File.WriteFile("", Type);
                        }

                        Transition(WORK.SUCCESS);
                    }
                    break;
                #endregion

                case WORK.SUCCESS:
                    {
                        ProgressBar.HideForm();
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
