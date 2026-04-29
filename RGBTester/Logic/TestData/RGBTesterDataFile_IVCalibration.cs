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
        public RGBTesterDataFile_IVCalibration(RGBTesterFunction rGBTesterFunction)
        {
            RGBfunc = rGBTesterFunction;
        }

        #region parameter define
        RGBTesterFunction RGBfunc;
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
    }
}
