using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGBTester.Base;
using ToolFunction;

namespace RGBTester.Logic
{
    public class OpticalData
    {
        public OpticalData()
        {
        }


        #region parameter define
        //public Dictionary<string, TestResult> dicTestResult = new Dictionary<string, TestResult>();

        //public class TestResult
        //{
        //    public double Lumen;
        //    public double OpticalPower;
        //    public double Wavelength;
        //    public double Temperature;
        //}
        #endregion

        #region public function
        public void WriteTestData(RGBTesterData result, params string[] copy_path)
        {
            DateTime DateNow = DateTime.Now;
            string timeDay = DateNow.ToString("yyyyMMdd");
            string FulltimeTime = DateNow.ToString("yyyyMMddHHmmss");
            string fileName = "";

            string folderPath = $"\\Result\\{timeDay}\\";       //檔案儲存資料夾路徑
            fileName = folderPath + $@"Z23A_LEDIV_{result.TestSide}_{result.SN}_{result.TestColor}_Luminous_{FulltimeTime}";

            StreamWriter file = Tool.CreateFile(fileName, ".csv", false);

            // Ttitle
            Tool.WriteFile(file, "Station,SN,TestDate,TestTime,CycleTime(ms),UserName,DirLogName,Current(mA),Luminous Flux(lm),Power(mW),Wavelength(λd),LED Temperature(℃),IntegralTime(ms)");
            // TestData
            for(int i=0; i< result.Currentpoint.Count; i++)
            {
                Tool.WriteFile(file, $"BFT,{result.SN},{timeDay},{DateNow.ToString("HH:mm:ss")},{result.CycleTime[i]},8888,Z23A_LEDIV_{result.SN}_Luminous,{result.Currentpoint[i]}," +
                                 $"{result.Lumens[i]:F2},{result.OpticalPower[i]:F2},{result.WLD[i]:F2},{result.Temperature[i]},{result.IntegralTime[i]}");
            }

            // Copy the result file to the specified paths
            string p1 = copy_path.Length > 0 ? copy_path[0] : "";
            string p2 = copy_path.Length > 1 ? copy_path[1] : "";

            if (p1 != "" || p2 != "")
            {
                Tool.CopyFile(file,
                            (copy_path.Length > 0 ? copy_path[0] : "") + $"\\{timeDay}",
                            (copy_path.Length > 1 ? copy_path[1] : "") + $"\\{timeDay}");
            }

            Tool.CloseFile(file);
        }
        #endregion





    }
}
