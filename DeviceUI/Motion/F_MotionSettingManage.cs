using DeviceCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ToolFunction;

namespace DeviceUI.Motion
{
    public interface IF_MotionSetting
    {
        void SetMediator(F_MotionSettingManage med);
    }
    
    public class F_MotionSettingManage
    {
        public F_AxisSetting f_AxisSetting = null;
        public F_AxisButton f_AxisButton = null;

        public void SetForm(IF_MotionSetting form)
        {
            form.SetMediator(this);
            
            if (form is F_AxisSetting setting)
                f_AxisSetting = setting;
            else if (form is F_AxisButton button)
                f_AxisButton = button;
        }

        public void UpdateParameter()
        {
            f_AxisSetting.UpdateParmeter();
        }

        public void SaveAxisParameter()
        {
            ApplicationSetting.SaveRecipeFromForm<eMotionSetting>(f_AxisSetting);
            ApplicationSetting.ReadAllRecipe<eMotionSetting>();

            int num = f_AxisButton.GetCurrentBtnNum();
            string set;

            Dictionary<string, string> param = new Dictionary<string, string>();

            eMotionSetting[] total_param = new eMotionSetting[]
            { 
                eMotionSetting.Cmbx_AxisType,
                eMotionSetting.TxtBx_AxisStation,
                eMotionSetting.Cmbx_AxisUse,
                eMotionSetting.Cmbx_AxisLimitLogic,
                eMotionSetting.Cmbx_AxisLimitStopMode,
            };

            for(int i=0; i<total_param.Length; i++)
            {
                set = ApplicationSetting.Get_String_Recipe<eMotionSetting>((int)total_param[i]);
                param.Add(total_param[i].ToString(), set);
            }

            //Scope.DML.SaveAxis(Application.StartupPath + @"\Setting\AxisConfig.xml", $"Axis{num}", param);
        }
    }
}
