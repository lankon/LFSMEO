using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using DeviceCore;
using RGBTester.Base;
using System.Threading;
using System.IO;
using RGBTester.Device;

namespace RGBTester.Logic
{
    public class F_StartFormLogic
    {
        public F_StartFormLogic(IRGBTesterMachine machine, IServiceProvider serviceProvider, RGBTesterFunction rGBTesterFunction)
        {
            Machine = machine;
            ServiceProvider = serviceProvider;
            RGBFunc = rGBTesterFunction;
        }

        #region parameter define
        IRGBTesterMachine Machine;
        IServiceProvider ServiceProvider;
        RGBTesterFunction RGBFunc;
        #endregion

        #region public function
        public string GetTemperature()
        {
            var lea = ServiceProvider.GetRequiredService<IFunction_LightEngine>();
            return lea.GetTemperature();
        }

        public int[] Get_DAC_Value()
        {
            var lea = ServiceProvider.GetRequiredService<IFunction_LightEngine>();
            return lea.Get_DAC();
        }

        public void Set_DAC_Test(int red, int green, int blue)
        {
            var lea = ServiceProvider.GetRequiredService<IFunction_LightEngine>();

            //因為Z23A沒有分左右邊，所以這邊只設定左邊
            if (red != 0)
                lea.SetLed_DAC(lea.LED_R, lea.LED_LeftSide, red);
            else if(green != 0)
                lea.SetLed_DAC(lea.LED_G, lea.LED_LeftSide, green);
            else if(blue != 0)
                lea.SetLed_DAC(lea.LED_B, lea.LED_LeftSide, blue);

            //Machine.DIOL.AddIORule(0, 0, 0, 0, true, (0,0,0,5,false), (0,0,0,7,true));
            //Machine.DIOL.AddIORule(0, 0, 0, 0, false, (0, 0, 0, 5, true), (0, 0, 0, 7, false));

            //Machine.DIOL.AddIORule(EIOName.Vacuum_Pump, true, (EIOName.Left_Iin_HCM, false), (EIOName.Right_Iin_HCM, true));
            //Machine.DIOL.AddIORule(EIOName.Vacuum_Pump, false, (EIOName.Left_Iin_HCM, true), (EIOName.Right_Iin_HCM, false));


            //Machine.DIOL.SetOutputStatus(EIOName.Vacuum_Pump, false);
        }

        public int StartTaskAction(string method = "")
        {
            var RGBFunc = ServiceProvider.GetRequiredService<RGBTesterFunction>();
            var MainTask = ServiceProvider.GetRequiredService<IBaseMainTask>();

            IFunction_LightEngine lea = ServiceProvider.GetRequiredService<IFunction_LightEngine>();
            lea.Open();

            int res = CheckTestCondition();

            if (res < 0)
                return res;
            
            if(method == "Left")
            {
                RGBFunc.SerialNumber = ApplicationSetting.Get_String_Recipe<eF_StartForm>((int)eF_StartForm.TxtBx_Left_SN);
                MainTask.SetTask<TaskRGBTest>("Left");
            }
            else if(method == "Right")
            {
                RGBFunc.SerialNumber = ApplicationSetting.Get_String_Recipe<eF_StartForm>((int)eF_StartForm.TxtBx_Right_SN);
                MainTask.SetTask<TaskRGBTest>("Right");
            }
            //else
            //    MainTask.SetTask<TaskRGBTest>("Both");
            
            MainTask.Run();

            return 0;
        }

