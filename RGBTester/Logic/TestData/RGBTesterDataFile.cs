using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using ToolFunction;
using RGBTester.Base;
using RGBTester.Logic._RGBTesterDataFile;

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
        private RGBTesterDataFile_FileType FileType;
        private eModuleType ModuleType;
        private DateTime DateNow;
        public double R_Offset_HCM { get; private set; }
        public double G_Offset_HCM { get; private set; }
        public double B_Offset_HCM { get; private set; }
        public double B2_Offset_HCM { get; private set; }
        public double R_Offset_LCM { get; private set; }
        public double G_Offset_LCM { get; private set; }
        public double B_Offset_LCM { get; private set; }
        public double B2_Offset_LCM { get; private set; }
        public double R_Slope_HCM { get; private set; }
        public double G_Slope_HCM { get; private set; }
        public double B_Slope_HCM { get; private set; }
        public double B2_Slope_HCM { get; private set; }
        public double R_Slope_LCM { get; private set; }
        public double G_Slope_LCM { get; private set; }
        public double B_Slope_LCM { get; private set; }
        public double B2_Slope_LCM { get; private set; }
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
        
        private void WriteTitle(string type)
        {
            string title = FileType.GetTitleStr(type);

            if (title == "")
                return;

            TestFiles.TryGetValue(type, out StreamWriter file);
            Tool.WriteFile(file, title);
        }
        #endregion

        #region public function
        public void SetModuleAndCustomer(eModuleType eModule)
        {
            ModuleType = eModule;

            if (FileType != null)
                return;

            if (ModuleType == eModuleType.IV_Calibration)
                FileType = new RGBTesterDataFile_IVCalibration(RGBfunc, this);
            else if (ModuleType == eModuleType.Function_Test)
                FileType = new RGBTesterDataFile_FunctionTester(this);
        }
        public void CreateFile(string describe = "")
        {
            string fileName = CreateFileName(describe);

            StreamWriter fileHandle = Tool.CreateFile(fileName, ".csv", false);
            TestFiles[describe] = fileHandle;

            WriteTitle(describe);
        }

        public void WriteTestResult(RGBTesterData test_data, int index, string type)
        {
            string context = FileType.GetTestReultStr(test_data, index);

            WriteFile(context, type, false);
        }
        public void WriteExtendTestResult(RGBTesterData test_data, int index, string type)
        {
            string context = FileType.GetExtendTestResultStr(test_data, index);

            WriteFile(context, type, false);
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
        public void WriteCalibrationResult(string sn, string describe = "")
        {
            List<string> calibration = FileType.GetCalibrationStr();

            for (int i = 0; i < calibration.Count; i++)
            {
                WriteFile(calibration[i], describe);
            }

            WriteFile($"0x0440,,Serial Number,,{sn}", describe);
        }

        public RGBTesterData SetNonData(RGBTesterData data)
        {
            data.DACpoint.Add(-99);
            
            data.Vled.Add(-99);
            
            data.Vin.Add(-99);
            data.Iin.Add(-99);
            data.Pin.Add(-99);
            data.Vf.Add(-99);
            data.Iled.Add(-99);
            data.Pled.Add(-99);
            data.Eff.Add(-99);
            data.DISP_6V0.Add(-99);
            data.DISP_1V2.Add(-99);

            data.CycleTime.Add(-99);
            data.Temperature.Add(-99);

            return data;
        }
        #endregion
    }
}
