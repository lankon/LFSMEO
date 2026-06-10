using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public enum eF_UploadDataSetting
    {
        Cmbx_UseUploadSystem,
        TxtBx_FixtureID,
        TxtBx_Station,
        TxtBx_Line,
        TxtBx_ProgramVer,
        TxtBx_Testplan,
        TxtBx_PCName,
    }
}

namespace RGBTester.Base.UploadDataSetting
{
    public enum eUseUploadSystem
    {
        PASS,
        USE,
    }
}
