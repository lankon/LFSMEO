using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAMachine.Base
{
    public enum eF_FrontSetting
    {
        TxtBx_SafePosUpPA_Z,
        TxtBx_SafePosPCCD_X,
        TxtBx_SafePosMirrorZ,
        TxtBx_SafePosNestX,

        LoadPos_NestTY,
        LoadPos_NestTX,
        LoadPos_NestA,
        LoadPos_NestX,
        LoadPos_NestY,
        LoadPos_NestZ,

        UnLoadPos_NestTY,
        UnLoadPos_NestTX,
        UnLoadPos_NestA,
        UnLoadPos_NestX,
        UnLoadPos_NestY,
        UnLoadPos_NestZ,

        TxtBx_3DProfile_A_Result,
        TxtBx_3DProfile_Ty_Result,

        TxtBx_SidePAOffsetNestY,
        TxtBx_SidePAOffsetNestZ,
        TxtBx_NEDOffsetNestY,
        TxtBx_NEDOffsetNestZ,
        TxtBx_PA_NED_OffsetZ,
        TxtBx_PA_NED_OffsetY,

        TxtBx_SidePAResult_TX,
        TxtBx_SidePAResult_Z,
        TxtBx_SidePAResult_Y,

        TxtBx_PCCD_WorkPos,

        TxtBx_FR_FUpPAOffsetFrontX,
        TxtBx_FR_FUpPAOffsetNestY,
        TxtBx_FR_RUpPAOffsetToolX,
        TxtBx_FR_RUpPAOffsetNestY,
        TxtBx_FR_CCD_OffsetY,
        TxtBx_FR_CCD_OffsetX,

        TxtBx_UpPAOffsetFrontX,
        TxtBx_UpPAOffsetDPA_Y,
        TxtBx_MirrorOffsetMirrorX,
        TxtBx_MirrorOffsetMirrorY,
        TxtBx_MirrorOffsetFrontX,
        TxtBx_MirrorOffsetDPA_Y,
        TxtBx_UpPAMirrorOffsetX,
        TxtBx_UpPAMirrorOffsetY,

        TxtBx_Mirror_0_degree_X,
        TxtBx_Mirror_0_degree_Y,
        TxtBx_Mirror_45_degree_X,
        TxtBx_Mirror_45_degree_Y,
        TxtBx_Mirror_0_45_offset_X,
        TxtBx_Mirror_0_45_offset_Y,

        Left_DE_MLO_Pos_NestTY,
        Left_DE_MLO_Pos_NestTX,
        Left_DE_MLO_Pos_NestA,
        Left_DE_MLO_Pos_NestX,
        Left_DE_MLO_Pos_NestY,
        Left_DE_MLO_Pos_NestZ,

        Right_DE_MLO_Pos_NestX,
        Right_DE_MLO_Pos_NestY,
        Right_DE_MLO_Pos_NestZ,
        Right_DE_MLO_Pos_NestA,
        Right_DE_MLO_Pos_NestTX,
        Right_DE_MLO_Pos_NestTY,
    }

    public enum eF_FrontSettingRecipe
    {
        Left_DE_3D_Pos_NestX,
        Left_DE_3D_Pos_NestZ,
        Left_DE_3D_Pos_NestA,
        Left_DE_3D_Pos_NestTX,
        Left_DE_3D_Pos_NestY,
        Left_DE_3D_Pos_NestTY,

        Right_DE_3D_Pos_NestY,
        Right_DE_3D_Pos_NestX,
        Right_DE_3D_Pos_NestA,
        Right_DE_3D_Pos_NestTX,
        Right_DE_3D_Pos_NestZ,
        Right_DE_3D_Pos_NestTY,

        Side_PA_Left_DE_Feature_Pos_NestTX,
        Side_PA_Left_DE_Feature_Pos_NestX,
        Side_PA_Left_DE_Feature_Pos_NestZ,
        Side_PA_Left_DE_Feature_Pos_NestY,

        Side_PA_Right_DE_Feature_Pos_NestTX,
        Side_PA_Right_DE_Feature_Pos_NestX,
        Side_PA_Right_DE_Feature_Pos_NestZ,
        Side_PA_Right_DE_Feature_Pos_NestY,

        Side_PA_Left_DE_Center_Pos_NestTX,
        Side_PA_Left_DE_Center_Pos_NestX,
        Side_PA_Left_DE_Center_Pos_NestY,
        Side_PA_Left_DE_Center_Pos_NestZ,

        Side_PA_Right_DE_Center_Pos_NestTX,
        Side_PA_Right_DE_Center_Pos_NestX,
        Side_PA_Right_DE_Center_Pos_NestY,
        Side_PA_Right_DE_Center_Pos_NestZ,

        Side_PA_Left_DE_Feature_Pos_Z3,
        Side_PA_Left_DE_Feature_Pos_Y3,
        Side_PA_Left_DE_Feature_Pos_Z2,
        Side_PA_Left_DE_Feature_Pos_Y1,
        Side_PA_Left_DE_Feature_Pos_Z1,
        Side_PA_Left_DE_Feature_Pos_Y2,

        Side_PA_Right_DE_Feature_Pos_Z3,
        Side_PA_Right_DE_Feature_Pos_Y3,
        Side_PA_Right_DE_Feature_Pos_Z2,
        Side_PA_Right_DE_Feature_Pos_Y1,
        Side_PA_Right_DE_Feature_Pos_Z1,
        Side_PA_Right_DE_Feature_Pos_Y2,
    }
}
