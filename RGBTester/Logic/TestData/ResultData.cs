using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;

namespace RGBTester.Logic
{
    public class  ResultData
    {
        public CheckSlopeData CheckSlopeData = new CheckSlopeData();

    }

    public class CheckSlopeData
    {
        #region parameter define
        private StreamWriter OutputFile;
        private bool PASS = false;
        private const int CheckCount = 5;
        private double Limit_Dev = 15;

        // [LCM Slope & Offset]
        private double R_LCM_Slope;
        private double R_LCM_Offset;
        private double G_LCM_Slope;
        private double G_LCM_Offset;
        private double B_LCM_Slope;
        private double B_LCM_Offset;

        // [HCM Slope & Offset]
        private double R_HCM_Slope;
        private double R_HCM_Offset;
        private double G_HCM_Slope;
        private double G_HCM_Offset;
        private double B_HCM_Slope;
        private double B_HCM_Offset;

        // [Check DAC]
        public int[] Check_LCM_DAC = new int[CheckCount];
        public int[] Check_HCM_DAC = new int[CheckCount];

        public double[] LCM_R = new double[CheckCount];
        public double[] LCM_R_Calculate = new double[CheckCount];
        public double[] LCM_R_Dev = new double[CheckCount];
        public double[] LCM_G = new double[CheckCount];
        public double[] LCM_G_Calculate = new double[CheckCount];
        public double[] LCM_G_Dev = new double[CheckCount];
        public double[] LCM_B = new double[CheckCount];
        public double[] LCM_B_Calculate = new double[CheckCount];
        public double[] LCM_B_Dev = new double[CheckCount];
        public double[] HCM_R = new double[CheckCount];
        public double[] HCM_R_Calculate = new double[CheckCount];
        public double[] HCM_R_Dev = new double[CheckCount];
        public double[] HCM_G = new double[CheckCount];
        public double[] HCM_G_Calculate = new double[CheckCount];
        public double[] HCM_G_Dev = new double[CheckCount];
        public double[] HCM_B = new double[CheckCount];
        public double[] HCM_B_Calculate = new double[CheckCount];
        public double[] HCM_B_Dev = new double[CheckCount];
        #endregion

        #region private function
        private bool CheckOutOfLimit(int check_dac, double slope, double offset, double actual, ref double dev, ref double calculate_current, string mode)
        {
            double CalculateCurrent = check_dac * slope + offset;
            double devation = CalculateCurrent - actual;
            CalculateCurrent = CalculateCurrent == 0 ? 0.00000001 : CalculateCurrent;
            actual = actual == 0? 0.00000001 : actual;

            dev = devation / actual * 100;
            calculate_current = CalculateCurrent;

            if (Math.Abs(devation) < actual * Limit_Dev / 100)
                return true;
            else
            {
                Tool.SaveLogToFile(string.Format($"Check Slope Calculate Current Out of Limit! {mode} Check DAC: {check_dac}, " +
                                                 $"Slope: {slope:F4}, Offset: {offset:F4}, Actual: {actual:F4}, " +
                                                 $"Calculate: {CalculateCurrent:F4}"), level:"WRN");

                return false;
            }
        }
        #endregion

