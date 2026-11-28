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
using DeviceCore;
using DeviceFunction;

namespace DeviceUI.Camera
{
    public partial class F_CameraButton : Form, IF_CameraButton
    {
        public F_CameraButton(ICameraDisplayService cameraDisplayService, IFunction_Camera function_Camera,
                              F_CameraSettingLogic f_CameraSettingLogic)
        {
            InitializeComponent();
            InitialForm();

            f_CameraSettingLogic.SetCameraButtonIF(this);            

            CameraSettingLogic = f_CameraSettingLogic;
            DisplayService = cameraDisplayService;
            FuncCamera = function_Camera;
        }

        #region parameter define
        Button[] CameraButton;
        List <Panel> PnlPartList = new List<Panel>();
        F_CameraSettingLogic CameraSettingLogic;
        private int curPnlPart = 0;
        private int CurBtnNum = 0;
        private const int CameraCount = 5;     //相機數量
        private ICameraDisplayService DisplayService;
        private IFunction_Camera FuncCamera;
        #endregion

        #region private function
        private void SetHint()
        {
            //toolTip1.SetToolTip(Btn_OEM_Setting, "OEM Setting");
        }
        private void InitialForm()
        {
            SetPnlPartPos(Pnl_Part1);

            SetHint();

            //if (ApplicationSetting.Get_Int_Recipe<eF_AxisSetting>((int)eF_AxisSetting.Cmbx_ShowFormName) == 1)
            //    Tool.ShowFormName(this);

            PnlPartList.Add(Pnl_Part1);
            PnlPartList.Add(Pnl_Part2);

            CreateDynamicElement();
        }
        private int PreviousPnlPart(List<Panel> list, int index)
        {
            index = index - 1;

            if (index < 0)
                index = 0;

            SetPnlPartPos(list[index]);

            return index;
        }
        private int NextPnlPart(List<Panel> list, int index)
        {
            index = index + 1;

            if (index >= list.Count)
                index = 0;

            SetPnlPartPos(list[index]);

            return index;
        }
        private void SetPnlPartPos(Panel pnl)   //應該可以移到ToolFunction
        {
            pnl.Location = new Point(0, 0);
            pnl.BringToFront();
        }
        private void CreateDynamicElement()
        {
            CameraButton = new Button[CameraCount];
            string[] CameraButtonName = new string[CameraCount] { "CCD_1", "CCD_2", "CCD_3", "CCD_4", "CCD_5"};

            for (int i=0; i<CameraButton.Length; i++)
            {
                CameraButton[i] = new Button();
                CameraButton[i].Font = new System.Drawing.Font("Times New Roman", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                CameraButton[i].Location = new System.Drawing.Point(141 + 116 * i, 3);
                CameraButton[i].Name = $"Btn_Camera{i + 1}";
                CameraButton[i].Size = new System.Drawing.Size(110, 113);
                CameraButton[i].TabIndex = 3;
                CameraButton[i].Tag = $"{i + 1}";
                CameraButton[i].Text = CameraButtonName[i];
                CameraButton[i].UseVisualStyleBackColor = true;
                CameraButton[i].Click += new System.EventHandler(this.Btn_Camera0_Click);

                Pnl_Part1.Controls.Add(CameraButton[i]);
            }
        }
        private void UpdateButtonName(IReadOnlyList<CAMERA_INFO> config)
        {
            Btn_Camera0.Text = config[0].CCD_NAME;
            for (int i = 0; i < CameraCount; i++)
            {
                CameraButton[i].Text = config[i + 1].CCD_NAME;
            }
        }
        #endregion

        #region public function
        public int GetCurrentBtnNum()
        {
            return CurBtnNum;
        }
        public void ShowFormName(bool show)
        {
            if (show)
                Tool.ShowFormName(this);
        }
        public void UpdateButtonNameInvoke()
        {
            FuncCamera.LoadCameraConfig();
            var config = FuncCamera.GetCameraConfig();

            if (InvokeRequired)
                Invoke(new Action(() => UpdateButtonName(config)));
            else
                UpdateButtonName(config);
        }
        #endregion

        private void Btn_PreviousPnlPart1_Click(object sender, EventArgs e)
        {
            curPnlPart = PreviousPnlPart(PnlPartList, curPnlPart);
        }

        private void Btn_NextPnlPart1_Click(object sender, EventArgs e)
        {
            curPnlPart = NextPnlPart(PnlPartList, curPnlPart);
        }

        private void Btn_Camera0_Click(object sender, EventArgs e)
        {
            CameraSettingLogic.SaveCamera();

            Button btn = sender as Button;
            CurBtnNum = Tool.StringToInt((string)btn.Tag);

            CameraSettingLogic.UpdateCameraInfo2Form(CurBtnNum);

            DisplayService.SwitchToCameraDisplay(CurBtnNum);
            DisplayService.FitWindow(CurBtnNum);
            FuncCamera.StartLive(CurBtnNum);
        }
    }
}
