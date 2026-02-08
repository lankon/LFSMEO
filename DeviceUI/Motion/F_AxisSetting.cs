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

            f_MotionSettingLogic.SetAxisSettingIF(this);
            MotionSettingLogic = f_MotionSettingLogic;

            ApplicationSetting.ReadAllRecipe<eF_AxisSetting>();
            //ApplicationSetting.UpdataRecipeToForm<eF_AxisSetting>(this);

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
            ApplicationSetting.SaveAllRecipe<eF_AxisSetting>();
            ApplicationSetting.UpdataRecipeToForm<eF_AxisSetting>(this);
        }
        public void SaveAxisParameter()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_AxisSetting>(this);
            ApplicationSetting.ReadAllRecipe<eF_AxisSetting>();
        }
        #endregion

        private void Btn_Homing_Click(object sender, EventArgs e)
        {
            MotionSettingLogic.GoHome();
        }

        private void Btn_Move_Click(object sender, EventArgs e)
        {
            MotionSettingLogic.PTP_MoveTest();
        }
    }
}
