using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGBTester.Logic;

namespace RGBTester.Base
{
    public interface IWriteFile
    {
        Dictionary<string, double> LED_Slope { get; }
        Dictionary<string, double> LED_Offset { get; }
        CheckSlopeData CheckSlope { get; }
        OpticalData OpticalResult { get;}

        void SetModuleAndCustomer(eModuleType eModule);
        void CreateFile(string describe = "");
        
        // [Test Data]
        void WriteFile(string context = "", string describe = "", bool NewLine = true);
        void WriteTestResult(RGBTesterData test_data, int index, string type);
        void WriteExtendTestResult(RGBTesterData test_data, int index, string type);

        // [Close File]
        void CloseFile(string describe = "");
        void CloseAndDeleteFile(string describe = "");
        void CopyAndCloseTestFile(string describe);
        
        // [Calibration File]
        void ResetCalibrationData();
        void SetCalibrationData(string color, string current_mode, double slope, double offset);
        bool WriteCalibrationResult(string sn, string describe = "");

        RGBTesterData SetNonData(RGBTesterData data);
    }

    public class RGBTesterData
    {
        // [TestCondition]
        public List<int> DACpoint = new List<int>();
        public List<int> Currentpoint = new List<int>();

        // [DAQ Point]
        public List<double> Vled = new List<double>();

        // [TestItem]
        public List<double> Vin = new List<double>();
        public List<double> Iin = new List<double>();
        public List<double> Pin = new List<double>();
        public List<double> Vf = new List<double>();
        public List<double> Iled = new List<double>();
        public List<double> Pled = new List<double>();
        public List<double> Eff = new List<double>();
        public List<double> DISP_6V0 = new List<double>();
        public List<double> DISP_1V2 = new List<double>();
        public List<double> WLD = new List<double>();
        public List<double> Lumens = new List<double>();
        public List<double> OpticalPower = new List<double>();

        // [Record]
        public List<double> CycleTime = new List<double>();
        public List<double> Temperature = new List<double>();
        public List<int> IntegralTime = new List<int>();
        public double DAC_Avg, Current_Avg, Slope, Offset;
        public string CurrentMode = "";
        public string SN = "";
        public string TestSide = "";
        public string TestColor = "";
    }

    public class RGBTesterDataFile_FileType
    {
        public virtual eModuleType GetModuleType()
        {
            return eModuleType.IV_Calibration;
        }

        public virtual string GetTitleStr(string describe)
        {
            return "";
        }

        public virtual string GetTestReultStr(RGBTesterData test_data, int index)
        {
            return "";
        }

        public virtual string GetExtendTestResultStr(RGBTesterData test_data, int index)
        {
            return "";
        }

        public virtual List<string> GetCalibrationStr()
        {
            return new List<string>();
        }

        public virtual string GetCheckSlopeStr(int index)
        {
            return "";
        }
    }
}
