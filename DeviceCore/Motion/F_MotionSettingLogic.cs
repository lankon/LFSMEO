using DeviceCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;

namespace DeviceCore
{
    public class F_MotionSettingLogic
    {
        public IF_AxisButton AxisButton;
        public IF_AxisSetting AxisSetting;
        public void SetAxisSettingForm(IF_AxisSetting f_AxisSetting)
        {
            AxisSetting = f_AxisSetting;
        }

        public void SetAxisButtonForm(IF_AxisButton f_AxisButton)
        {
            AxisButton = f_AxisButton;
        }

        public void UpdateParameter()
        {
            AxisSetting.UpdateParmeter();
        }

        public int GetCurrentBtnNum()
        {
            return AxisButton.GetCurrentBtnNum();
        }
    }
}
