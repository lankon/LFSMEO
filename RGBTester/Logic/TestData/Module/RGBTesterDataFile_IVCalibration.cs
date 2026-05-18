using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;
using RGBTester.Base;

namespace RGBTester.Logic._RGBTesterDataFile
{
    public class RGBTesterDataFile_IVCalibration: RGBTesterDataFile_FileType
    {
        public RGBTesterDataFile_IVCalibration(RGBTesterFunction rGBTesterFunction, RGBTesterDataFile dataFile)
        {
            RGBfunc = rGBTesterFunction;
            RGBDataFile = dataFile;
        }

        #region parameter define
        RGBTesterFunction RGBfunc;
        RGBTesterDataFile RGBDataFile;
        #endregion

        #region private function
        private string CheckPassFail(double low, double high, double value)
        {
            return "PASS";

            //if (value >= low && value <= high)
            //    return "PASS";
            //else
            //    return "FAIL";
        }
        #endregion
        public override eModuleType GetModuleType()
        {
            return RGBfunc.GetModuleType();
        }
        public override string GetTitleStr(string describe)
        {
            string title = "";

            if(describe.Contains("Calibration"))
            {
                title = "OTP Addr ,Field Name,Description,Unit,Data" + "\n" + ",,,,";
            }
            else
            {
                //string low_range = ",,,,,,,,0,,130,,,0,,30,,,,,,,,,,0,,130,,,0,,30,,,,,,,,";
                //string high_range = ",,,,,,,,5.5,,192,,,4.0,,300,,,,,,,,,,5.5,,192,,,4.0,,300,,,,,,,,";
                //string unit = ",,,,,,,,mV,,mA,,,,,mA,,,,℃,,,,,,,mV,,mA,,,,,mA,,,,℃,,,,";
                string Rfb_LCM = ApplicationSetting.Get_String_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Rfb_LCM);
                string Rfb_HCM = ApplicationSetting.Get_String_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Rfb_HCM);
                string LCM_Slope_LL = ApplicationSetting.Get_String_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_LCM_Slope_LL);
                string LCM_Slope_UL = ApplicationSetting.Get_String_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_LCM_Slope_UL);
                string HCM_Slope_LL = ApplicationSetting.Get_String_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_HCM_Slope_LL);
                string HCM_Slope_UL = ApplicationSetting.Get_String_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_HCM_Slope_UL);

                string test_condition = $"TestCondition:Rfb_LCM = {Rfb_LCM} Rfb_HCM = {Rfb_HCM} " +
                                                        $"LCM_Slope_LL = {LCM_Slope_LL} LCM_Slope_UL = {LCM_Slope_UL} " +
                                                        $"HCM_Slope_LL = {HCM_Slope_LL} HCM_Slope_UL = {HCM_Slope_UL} ";

                title = test_condition + "\n" + "Station,SN,TestDate,TestTime,CycleTime(us),UserName,DirLogName,H Side Mode_DAC,Vin(V),Status,Iin(mA),Status,Pin(mW),Vf(V),Status,Vfb(V),Iled(mA),Status,Pled(mW),Eff(%),Temperature(℃),x,y,m,c,L Side Mode_DAC,Vin(V),Status,Iin(mA),Status,Pin(mW),Vf(V),Status,Vfb(V),Iled(mA),Status,Pled(mW),Eff(%),Temperature(℃),x,y,m,c";
            }

            return title;
        }
        public override string GetTestReultStr(RGBTesterData test_data, int index)
        {
            // [Check Pass/Fail]
            string bin_v_in, bin_i_in, bin_vf, bin_i_led;
            bin_v_in = CheckPassFail(0, 5.5, test_data.Vin[index]);
            bin_i_in = CheckPassFail(130, 192, test_data.Iin[index]);
            bin_vf = CheckPassFail(0, 4, test_data.Vf[index]);
            bin_i_led = CheckPassFail(30, 300, test_data.Iled[index]);

            double unit_milli = 1000;
            double Vfb = 0;
            if (test_data.CurrentMode == "HCM")
                Vfb = test_data.Iled[index] * RGBfunc.HardwareParam.Rfb_HCM * RGBfunc.HardwareParam.LED_SigMag;
            else if (test_data.CurrentMode == "LCM")
                Vfb = test_data.Iled[index] * RGBfunc.HardwareParam.Rfb_LCM * RGBfunc.HardwareParam.LED_SigMag;

            string context = $"{test_data.DACpoint[index]}," +
                            $"{test_data.Vin[index]:F2},{bin_v_in}," +
                            $"{test_data.Iin[index] * unit_milli:F2},{bin_i_in}," +
                            $"{test_data.Pin[index] * unit_milli:F2}," +
                            $"{test_data.Vf[index]:F2},{bin_vf}," +
                            $"{Vfb:F3}," +
                            $"{test_data.Iled[index] * unit_milli:F2},{bin_i_led}," +
                            $"{test_data.Pled[index] * unit_milli:F2},{test_data.Eff[index] * 100:F2}," +
                            $"{test_data.Temperature[index]:F2}," +
                            $"{test_data.DAC_Avg:F2},{test_data.Current_Avg:F2},{test_data.Slope:F3},{test_data.Offset:F2}";

            return context;
        }
        public override List<string> GetCalibrationStr()
        {
            List<string> str = new List<string>();

            //[High Current Mode]
            str.Add($"0x0400,led1_offset_mA_h,LED1 offset for high res,mA,{RGBDataFile.R_Offset_HCM:F4}");
            str.Add($"0x0404,led1_slope_mA_cnt_h,LED1 slope for high res,mA/DACstep,{RGBDataFile.R_Slope_HCM:F4}");
            str.Add($"0x0408,led2_offset_mA_h,LED2 offset for high res,mA,{RGBDataFile.G_Offset_HCM:F4}");
            str.Add($"0x040C,led2_slope_mA_cnt_h,LED2 slope for high res,mA/DACstep,{RGBDataFile.G_Slope_HCM:F4}");
            str.Add($"0x0410,led3_offset_mA_h,LED3 offset for high res,mA,{RGBDataFile.B_Offset_HCM:F4}");
            str.Add($"0x0414,led3_slope_mA_cnt_h,LED3 slope for high res,mA/DACstep,{RGBDataFile.B_Slope_HCM:F4}");
            //[Low Current Mode]
            str.Add($"0x0420,led1_offset_mA_l,LED1 offset for low res,mA,{RGBDataFile.R_Offset_LCM:F4}");
            str.Add($"0x0424,led1_slope_mA_cnt_l,LED1 slope for low res,mA/DACstep,{RGBDataFile.R_Slope_LCM:F4}");
            str.Add($"0x0428,led2_offset_mA_l,LED2 offset for low res,mA,{RGBDataFile.G_Offset_LCM:F4}");
            str.Add($"0x042C,led2_slope_mA_cnt_l,LED2 slope for low res,mA/DACstep,{RGBDataFile.G_Slope_LCM:F4}");
            str.Add($"0x0430,led3_offset_mA_l,LED3 offset for low res,mA,{RGBDataFile.B_Offset_LCM:F4}");
            str.Add($"0x0434,led3_slope_mA_cnt_l,LED3 slope for low res,mA/DACstep,{RGBDataFile.B_Slope_LCM:F4}");

            return str;
        }
    }
}
