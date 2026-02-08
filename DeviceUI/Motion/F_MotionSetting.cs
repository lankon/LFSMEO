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
        private void UpdatePage()
        {
            MotionSettingLogic.UpdateAxisInfo2Form(0);
        }
        #endregion

        #region public function
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


        private void F_Template_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                //儲存參數
                ApplicationSetting.SaveRecipeFromForm<eF_AxisSetting>(this);
                //重新讀取變數值
                ApplicationSetting.ReadAllRecipe<eF_AxisSetting>();

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
    }
}
