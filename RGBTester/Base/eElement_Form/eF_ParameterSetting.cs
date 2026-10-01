using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public enum eF_ParameterSetting
    {
        TxtBx_TestFileCopyPath,
        TxtBx_TestFileCopyPath1,

        TxtBx_LCM_Check_DAC1,
        TxtBx_LCM_Check_DAC2,
        TxtBx_LCM_Check_DAC3,
        TxtBx_LCM_Check_DAC4,
        TxtBx_LCM_Check_DAC5,
        TxtBx_HCM_Check_DAC1,
        TxtBx_HCM_Check_DAC2,
        TxtBx_HCM_Check_DAC3,
        TxtBx_HCM_Check_DAC4,
        TxtBx_HCM_Check_DAC5,

        TxtBx_DeviationLimit,
        //Cmbx_YieldRecord,
    }

    public enum eF_ParameterSettingRecipe
    {
        TxtBx_LCM_Slope_UL,
        TxtBx_LCM_Slope_LL,
        TxtBx_HCM_Slope_UL,
        TxtBx_HCM_Slope_LL,
        TxtBx_ClampingFailDAC,

        TxtBx_BurnInCurrent_R,
        TxtBx_BurnInCurrent_G,
        TxtBx_BurnInCurrent_B,

        TxtBx_BurnInTime,
        TxtBx_BurnInRepeat,
        TxtBx_FailOverTemp,

        TxtBx_Voltage_6V_UL,
        TxtBx_Voltage_6V_LL,
        TxtBx_Voltage_1V2_UL,
        TxtBx_Voltage_1V2_LL,
    }
}