        public int ReadVirtual_AI_Data()
        {
            SetVirtual_IO_Data();

            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Setting\\Virtual_AI_Data.csv";

            if (!File.Exists(filePath))
                return -1;

            EIOName[] eIONames = null;
            if (RGBFunc.GetModuleType() == eModuleType.IV_Calibration)
            {
                eIONames = new EIOName[] { EIOName.Left_Iin_HCM, EIOName.Left_Iin_LCM, EIOName.Left_Vin,
                                                 EIOName.Right_VLED, EIOName.Left_ILED, EIOName.Right_Iin_LCM,
                                                 EIOName.Right_Vin, EIOName.Right_ILED, EIOName.Left_VLED,
                                                 EIOName.Left_VLED_R, EIOName.Left_VLED_B, EIOName.Left_VLED_G,
                                                 EIOName.Right_Iin_HCM, EIOName.Right_VLED_R, EIOName.Right_VLED_G,
                                                 EIOName.Right_VLED_B};
            }
            else if(RGBFunc.GetModuleType() == eModuleType.Function_Test)
            {
                eIONames = new EIOName[] { EIOName.Left_6V, EIOName.Left_1V2, EIOName.Left_Vin,
                                            EIOName.Left_VLED_R, EIOName.Left_VLED_G, EIOName.Left_VLED_B,
                                            EIOName.Left_V_R, EIOName.Left_V_G, EIOName.Left_V_B1,
                                            EIOName.Left_V_B2, EIOName.Left_V_FB1, EIOName.Left_V_FB2,};
            }

            string[] lines = File.ReadAllLines(filePath);

            IEnumerable<string> dataLines = lines.Skip(1);

            foreach (string line in dataLines)
            {
                // 如果是空行或無效行，則跳過
                if (string.IsNullOrWhiteSpace(line)) continue;

                // 使用逗號分隔符號分割字串
                string[] values = line.Split(',');

                if (values.Length < 2) continue; // 確保至少有 DAC 索引和一個通道值

                for(int i=1; i<values.Length; i++)
                {
                    if(double.TryParse(values[i], out double d_value))
                        Machine.DIOL.Add_AI_VirtualData(eIONames[i-1], d_value);
                    else
                        Machine.DIOL.Add_AI_VirtualData(eIONames[i-1], 99);
                }
            }

            return 0;
        }
        public void Test()
        {
            var cmd = ServiceProvider.GetRequiredService<ILightEngineCommand>();
            byte[] res1, res2, res3;

            byte[] intput = new byte[1];
            intput[0] = 0x24;

            if(cmd is Z23A_API_Command)
            {
                cmd.Set_RegisterValue(0x02, 1, intput);
                intput[0] = 0xFF;
                cmd.Set_RegisterValue(0x018, 1, intput);
                intput[0] = 0xC0;
                cmd.Set_RegisterValue(0x1E, 1, intput);
            }
        }
        public void SetVirtual_IO_Data()
        {
            //Machine.DIOL.AddIORule(EIOName.Vacuum_Pump, false, (EIOName.SafePos_Sensor, false));
            //Machine.DIOL.AddIORule(EIOName.Vacuum_Pump, true, (EIOName.SafePos_Sensor, true));
        }
        #endregion

        #region private function
        private int CheckTestCondition()
        {
            int res = 0;
            
            //int LeftAvg = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_AvgCount);
            //int LeftStart = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_DAC_Start);
            //int LeftEnd = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_DAC_End);
            //int LeftStep = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_DAC_Step);
            //int RightAvg = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_AvgCount);
            //int RightStart = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_DAC_Start);
            //int RightEnd = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_DAC_End);
            //int RightStep = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_DAC_Step);

            //if (LeftAvg <= 0 || LeftStart < 0 || LeftEnd <= 0 || LeftStep<= 0 ||
            //    RightAvg <= 0 || RightStart < 0 || RightEnd <= 0 || RightStep <= 0)
            //{
            //    Tool.SaveLogToFile("測試條件輸入小於等於零");

            //    var box1 = ServiceProvider.GetRequiredService<IF_StatusBox>();
            //    box1.ShowMessage("Test Condition Setting Fail");
                
            //    res = -1;
            //    return res;
            //}

            //if(((LeftStart - LeftEnd) % LeftStep != 0) || ((RightStart - RightEnd) % RightStep != 0))
            //{
            //    Tool.SaveLogToFile("Step無法整除");
            //    res = -2;
            //}

            if(res < 0)
            {
                var box = ServiceProvider.GetRequiredService<IF_StatusBox>();
                box.ShowMessage("Test Condition Setting Fail");
            }

            Machine.DIOL.Clear_AI_VirtualData();
            ReadVirtual_AI_Data();

            return res;
        }

        #endregion
    }
}
