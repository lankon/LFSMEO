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
using RGBTester.UI;
using RGBTester.Base;

namespace RGBTester.UI
{
    public partial class F_DAQ_ChartSetting : Form
    {
        public F_DAQ_ChartSetting(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            InitialForm();
            ServiceProvider = serviceProvider;
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
        void ShowHint()
        {

        }
        private void ReadAllEnumSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_DAQ_ChartSetting>();
            //ApplicationSetting.ReadAllRecipe<eF_StartForm>();

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.ReadAllRecipe<eF_StartFormRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            ApplicationSetting.UpdataRecipeToForm<eF_DAQ_ChartSetting>(this);
            //ApplicationSetting.UpdataRecipeToForm<eF_StartFormRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_DAQ_ChartSetting>(this);

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.SaveRecipeFromForm<eF_StartFormRecipe>(this, recipe_name);
        }
        private void UpdatePage()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();
        }
        private void LeavePage()
        {
        }
        private void InvokeShowForm(Form form)
        {
            this.TopMost = false;           // 顯示在最上層（避免被遮）
            this.ShowInTaskbar = false;     //不顯示於工具列
            this.Owner = form;              // 指定主窗
            this.StartPosition = FormStartPosition.Manual;

            int x = 10;
            x = form.Left + (form.Width / 2 - this.Width / 2);

            int y = form.Top + (form.Height - this.Height) / 2;
            this.Location = new Point(x, y);

            this.Show();
        }
        private void UpdateUI(string msg, Form form)
        {
            InvokeShowForm(form);
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {

        }
        public void ShowSetting(string msg)
        {
            var main_form = ServiceProvider.GetRequiredService<F_MainForm>();

            if (!main_form.IsHandleCreated)
                _ = main_form.Handle;

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    UpdateUI(msg, main_form);
                }));
            }
            else
            {
                UpdateUI(msg, main_form);
            }
        }
        #endregion
        private void F_Equipment_Setting_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                SaveAllEnumSetting();
                ReadAllEnumSetting();

                LeavePage();
                ////釋放記憶體資源
                Tool.ReleaseButtonImages(this);
                this.Close();
                this.Dispose();
            }
            else
            {
                UpdatePage();
            }
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            SaveAllEnumSetting();
            ReadAllEnumSetting();

            var main_form = ServiceProvider.GetRequiredService<F_MainForm>();

            // 顯示到最上層
            main_form.Activate();
            main_form.BringToFront();

            this.Hide();
        }
    }
}
