using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ToolFunction;
using RGBTester.Base;
using Microsoft.Extensions.DependencyInjection;

namespace RGBTester.UI
{
    public partial class F_OEM_Setting : Form
    {
        public F_OEM_Setting(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
        }

        #region parameter define
        private IServiceProvider ServiceProvider;
        #endregion

        #region private function
        void InitialApplication()
        {
            ApplicationSetting.ReadAllRecipe<eOEMSetting>();
            ApplicationSetting.UpdataRecipeToForm<eOEMSetting>(this);

            ShowHint();
        }
        void ShowHint()
        {

        }
        #endregion

        #region public function
        #endregion

        

        private void Btn_IO_Form_Click(object sender, EventArgs e)
        {
            //Tool.HideElementOnPanel(Scope.MainPanel);

            //F_IO_Setting f_IO_Setting = new F_IO_Setting();
            //Tool.SetForm(Scope.MainPanel, f_IO_Setting);
            //f_IO_Setting.Show();
        }

        private void Btn_EquipmentSetting_Click(object sender, EventArgs e)
        {
            Tool.HideElementOnPanel(Scope.MainPanel);

            var form = ServiceProvider.GetRequiredService<F_Equipment_Setting>();
            Tool.SetForm(Scope.MainPanel, form);
            form.Show();
        }

        private void F_OEM_Setting_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                ApplicationSetting.SaveRecipeFromForm<eOEMSetting>(this);
                ApplicationSetting.ReadAllRecipe<eOEMSetting>();

                Tool.ReleaseButtonImages(this);

                this.Close();
                this.Dispose();
            }
        }

        private void Btn_MotionSetting_Click(object sender, EventArgs e)
        {
            //Tool.HideElementOnPanel(Scope.MainPanel);

            //F_MotionSetting f_Motion_Setting = new F_MotionSetting();
            //Tool.SetForm(Scope.MainPanel, f_Motion_Setting);
            //f_Motion_Setting.Show();
        }
    }
}
