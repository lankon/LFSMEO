using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public class YieldData
    {
        public string Title_ProductName { get; set; }
        public string Title_SN_Key { get; set; }
        public string Title_StartTime { get; set; }
        public string Title_EndTime { get; set; }
        public int Title_TotalUnits { get; set; }
        public int Title_PassUnits { get; set; }
        public int Title_FailUnits { get; set; }
        public double Title_Yield { get; set; }
    }

    public interface IF_ParameterSetting
    {
        void ShowSlopeCheckDataInvoke(double[] r_current_LCM, double[] r_dev_LCM, double[] g_current_LCM, double[] g_dev_LCM,
                                           double[] b_current_LCM, double[] b_dev_LCM, double[] r_current_HCM, double[] r_dev_HCM,
                                           double[] g_current_HCM, double[] g_dev_HCM, double[] b_current_HCM, double[] b_dev_HCM);
    }
}
