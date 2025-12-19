using System;
using System.Windows.Forms;
using ToolFunction;
using DeviceCore;
using System.Collections.Generic;

using DeviceFunction;

namespace DeviceUI.Motion
{
    public partial class F_AxisSetting : Form, IF_AxisSetting
    {
        public F_AxisSetting(F_MotionSettingLogic f_MotionSettingLogic)
        {
            InitializeComponent();

            MotionSettingLogic = f_MotionSettingLogic;

            ApplicationSetting.ReadAllRecipe<eF_AxisSetting>();
            ApplicationSetting.UpdataRecipeToForm<eF_AxisSetting>(this);

            InitialForm();
        }

        #region parameter define
        F_MotionSettingLogic MotionSettingLogic;
        #endregion

        #region private function
        private void SetHint()
        {
            //toolTip1.SetToolTip(Btn_OEM_Setting, "OEM Setting");
        }
        private void InitialForm()
        {
            SetHint();

            //if (ApplicationSetting.Get_Int_Recipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_ShowFormName) == 1)
            //    Tool.ShowFormName(this);
        }
        #endregion

        #region public function
        public void UpdateParmeter()
        {
            ApplicationSetting.ReadAllRecipe<eF_AxisSetting>();
            ApplicationSetting.UpdataRecipeToForm<eF_AxisSetting>(this);
        }
        public void SaveAxisParameter()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_AxisSetting>(this);
            ApplicationSetting.ReadAllRecipe<eF_AxisSetting>();

            int num = MotionSettingLogic.GetCurrentBtnNum();
            string set;

            Dictionary<string, string> param = new Dictionary<string, string>();

            eF_AxisSetting[] total_param = new eF_AxisSetting[]
            {
                eF_AxisSetting.Cmbx_AxisType,
                eF_AxisSetting.TxtBx_AxisStation,
                eF_AxisSetting.Cmbx_AxisUse,
                eF_AxisSetting.Cmbx_AxisLimitLogic,
                eF_AxisSetting.Cmbx_AxisLimitStopMode,
            };

            for (int i = 0; i < total_param.Length; i++)
            {
                set = ApplicationSetting.Get_String_Recipe<eF_AxisSetting>((int)total_param[i]);
                param.Add(total_param[i].ToString(), set);
            }

            MotionSettingLogic.SaveAxis(Application.StartupPath + @"\Setting\AxisConfig.xml", $"Axis{num}", param);
        }
        #endregion

        private void Btn_AllSetting_Click(object sender, EventArgs e)
        {
            SaveAxisParameter();
        }
    }
}
