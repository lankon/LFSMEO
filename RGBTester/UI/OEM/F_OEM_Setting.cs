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
using RGBTester.Base;
using RGBTester.Logic;
using System.IO;

namespace RGBTester.UI
{
    public partial class F_OEM_Setting : Form
    {
        public F_OEM_Setting(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;

            InitialForm();
        }

        #region parameter define
        private IServiceProvider ServiceProvider;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);
        }
        private void ShowHint()
        {

        }
        private void ReadAllEnumSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();
        }
        private void UpdateEnumSettingToForm()
        {
            ApplicationSetting.UpdataRecipeToForm<eF_Equipment_Setting>(this);
        }
        private void SaveAllEnumSetting()
        {
            //ApplicationSetting.SaveRecipeFromForm<eF_StartForm>(this);

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.SaveRecipeFromForm<eF_StartFormRecipe>(this, recipe_name);
        }
        private void UpdatePage()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();
        }
        #endregion

        #region public function
        #endregion

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
                ApplicationSetting.SaveRecipeFromForm<eF_Equipment_Setting>(this);
                ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();

                Tool.ReleaseButtonImages(this);

                this.Close();
                this.Dispose();
            }
        }

        private void Btn_MotionSetting_Click(object sender, EventArgs e)
        {
            var oem_set = ServiceProvider.GetRequiredService<IF_MotionSetting>();

            if(oem_set is Form form)
            {
                Tool.HideElementOnPanel(Scope.MainPanel);
                Tool.SetForm(Scope.MainPanel, form);
                form.Show();

                if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                    oem_set.ShowFormName(true);
            }
        }

        private void Btn_IO_Setting_Click(object sender, EventArgs e)
        {
            var io_set = ServiceProvider.GetRequiredService<IF_IO_Card>();

            if (io_set is Form form)
            {
                Tool.HideElementOnPanel(Scope.MainPanel);
                Tool.SetForm(Scope.MainPanel, form);
                form.Show();
            }
        }

        private void Btn_DAQ_SamplingTest_Click(object sender, EventArgs e)
        {
            Tool.HideElementOnPanel(Scope.MainPanel);

            var form = ServiceProvider.GetRequiredService<F_DAQ_SamplingTest>();
            Tool.SetForm(Scope.MainPanel, form);
            form.Show();
        }

        private void Btn_Spectrometer_Click(object sender, EventArgs e)
        {
            var spec_form = ServiceProvider.GetRequiredService<IF_Spectrometer>();

            if (spec_form is Form form)
            {
                Tool.HideElementOnPanel(Scope.MainPanel);
                Tool.SetForm(Scope.MainPanel, form);
                form.Show();

                if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                    spec_form.ShowFormName(true);
            }



            //var spec = ServiceProvider.GetRequiredService<IFunction_Spectrometer>();
            //spec.Initial_All_Spectrometer();

            //float[] spectrum = null;
            //spectrum = spec.GetSpectrumOneShot(100);

            //if (spectrum == null)
            //    return;

            //StreamWriter file = Tool.CreateFile("\\Result\\spectrum", ".csv", false);
            //for (int i = 0; i < spectrum.Length; i++)
            //{
            //    Tool.WriteFile(file, spectrum[i].ToString());
            //}
            //Tool.CloseFile(file);
        }

        private void Btn_Light_Click(object sender, EventArgs e)
        {
            var light_set = ServiceProvider.GetRequiredService<IF_LightControl>();

            if (light_set is Form form)
            {
                Tool.HideElementOnPanel(Scope.MainPanel);
                Tool.SetForm(Scope.MainPanel, form);
                form.Show();

                if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                    light_set.ShowFormName(true);
            }
        }
    }
}
