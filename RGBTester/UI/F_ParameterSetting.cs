using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using RGBTester.Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolFunction;
using static RGBTester.Logic.TestResultDataBase;

namespace RGBTester.UI
{
    public partial class F_ParameterSetting : Form, IF_ParameterSetting
    {
        public F_ParameterSetting(IServiceProvider serviceProvider, F_ParameterSettingLogic f_ParameterSettingLogic)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            ParameterSettingLogic1 = f_ParameterSettingLogic;

            InitialForm();
        }

        #region parameter define
        IServiceProvider ServiceProvider;
        F_ParameterSettingLogic ParameterSettingLogic1;
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
            ApplicationSetting.ReadAllRecipe<eF_ParameterSetting>();
            //ApplicationSetting.ReadAllRecipe<eF_ParameterSetting>();

            string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
            ApplicationSetting.ReadAllRecipe<eF_ParameterSettingRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            ApplicationSetting.UpdataRecipeToForm<eF_ParameterSetting>(this);
            ApplicationSetting.UpdataRecipeToForm<eF_ParameterSettingRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_ParameterSetting>(this);

            string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
            ApplicationSetting.SaveRecipeFromForm<eF_ParameterSettingRecipe>(this, recipe_name);
        }
        private void UpdatePage()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();
        }
        #endregion

        #region public function
        #endregion

        private void F_Equipment_Setting_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                SaveAllEnumSetting();
                ReadAllEnumSetting();

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
    }
}
