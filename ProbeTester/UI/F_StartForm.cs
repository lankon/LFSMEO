using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

using ToolFunction;
using DeviceUI.Camera;
using DeviceCore;
using ProbeTester.Base;
using ProbeTester.Logic;

namespace ProbeTester.UI
{
    public partial class F_StartForm : Form
    {
        public F_StartForm(IServiceProvider serviceProvider,  IProbeTesterMachine machine, F_StartFormLogic startFormLogic)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            Machine = machine;
            StartFormLogic = startFormLogic;

            InitialForm();
        }

        #region parameter define
        private CameraDisplayPanel CameraDisplay;
        F_StartFormLogic StartFormLogic;
        IServiceProvider ServiceProvider;
        IProbeTesterMachine Machine;
        #endregion

        #region private function
        void InitialForm()
        {
            ReadAllEnumRecipe();
            ApplicationSetting.UpdataRecipeToForm<eF_Equipment_Setting>(this);

            CreateDynamicElemet();
            StartFormLogic.SetVirtual_IO_Rule();

            ShowHint();

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);
        }
        private void ReadAllEnumRecipe()
        {
            ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();
            ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();
        }
        void ShowHint()
        {
        }
        private void CreateDynamicElemet()
        {
            //CameraDisplay = new CameraDisplayPanel();
            //this.panel1.Controls.Add(CameraDisplay);
            //CameraDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            //CameraDisplay.Location = new System.Drawing.Point(0, 0);
            //CameraDisplay.Name = "CameraDisplay_1";
            //CameraDisplay.Size = new System.Drawing.Size(807, 523);
            //CameraDisplay.TabIndex = 0;
        }
        private void UpdatePage()
        {
            IF_CameraSetting setting = ServiceProvider.GetRequiredService<IF_CameraSetting>();
            IF_CameraButton btn = ServiceProvider.GetRequiredService<IF_CameraButton>();

            setting.SwitchToCameraDisplay(btn.GetCurrentBtnNum());
            setting.DockDisplayToPanel(Pnl_CCD);
        }
        private void LeavePage()
        {

        }
        #endregion

        #region public function
        #endregion

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            //GC.Collect();

            //using (OpenFileDialog ofd = new OpenFileDialog())
            //{
            //    //ofd.Filter = "Image Files|*.bmp;*.jpg;*.png;*.tif";
            //    //if (ofd.ShowDialog() == DialogResult.OK)
            //    {
            //        // 讀取圖片檔案
            //        // 注意：直接 new Bitmap(path) 會導致檔案被程式鎖住
            //        // 建議用以下方式讀取，讀完後檔案就不會被佔用
            //        string fileName = "C:\\Users\\lankon\\Desktop\\Desktop.PNG";
            //        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            //        {
            //            Bitmap bmp = new Bitmap(fs);

            //            // 丟進你的 Panel 測試
            //            CameraDisplay.CurrentImage = (Bitmap)bmp.Clone();
            //        }
            //    }
            //}
            var MainTask = ServiceProvider.GetRequiredService<IBaseMainTask>();
            MainTask.SetTask<TaskInitial>();
            MainTask.Run();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            

            //// 訂閱中介層的影像事件
            //function_Camera.OnImageUpdated += (s, fe) =>
            //{
            //    // 使用你寫好的 CreateUniversalBitmap 轉換成 Bitmap
            //    Bitmap bmp = CameraDisplay.CreateUniversalBitmap(
            //        fe.Width, fe.Height, fe.ImageData, fe.Format);

            //    // 更新到 UI (注意跨執行緒問題)
            //    if (this.InvokeRequired)
            //    {
            //        this.BeginInvoke(new Action(() =>
            //        {
            //            CameraDisplay.CurrentImage = bmp;
            //        }));
            //    }
            //    else
            //    {
            //        CameraDisplay.CurrentImage = bmp;
            //    }
            //};

            //IF_CameraSetting setting = ServiceProvider.GetRequiredService<IF_CameraSetting>();
            //setting.SwitchToCameraDisplay(2);
            //setting.DockDisplayToPanel(panel1);

        }

        private void button9_Click(object sender, EventArgs e)
        {
            IFunction_Camera function_Camera = ServiceProvider.GetRequiredService<IFunction_Camera>();

            function_Camera.StartGrab(0);
            function_Camera.SoftTrigger(0);
        }

        private void Btn_PTPA_Click(object sender, EventArgs e)
        {
            var MainTask = ServiceProvider.GetRequiredService<IBaseMainTask>();
            MainTask.SetTask<Task_PTPA>();
            MainTask.Run();

        }

        private void F_StartForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                //SaveAllEnumSetting();
                //ReadAllEnumSetting();

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

        private void Btn_Test_Click(object sender, EventArgs e)
        {
            //RTCP_Controller rtcp_ctrl = new RTCP_Controller();

            //double[] L1 = new double[] { 0, -178, 0};   // XY平面到 RZ
            //double[] L2 = new double[] { 0, 0, -50 };   // RZ 到 RX
            //double[] L3 = new double[] { 0, 0, 0 };     // RX 到 RY
            //double[] L4 = new double[] { 53.5, 49, 53.5 };  // RY 到 45度懸臂轉折點
            //double[] Tool = new double[] { -53.316, -49, -53.316 };   // 懸臂轉折點到工具端點
            ////double[] Tool = new double[] { 100, 0, -100 };

            //rtcp_ctrl.SetKinematicParameters(L1, L2, L3, L4, Tool, 225);

            //double cur_x = Machine.DML.GetPosition(0);
            //double cur_y = Machine.DML.GetPosition(1);
            //double cur_z = Machine.DML.GetPosition(2);
            //double cur_tx = Machine.DML.GetPosition(3);
            //double cur_ty = Machine.DML.GetPosition(4);
            //double cur_tz = Machine.DML.GetPosition(5);

            //RTCP_Controller.RobotPose pos = new RTCP_Controller.RobotPose();

            //pos = rtcp_ctrl.CalculateRTCPTarget(new RTCP_Controller.RobotPose() { X = cur_x, Y = cur_y, Z = cur_z, Tz = cur_tz, Tx = cur_tx, Ty = cur_ty },
            //                                    0, -5, 0);

            ////pos = rtcp_ctrl.CalculateLocalTranslationTarget(new RTCP_Controller.RobotPose() { X = cur_x, Y = cur_y, Z = cur_z, Tz = cur_tz, Tx = cur_tx, Ty = cur_ty }, 
            ////                                                -10, 0, 0);

            //Machine.DML.PTP_Move(0, pos.X);
            //Machine.DML.PTP_Move(1, pos.Y);
            //Machine.DML.PTP_Move(2, pos.Z);
            //Machine.DML.PTP_Move(3, pos.Tx);
            //Machine.DML.PTP_Move(4, pos.Ty);
            //Machine.DML.PTP_Move(5, pos.Tz);


            var rtcp = new BlackBoxRTCP_Controller();

            var result = rtcp.Fit(new[]
            {
                new BlackBoxRTCP_Controller.CalibrationSample { Group = "A", X = 0,     Y = 0,     Z = 0,     Tx = 0,  Ty = 0,  Tz = 0  },
                new BlackBoxRTCP_Controller.CalibrationSample { Group = "A", X = 0,     Y = 1.8,   Z = -0.4,  Tx = 5,  Ty = 0,  Tz = 0  },
                new BlackBoxRTCP_Controller.CalibrationSample { Group = "A", X = 0,     Y = -1.75, Z = 0.6,   Tx = -5, Ty = 0,  Tz = 0  },
                new BlackBoxRTCP_Controller.CalibrationSample { Group = "A", X = -2.54, Y = 0,     Z = 0.05,  Tx = 0,  Ty = 5,  Tz = 0  },
                new BlackBoxRTCP_Controller.CalibrationSample { Group = "A", X = 2.5,   Y = 0,     Z = 0.2,   Tx = 0,  Ty = -5, Tz = 0  },
                new BlackBoxRTCP_Controller.CalibrationSample { Group = "A", X = 0.68,  Y = 0.04,  Z = 0.1,   Tx = 0,  Ty = 0,  Tz = 5  },
                new BlackBoxRTCP_Controller.CalibrationSample { Group = "A", X = -0.63, Y = 0.04,  Z = 0.04,  Tx = 0,  Ty = 0,  Tz = -5 },
                new BlackBoxRTCP_Controller.CalibrationSample { Group = "A", X = -2.51, Y = 1.82,  Z = -0.28, Tx = 5,  Ty = 5,  Tz = 0  },
                new BlackBoxRTCP_Controller.CalibrationSample { Group = "A", X = 2.55,  Y = 1.8,   Z = -0.1,  Tx = 5,  Ty = -5, Tz = 0  },
                new BlackBoxRTCP_Controller.CalibrationSample { Group = "A", X = -2.51, Y = -1.76, Z = 0.63,  Tx = -5, Ty = 5,  Tz = 0  },
                new BlackBoxRTCP_Controller.CalibrationSample { Group = "A", X = 2.55,  Y = -1.75, Z = 0.78,  Tx = -5, Ty = -5, Tz = 0  },
                new BlackBoxRTCP_Controller.CalibrationSample { Group = "A", X = 0.52,  Y = 1.84,  Z = -0.3,  Tx = 5,  Ty = 0,  Tz = 5  },
                new BlackBoxRTCP_Controller.CalibrationSample { Group = "A", X = -0.84, Y = -1.74, Z = 0.58,  Tx = -5, Ty = 0,  Tz = -5 },
            });

            Console.WriteLine($"BlackBox RTCP Fit RMS={result.RmsError:F4} mm, Max={result.MaxError:F4} mm");
            Console.WriteLine($"A=({result.A[0]:F4}, {result.A[1]:F4}, {result.A[2]:F4})");
            Console.WriteLine($"B=({result.B[0]:F4}, {result.B[1]:F4}, {result.B[2]:F4})");
            Console.WriteLine($"C=({result.C[0]:F4}, {result.C[1]:F4}, {result.C[2]:F4})");

            double cur_x = Machine.DML.GetPosition(0);
            double cur_y = Machine.DML.GetPosition(1);
            double cur_z = Machine.DML.GetPosition(2);
            double cur_tx = Machine.DML.GetPosition(3);
            double cur_ty = Machine.DML.GetPosition(4);
            double cur_tz = Machine.DML.GetPosition(5);

            BlackBoxRTCP_Controller.RobotPose pos = new BlackBoxRTCP_Controller.RobotPose();
            //pos = rtcp.CalculateRTCPTargetByMachineAngles(new BlackBoxRTCP_Controller.RobotPose() { X = cur_x, Y = cur_y, Z = cur_z, Tz = cur_tz, Tx = cur_tx, Ty = cur_ty },
            //                                                1, 0, 0);

            pos = rtcp.CalculateRTCPTargetByTcpLocalRotation(new BlackBoxRTCP_Controller.RobotPose() { X = cur_x, Y = cur_y, Z = cur_z, Tz = cur_tz, Tx = cur_tx, Ty = cur_ty },
                                                            0.5, 0, 0);

            if(pos.Z >= 0)
                Machine.DML.PTP_Move(2, pos.Z);

            Machine.DML.PTP_Move(0, pos.X);
            Machine.DML.PTP_Move(1, pos.Y);

            if (pos.Z < 0)
                Machine.DML.PTP_Move(2, pos.Z);

            Machine.DML.PTP_Move(3, pos.Tx);
            Machine.DML.PTP_Move(4, pos.Ty);
            Machine.DML.PTP_Move(5, pos.Tz);
        }

        private void Btn_Calibration_Click(object sender, EventArgs e)
        {
            //// 實例化四點校正解算器
            //RTCP_Controller solver = new RTCP_Controller();

            //// 建立長度為 4 的陣列來裝您的數據
            //RTCP_Controller.RobotPose[] realPoses = new RTCP_Controller.RobotPose[4];

            //// -----------------------------------------------------
            //// 將您記事本中的數據依序填入 (注意：RX 對應 Tx, RY 對應 Ty, RZ 對應 Tz)
            //// -----------------------------------------------------

            //// 第一點 (基準點：全 0 度)
            //realPoses[0] = new RTCP_Controller.RobotPose { X = -2, Y = -1.65, Z = -2.3, Tx = 0, Ty = 0, Tz = 0 };

            //// 第二點 (Tx 轉 5 度)
            //realPoses[1] = new RTCP_Controller.RobotPose { X = -2.03, Y = 0.16, Z = -2.69, Tx = 5, Ty = 0, Tz = 0 };

            //// 第三點 (Ty 轉 -5 度)
            //realPoses[2] = new RTCP_Controller.RobotPose { X = 0.55, Y = -1.65, Z = -2.15, Tx = 0, Ty = -5, Tz = 0 };

            //// 第四點 (Tz 轉 5 度)
            //realPoses[3] = new RTCP_Controller.RobotPose { X = 1.2, Y = -1.4, Z = -2.15, Tx = 0, Ty = -5, Tz = 5 };


            //try
            //{
            //    // 執行核心運算：最小平方法逼近
            //    Vector<double> result = solver.CalculateTCPOffset(realPoses);

            //    // 取得結果
            //    double dx = result[0];
            //    double dy = result[1];
            //    double dz = result[2];

            //    Console.WriteLine("====== 真實數據四點 TCP 校正完成 ======");
            //    Console.WriteLine($"Dx 偏移量: {dx:F4} mm");
            //    Console.WriteLine($"Dy 偏移量: {dy:F4} mm");
            //    Console.WriteLine($"Dz 偏移量: {dz:F4} mm");
            //    Console.WriteLine("=======================================");
            //    Console.WriteLine("請將這三個數值代入您的 RTCP_Controller 中作為常數使用！");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"解算失敗: {ex.Message}");
            //    Console.WriteLine("請確認 MathNet.Numerics 套件已安裝，且數據無誤。");
            //}


            //RTCP_Controller.Point2D p1 = new RTCP_Controller.Point2D { X = -1.65, Y = 0.84 };   // 0度時
            //RTCP_Controller.Point2D p2 = new RTCP_Controller.Point2D { X = 2.41, Y = 0.81 };   // 轉 +20度時
            //RTCP_Controller.Point2D p3 = new RTCP_Controller.Point2D { X = 1.48, Y = 0.62 };   // 轉 -20度時

            //// 2. 算出真實的物理旋轉中心
            //RTCP_Controller.Point2D center = solver.FindRotationCenter(p1, p2, p3);

            //Console.WriteLine($"🎉 找到真實旋轉中心啦！");
            //Console.WriteLine($"Cx (懸臂X向誤差): {center.X:F4} mm");
            //Console.WriteLine($"Cy (懸臂Y向誤差): {center.Y:F4} mm");


        }
    }
}
