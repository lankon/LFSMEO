using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public enum eF_OpticalTest
    {
        TxtBx_Left_SN,
        TxtBx_Right_SN,
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
        TxtBx_Left_R_IntgTimeStart,
        TxtBx_Left_R_IntgTimeStep,
        TxtBx_Left_R_IntgTimeEnd,

        TxtBx_Left_G_I_Start,
        TxtBx_Left_G_I_Step,
        TxtBx_Left_G_I_End,
        TxtBx_Left_G_IntgTimeStart,
        TxtBx_Left_G_IntgTimeStep,
        TxtBx_Left_G_IntgTimeEnd,

        TxtBx_Left_B_I_Start,
        TxtBx_Left_B_I_Step,
        TxtBx_Left_B_I_End,
        TxtBx_Left_B_IntgTimeStart,
        TxtBx_Left_B_IntgTimeStep,
        TxtBx_Left_B_IntgTimeEnd,

        TxtBx_Left_B2_I_Start,
        TxtBx_Left_B2_I_Step,
        TxtBx_Left_B2_I_End,
        TxtBx_Left_B2_IntgTimeStart,
        TxtBx_Left_B2_IntgTimeStep,
        TxtBx_Left_B2_IntgTimeEnd,

        TxtBx_Left_AvgCount,

        //[Right]
        TxtBx_Right_R_I_Start,
        TxtBx_Right_R_I_Step,
        TxtBx_Right_R_I_End,
        TxtBx_Right_R_IntgTimeStart,
        TxtBx_Right_R_IntgTimeStep,
        TxtBx_Right_R_IntgTimeEnd,

        TxtBx_Right_G_I_Start,
        TxtBx_Right_G_I_Step,
        TxtBx_Right_G_I_End,
        TxtBx_Right_G_IntgTimeStart,
        TxtBx_Right_G_IntgTimeStep,
        TxtBx_Right_G_IntgTimeEnd,

        TxtBx_Right_B_I_Start,
        TxtBx_Right_B_I_Step,
        TxtBx_Right_B_I_End,
        TxtBx_Right_B_IntgTimeStart,
        TxtBx_Right_B_IntgTimeStep,
        TxtBx_Right_B_IntgTimeEnd,

        TxtBx_Right_B2_I_Start,
        TxtBx_Right_B2_I_Step,
        TxtBx_Right_B2_I_End,
        TxtBx_Right_B2_IntgTimeStart,
        TxtBx_Right_B2_IntgTimeStep,
        TxtBx_Right_B2_IntgTimeEnd,

        TxtBx_Right_AvgCount,

        ////[Left WPC]
        //TxtBx_Left_R_I_Start_WPC,
        //TxtBx_Left_R_I_Step_WPC,
        //TxtBx_Left_R_I_End_WPC,
        //TxtBx_Left_R_IntgTimeStart_WPC,
        //TxtBx_Left_R_IntgTimeStep_WPC,
        //TxtBx_Left_R_IntgTimeEnd_WPC,

        //TxtBx_Left_G_I_Start_WPC,
        //TxtBx_Left_G_I_Step_WPC,
        //TxtBx_Left_G_I_End_WPC,
        //TxtBx_Left_G_IntgTimeStart_WPC,
        //TxtBx_Left_G_IntgTimeStep_WPC,
        //TxtBx_Left_G_IntgTimeEnd_WPC,

        //TxtBx_Left_B_I_Start_WPC,
        //TxtBx_Left_B_I_Step_WPC,
        //TxtBx_Left_B_I_End_WPC,
        //TxtBx_Left_B_IntgTimeStart_WPC,
        //TxtBx_Left_B_IntgTimeStep_WPC,
        //TxtBx_Left_B_IntgTimeEnd_WPC,

        //TxtBx_Left_B2_I_Start_WPC,
        //TxtBx_Left_B2_I_Step_WPC,
        //TxtBx_Left_B2_I_End_WPC,
        //TxtBx_Left_B2_IntgTimeStart_WPC,
        //TxtBx_Left_B2_IntgTimeStep_WPC,
        //TxtBx_Left_B2_IntgTimeEnd_WPC,

        TxtBx_Left_IntgTimeStart_WPC,
        TxtBx_Left_IntgTimeStep_WPC,
        TxtBx_Left_IntgTimeEnd_WPC,

        ////[Right WPC]
        //TxtBx_Right_R_I_Start_WPC,
        //TxtBx_Right_R_I_Step_WPC,
        //TxtBx_Right_R_I_End_WPC,
        //TxtBx_Right_R_IntgTime_WPC,

        //TxtBx_Right_G_I_Start_WPC,
        //TxtBx_Right_G_I_Step_WPC,
        //TxtBx_Right_G_I_End_WPC,
        //TxtBx_Right_G_IntgTime_WPC,

        //TxtBx_Right_B_I_Start_WPC,
        //TxtBx_Right_B_I_Step_WPC,
        //TxtBx_Right_B_I_End_WPC,
        //TxtBx_Right_B_IntgTime_WPC,

        //TxtBx_Right_B2_I_Start_WPC,
        //TxtBx_Right_B2_I_Step_WPC,
        //TxtBx_Right_B2_I_End_WPC,
        //TxtBx_Right_B2_IntgTime_WPC,

        TxtBx_Right_IntgTimeStart_WPC,
        TxtBx_Right_IntgTimeStep_WPC,
        TxtBx_Right_IntgTimeEnd_WPC,
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
