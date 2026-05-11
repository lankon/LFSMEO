using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public enum eF_OpticalTest
    {
        Cmbx_TestMode,
    }

    public enum eF_OpticalTestRecipe
    {
        TxtBx_LCM_TestSlope,
        TxtBx_LCM_TestOffset,
        TxtBx_HCM_TestSlope,
        TxtBx_HCM_TestOffset,
        TxtBx_LCM_MaxCurrent,
        TxtBx_HCM_MaxCurrent,

        //[Left]
        TxtBx_Left_R_I_Start,
        TxtBx_Left_R_I_Step,
        TxtBx_Left_R_I_End,
        TxtBx_Left_R_IntgTime,

        TxtBx_Left_G_I_Start,
        TxtBx_Left_G_I_Step,
        TxtBx_Left_G_I_End,
        TxtBx_Left_G_IntgTime,

        TxtBx_Left_B_I_Start,
        TxtBx_Left_B_I_Step,
        TxtBx_Left_B_I_End,
        TxtBx_Left_B_IntgTime,

        TxtBx_Left_B1_I_Start,
        TxtBx_Left_B1_I_Step,
        TxtBx_Left_B1_I_End,
        TxtBx_Left_B1_IntgTime,

        TxtBx_Left_AvgCount,

        //[Right]
        TxtBx_Right_R_I_Start,
        TxtBx_Right_R_I_Step,
        TxtBx_Right_R_I_End,
        TxtBx_Right_R_IntgTime,

        TxtBx_Right_G_I_Start,
        TxtBx_Right_G_I_Step,
        TxtBx_Right_G_I_End,
        TxtBx_Right_G_IntgTime,

        TxtBx_Right_B_I_Start,
        TxtBx_Right_B_I_Step,
        TxtBx_Right_B_I_End,
        TxtBx_Right_B_IntgTime,

        TxtBx_Right_B1_I_Start,
        TxtBx_Right_B1_I_Step,
        TxtBx_Right_B1_I_End,
        TxtBx_Right_B1_IntgTime,

        TxtBx_Right_AvgCount,
    }
}

//Form中的選項
namespace RGBTester.Base.F_OpticalTest
{
    enum eTestMode
    {
        LEFT,
        RIGHT,
        BOTH,
    }
}
