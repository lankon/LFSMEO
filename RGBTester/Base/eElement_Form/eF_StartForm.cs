using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public enum eF_StartForm
    {
        Cmbx_TestMode,
        Cmbx_PartTest,
        TxtBx_Left_SN,
        TxtBx_Right_SN,
        Cmbx_ProductType,
    }
    public enum eF_StartFormRecipe
    {
        //[Right]
        TxtBx_Right_AvgCount,

        TxtBx_Right_R_DAC_Start,
        TxtBx_Right_R_DAC_Step,
        TxtBx_Right_R_HCM_DAC_End,
        TxtBx_Right_R_LCM_DAC_End,

        TxtBx_Right_G_DAC_Start,
        TxtBx_Right_G_DAC_Step,
        TxtBx_Right_G_HCM_DAC_End,
        TxtBx_Right_G_LCM_DAC_End,

        TxtBx_Right_B_DAC_Start,
        TxtBx_Right_B_DAC_Step,
        TxtBx_Right_B_HCM_DAC_End,
        TxtBx_Right_B_LCM_DAC_End,

        TxtBx_Left_B2_DAC_Start,
        TxtBx_Left_B2_DAC_Step,
        TxtBx_Left_B2_HCM_DAC_End,
        TxtBx_Left_B2_LCM_DAC_End,

        //[Left]
        TxtBx_Left_AvgCount,

        TxtBx_Left_R_DAC_Start,
        TxtBx_Left_R_DAC_Step,
        TxtBx_Left_R_HCM_DAC_End,
        TxtBx_Left_R_LCM_DAC_End,

        TxtBx_Left_G_DAC_Start,
        TxtBx_Left_G_DAC_Step,
        TxtBx_Left_G_HCM_DAC_End,
        TxtBx_Left_G_LCM_DAC_End,

        TxtBx_Left_B_DAC_Start,
        TxtBx_Left_B_DAC_Step,
        TxtBx_Left_B_HCM_DAC_End,
        TxtBx_Left_B_LCM_DAC_End,

        TxtBx_Right_B2_DAC_Start,
        TxtBx_Right_B2_DAC_Step,
        TxtBx_Right_B2_HCM_DAC_End,
        TxtBx_Right_B2_LCM_DAC_End,

        TxtBx_Rfb_HCM,
        TxtBx_Rfb_LCM,

        TxtBx_LCM_MaxCurrent,
        TxtBx_HCM_MaxCurrent,
    }

    public enum ePartTestItem
    {
        IV_Test_HCM,
        IV_Test_LCM,
        IV_Test,
        BurinIn,
    }
}

//Form中的選項
namespace RGBTester.Base.F_StartForm
{
    
}
