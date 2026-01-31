using DeviceCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using ToolFunction;

namespace DeviceFunction
{
    public class F_MotionSettingLogic
    {
        public F_MotionSettingLogic(IFunction_MotionCard function_MotionCard)
        {
            Function_MotionCard = function_MotionCard;
        }

        #region parameter define
        public IF_MotionSetting MotionSetting;
        public IFunction_MotionCard Function_MotionCard;
        public IF_AxisButton AxisButton;
        public IF_AxisSetting AxisSetting;
        eF_AxisSetting[] AxisParam = new eF_AxisSetting[]
            {
                eF_AxisSetting.Cmbx_AxisType,
                eF_AxisSetting.TxtBx_AxisStation,
                eF_AxisSetting.Cmbx_AxisUse,
                eF_AxisSetting.Cmbx_AxisLimitLogic,
                eF_AxisSetting.Cmbx_AxisLimitStopMode,
            };
        #endregion

        #region public function
        //[Set Form Interface]
        public void SetAxisButtonIF(IF_AxisButton f_AxisButton)
        {
            AxisButton = f_AxisButton;
        }

        public void SetAxisSettingIF(IF_AxisSetting f_AxisSetting)
        {
            AxisSetting = f_AxisSetting;
        }

        //[Save & Update Axis Info]
        public void UpdateAxisInfo2Form(int axis)
        {
            Function_MotionCard.LoadAxisConfig();
            var config = Function_MotionCard.GetAxisConfig();

            //[Axis Configuration]
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_AxisType, config[axis].AXIS_TYPE);
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_LineNo, config[axis].LINE_NO.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.TxtBx_AxisStation, config[axis].DEV_NO.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_AxisUse, config[axis].AXIS_USE.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_AxisLimitLogic, config[axis].LIMIT_LOGIC.ToString());
            ApplicationSetting.SetRecipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_AxisLimitStopMode, config[axis].STOP_MODE.ToString());

            AxisSetting.UpdateParmeter();
        }

        public void SaveAxis()
        {
            AxisSetting.SaveAxisParameter();

            int num = GetCurrentBtnNum();
            string set;

            Dictionary<string, string> param = new Dictionary<string, string>();
            for (int i = 0; i < AxisParam.Length; i++)
            {
                set = ApplicationSetting.Get_String_Recipe<eF_AxisSetting>((int)AxisParam[i]);
                param.Add(AxisParam[i].ToString(), set);
            }

            string axisName = $"Axis{num}";
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            Function_MotionCard.SaveAxisConfig(AppPath + @"\Setting\AxisConfig.xml", axisName, param);
        }

        //[Motion]
        public void GoHome()
        {
            SaveAxis();
            Function_MotionCard.LoadAxisConfig();

            Task.Run(async () =>
            { 
                await Function_MotionCard.GoHome(GetCurrentBtnNum());
            });
        }

        public int GetCurrentBtnNum()
        {
            return AxisButton.GetCurrentBtnNum();
        }

        
        #endregion
    }
}
