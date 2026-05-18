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
        public RGBTesterDataFile_FunctionTester(RGBTesterFunction rGBTesterFunction, RGBTesterDataFile dataFile)
        {
            RGBDataFile = dataFile;
            RGBFunc = rGBTesterFunction;
        }

        #region parameter define
        RGBTesterFunction RGBFunc;
        RGBTesterDataFile RGBDataFile;
        #endregion
        public override eModuleType GetModuleType()
        {
            return RGBFunc.GetModuleType();
        }
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
            string context =$"{test_data.DACpoint[index]:F2}," +
                            $"{test_data.Vin[index]:F2}," +
                            $"{test_data.Vf[index]:F2}," +
                            $"{test_data.Iled[index] * unit_milli:F2}," +
                            $"{test_data.Pled[index] * unit_milli:F2}," +
                            $"{test_data.Temperature[index]:F2}," +
                            $"{test_data.DAC_Avg:F2},{test_data.Current_Avg:F2},{test_data.Slope:F3},{test_data.Offset:F2}";

            return context;
        }
        public override string GetExtendTestResultStr(RGBTesterData test_data, int index)
        {
            string context = $"{test_data.DISP_6V0[0]:F2}," +       //只測一次
                             $"{test_data.DISP_1V2[0]:F2},";        //只測一次

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
            str.Add($",led4_offset_mA_h,LED4 offset for high res,mA,{RGBDataFile.B2_Offset_HCM:F4}");
            str.Add($",led4_slope_mA_cnt_h,LED4 slope for high res,mA/DACstep,{RGBDataFile.B2_Slope_HCM:F4}");
            //[Low Current Mode]
            str.Add($"0x0420,led1_offset_mA_l,LED1 offset for low res,mA,{RGBDataFile.R_Offset_LCM:F4}");
            str.Add($"0x0424,led1_slope_mA_cnt_l,LED1 slope for low res,mA/DACstep,{RGBDataFile.R_Slope_LCM:F4}");
            str.Add($"0x0428,led2_offset_mA_l,LED2 offset for low res,mA,{RGBDataFile.G_Offset_LCM:F4}");
            str.Add($"0x042C,led2_slope_mA_cnt_l,LED2 slope for low res,mA/DACstep,{RGBDataFile.G_Slope_LCM:F4}");
            str.Add($"0x0430,led3_offset_mA_l,LED3 offset for low res,mA,{RGBDataFile.B_Offset_LCM:F4}");
            str.Add($"0x0434,led3_slope_mA_cnt_l,LED3 slope for low res,mA/DACstep,{RGBDataFile.B_Slope_LCM:F4}");
            str.Add($",led4_offset_mA_l,LED4 offset for low res,mA,{RGBDataFile.B2_Offset_LCM:F4}");
            str.Add($",led4_slope_mA_cnt_l,LED4 slope for low res,mA/DACstep,{RGBDataFile.B2_Slope_LCM:F4}");

            return str;
        }
        public override string GetCheckSlopeStr(int index)
        {
            var cs = RGBDataFile.CheckSlope;

            string str = $"{cs.Check_LCM_DAC[index]},";

            for (int i=0; i< cs.TestMode.Length; i++)
            {
                var data = cs.dicCheckResult[cs.TestMode[i]];
                str = str + $"{data.CalCurrent[index]:F2},{data.Dev[index]:F2},";

                if(i == 3)  //增加測試顏色時需添加
                    str += $"{cs.Check_LCM_DAC[index]},";
            }

            return str;
        }
    }
}
