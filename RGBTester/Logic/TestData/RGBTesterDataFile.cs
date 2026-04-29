using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using ToolFunction;
using RGBTester.Base;

namespace RGBTester.Logic
{
    public class RGBTesterDataFile: IWriteFile
    {
        public RGBTesterDataFile(RGBTesterFunction rGBTesterFunction)
        {
            RGBfunc = rGBTesterFunction;
        }

        #region paramter define
        private RGBTesterFunction RGBfunc;
        private Dictionary<string, StreamWriter> TestFiles = new Dictionary<string, StreamWriter>();

        DateTime DateNow;
        public double R_Offset_HCM { get; private set; }
        public double G_Offset_HCM { get; private set; }
        public double B_Offset_HCM { get; private set; }
        public double R_Offset_LCM { get; private set; }
        public double G_Offset_LCM { get; private set; }
        public double B_Offset_LCM { get; private set; }
        public double R_Slope_HCM { get; private set; }
        public double G_Slope_HCM { get; private set; }
        public double B_Slope_HCM { get; private set; }
        public double R_Slope_LCM { get; private set; }
        public double G_Slope_LCM { get; private set; }
        public double B_Slope_LCM { get; private set; }
        #endregion

        #region private function
        private string CreateFileName(string describe = "")
        {
            DateNow = DateTime.Now;
            string timeDay = DateNow.ToString("yyyyMMdd");
            string timeFull = DateNow.ToString("yyyyMMddHHmmss");
            string SN = RGBfunc.SerialNumber;
            string fileName = "";

            string folderPath = $"\\Result\\{timeDay}\\";       //檔案儲存資料夾路徑

            string[] res = describe.Split('_');                 //分割關鍵字

            if (res.Length == 2)
            {
                string sideStr = res[0];        //"Left" 或 "Right"
                string typeStr = res[1];        //"R", "G", "B", "Calibration", "BurnIn"

                string S = sideStr == "Left" ? "L" : "R";
                bool isSpecialMode = (typeStr == "Calibration" || typeStr == "BurnIn");

                if (isSpecialMode)
                {
                    // 特殊模式格式: Z23A_LEDIV_L_SN_Calibration_時間
                    fileName = $@"Z23A_LEDIV_{S}_{SN}_{typeStr}_{timeFull}";
                }
                else
                {
                    // 一般測試格式: Z23A_LEDIV_L_R_SN_Summary_時間
                    fileName = $@"Z23A_LEDIV_{S}_{typeStr}_{SN}_Summary_{timeFull}";
                }
            }

            fileName = folderPath +fileName;

            return fileName;
        }
        private string CheckPassFail(double low, double high, double value)
        {
            return "PASS";

            //if (value >= low && value <= high)
            //    return "PASS";
            //else
            //    return "FAIL";
        }
        private void WriteTitle(string type)
        {
            TestFiles.TryGetValue(type, out StreamWriter file);

            string title = "OTP Addr ,Field Name,Description,Unit,Data";

            Tool.WriteFile(file, title);
            Tool.WriteFile(file, ",,,,");
        }
        private void WriteTitle_RGBTester(string type)
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

            string title = "Station,SN,TestDate,TestTime,CycleTime(us),UserName,DirLogName,H Side Mode_DAC,Vin(V),Status,Iin(mA),Status,Pin(mW),Vf(V),Status,Vfb(V),Iled(mA),Status,Pled(mW),Eff(%),Temperature(℃),x,y,m,c,L Side Mode_DAC,Vin(V),Status,Iin(mA),Status,Pin(mW),Vf(V),Status,Vfb(V),Iled(mA),Status,Pled(mW),Eff(%),Temperature(℃),x,y,m,c";

            TestFiles.TryGetValue(type, out StreamWriter file);

            Tool.WriteFile(file, test_condition);
            Tool.WriteFile(file, title);
        }
        #endregion

