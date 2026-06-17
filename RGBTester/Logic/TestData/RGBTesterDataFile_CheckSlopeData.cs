using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;
using RGBTester.Base;

namespace RGBTester.Logic
{
    public class CheckSlopeData
    {
        public CheckSlopeData(RGBTesterDataFile_FileType file_type, RGBTesterFunction rGBTesterFunction)
        {
            FileType = file_type;
            RGBfunc = rGBTesterFunction;

            //增加測試顏色時需添加
            if (FileType.GetModuleType() == eModuleType.IV_Calibration)
                TestMode = new string[] { "R_LCM", "G_LCM", "B_LCM", "R_HCM", "G_HCM", "B_HCM" };
            else
                TestMode = new string[] { "R_LCM", "G_LCM", "B_LCM", "B2_LCM", "R_HCM", "G_HCM", "B_HCM", "B2_HCM" };
        }

        #region parameter define
        private RGBTesterDataFile_FileType FileType;
        private StreamWriter OutputFile;
        private RGBTesterFunction RGBfunc;
        private bool PASS = false;
        private const int CheckCount = 5;
        private double Limit_Dev = 15;
        public Dictionary<string, TestReultItem> dicTestResult = new Dictionary<string, TestReultItem>();
        public Dictionary<string, CheckSlopeResult> dicCheckResult = new Dictionary<string, CheckSlopeResult>();

        // [Check DAC]
        public int[] Check_LCM_DAC = new int[CheckCount];
        public int[] Check_HCM_DAC = new int[CheckCount];
        public string[] TestMode; 
        
        public class TestReultItem
        {
            public double Slope;    //斜率
            public double Offset;   //位移量
            public double[] Current;//量測電流值
        }
        public class CheckSlopeResult
        {
            public int[] CheckDac = new int[CheckCount];
            public double[] CalCurrent = new double[CheckCount];
            public double[] Dev = new double[CheckCount];
        }
        #endregion

        #region private function
        private bool CheckOutOfLimit(int check_dac, int index, string mode)
        {
            double slope = dicTestResult[mode].Slope;
            double offset = dicTestResult[mode].Offset;
            double actual = dicTestResult[mode].Current[index];

            double CalculateCurrent = check_dac * slope + offset;
            double devation = CalculateCurrent - actual;
            CalculateCurrent = CalculateCurrent == 0 ? 0.00000001 : CalculateCurrent;
            actual = actual == 0 ? 0.00000001 : actual;

            dicCheckResult[mode].Dev[index] = devation / actual * 100;
            dicCheckResult[mode].CalCurrent[index] = CalculateCurrent;

            if (Math.Abs(devation) < actual * Limit_Dev / 100)
                return true;
            else
            {
                Tool.SaveLogToFile(string.Format($"Check Slope Calculate Current Out of Limit! {mode} Check DAC: {check_dac}, " +
                                                 $"Slope: {slope:F4}, Offset: {offset:F4}, Actual: {actual:F4}, " +
                                                 $"Calculate: {CalculateCurrent:F4}"), level: "WRN");

                return false;
            }
        }
        #endregion

        #region public function
        public void ResetParameter()
        {
            PASS = false;

            dicTestResult.Clear();

            for (int i=0; i< Check_LCM_DAC.Length; i++)
            {
                Check_LCM_DAC[i] = 0;
                Check_HCM_DAC[i] = 0;

                for (int j = 0; j < TestMode.Length; j++)
                {
                    if (!dicCheckResult.ContainsKey(TestMode[j]))
                        dicCheckResult[TestMode[j]] = new CheckSlopeResult();

                    dicCheckResult[TestMode[j]].Dev[i] = 0; 
                }
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
            string key = $"{color}_{mode}";

            TestReultItem item = new TestReultItem();
            item.Slope = slope;
            item.Offset = offset;
            item.Current = current.ToArray();   //複製陣列

            dicTestResult[key] = item;
        }
        public bool CheckSlopeCorrect()
        {
            bool res = true;

            for (int i = 0; i < Check_LCM_DAC.Length; i++)
            {
                bool is_LCM = dicTestResult.TryGetValue("R_LCM", out var data);
                bool is_HCM = dicTestResult.TryGetValue("R_HCM", out var data1);

                if (is_LCM)
                {
                    for(int j=0; j<TestMode.Length; j++)
                    {
                        if (TestMode[j].Contains("HCM"))
                            continue;
                        
                        if (!CheckOutOfLimit(Check_LCM_DAC[i], i ,TestMode[j]))
                            res = false;
                    }
                }

                if(is_HCM)
                {
                    for (int j = 0; j < TestMode.Length; j++)
                    {
                        if (TestMode[j].Contains("LCM"))
                            continue;

                        if (!CheckOutOfLimit(Check_HCM_DAC[i], i, TestMode[j]))
                            res = false;
                    }
                }
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

            //增加測試顏色時需添加
            if (FileType.GetModuleType() == eModuleType.IV_Calibration)
                Tool.WriteFile(OutputFile, $"LCM DAC,R(mA),R Dev(%),G(mA),G Dev(%),B(mA),B Dev(%),HCM DAC,R(mA),R Dev(%),G(mA),G Dev(%),B(mA),B Dev(%)");
            else
                Tool.WriteFile(OutputFile, $"LCM DAC,R(mA),R Dev(%),G(mA),G Dev(%),B(mA),B Dev(%),B2(mA),B2 Dev(%),HCM DAC,R(mA),R Dev(%),G(mA),G Dev(%),B(mA),B Dev(%),B2(mA),B2 Dev(%)");

            for (int i = 0; i < CheckCount; i++)
            {
                Tool.WriteFile(OutputFile, FileType.GetCheckSlopeStr(i));
            }

            string pass_fail = "";
            if (RGBfunc.GetModuleType() == eModuleType.Function_Test)
            {
                if (RGBfunc.FailReasonFlag.IsTestFail() == true)
                    pass_fail = "FAIL";
                else
                    pass_fail = "PASS";
            }

            // Copy the result file to the specified paths
            string p1 = copy_path.Length > 0 ? copy_path[0] : "";
            string p2 = copy_path.Length > 1 ? copy_path[1] : "";

            if (p1 != "" || p2 != "")
            {
                Tool.CopyFile(OutputFile,
                            (copy_path.Length > 0 && copy_path[0] != "" ? copy_path[0] + $"\\{now_date}\\{pass_fail}" : ""),
                            (copy_path.Length > 1 && copy_path[1] != "" ? copy_path[1] + $"\\{now_date}\\{pass_fail}" : ""));
            }

            Tool.CloseFile(OutputFile);
        }
        #endregion

    }
}
