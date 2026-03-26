using Microsoft.Extensions.DependencyInjection;
using RGBTester.Logic;
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

namespace RGBTester.UI
{
    public partial class F_FunctionTester : Form
    {
        public F_FunctionTester(IServiceProvider serviceProvider, F_FunctionTesterLogic f_FunctionTesterLogic)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            FunctionTesterLogic = f_FunctionTesterLogic;

            InitialForm();
        }

        #region parameter define
        IServiceProvider ServiceProvider;
        F_FunctionTesterLogic FunctionTesterLogic;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            FunctionTesterLogic.SetVirtual_IO_Rule();

            ShowHint();

            //if (ApplicationSetting.Get_Int_Recipe<eOEMSetting>((int)eOEMSetting.Cmbx_ShowFormName) == 1)
            //    Tool.ShowFormName(this);
        }
        void ShowHint()
        {

        }
        private void ReadAllEnumSetting()
        {
            //ApplicationSetting.ReadAllRecipe<eOEMSetting>();
            //ApplicationSetting.ReadAllRecipe<eF_StartForm>();

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.ReadAllRecipe<eF_StartFormRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            //ApplicationSetting.UpdataRecipeToForm<eF_StartForm>(this);
            //ApplicationSetting.UpdataRecipeToForm<eF_StartFormRecipe>(this);
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
        private void LeavePage()
        {
        }
        private void ShowForm<T>() where T : class
        {
            var startForm = ServiceProvider.GetRequiredService<T>();
            if (startForm is Form form)
            {
                Tool.HideElementOnPanel(Scope.MainPanel);
                Tool.SetForm(Scope.MainPanel, form);
                form.Show();
            }
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {

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
                //Tool.ReleaseButtonImages(this);
                //this.Close();
                //this.Dispose();
            }
            else
            {
                UpdatePage();
            }
        }

        private void Btn_ElectricalFrom_Click(object sender, EventArgs e)
        {
            ShowForm<IF_StartForm>();
        }
    }
}
