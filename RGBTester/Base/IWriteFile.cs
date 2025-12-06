using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public interface IWriteFile
    {
        void CreateFile(string describe = "");
        void WriteFile(string context = "", string describe = "", bool NewLine = true);
        void CloseFile(string describe = "");
        void WriteTestResult(int dac, double v_in, double i_in, double p_in, double vf,
                                    double i_led, double p_led, double eff, double temperature,
                                    double x, double y, double m, double c, string color);
    }
}
