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
        public RGBTesterDataFile(IFunction_DataUpload data_update, RGBTesterFunction rGBTesterFunction)
        {
            RGBfunc = rGBTesterFunction;
            DataUpdate = data_update;
            SetModuleAndCustomer(RGBfunc.GetModuleType());
        }

        #region paramter define
        private Dictionary<string, StreamWriter> TestFiles = new Dictionary<string, StreamWriter>();    //電性檔案
        private RGBTesterDataFile_FileType FileType;    //依客戶別或機型產生對應的資料
        private RGBTesterFunction RGBfunc;              //RGBTester相關Function
        private IFunction_DataUpload DataUpdate;        //資料上傳函式
        private eModuleType ModuleType;                 //機型
        private DateTime DateNow;                       //時間

        public CheckSlopeData CheckSlope { get; private set; }      //檢查Slope結果
        public OpticalData OpticalResult { get; private set; }      //光性量測資料
        public UploadData UploadData { get; private set; }          //資料上傳系統
        public Dictionary<string, double> LED_Slope { get; } = new Dictionary<string, double>();    //R/G/B...Slope結果
        public Dictionary<string, double> LED_Offset { get; } = new Dictionary<string, double>();   //R/G/B...Offset結果
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
                FileType = new RGBTesterDataFile_FunctionTester(RGBfunc, this);

            CheckSlope = new CheckSlopeData(FileType, RGBfunc);
            OpticalResult = new OpticalData(RGBfunc);
            UploadData = new UploadData(DataUpdate);
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

            string result = "";

            if(ModuleType == eModuleType.Function_Test)
            {
                if (RGBfunc.FailReasonFlag.IsTestFail() == true)
                    result = "FAIL";
                else
                    result = "PASS";
            }

            if (copy_path != "")
                copy_path = copy_path + $"\\{DateNow.ToString("yyyyMMdd")}\\{result}";

            if (copy_path1 != "")
                copy_path1 = copy_path1 + $"\\{DateNow.ToString("yyyyMMdd")}\\{result}";

            TestFiles.TryGetValue(describe, out StreamWriter file);

            if (file == null)
                return;

            Tool.CopyFile(file, copy_path, copy_path1);
        }

        public void ResetCalibrationData()
        {
            LED_Offset["R_Offset_HCM"] = -99;
            LED_Offset["R_Offset_LCM"] = -99;
            LED_Slope["R_Slope_HCM"] = -99;
            LED_Slope["R_Slope_LCM"] = -99;

            LED_Offset["G_Offset_HCM"] = -99;
            LED_Offset["G_Offset_LCM"] = -99;
            LED_Slope["G_Slope_HCM"] = -99;
            LED_Slope["G_Slope_LCM"] = -99;

            LED_Offset["B_Offset_HCM"] = -99;
            LED_Offset["B_Offset_LCM"] = -99;
            LED_Slope["B_Slope_HCM"] = -99;
            LED_Slope["B_Slope_LCM"] = -99;

            LED_Offset["B2_Offset_HCM"] = -99;
            LED_Offset["B2_Offset_LCM"] = -99;
            LED_Slope["B2_Slope_HCM"] = -99;
            LED_Slope["B2_Slope_LCM"] = -99;
        }
        public void SetCalibrationData(string color, string current_mode, double slope, double offset)
        {
            string offset_item = $"{color}_Offset_{current_mode}";
            string slope_item = $"{color}_Slope_{current_mode}";

            try
            {
                LED_Offset[offset_item] = offset;
                LED_Slope[slope_item] = slope;
            }
            catch(Exception ex)
            {
                Tool.SaveLogToFile("SetCalibrationData找不到對應的Key", level: "ERR");
            }
        }
        public bool WriteCalibrationResult(string sn, string describe = "")
        {
            List<string> calibration = FileType.GetCalibrationStr();

            for (int i = 0; i < calibration.Count; i++)
            {
                WriteFile(calibration[i], describe);
            }

            string sn_command = $"0x0440,SN,Serial_Number,NA,{sn}";
            WriteFile(sn_command, describe);    //新增SN,NA會連同Phase1也修改

            if (ModuleType == eModuleType.Function_Test)
            {
                calibration.Add(sn_command);
                UploadData.InputMessage(calibration, "");
            }

            return true;
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
