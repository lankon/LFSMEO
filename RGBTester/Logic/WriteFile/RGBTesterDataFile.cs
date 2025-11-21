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
        private StreamWriter LeftGlassesFile;
        private StreamWriter RightGlassesFile;
        #endregion

        #region public function
        public void CreateFile(string describe = "")
        {
            DateTime now = DateTime.Now;
            string file_name = $"\\Result\\{describe}_LEDResult_{now.ToString("yyyyMMddHHmmss")}";

            if(describe == "Left")
                LeftGlassesFile = Tool.CreateFile(file_name, ".csv", false);
            else if(describe == "Right")
                RightGlassesFile = Tool.CreateFile(file_name, ".csv", false);

            WriteTitle(describe);
        }

        public void WriteFile(string context = "", string describe = "")
        {
            if (describe == "Left")
                Tool.WriteFile(LeftGlassesFile, context);
            else
                Tool.WriteFile(RightGlassesFile, context);
        }
        public void CloseFile(string describe = "")
        {
            if (describe == "Left" && LeftGlassesFile != null)
                Tool.CloseFile(LeftGlassesFile);
            else if (describe == "Right" && RightGlassesFile != null)
                Tool.CloseFile(RightGlassesFile);
        }
        #endregion

        #region private function
        private void WriteTitle(string type)
        {
            string title = "Station,Comment,Category,SN,TestResult,TestDate,TestTime,CycleTime(S),UserName,DirLogName,First Fail Item,fixture_load,Status,Vin,Status,Iin,Status,Pin,Status,Vf,Status,Iled,Status,Pled,Status,Eff,Status,x,Status,y,Status,m,Status,y,Status,c";

            if (type == "Left")
                Tool.WriteFile(LeftGlassesFile, title);
            else
                Tool.WriteFile(RightGlassesFile, title);
        }
        #endregion
    }
}
