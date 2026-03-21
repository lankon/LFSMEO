using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;
using DeviceCore;

namespace DeviceFunction
{
    public class F_CameraSettingLogic
    {
        public F_CameraSettingLogic(IFunction_Camera function_Camera)
        {
            Function_Camera = function_Camera;
        }

        #region parameter function
        IFunction_Camera Function_Camera;
        IF_CameraSetting CameraSetting;
        IF_CameraButton CameraButton;
        eF_CameraSetting[] CameraParam = new eF_CameraSetting[]
            {
                //新增相機參數時需添加
                
                //[Connect Config]
                eF_CameraSetting.Cmbx_AxisType,
                eF_CameraSetting.Cmbx_AxisUse,
                eF_CameraSetting.TxtBx_ID_IP,
                eF_CameraSetting.TxtBx_CCD_Name,
            };
        #endregion

        #region public function

        #region [Set Form Interface]
        //[Set Form Interface]
        public void SetCameraSettingIF(IF_CameraSetting f_CameraSetting)
        {
            CameraSetting = f_CameraSetting;
        }
        public void SetCameraButtonIF(IF_CameraButton f_CameraButton)
        {
            CameraButton = f_CameraButton;
        }
        #endregion

        #region [Save & Update Axis Info]
        //[Save & Update Axis Info]
        public void UpdateCameraInfo2Form(int axis)
        {
            //新增相機參數時需添加

            Function_Camera.LoadCameraConfig();
            var config = Function_Camera.GetCameraConfig();

            //[Connect Configuration]
            ApplicationSetting.SetRecipe<eF_CameraSetting>((int)eF_CameraSetting.Cmbx_AxisType, config[axis].CCD_TYPE.ToString());
            ApplicationSetting.SetRecipe<eF_CameraSetting>((int)eF_CameraSetting.Cmbx_AxisUse, config[axis].CCD_USE.ToString());
            ApplicationSetting.SetRecipe<eF_CameraSetting>((int)eF_CameraSetting.TxtBx_CCD_Name, config[axis].CCD_NAME);
            ApplicationSetting.SetRecipe<eF_CameraSetting>((int)eF_CameraSetting.TxtBx_ID_IP, config[axis].IP_ID);

            CameraSetting.UpdateParmeter();
        }
        public void SaveCamera()
        {
            CameraSetting.SaveCameraParameter();

            int num = GetCurrentBtnNum();
            string set;

            Dictionary<string, string> param = new Dictionary<string, string>();
            for (int i = 0; i < CameraParam.Length; i++)
            {
                set = ApplicationSetting.Get_String_Recipe<eF_CameraSetting>((int)CameraParam[i]);
                param.Add(CameraParam[i].ToString(), set);
            }

            string cameraName = $"Camera{num}";
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            Function_Camera.SaveCameraConfig(AppPath + @"\Setting\CameraConfig.xml", cameraName, param);
        }
        public bool SaveAndLoadCameraConfig()
        {
            SaveCamera();
            bool res = Function_Camera.LoadCameraConfig();

            return res;
        }
        #endregion


        public int GetCurrentBtnNum()
        {
            return CameraButton.GetCurrentBtnNum();
        }
        #endregion

    }
}