        #region public function
        public void ResetParameter()
        {
            PASS = false;
            R_LCM_Slope = 0;
            R_LCM_Offset = 0;
            G_LCM_Slope = 0;
            G_LCM_Offset = 0;
            B_LCM_Slope = 0;
            B_LCM_Offset = 0;
            R_HCM_Slope = 0;
            R_HCM_Offset = 0;
            G_HCM_Slope = 0;
            G_HCM_Offset = 0;
            B_HCM_Slope = 0;
            B_HCM_Offset = 0;
            for (int i=0; i< Check_LCM_DAC.Length; i++)
            {
                Check_LCM_DAC[i] = 0;
                Check_HCM_DAC[i] = 0;
                LCM_R[i] = 0;
                LCM_R_Dev[i] = 0;
                LCM_G[i] = 0;
                LCM_G_Dev[i] = 0;
                LCM_B[i] = 0;
                LCM_B_Dev[i] = 0;
                HCM_R[i] = 0;
                HCM_R_Dev[i] = 0;
                HCM_G[i] = 0;
                HCM_G_Dev[i] = 0;
                HCM_B[i] = 0;
                HCM_B_Dev[i] = 0;
            }
        }
        public void SetDeviationLimit(double limit)
        {
            Limit_Dev = limit;
        }
        public void SetCheck_HCM_DAC(params int[] dacs)
        {
            Check_HCM_DAC = dacs;
        }
        public void SetCheck_LCM_DAC(params int[] dacs)
        {
            Check_LCM_DAC = dacs;
        }
        public void SetCurrentData(string color, string mode, double[] current, double slope, double offset)
        {
            if(color == "R" && mode == "LCM")
            {
                LCM_R = current;
                R_LCM_Slope = slope;
                R_LCM_Offset = offset;
            }
            else if (color == "G" && mode == "LCM")
            {
                LCM_G = current;
                G_LCM_Slope = slope;
                G_LCM_Offset = offset;
            }
            else if (color == "B" && mode == "LCM")
            {
                LCM_B = current;
                B_LCM_Slope = slope;
                B_LCM_Offset = offset;
            }
            else if (color == "R" && mode == "HCM")
            {
                HCM_R = current;
                R_HCM_Slope = slope;
                R_HCM_Offset = offset;
            }
            else if (color == "G" && mode == "HCM")
            {
                HCM_G = current;
                G_HCM_Slope = slope;
                G_HCM_Offset = offset;
            }
            else if (color == "B" && mode == "HCM")
            {
                HCM_B = current;
                B_HCM_Slope = slope;
                B_HCM_Offset = offset;
            }
        }
        public bool CheckSlopeCorrect()
        {
            bool res = true;

            for (int i = 0; i < Check_LCM_DAC.Length; i++)
            {
                if (CheckOutOfLimit(Check_LCM_DAC[i], R_LCM_Slope, R_LCM_Offset, LCM_R[i], ref LCM_R_Dev[i], ref LCM_R_Calculate[i], "LCM_R") == false)
                    res = false;
                if (CheckOutOfLimit(Check_LCM_DAC[i], G_LCM_Slope, G_LCM_Offset, LCM_G[i], ref LCM_G_Dev[i], ref LCM_G_Calculate[i], "LCM_G") == false)
                    res = false;
                if (CheckOutOfLimit(Check_LCM_DAC[i], B_LCM_Slope, B_LCM_Offset, LCM_B[i], ref LCM_B_Dev[i], ref LCM_B_Calculate[i], "LCM_B") == false)
                    res = false;
                if (CheckOutOfLimit(Check_HCM_DAC[i], R_HCM_Slope, R_HCM_Offset, HCM_R[i], ref HCM_R_Dev[i], ref HCM_R_Calculate[i], "HCM_R") == false)
                    res = false;
                if (CheckOutOfLimit(Check_HCM_DAC[i], G_HCM_Slope, G_HCM_Offset, HCM_G[i], ref HCM_G_Dev[i], ref HCM_G_Calculate[i], "HCM_G") == false)
                    res = false;
                if (CheckOutOfLimit(Check_HCM_DAC[i], B_HCM_Slope, B_HCM_Offset, HCM_B[i], ref HCM_B_Dev[i], ref HCM_B_Calculate[i], "HCM_B") == false)
                    res = false;
            }

            PASS = res;
            return res;
        }
        public void OutputResult(string sn, string test_side, params string[] copy_path)
        {
            string now_date = DateTime.Now.ToString("yyyyMMdd");
            string result = PASS ? "PASS" : "FAIL";
            string file_name = $"Z23A_LEDIV_{test_side}_{sn}_CheckSlopeResult_{DateTime.Now:yyyyMMdd_HHmmss}";
            
            OutputFile = Tool.CreateFile($"\\Result\\{now_date}\\{file_name}", ".csv", false);

            Tool.WriteFile(OutputFile, $"SN,{sn}");
            Tool.WriteFile(OutputFile, $"Result,{result}");
            Tool.WriteFile(OutputFile, $"Deviation Limit(%),{Limit_Dev}");
            Tool.WriteFile(OutputFile, $"");    //空一列
            Tool.WriteFile(OutputFile, $"LCM DAC,R(mA),R Dev(%),G(mA),G Dev(%),B(mA),B Dev(%),HCM DAC,R(mA),R Dev(%),G(mA),G Dev(%),B(mA),B Dev(%)");

            for(int i=0; i< CheckCount; i++)
            {
                Tool.WriteFile(OutputFile, $"{Check_LCM_DAC[i]}," +
                                            $"{LCM_R_Calculate[i]:F2},{LCM_R_Dev[i]:F2}," +
                                            $"{LCM_G_Calculate[i]:F2},{LCM_G_Dev[i]:F2}," +
                                            $"{LCM_B_Calculate[i]:F2},{LCM_B_Dev[i]:F2}," +
                                            $"{Check_HCM_DAC[i]}," +
                                            $"{HCM_R_Calculate[i]:F2},{HCM_R_Dev[i]:F2}," +
                                            $"{HCM_G_Calculate[i]:F2},{HCM_G_Dev[i]:F2}," +
                                            $"{HCM_B_Calculate[i]:F2},{HCM_B_Dev[i]:F2}");
            }

            // Copy the result file to the specified paths
            Tool.CopyFile(OutputFile,
                            (copy_path.Length > 0 ? copy_path[0] : "") + $"\\{now_date}",
                            (copy_path.Length > 1 ? copy_path[1] : "") + $"\\{now_date}");

            Tool.CloseFile(OutputFile);
        }
        #endregion

    }
}
