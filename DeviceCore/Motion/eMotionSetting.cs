using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public enum eF_AxisSetting
    {
        Cmbx_AxisType,
        TxtBx_LineNo,
        TxtBx_AxisStation,
        Cmbx_AxisUse,
        Cmbx_AxisLimitLogic,
        Cmbx_AxisLimitStopMode,

        #region Home Configuration
        Cmbx_HomeMode,
        Cmbx_HomeDirection,
        TxtBx_ORGPosition,
        TxtBx_ORGShiftPosition,
        TxtBx_HomeVelocity,
        TxtBx_ORGVelocity,
        TxtBx_HomeAcc,
        #endregion


    }
}
