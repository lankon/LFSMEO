using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;
using DeviceCore;

namespace DeviceFunction
{
    class F_CameraSettingLogic
    {
        public F_CameraSettingLogic(IFunction_Camera function_Camera)
        {
            Function_Camera = function_Camera;
        }

        #region parameter function
        IFunction_Camera Function_Camera;
        IF_CameraSetting CameraSetting;
        #endregion

        #region public function
        public void SetCameraSettingIF(IF_CameraSetting f_CameraSetting)
        {
            CameraSetting = f_CameraSetting;
        }

        public void UpdateAxisInfo2Form(int axis)
        {
            //新增相機參數時需添加

            Function_Camera.LoadCameraConfig();
            var config = Function_Camera.GetCameraConfig();

            //[Connect Configuration]
            ApplicationSetting.SetRecipe<eF_CaneraSetting>((int)eF_CaneraSetting.Cmbx_AxisType, config[axis].CCD_TYPE.ToString());
            ApplicationSetting.SetRecipe<eF_CaneraSetting>((int)eF_CaneraSetting.Cmbx_AxisUse, config[axis].CCD_USE.ToString());
            ApplicationSetting.SetRecipe<eF_CaneraSetting>((int)eF_CaneraSetting.TxtBx_CCD_Name, config[axis].CCD_NAME);
            ApplicationSetting.SetRecipe<eF_CaneraSetting>((int)eF_CaneraSetting.TxtBx_ID_IP, config[axis].IP_ID);

            CameraSetting.UpdateParmeter();
        }

        #endregion

    }
}