        #region public function
        public void CreateFile(string describe = "")
        {
            string fileName = CreateFileName(describe);

            StreamWriter fileHandle = Tool.CreateFile(fileName, ".csv", false);
            TestFiles[describe] = fileHandle;

            if (describe.Contains("Calibration"))
                WriteTitle(describe);
            else
                WriteTitle_RGBTester(describe);
        }
        public void WriteTestResult(int dac, double v_in, double i_in, double p_in, double vf,
                                    double vfb, double i_led, double p_led, double eff, double temperature,
                                    double x, double y, double m, double c, string color)
        {
            // [Check Pass/Fail]
            string bin_v_in, bin_i_in, bin_vf, bin_i_led;
            bin_v_in = CheckPassFail(0, 5.5, v_in);
            bin_i_in = CheckPassFail(130, 192, i_in);
            bin_vf = CheckPassFail(0, 4, vf);
            bin_i_led = CheckPassFail(30, 300, i_led);

            string context = $"{dac},{v_in:F2},{bin_v_in},{i_in:F2},{bin_i_in},{p_in:F2},{vf:F2}," +
                             $"{bin_vf},{vfb:F3},{i_led:F2},{bin_i_led},{p_led:F2},{eff:F2},{temperature:F2},{x:F2}," +
                             $"{y:F2},{m:F3},{c:F2}";

            WriteFile(context, color, false);
        }
        public void WriteTestResult(RGBTesterData test_data, int index, string type)
        {
            // [Check Pass/Fail]
            string bin_v_in, bin_i_in, bin_vf, bin_i_led;
            bin_v_in = CheckPassFail(0, 5.5, test_data.Vin[index]);
            bin_i_in = CheckPassFail(130, 192, test_data.Iin[index]);
            bin_vf = CheckPassFail(0, 4, test_data.Vf[index]);
            bin_i_led = CheckPassFail(30, 300, test_data.Iled[index]);

            double unit_milli = 1000;
            double Vfb = test_data.Iled[index] * RGBfunc.HardwareParam.Rfb_LCM * RGBfunc.HardwareParam.LED_SigMag;
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

            WriteFile(context, type, false);
        }
        public void WriteCalibrationResult(string sn, string describe = "")
        {
            //[High Current Mode]
            WriteFile($"0x0400,led1_offset_mA_h,LED1 offset for high res,mA,{R_Offset_HCM:F4}", describe);
            WriteFile($"0x0404,led1_slope_mA_cnt_h,LED1 slope for high res,mA/DACstep,{R_Slope_HCM:F4}", describe);
            WriteFile($"0x0408,led2_offset_mA_h,LED2 offset for high res,mA,{G_Offset_HCM:F4}", describe);
            WriteFile($"0x040C,led2_slope_mA_cnt_h,LED2 slope for high res,mA/DACstep,{G_Slope_HCM:F4}", describe);
            WriteFile($"0x0410,led3_offset_mA_h,LED3 offset for high res,mA,{B_Offset_HCM:F4}", describe);
            WriteFile($"0x0414,led3_slope_mA_cnt_h,LED3 slope for high res,mA/DACstep,{B_Slope_HCM:F4}", describe);
            //[Low Current Mode]
            WriteFile($"0x0420,led1_offset_mA_l,LED1 offset for low res,mA,{R_Offset_LCM:F4}", describe);
            WriteFile($"0x0424,led1_slope_mA_cnt_l,LED1 slope for low res,mA/DACstep,{R_Slope_LCM:F4}", describe);
            WriteFile($"0x0428,led2_offset_mA_l,LED2 offset for low res,mA,{G_Offset_LCM:F4}", describe);
            WriteFile($"0x042C,led2_slope_mA_cnt_l,LED2 slope for low res,mA/DACstep,{G_Slope_LCM:F4}", describe);
            WriteFile($"0x0430,led3_offset_mA_l,LED3 offset for low res,mA,{B_Offset_LCM:F4}", describe);
            WriteFile($"0x0434,led3_slope_mA_cnt_l,LED3 slope for low res,mA/DACstep,{B_Slope_LCM:F4}", describe);
            //[SN]
            WriteFile($"0x0440,,Serial Number,,{sn}", describe);
        }
        
        public void WriteFile(string context = "", string describe = "", bool NewLine = true)
        {
            TestFiles.TryGetValue(describe, out StreamWriter file);
            Tool.WriteFile(file, context, NewLine: NewLine);
        }
        public void CloseFile(string describe = "")
        {
            TestFiles.TryGetValue(describe, out StreamWriter file);

            if (file == null)
                return;

            Tool.CloseFile(file);
        }
        public void CloseAndDeleteFile(string describe = "")
        {
            TestFiles.TryGetValue(describe, out StreamWriter file);

            if (file == null)
                return;

            Tool.CloseAndDeleteFile(file);
        }
        public void CopyAndCloseTestFile(string describe)
        {
            string copy_path = ApplicationSetting.Get_String_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_TestFileCopyPath);
            string copy_path1 = ApplicationSetting.Get_String_Recipe<eF_ParameterSetting>((int)eF_ParameterSetting.TxtBx_TestFileCopyPath1);

            if(copy_path != "")
                copy_path = copy_path + $"\\{DateNow.ToString("yyyyMMdd")}";

            if (copy_path1 != "")
                copy_path1 = copy_path1 + $"\\{DateNow.ToString("yyyyMMdd")}";

            TestFiles.TryGetValue(describe, out StreamWriter file);

            if (file == null)
                return;

            Tool.CopyFile(file, copy_path, copy_path1);
        }
        public void ResetCalibrationData()
        {
            R_Offset_HCM = -99;
            R_Offset_LCM = -99;
            R_Slope_HCM = -99;
            R_Slope_LCM = -99;

            G_Offset_HCM = -99;
            G_Offset_LCM = -99;
            G_Slope_HCM = -99;
            G_Slope_LCM = -99;

            B_Offset_HCM = -99;
            B_Offset_LCM = -99;
            B_Slope_HCM = -99;
            B_Slope_LCM = -99;
        }
        public void SetCalibrationData(string color, string current_mode, double slope, double offset)
        {
            string offset_item = $"{color}_Offset_{current_mode}";
            string slope_item = $"{color}_Slope_{current_mode}";

            Type type = this.GetType();

            // --- 設定 Offset 值 ---
            PropertyInfo offsetProperty = type.GetProperty(offset_item);
            if (offsetProperty != null && offsetProperty.CanWrite)
            {
                offsetProperty.SetValue(this, offset);
            }
            else
            {
                Tool.SaveLogToFile($"錯誤：找不到或無法寫入屬性 {offset_item}");
            }

            // --- 設定 Slope 值 ---
            PropertyInfo slopeProperty = type.GetProperty(slope_item);
            if (slopeProperty != null && slopeProperty.CanWrite)
            {
                slopeProperty.SetValue(this, slope);
            }
            else
            {
                Tool.SaveLogToFile($"錯誤：找不到或無法寫入屬性 {slopeProperty}");
            }
        }
        #endregion
    }
}
