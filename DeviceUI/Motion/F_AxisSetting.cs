using System;
using System.Windows.Forms;
using ToolFunction;
using DeviceCore;
using System.Collections.Generic;

namespace DeviceUI.Motion
{
    public partial class F_AxisSetting : Form, IF_MotionSetting
    {
        public F_AxisSetting(F_MotionSettingLogic f_MotionSettingLogic)
        {
            InitializeComponent();

            MotionSettingLogic = f_MotionSettingLogic;

            ApplicationSetting.ReadAllRecipe<eMotionSetting>();
            ApplicationSetting.UpdataRecipeToForm<eMotionSetting>(this);

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

            //if (ApplicationSetting.Get_Int_Recipe<eMotionSetting>((int)eMotionSetting.Cmbx_ShowFormName) == 1)
            //    Tool.ShowFormName(this);
        }
        #endregion

        #region public function
        public void UpdateParmeter()
        {
            ApplicationSetting.ReadAllRecipe<eMotionSetting>();
            ApplicationSetting.UpdataRecipeToForm<eMotionSetting>(this);
        }
        public void SaveAxisParameter()
        {
            ApplicationSetting.SaveRecipeFromForm<eMotionSetting>(this);
            ApplicationSetting.ReadAllRecipe<eMotionSetting>();

            int num = MotionSettingLogic.GetCurrentBtnNum();
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

            for (int i = 0; i < total_param.Length; i++)
            {
                set = ApplicationSetting.Get_String_Recipe<eMotionSetting>((int)total_param[i]);
                param.Add(total_param[i].ToString(), set);
            }

            //Scope.DML.SaveAxis(Application.StartupPath + @"\Setting\AxisConfig.xml", $"Axis{num}", param);
        }
        #endregion

        private void Btn_AllSetting_Click(object sender, EventArgs e)
        {
            SaveAxisParameter();
        }
    }
}
