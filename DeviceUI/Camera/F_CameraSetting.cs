using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
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

namespace DeviceUI.Camera
{
    public partial class F_CameraSetting : Form, IF_CameraSetting
    {
        public F_CameraSetting(IServiceProvider serviceProvider, IF_CameraButton f_CameraButton)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            CameraButton = f_CameraButton;

            InitialForm();
        }

        #region parameter define
        IServiceProvider ServiceProvider;
        IF_CameraButton CameraButton;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            DockCameraButton(typeof(IF_CameraButton));

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

        private void DockCameraButton(Type child_form)
        {
            object service = ServiceProvider.GetRequiredService(child_form);

            if (service is Form childForm)
            {
                Tool.SetForm(this.Pnl_CameraButton, childForm);
                childForm.Show();
            }
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {

        }
        public void UpdateParmeter()
        {
            ApplicationSetting.SaveAllRecipe<eF_CameraSetting>();
            ApplicationSetting.UpdataRecipeToForm<eF_CameraSetting>(this);
        }
        public void SaveCameraParameter()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_CameraSetting>(this);
            ApplicationSetting.ReadAllRecipe<eF_CameraSetting>();
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

    }
}
