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
        private StreamWriter RedFile;
        private StreamWriter GreenFile;
        private StreamWriter BlueFile;
        #endregion

        #region public function
        public void CreateFile(string describe = "")
        {
            DateTime now = DateTime.Now;
            string file_name = "";
            string SN = "";

            string[] res = describe.Split('_'); //ex.Right_G

            for(int i=0; i<res.Length; i++)
            {
                if (res[i] == "Left")
                    SN = ApplicationSetting.Get_String_Recipe<eF_StartForm>((int)eF_StartForm.TxtBx_Left_SN);
                else if (res[i] == "Right")
                    SN = ApplicationSetting.Get_String_Recipe<eF_StartForm>((int)eF_StartForm.TxtBx_Right_SN);

                if (res[i] == "R")
                {
                    file_name = $"\\Result\\BFT_Z23A_LEDIV-R{SN}_Summary_{now.ToString("yyyyMMddHHmmss")}";
                    RedFile = Tool.CreateFile(file_name, ".csv", false);
                }
                else if (res[i] == "G")
                {
                    file_name = $"\\Result\\BFT_Z23A_LEDIV-G{SN}_Summary_{now.ToString("yyyyMMddHHmmss")}";
                    RedFile = Tool.CreateFile(file_name, ".csv", false);
                }
                else if (res[i] == "B")
                {
                    file_name = $"\\Result\\BFT_Z23A_LEDIV-B{SN}_Summary_{now.ToString("yyyyMMddHHmmss")}";
                    RedFile = Tool.CreateFile(file_name, ".csv", false);
                }
            }

            WriteTitle(res[1]);
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
            bin_i_led = CheckPassFail(0, 4, i_led);

            //string[] contect = new string[] {dac.ToString(), v_in.ToString(), bin_v_in,
            //                                    i_in.ToString(), bin_i_in, p_in.ToString(),
            //                                    vf.ToString(), bin_vf,i_led.ToString(),
            //                                    bin_i_led, p_led.ToString(), eff.ToString(),
            //                                    temperature.ToString(), x.ToString(), y.ToString(),
            //                                    m.ToString(),c.ToString()};

            string context = $"{dac},{v_in},{bin_v_in},{i_in},{bin_i_in},{p_in},{vf}," +
                             $"{bin_vf},{i_led},{bin_i_led},{p_led},{eff},{temperature},{x}," +
                             $"{y},{m},{c}";

            WriteFile(context, color, false);
        }

        //public void WriteTestResult(string h_dac, double h_v_in, double h_i_in, double h_p_in, double h_vf,
        //                            double h_i_led, double h_p_led, double h_eff, double h_temp,
        //                            double h_x, double h_y, double h_m, double h_c,
        //                            string l_dac, double l_v_in, double l_i_in, double l_p_in, double l_vf,
        //                            double l_i_led, double l_p_led, double l_eff, double l_temp,
        //                            double l_x, double l_y, double l_m, double l_c,
        //                            double cycle_time, string side, string color, string current_mode)
        //{
        //    StreamWriter file;
        //    DateTime now = DateTime.Now;
        //    string SN = "";
        //    string log_name = "";

        //    if(side == "Left")
        //    {
        //        SN = ApplicationSetting.Get_String_Recipe<eF_StartForm>((int)eF_StartForm.TxtBx_Left_SN);
        //        log_name = $"Z23A_LEDIV L{SN}_Summary{now.ToString("yyyyMMdd")}()";
        //    }
        //    else if(side == "Right")
        //    {
        //        SN = ApplicationSetting.Get_String_Recipe<eF_StartForm>((int)eF_StartForm.TxtBx_Right_SN);
        //        log_name = $"Z23A_LEDIV R{SN}_Summary{now.ToString("yyyyMMdd")}()";
        //    }

        //    if (color == "R")
        //        file = RedFile;
        //    else if (color == "G")
        //        file = GreenFile;
        //    else if (color == "B")
        //        file = BlueFile;

        //    // [Check Pass/Fail]
        //    bool bin_h_v_in, bin_h_i_in, bin_h_vf, bin_h_i_led;
        //    bool bin_l_v_in, bin_l_i_in, bin_l_vf, bin_l_i_led;
        //    bin_h_v_in = CheckPassFail(0, 5.5, h_v_in);
        //    bin_h_i_in = CheckPassFail(130, 192, h_i_in);
        //    bin_h_vf = CheckPassFail(0, 4, h_vf);
        //    bin_h_i_led = CheckPassFail(0, 4, h_i_led);

        //    //bin_l_v_in = CheckPassFail(0, 5.5, l_v_in);
        //    //bin_l_i_in = CheckPassFail(130, 192, l_i_in);
        //    //bin_l_vf = CheckPassFail(0, 4, l_vf);
        //    //bin_l_i_led = CheckPassFail(0, 4, l_i_led);




        //    string[] item = new string[] {"BFT" ,SN, now.ToString("yyyyMMdd"), now.ToString("HH:mm:ss"),
        //                                   (cycle_time/1000).ToString(), "88888", log_name, h_dac, h_v_in.ToString(),
        //                                    bin_h_v_in.ToString()};

        //}
        public void WriteFile(string context = "", string describe = "", bool NewLine = true)
        {
            if (describe == "R")
                Tool.WriteFile(RedFile, context);
            else if(describe == "G")
                Tool.WriteFile(GreenFile, context);
            else
                Tool.WriteFile(BlueFile, context);
        }
        public void CloseFile(string describe = "")
        {
            if (describe == "R" && RedFile != null)
                Tool.CloseFile(RedFile);
            else if (describe == "G" && GreenFile != null)
                Tool.CloseFile(GreenFile);
            else if (describe == "B" && BlueFile != null)
                Tool.CloseFile(BlueFile);
        }
        #endregion

        #region private function
        private void WriteTitle(string type)
        {
            StreamWriter file = null;
            string low_range = ",,,,,,,,0,,130,,,0,,0,,,,,,,,,,0,,130,,,0,,0,,,,,,,,";
            string high_range = ",,,,,,,,5.5,,192,,,4.0,,30~300,,,,,,,,,,5.5,,192,,,4.0,,30~300,,,,,,,,";
            string unit = ",,,,,,,,mV,,mA,,,,,mA,,,,℃,,,,,,mV,,mA,,,,,mA,,,,℃,,,,";
            string title = "Station,SN,TestDate,TestTime,CycleTime(S),UserName,DirLogName,H Side Mode_DAC,Vin,Status,Iin,Status,Pin,Vf,Status,Iled,Status,Pled,Eff,Temperature,x,y,m,c,L Side Mode_DAC,Vin,Status,Iin,Status,Pin,Vf,Status,Iled,Status,Pled,Eff,Temperature,x,y,m,c";

            if (type == "R")
                file = RedFile;
            else if (type == "G")
                file = GreenFile;
            else if (type == "B")
                file = BlueFile;

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
