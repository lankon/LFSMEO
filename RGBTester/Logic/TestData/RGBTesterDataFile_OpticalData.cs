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
        public OpticalData(RGBTesterFunction rGBTesterFunction)
        {
            RGBfunc = rGBTesterFunction;
        }

        #region parameter define
        RGBTesterFunction RGBfunc;
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

            string pass_fail = "";
            if (RGBfunc.GetModuleType() == eModuleType.Function_Test)
            {
                if (RGBfunc.FailReasonFlag.IsTestFail() == true)
                    pass_fail = "FAIL";
                else
                    pass_fail = "PASS";
            }

            if (p1 != "" || p2 != "")
            {
                Tool.CopyFile(file,
                            (copy_path.Length > 0 ? copy_path[0] : "") + $"\\{timeDay}\\{pass_fail}",
                            (copy_path.Length > 1 ? copy_path[1] : "") + $"\\{timeDay}\\{pass_fail}");
            }

            Tool.CloseFile(file);
        }
        #endregion
    }
}
