using System;
using System.Windows.Forms;
using ToolFunction;
using DeviceCore;
using System.Collections.Generic;

using DeviceFunction;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceUI.Motion
{
    public partial class F_AxisSetting : Form, IF_AxisSetting
    {
        public F_AxisSetting(IServiceProvider serviceProvider, F_MotionSettingLogic f_MotionSettingLogic)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            f_MotionSettingLogic.SetAxisSettingIF(this);
            MotionSettingLogic = f_MotionSettingLogic;

            ApplicationSetting.ReadAllRecipe<eF_AxisSetting>();
            //ApplicationSetting.UpdataRecipeToForm<eF_AxisSetting>(this);

            InitialForm();
        }

        #region parameter define
        F_MotionSettingLogic MotionSettingLogic;
        IServiceProvider ServiceProvider;
        IFunction_MotionCard DML;
        #endregion

        #region private function
        private void SetHint()
        {
            //toolTip1.SetToolTip(Btn_OEM_Setting, "OEM Setting");
        }
        private void InitialForm()
        {
            SetHint();

            DML = ServiceProvider.GetRequiredService<IFunction_MotionCard>();
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
        public void ShowFormName(bool show)
        {
            if (show)
                Tool.ShowFormName(this, 1);
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

        private void Btn_MoveMode_Click(object sender, EventArgs e)
        {
            SINGLE_MOVE_MODE current_mode = DML.GetSingleMoveMode();
            int next_mode = ((int)current_mode + 1) % Enum.GetNames(typeof(SINGLE_MOVE_MODE)).Length;

            if ((SINGLE_MOVE_MODE)next_mode == SINGLE_MOVE_MODE.CONTINUOUS_SLOW)
                Btn_MoveMode.Text = "Slow";
            else if ((SINGLE_MOVE_MODE)next_mode == SINGLE_MOVE_MODE.CONTINUOUS_FAST)
                Btn_MoveMode.Text = "Fast";
            else if ((SINGLE_MOVE_MODE)next_mode == SINGLE_MOVE_MODE.CONTINUOUS_NORMAL)
                Btn_MoveMode.Text = "Normal";
            else
                Btn_MoveMode.Text = "Index";

            DML.SetSingleMoveMode((SINGLE_MOVE_MODE)next_mode);
        }

        private void Btn_PositiveMove_MouseDown(object sender, MouseEventArgs e)
        {
            double.TryParse(TxtBx_IndexStep.Text, out double distence);
            bool res = DML.SingleMove(MotionSettingLogic.GetCurrentBtnNum(), 0, distence);
        }

        private void Btn_PositiveMove_MouseUp(object sender, MouseEventArgs e)
        {
            if (DML.GetSingleMoveMode() == SINGLE_MOVE_MODE.INDEX)
                return;
            
            bool res = DML.StopAxisMove(MotionSettingLogic.GetCurrentBtnNum());
        }

        private void Btn_NegativeMove_MouseDown(object sender, MouseEventArgs e)
        {
            double.TryParse(TxtBx_IndexStep.Text, out double distence);
            bool res = DML.SingleMove(MotionSettingLogic.GetCurrentBtnNum(), 1, distence * -1);
        }
    }
}
