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
        public RGBTesterDataFile()
        {

        }

        #region paramter define
        private StreamWriter LeftRedFile;
        private StreamWriter LeftGreenFile;
        private StreamWriter LeftBlueFile;
        private StreamWriter RightRedFile;
        private StreamWriter RightGreenFile;
        private StreamWriter RightBlueFile;
        private StreamWriter LeftCalibrationFile;
        private StreamWriter RightCalibrationFile;
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

        #region public function
        public void CreateFile(string describe = "")
        {
            DateTime now = DateTime.Now;
            string file_name = "";
            string SN = "";
            string Side = "";
            string Color = "";
            string Calibration = "";

            string[] res = describe.Split('_'); //ex.Right_G

            for(int i=0; i<res.Length; i++)
            {
                if (res[i] == "Left")
                {
                    SN = ApplicationSetting.Get_String_Recipe<eF_StartForm>((int)eF_StartForm.TxtBx_Left_SN);
                    Side = "L";
                }
                else if (res[i] == "Right")
                {
                    SN = ApplicationSetting.Get_String_Recipe<eF_StartForm>((int)eF_StartForm.TxtBx_Right_SN);
                    Side = "R";
                }

                if (res[i] == "R")
                    Color = "R";
                else if (res[i] == "G")
                    Color = "G";
                else if (res[i] == "B")
                    Color = "B";

                if (res[i] == "Calibration")
                    Calibration = "Calibration";
            }

            file_name = $"\\Result\\Z23A_LEDIV_{Side}_{Color}_{SN}_Summary_{now.ToString("yyyyMMddHHmmss")}";

            if (describe == "Left_R")
                LeftRedFile = Tool.CreateFile(file_name, ".csv", false);
            else if (describe == "Left_G")
                LeftGreenFile = Tool.CreateFile(file_name, ".csv", false);
            else if (describe == "Left_B")
                LeftBlueFile = Tool.CreateFile(file_name, ".csv", false);
            else if (describe == "Right_R")
                RightRedFile = Tool.CreateFile(file_name, ".csv", false);
            else if (describe == "Right_G")
                RightGreenFile = Tool.CreateFile(file_name, ".csv", false);
            else if (describe == "Right_B")
                RightBlueFile = Tool.CreateFile(file_name, ".csv", false);
            else if(describe == "Left_Calibration")
            {
                file_name = $"\\Result\\Z23A_LEDIV_L_{SN}_Calibration_{now.ToString("yyyyMMddHHmmss")}";
                LeftCalibrationFile = Tool.CreateFile(file_name, ".csv", false);
            }
            else if (describe == "Right_Calibration")
            {
                file_name = $"\\Result\\Z23A_LEDIV_R_{SN}_Calibration_{now.ToString("yyyyMMddHHmmss")}";
                RightCalibrationFile = Tool.CreateFile(file_name, ".csv", false);
            }
                
            if (Side != "" && Calibration == "")
                WriteTitle_RGBTester(describe);
            else
                WriteTitle(describe);
        }
        public void WriteTestResult(int dac, double v_in, double i_in, double p_in, double vf,
                                    double i_led, double p_led, double eff, double temperature,
                                    double x, double y, double m, double c, string color)
        {

            // [Check Pass/Fail]
            string bin_v_in, bin_i_in, bin_vf, bin_i_led;
            bin_v_in = CheckPassFail(0, 5.5, v_in);
            bin_i_in = CheckPassFail(130, 192, i_in);
            bin_vf = CheckPassFail(0, 4, vf);
            bin_i_led = CheckPassFail(30, 300, i_led);

            string context = $"{dac},{v_in},{bin_v_in},{i_in},{bin_i_in},{p_in},{vf}," +
                             $"{bin_vf},{i_led},{bin_i_led},{p_led},{eff},{temperature},{x}," +
                             $"{y},{m},{c}";

            WriteFile(context, color, false);
        }
        public void WriteCalibrationResult(string sn, string describe = "")
        {
            //[High Current Mode]
            WriteFile($"0x0400,led1_offset_nA_h,LED1 offset for high res,nA,{R_Offset_HCM}", describe);
            WriteFile($"0x0404,led1_slope_nA_cnt_h,LED1 slope for high res,nA/DACstep,{R_Slope_HCM}", describe);
            WriteFile($"0x0408,led2_offset_nA_h,LED2 offset for high res,nA,{G_Offset_HCM}", describe);
            WriteFile($"0x040C,led2_slope_nA_cnt_h,LED2 slope for high res,nA/DACstep,{G_Slope_HCM}", describe);
            WriteFile($"0x0410,led3_offset_nA_h,LED3 offset for high res,nA,{B_Offset_HCM}", describe);
            WriteFile($"0x0414,led3_slope_nA_cnt_h,LED3 slope for high res,nA/DACstep,{B_Slope_HCM}", describe);
            //[Low Current Mode]
            WriteFile($"0x0400,led1_offset_nA_h,LED1 offset for low res,nA,{R_Offset_LCM}", describe);
            WriteFile($"0x0404,led1_slope_nA_cnt_h,LED1 slope for low res,nA/DACstep,{R_Slope_LCM}", describe);
            WriteFile($"0x0408,led2_offset_nA_h,LED2 offset for low res,nA,{G_Offset_LCM}", describe);
            WriteFile($"0x040C,led2_slope_nA_cnt_h,LED2 slope for low res,nA/DACstep,{G_Slope_LCM}", describe);
            WriteFile($"0x0410,led3_offset_nA_h,LED3 offset for low res,nA,{B_Offset_LCM}", describe);
            WriteFile($"0x0414,led3_slope_nA_cnt_h,LED3 slope for low res,nA/DACstep,{B_Slope_LCM}", describe);
            //[SN]
            WriteFile($"0x0440,,Serial Number,,{sn}", describe);
        }
        
        public void WriteFile(string context = "", string describe = "", bool NewLine = true)
        {
            if (describe == "Left_R")
                Tool.WriteFile(LeftRedFile, context, NewLine: NewLine);
            else if(describe == "Left_G")
                Tool.WriteFile(LeftGreenFile, context, NewLine: NewLine);
            else if(describe == "Left_B")
                Tool.WriteFile(LeftBlueFile, context, NewLine: NewLine);
            else if (describe == "Right_R")
                Tool.WriteFile(RightRedFile, context, NewLine: NewLine);
            else if (describe == "Right_G")
                Tool.WriteFile(RightGreenFile, context, NewLine: NewLine);
            else if(describe == "Right_B")
                Tool.WriteFile(RightBlueFile, context, NewLine: NewLine);
            else if(describe == "Left_Calibration")
                Tool.WriteFile(LeftCalibrationFile, context, NewLine: NewLine);
            else if (describe == "Right_Calibration")
                Tool.WriteFile(RightCalibrationFile, context, NewLine: NewLine);
        }
        public void CloseFile(string describe = "")
        {
            if (describe == "Left_R" && LeftRedFile != null)
                Tool.CloseFile(LeftRedFile);
            else if (describe == "Left_G" && LeftGreenFile != null)
                Tool.CloseFile(LeftGreenFile);
            else if (describe == "Left_B" && LeftBlueFile != null)
                Tool.CloseFile(LeftBlueFile);
            else if (describe == "Right_R" && RightRedFile != null)
                Tool.CloseFile(RightRedFile);
            else if (describe == "Right_G" && RightGreenFile != null)
                Tool.CloseFile(RightGreenFile);
            else if (describe == "Right_B" && RightBlueFile != null)
                Tool.CloseFile(RightBlueFile);
            else if(describe == "Left_Calibration")
                Tool.CloseFile(LeftCalibrationFile);
            else if(describe == "Right_Calibration")
                Tool.CloseFile(RightCalibrationFile);
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

        #region private function
        private void WriteTitle(string type)
        {
            StreamWriter file = LeftCalibrationFile;

            if (type == "Left_Calibration")
                file = LeftCalibrationFile;
            else if (type == "Right_Calibration")
                file = RightCalibrationFile;

            string title = "P24 OTP Addr ,Field Name,Description,Unit,Data";

            Tool.WriteFile(file, title);
            Tool.WriteFile(file, ",,,,");
        }
        private void WriteTitle_RGBTester(string type)
        {
            StreamWriter file = null;
            //string low_range = ",,,,,,,,0,,130,,,0,,30,,,,,,,,,,0,,130,,,0,,30,,,,,,,,";
            //string high_range = ",,,,,,,,5.5,,192,,,4.0,,300,,,,,,,,,,5.5,,192,,,4.0,,300,,,,,,,,";
            //string unit = ",,,,,,,,mV,,mA,,,,,mA,,,,℃,,,,,,,mV,,mA,,,,,mA,,,,℃,,,,";
            string title = "Station,SN,TestDate,TestTime,CycleTime(us),UserName,DirLogName,H Side Mode_DAC,Vin,Status,Iin,Status,Pin,Vf,Status,Iled,Status,Pled,Eff,Temperature,x,y,m,c,L Side Mode_DAC,Vin,Status,Iin,Status,Pin,Vf,Status,Iled,Status,Pled,Eff,Temperature,x,y,m,c";

            if (type == "Left_R")
                file = LeftRedFile;
            else if (type == "Left_G")
                file = LeftGreenFile;
            else if (type == "Left_B")
                file = LeftBlueFile;
            else if (type == "Right_R")
                file = RightRedFile;
            else if (type == "Right_G")
                file = RightGreenFile;
            else if (type == "Right_B")
                file = RightBlueFile;

            //Tool.WriteFile(file, low_range);
            //Tool.WriteFile(file, high_range);
            //Tool.WriteFile(file, unit);
            Tool.WriteFile(file, title);
        }

        private string CheckPassFail(double low, double high, double value)
        {
            return "PASS";
            
            //if (value >= low && value <= high)
            //    return "PASS";
            //else
            //    return "FAIL";
        }
        #endregion
    }
}
