using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public enum eF_AxisSetting
    {
        //新增軸參數時需添加

        #region Axis Configuration
        Cmbx_AxisType,
        TxtBx_LineNo,
        TxtBx_AxisStation,
        Cmbx_AxisUse,
        Cmbx_AxisLimitLogic,
        Cmbx_AxisLimitStopMode,
        TxtBx_DriverResolution,
        #endregion

        #region Hardware Configuration
        TxtBx_AxisPitch,
        #endregion

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
