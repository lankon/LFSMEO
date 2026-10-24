using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAMachine.Machine.BDSA.Base
{
    public enum eF_BackSetting
    {
        TxtBx_CrtLeft_WG_X,
        TxtBx_CrtLeft_WG_Y,

        TxtBx_CrtRight_WG_X,
        TxtBx_CrtRight_WG_Y,

        Txt_ToolModelZSafePos,
        TxtBx_DispMoveZ,

        DispensingNeedleX,
        DispensingNeedleY,
        DispensingNeedleZ,
    }

    public enum eF_BackSettingRecipe
    {
        TxtBx_LeftWGCenterX,
        TxtBx_LeftWGCenterY,
        TxtBx_LeftWGCenterZ,

        TxtBx_RightWGCenterX,
        TxtBx_RightWGCenterY,
        TxtBx_RightWGCenterZ,

        TxtBx_NeedleAxisLight,
    }
}
