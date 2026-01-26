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
        void WriteTestResult(int dac, double v_in, double i_in, double p_in, double vf,
                                    double vfb, double i_led, double p_led, double eff, double temperature,
                                    double x, double y, double m, double c, string color);
        void WriteCalibrationResult(string sn, string describe = "");
        void ResetCalibrationData();
        void SetCalibrationData(string color, string current_mode, double slope, double offset);
    }
}
