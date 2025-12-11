using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
        private StreamWriter FileWriter;
        #endregion

        #region public function
        public void CreateFile(string describe = "")
        {
            DateTime now = DateTime.Now;
            string file_name = "";
            string SN = "";
            string Side = "";
            string Color = "";

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
            else 
                FileWriter = Tool.CreateFile(file_name, ".csv", false);

            if (Side != "")
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
            else
                Tool.WriteFile(FileWriter, context, NewLine: NewLine);
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
        }
        #endregion

        #region private function
        private void WriteTitle(string type)
        {
            StreamWriter file = FileWriter;
            string title = "P24 OTP Addr ,Field Name,Description,Unit,Data";

            Tool.WriteFile(file, title);
            Tool.WriteFile(file, ",,,,");
        }
        private void WriteTitle_RGBTester(string type)
        {
            StreamWriter file = null;
            string low_range = ",,,,,,,,0,,130,,,0,,30,,,,,,,,,,0,,130,,,0,,30,,,,,,,,";
            string high_range = ",,,,,,,,5.5,,192,,,4.0,,300,,,,,,,,,,5.5,,192,,,4.0,,300,,,,,,,,";
            string unit = ",,,,,,,,mV,,mA,,,,,mA,,,,℃,,,,,,,mV,,mA,,,,,mA,,,,℃,,,,";
            string title = "Station,SN,TestDate,TestTime,CycleTime(S),UserName,DirLogName,H Side Mode_DAC,Vin,Status,Iin,Status,Pin,Vf,Status,Iled,Status,Pled,Eff,Temperature,x,y,m,c,L Side Mode_DAC,Vin,Status,Iin,Status,Pin,Vf,Status,Iled,Status,Pled,Eff,Temperature,x,y,m,c";

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

            Tool.WriteFile(file, low_range);
            Tool.WriteFile(file, high_range);
            Tool.WriteFile(file, unit);
            Tool.WriteFile(file, title);
        }

        private string CheckPassFail(double low, double high, double value)
        {
            if (value >= low && value <= high)
                return "PASS";
            else
                return "FAIL";
        }
        #endregion
    }
}
