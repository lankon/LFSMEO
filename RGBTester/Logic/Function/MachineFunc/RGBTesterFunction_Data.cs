using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Logic
{
    public partial class RGBTesterFunction
    {
        public class AvgData
        {
            public double Avg_Vin;
            public double Avg_Iin;
            public double Avg_Vled;
            public double Avg_Vf;
            public double Avg_Iled;
        }
        public class AvgData_FuncTester
        {
            public double Avg_Vin;
            public double Avg_Vf;
            public double Avg_Iled;
            public double Avg_DISP_6V0;
            public double Avg_DISP_1V2;
        }
        public class TestSlopeOffsetResult
        {
            public double R_Slope_HCM;
            public double R_Slope_LCM;
            public double G_Slope_HCM;
            public double G_Slope_LCM;
            public double B_Slope_HCM;
            public double B_Slope_LCM;
            public double B2_Slope_HCM;
            public double B2_Slope_LCM;
            public double R_Offset_HCM;
            public double R_Offset_LCM;
            public double G_Offset_HCM;
            public double G_Offset_LCM;
            public double B_Offset_HCM;
            public double B_Offset_LCM;
            public double B2_Offset_HCM;
            public double B2_Offset_LCM;

            public void SetResult(string color, string mode, double slope, double offset)
            {
                //[RGB/High]
                if (color == "R" && mode == "HCM")
                {
                    R_Slope_HCM = slope;
                    R_Offset_HCM = offset;
                }
                else if (color == "G" && mode == "HCM")
                {
                    G_Slope_HCM = slope;
                    G_Offset_HCM = offset;
                }
                else if (color == "B" && mode == "HCM")
                {
                    B_Slope_HCM = slope;
                    B_Offset_HCM = offset;
                }
                else if(color == "B" && mode == "HCM")
                {
                    B2_Slope_HCM = slope;
                    B2_Offset_HCM = offset;
                }
                //[RGB/Low]
                else if (color == "R" && mode == "LCM")
                {
                    R_Slope_LCM = slope;
                    R_Offset_LCM = offset;
                }
                else if (color == "G" && mode == "LCM")
                {
                    G_Slope_LCM = slope;
                    G_Offset_LCM = offset;
                }
                else if (color == "B" && mode == "LCM")
                {
                    B_Slope_LCM = slope;
                    B_Offset_LCM = offset;
                }
                else if (color == "B2" && mode == "LCM")
                {
                    B2_Slope_LCM = slope;
                    B2_Offset_LCM = offset;
                }
            }
        }
    }
}
