using DeviceCore;
using DeviceFunction;
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
        public F_CameraSetting(IServiceProvider serviceProvider, IF_CameraButton f_CameraButton,
                                IFunction_Camera function_Camera, F_CameraSettingLogic f_CameraSettingLogic)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            CameraButton = f_CameraButton;
            Function_Camera = function_Camera;
            f_CameraSettingLogic.SetCameraSettingIF(this);
            CameraSettingLogic = f_CameraSettingLogic;

            InitialForm();
        }

        #region parameter define
        CameraDisplayPanel[] DisplayPanels;
        IServiceProvider ServiceProvider;
        IF_CameraButton CameraButton;
        F_CameraSettingLogic CameraSettingLogic;
        IFunction_Camera Function_Camera;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            DockCameraButton(typeof(IF_CameraButton));
            DockCameraDisplay();

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
            CameraSettingLogic.UpdateCameraInfo2Form(CameraSettingLogic.GetCurrentBtnNum());

            DockDisplayToPanel(Pnl_DockCameraDisplay);
            SwitchToCameraDisplay(CameraButton.GetCurrentBtnNum());

            //ReadAllEnumSetting();
            //UpdateEnumSettingToForm();
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
        private void DockCameraDisplay()
        {
            //此Function程式執行後只能呼叫一次
            int length = CCD_NAME.GetNames(typeof(CCD_NAME)).Length;
            DisplayPanels = new CameraDisplayPanel[length];
            for (int i = 0; i < DisplayPanels.Length; i++)
            {
                DisplayPanels[i] = new CameraDisplayPanel(i);
            }

            DockDisplayToPanel(Pnl_DockCameraDisplay);
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {

        }
        public void BindingDisplayEvent()
        {
            //此Fcuntion程式執行後只能呼叫一次
            int i = 0;

            foreach (CameraDisplayPanel panel in DisplayPanels)
            {
                Function_Camera.OnImageUpdates[i] += (s, fe) =>
                {
                    int realID = fe.CCD_Index;

                    Bitmap bmp = DisplayPanels[realID].CreateUniversalBitmap(
                        fe.Width, fe.Height, fe.ImageData, fe.Format);

                    if (this.InvokeRequired)
                    {
                        int index = i;
                        this.BeginInvoke(new Action(() =>
                        {
                            DisplayPanels[realID].CurrentImage = bmp;
                        }));
                    }
                    else
                    {
                        DisplayPanels[realID].CurrentImage = bmp;
                    }
                };
                i++;
            }
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
        public void SwitchToCameraDisplay(int ccd)
        {
            DisplayPanels[ccd].BringToFront();
        }
        public void DockDisplayToPanel(object container)
        {
            if (container is Control parent)
            {
                foreach (CameraDisplayPanel panel in DisplayPanels)
                {
                    parent.Controls.Add(panel);
                    panel.Dock = DockStyle.Fill;
                }
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
                //Tool.ReleaseButtonImages(this);
                //this.Close();
                //this.Dispose();
            }
            else
            {
                UpdatePage();
            }
        }

        private void Btn_FunctionTest_Click(object sender, EventArgs e)
        {
            int index = CameraButton.GetCurrentBtnNum();

            SwitchToCameraDisplay(index);
            Function_Camera.StartGrab(index);
            Function_Camera.SoftTrigger(index);
            Function_Camera.GetImageDisplay(index);
        }
    }
}
