using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using ToolFunction;
using DeviceCore;
using DeviceFunction;

namespace DeviceUI.Camera
{
    public partial class F_CameraSetting : Form, IF_CameraSetting
    {
        public F_CameraSetting(IServiceProvider serviceProvider, IF_CameraButton f_CameraButton,
                               IFunction_Camera function_Camera, ICameraDisplayService cameraDisplayService,
                               F_CameraSettingLogic f_CameraSettingLogic)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            CameraButton = f_CameraButton;
            Function_Camera = function_Camera;
            CameraSettingLogic = f_CameraSettingLogic;
            CameraDisplayService = cameraDisplayService;
            f_CameraSettingLogic.SetCameraSettingIF(this);

            InitialForm();
        }

        #region parameter define
        IServiceProvider ServiceProvider;
        IF_CameraButton CameraButton;
        IFunction_Camera Function_Camera;
        ICameraDisplayService CameraDisplayService;
        F_CameraSettingLogic CameraSettingLogic;
        bool isMouseInCameraConfig;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            DockCameraButton(typeof(IF_CameraButton));
            CameraDisplayService.DockDisplayToPanel(Pnl_DockCameraDisplay);

            BindCameraConfigMouseEvents(TbCtrl_CCD_Config);

            //if (ApplicationSetting.Get_Int_Recipe<eOEMSetting>((int)eOEMSetting.Cmbx_ShowFormName) == 1)
            //    Tool.ShowFormName(this);
        }
        private void BindCameraConfigMouseEvents(Control control)
        {
            control.MouseEnter += (sender, e) => isMouseInCameraConfig = true;
            control.MouseLeave += CameraConfig_MouseLeave;

            foreach (Control child in control.Controls)
                BindCameraConfigMouseEvents(child);
        }
        private void CameraConfig_MouseLeave(object sender, EventArgs e)
        {
            if (IsDisposed || Disposing || !IsHandleCreated)
                return;

            // 等內部控制項的滑鼠事件處理完，再確認是否離開整個設定區。
            BeginInvoke((Action)(() =>
            {
                if (IsDisposed || Disposing || !Visible || !TbCtrl_CCD_Config.Visible)
                    return;

                if (isMouseInCameraConfig &&
                    !TbCtrl_CCD_Config.RectangleToScreen(TbCtrl_CCD_Config.ClientRectangle).Contains(Control.MousePosition))
                {
                    isMouseInCameraConfig = false;
                    CameraSettingLogic.SaveCamera();
                }
            }));
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
            CameraButton.UpdateButtonNameInvoke();

            CameraDisplayService.DockDisplayToPanel(Pnl_DockCameraDisplay);
            //CameraDisplayService.DockDisplayToLyPnl(LyPnl_DockCameraDisplay);
            CameraDisplayService.SwitchToCameraDisplay(CameraButton.GetCurrentBtnNum());
            CameraDisplayService.FitWindow(CameraButton.GetCurrentBtnNum());
            //ReadAllEnumSetting();
            //UpdateEnumSettingToForm();
        }
        private void LeavePage()
        {
            int length = CCD_NAME.GetNames(typeof(CCD_NAME)).Length;

            for(int i=0; i<length; i++)
            {
                Function_Camera.StopLive(i);
            }
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

        private void Btn_FunctionTest_Click(object sender, EventArgs e)
        {
            int index = CameraButton.GetCurrentBtnNum();

            //CameraDisplayService.SwitchToCameraDisplay(index);
            Function_Camera.StartLive(index);
            //Function_Camera.StartGrab(index);
            //Function_Camera.SoftTrigger(index);
            //Function_Camera.GetImageDisplay(index, $@"C:\Users\lankon\Desktop\tmep\picture{index}.png");

            CameraDisplayService.FitWindow(index);
        }

        private void Btn_FunctionTest1_Click(object sender, EventArgs e)
        {
            int index = CameraButton.GetCurrentBtnNum();
            Function_Camera.StopLive(index);
        }
    }
}
