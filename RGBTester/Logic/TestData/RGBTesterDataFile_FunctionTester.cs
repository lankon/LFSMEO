using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RGBTester.Base;
using ToolFunction;

namespace RGBTester.Logic._RGBTesterDataFile
{
    public class RGBTesterDataFile_FunctionTester: RGBTesterDataFile_FileType
    {

        public override string GetTitleStr(string describe)
        {
            string title = "";

            if (describe.Contains("Calibration"))
            {
                title = "OTP Addr ,Field Name,Description,Unit,Data" + "\n" + ",,,,";
            }
            else if(describe.Contains("BurnIn"))
            {

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

                title = test_condition + "\n" + "Station,SN,TestDate,TestTime,CycleTime(us),UserName,DirLogName,PP_DISP_6V0(V),PP_DISP_1V2(V),H Side Mode_DAC,Vin(V),Vf(V),Iled1(mA),Pled(mW),Temperature(℃),x,y,m,c,L Side Mode_DAC,Vin(V),Vf(V),Iled1(mA),Pled(mW),Temperature(℃),x,y,m,c";
            }

            return title;
        }
        public override string GetTestReultStr(RGBTesterData test_data, int index)
        {
            double unit_milli = 1000;
            string context = $"{test_data.DISP_6V0[0]:F2}," +       //只測一次
                            $"{test_data.DISP_1V2[0]:F2}," +        //只測一次
                            $"{test_data.DACpoint[index]:F2}," +
                            $"{test_data.Vin[index]:F2}," +
                            $"{test_data.Vf[index]:F2}," +
                            $"{test_data.Iled[index] * unit_milli:F2}," +
                            $"{test_data.Pled[index] * unit_milli:F2}," +
                            $"{test_data.Temperature[index]:F2}," +
                            $"{test_data.DAC_Avg:F2},{test_data.Current_Avg:F2},{test_data.Slope:F3},{test_data.Offset:F2}";

            return context;
        }

    }
}
