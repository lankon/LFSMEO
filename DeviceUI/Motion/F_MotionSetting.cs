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

namespace DeviceUI.Motion
{    
    public partial class F_MotionSetting : Form, IF_MotionSetting
    {
        public F_MotionSetting(IServiceProvider serviceProvider, IF_AxisButton f_AxisButton, IF_AxisSetting f_AxisSetting)
        {
            InitializeComponent();

            InitialForm();

            ServiceProvider = serviceProvider;
            AxisSetting = f_AxisSetting;
            AxisButton = f_AxisButton;
            DockAxisSetting(typeof(IF_AxisSetting));
            DockAxisButton(typeof(IF_AxisButton));
        }

        #region parameter define
        IServiceProvider ServiceProvider;
        IF_AxisButton AxisButton;
        IF_AxisSetting AxisSetting;
        #endregion

        #region private function
        private void InitialForm()
        {
            ApplicationSetting.ReadAllRecipe<eMotionSetting>();
            ApplicationSetting.UpdataRecipeToForm<eMotionSetting>(this);

            ShowHint();

            //if (ApplicationSetting.Get_Bool_Recipe<eMotionSetting>((int)eMotionSetting.Cmbx_ShowFormName) == true)
            //    Tool.ShowFormName(this, 1);    //可開選項設定是否顯示

        }
        private void ShowHint()
        {

        }
        #endregion

        #region public function
        public void DockAxisSetting(Type child_form)
        {
            object service = ServiceProvider.GetRequiredService(child_form);

            if (service is Form childForm)
            {
                Tool.SetForm(this.Pnl_AxisSetting, childForm);
                childForm.Show();
            }
        }
        public void DockAxisButton(Type child_form)
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
                ApplicationSetting.SaveRecipeFromForm<eMotionSetting>(this);
                //重新讀取變數值
                ApplicationSetting.ReadAllRecipe<eMotionSetting>();

                //釋放記憶體資源
                //Tool.ReleaseButtonImages(this);
                //this.Close();
                //this.Dispose();
            }
        }
    }
}
