using LFSMEO.Base_LFSMEO;
using System;
using System.Windows.Forms;
using ToolFunction;


namespace DeviceUI.Motion
{
    public partial class F_AxisSetting : Form, IF_MotionSetting
    {
        #region parameter define
        F_MotionSettingManage f_MotionSettingManage;
        #endregion

        #region private function
        private void SetHint()
        {
            //toolTip1.SetToolTip(Btn_OEM_Setting, "OEM Setting");
        }
        private void InitialForm()
        {
            SetHint();

            if (ApplicationSetting.Get_Int_Recipe<eMotionSetting>((int)eMotionSetting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);
        }
        #endregion

        #region public function
        public void SetMediator(F_MotionSettingManage med)
        {
            f_MotionSettingManage = med;
        }
        public void UpdateParmeter()
        {
            ApplicationSetting.ReadAllRecipe<eMotionSetting>();
            ApplicationSetting.UpdataRecipeToForm<eMotionSetting>(this);
        }
        #endregion

        public F_AxisSetting()
        {
            InitializeComponent();

            ApplicationSetting.ReadAllRecipe<eMotionSetting>();
            ApplicationSetting.UpdataRecipeToForm<eMotionSetting>(this);

            InitialForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Btn_AllSetting_Click(object sender, EventArgs e)
        {
            
            
            f_MotionSettingManage.SaveAxisParameter();
        }
    }
}
