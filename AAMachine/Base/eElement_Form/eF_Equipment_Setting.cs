using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAMachine.Base
{
    public enum eF_Equipment_Setting
    {
        Cmbx_ShowFormName,
        Cmbx_ModuleType,
    }

    public enum eMachineSetting
    {
        //所有專案共用的enum名稱

        Cmbx_MachineType,
    }
}

namespace AAMachine.Base.Equipment_Setting
{
    public enum eModuleType
    {
        MIRROR_AA,
        DETESTER,
    }
}
