using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public interface IWriteFile
    {
        double R_Offset_HCM { get;}
        double G_Offset_HCM { get;}
        double B_Offset_HCM { get;}
        double R_Offset_LCM { get;}
        double G_Offset_LCM { get;}
        double B_Offset_LCM { get;}
        double R_Slope_HCM { get;}
        double G_Slope_HCM { get;}
        double B_Slope_HCM { get;}
        double R_Slope_LCM { get;}
        double G_Slope_LCM { get;}
        double B_Slope_LCM { get;}

        void CreateFile(string describe = "");
        void WriteFile(string context = "", string describe = "", bool NewLine = true);
        void CloseFile(string describe = "");
        void CloseAndDeleteFile(string describe = "");
        void CopyAndCloseTestFile(string describe);
        void WriteTestResult(int dac, double v_in, double i_in, double p_in, double vf,
                                    double vfb, double i_led, double p_led, double eff, double temperature,
                                    double x, double y, double m, double c, string color);
        void WriteTestResult(RGBTesterData test_data, int index, string type);
        void WriteCalibrationResult(string sn, string describe = "");
        void ResetCalibrationData();
        void SetCalibrationData(string color, string current_mode, double slope, double offset);
    }

    public class RGBTesterData
    {
        // [TestCondition]
        public List<int> DACpoint = new List<int>();

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

        // [Record]
        public List<double> CycleTime = new List<double>();
        public List<double> Temperature = new List<double>();
        public double DAC_Avg, Current_Avg, Slope, Offset;
    }
}
