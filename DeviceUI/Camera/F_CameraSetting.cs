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
                                IFunction_Camera function_Camera, F_CameraSettingLogic f_CameraSettingLogic)
        {
            InitializeComponent();

            IntPtr forceHandle = this.Handle;   //強制先建立Handle

            ServiceProvider = serviceProvider;
            CameraButton = f_CameraButton;
            Function_Camera = function_Camera;
            f_CameraSettingLogic.SetCameraSettingIF(this);
            CameraSettingLogic = f_CameraSettingLogic;

            InitialForm();
        }

        #region parameter define
        IServiceProvider ServiceProvider;
        IF_CameraButton CameraButton;
        IFunction_Camera Function_Camera;
        F_CameraSettingLogic CameraSettingLogic;
        CameraDisplayPanel[] DisplayPanels;
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

            //DockDisplayToPanel(Pnl_DockCameraDisplay);
            DockDisplayToLyPnl(LyPnl_DockCameraDisplay);
            SwitchToCameraDisplay(CameraButton.GetCurrentBtnNum());
            FitWindow(CameraButton.GetCurrentBtnNum());
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
        private void DockCameraDisplay()
        {
            //此Function程式執行後只能呼叫一次
            int length = CCD_NAME.GetNames(typeof(CCD_NAME)).Length;
            DisplayPanels = new CameraDisplayPanel[length];
            for (int i = 0; i < DisplayPanels.Length; i++)
            {
                DisplayPanels[i] = new CameraDisplayPanel(Function_Camera, i);
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
                Function_Camera.Subscribe(i, (s, fe) =>
                {
                    int realID = fe.CCD_Index;

                    if (DisplayPanels[realID].IsUpdating == true)   //UI還在更新,放棄更新畫面(丟禎) 
                        return;

                    if (fe.Frame == null || !fe.Frame.TryAddRef())
                        return;

                    DisplayPanels[realID].IsUpdating = true;

                    try
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                DisplayPanels[realID].CurrentFrame = fe.Frame;
                                DisplayPanels[realID].Update(); //呼叫UI立刻繪圖
                            }
                            finally
                            {
                                DisplayPanels[realID].IsUpdating = false;
                            }
                        }));
                    }
                    catch
                    {
                        fe.Frame.Dispose();
                        DisplayPanels[realID].IsUpdating = false;
                    }
                });
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
            if (DisplayPanels[ccd].InvokeRequired)
                DisplayPanels[ccd].Invoke(new Action(() => SwitchToCameraDisplay(ccd)));
            else
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
        public void DockDisplayToLyPnl(TableLayoutPanel tableLayoutPanel)
        {
            foreach (CameraDisplayPanel panel in DisplayPanels)
            {
                tableLayoutPanel.Controls.Add(panel, 0, 0);
                panel.Dock = DockStyle.Fill;
            }
        }
        public void FitWindow(int ccd)
        {
            DisplayPanels[ccd].FitWindow();
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

            //SwitchToCameraDisplay(index);
            Function_Camera.StartLive(index);
            //Function_Camera.StartGrab(index);
            //Function_Camera.SoftTrigger(index);
            //Function_Camera.GetImageDisplay(index, $@"C:\Users\lankon\Desktop\tmep\picture{index}.png");

            DisplayPanels[index].FitWindow();
        }

        private void Btn_FunctionTest1_Click(object sender, EventArgs e)
        {
            int index = CameraButton.GetCurrentBtnNum();
            Function_Camera.StopLive(index);
        }
    }
}
