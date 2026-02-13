using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using DeviceCore;
using DeviceFunction;

namespace DeviceUI.Motion
{    
    public partial class F_MotionSetting : Form, IF_MotionSetting
    {
        public F_MotionSetting(IServiceProvider serviceProvider, IF_AxisButton f_AxisButton, IF_AxisSetting f_AxisSetting,
                                F_MotionSettingLogic f_MotionSettingLogic)
        {
            InitializeComponent();

            InitialForm();

            ServiceProvider = serviceProvider;
            AxisSetting = f_AxisSetting;
            AxisButton = f_AxisButton;
            MotionSettingLogic = f_MotionSettingLogic;
            DockAxisSetting(typeof(IF_AxisSetting));
            DockAxisButton(typeof(IF_AxisButton));
        }

        #region parameter define
        IServiceProvider ServiceProvider;
        IF_AxisButton AxisButton;
        IF_AxisSetting AxisSetting;
        F_MotionSettingLogic MotionSettingLogic;
        #endregion

        #region private function
        private void InitialForm()
        {
            ApplicationSetting.ReadAllRecipe<eF_AxisSetting>();
            ApplicationSetting.UpdataRecipeToForm<eF_AxisSetting>(this);

            ShowHint();

            //if (ApplicationSetting.Get_Bool_Recipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_ShowFormName) == true)
            //    Tool.ShowFormName(this, 1);    //可開選項設定是否顯示

        }
        private void ShowHint()
        {

        }
        private void StartUpdateStatus(bool start)
        {
            if (start)
                Timer_UpdateStatus.Start();
            else
                Timer_UpdateStatus.Stop();
        }
        private void StartUpdateStatusInvoke(bool start)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    StartUpdateStatus(start);
                }));
            }
            else
            {
                StartUpdateStatus(start);
            }
        }
        private void UpdatePage()
        {
            MotionSettingLogic.UpdateAxisInfo2Form(MotionSettingLogic.GetCurrentBtnNum());
            AxisButton.StartUpdatePositionInvoke(true);
            StartUpdateStatusInvoke(true);
        }
        private void LeavePage()
        {
            AxisButton.StartUpdatePositionInvoke(false);
            StartUpdateStatusInvoke(false);
        }
        private void DockAxisSetting(Type child_form)
        {
            object service = ServiceProvider.GetRequiredService(child_form);

            if (service is Form childForm)
            {
                Tool.SetForm(this.Pnl_AxisSetting, childForm);
                childForm.Show();
            }
        }
        private void DockAxisButton(Type child_form)
        {
            object service = ServiceProvider.GetRequiredService(child_form);

            if (service is Form childForm)
            {
                Tool.SetForm(this.Pnl_AxisButton, childForm);
                childForm.Show();
            }
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {
            if(show)
            {
                Tool.ShowFormName(this);
                AxisButton.ShowFormName(true);
                AxisSetting.ShowFormName(true);
            }
        }
        #endregion


        private void F_Template_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                //儲存參數
                ApplicationSetting.SaveRecipeFromForm<eF_AxisSetting>(this);
                //重新讀取變數值
                ApplicationSetting.ReadAllRecipe<eF_AxisSetting>();
                
                LeavePage();

                //釋放記憶體資源
                //Tool.ReleaseButtonImages(this);
                //this.Close();
                //this.Dispose();
            }
            else
            {
                UpdatePage();
            }
        }

        private void Timer_UpdateStatus_Tick(object sender, EventArgs e)
        {
            IFunction_MotionCard func = ServiceProvider.GetRequiredService<IFunction_MotionCard>();
            int axis_num = AxisButton.GetCurrentBtnNum();

            Label[] labl_status = new Label[] { Labl_Alarm, Labl_PEL, Labl_MEL,
                                                Labl_ORG, Labl_Servo, Labl_INP,
                                                Labl_RDY};

            func.GetMotionStatus(axis_num, out bool[] status);

            for (int i = 0; i < status.Length; i++)
            {
                if (status[i] == true)
                    labl_status[i].BackColor = Color.LimeGreen;
                else
                    labl_status[i].BackColor = Color.White;
            }
        }
    }
}
