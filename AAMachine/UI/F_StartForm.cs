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

using ToolFunction;

using DeviceCore;
using AAMachine.Base;
using AAMachine.Logic;
using Matrox.MatroxImagingLibrary;
using AAMachine.Device.QuantaMeasureAPI.Z23A;
using AAMachine.Device.QuantaMeasureAPI.Base;      // 違規用法


namespace AAMachine.UI
{
    public partial class F_StartForm : Form
    {
        public F_StartForm(IServiceProvider serviceProvider, F_StartFormLogic startFormLogic)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            StartFormLogic = startFormLogic;

            InitialForm();
        }

        #region parameter define
        //private CameraDisplayPanel CameraDisplay;
        F_StartFormLogic StartFormLogic;
        IServiceProvider ServiceProvider;
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
            //IF_CameraSetting setting = ServiceProvider.GetRequiredService<IF_CameraSetting>();
            //IF_CameraButton btn = ServiceProvider.GetRequiredService<IF_CameraButton>();

            //setting.SwitchToCameraDisplay(btn.GetCurrentBtnNum());
            //setting.DockDisplayToPanel(Pnl_CCD);
        }
        private void LeavePage()
        {

        }
        #endregion

        #region public function
        #endregion

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            IFunction_Camera function_Camera = ServiceProvider.GetRequiredService<IFunction_Camera>();

            function_Camera.Initial_All_Camera();

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
            //var MainTask = ServiceProvider.GetRequiredService<IBaseMainTask>();
            //MainTask.SetTask<TaskInitial>();
            //MainTask.Run();
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
            //IFunction_Camera function_Camera = ServiceProvider.GetRequiredService<IFunction_Camera>();

            //function_Camera.StartGrab(0);
            //function_Camera.SoftTrigger(0);
        }

        private void Btn_PTPA_Click(object sender, EventArgs e)
        {
            //var MainTask = ServiceProvider.GetRequiredService<IBaseMainTask>();
            //MainTask.SetTask<Task_PTPA>();
            //MainTask.Run();

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

        Z23A_MirrorAA_API_Command Z23A_API = new Z23A_MirrorAA_API_Command();

        private void Btn_TestZ23A_API_Click(object sender, EventArgs e)
        {
            MIL_ID milApp = MIL.M_NULL;
            MIL_ID milSys = MIL.M_NULL;
            MIL_ID milImage = MIL.M_NULL;

            MIL.MappAlloc(MIL.M_NULL, MIL.M_DEFAULT, ref milApp);
            MIL.MsysAlloc(milApp, MIL.M_SYSTEM_HOST, MIL.M_DEFAULT, MIL.M_DEFAULT, ref milSys);

            MIL.MbufImport(
                @"D:\0.桌面雜物\Uniformity_W.tiff",
                MIL.M_DEFAULT,
                MIL.M_RESTORE,
                milSys,
                ref milImage
            );
            
            MeasureImageInfo res = Z23A_API.ConvertMilImageToImageInfo(milImage);

            //long time;
            //int spend_time;
            
            //Tool.ResetTimeCount(out time);
            //UniformityResultInfo info = Z23A_API.GetUniformity(res, TestSide.Left, new PointF(6958, 4922), 28.01027058, 204.248366, 22222);
            //spend_time = Tool.GetTime(time);
            //Console.WriteLine($"GetUniformity spend time: {spend_time} ms");

            ////(double,double) center = Z23A_API.GetLightSpotGravityCenter(info.IntensityMap, 0.0);

            //Z23A_API.SaveRawImage(res, "D:\\abcd");

            MIL.MbufFree(milImage);
            MIL.MsysFree(milSys);
            MIL.MappFree(milApp);

            GC.Collect();

            Console.WriteLine(GC.GetTotalMemory(false));
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Console.WriteLine(GC.GetTotalMemory(true));

        }

        private void Btn_RolloffTest_Click(object sender, EventArgs e)
        {
            MIL_ID milApp = MIL.M_NULL;
            MIL_ID milSys = MIL.M_NULL;
            MIL_ID milImage = MIL.M_NULL;

            MIL.MappAlloc(MIL.M_NULL, MIL.M_DEFAULT, ref milApp);
            MIL.MsysAlloc(milApp, MIL.M_SYSTEM_HOST, MIL.M_DEFAULT, MIL.M_DEFAULT, ref milSys);

            MIL.MbufImport(
                @"D:\0.桌面雜物\Uniformity_W.tiff",
                MIL.M_DEFAULT,
                MIL.M_RESTORE,
                milSys,
                ref milImage
            );

            MeasureImageInfo res = Z23A_API.ConvertMilImageToImageInfo(milImage);

            long time;
            int spend_time;

            Tool.ResetTimeCount(out time);
            RollOffResultInfo result = Z23A_API.GetBrightnessRolloff(res, TestSide.Left, new PointF(6958, 4922), 28.01027058, 204.248366);
            spend_time = Tool.GetTime(time);
            Console.WriteLine($"GetBrightnessRolloff spend time: {spend_time} ms");

            MIL.MbufFree(milImage);
            MIL.MsysFree(milSys);
            MIL.MappFree(milApp);

            GC.Collect();
        }

        private void CalculateSequentialContrast(List<PointF> nine_pos)
        {
            MIL_ID milApp = MIL.M_NULL;
            MIL_ID milSys = MIL.M_NULL;
            MIL_ID milImage = MIL.M_NULL;
            MIL_ID milImageDark = MIL.M_NULL;

            try
            {
                MIL.MappAlloc(MIL.M_NULL, MIL.M_DEFAULT, ref milApp);
                MIL.MsysAlloc(milApp, MIL.M_SYSTEM_HOST, MIL.M_DEFAULT, MIL.M_DEFAULT, ref milSys);

                MIL.MbufImport(
                    @"D:\0.桌面雜物\SequentialContrast_R.tiff",
                    MIL.M_DEFAULT,
                    MIL.M_RESTORE,
                    milSys,
                    ref milImage
                );

                MIL.MbufImport(
                    @"D:\0.桌面雜物\SequentialContrast_R_BK.tiff",
                    MIL.M_DEFAULT,
                    MIL.M_RESTORE,
                    milSys,
                    ref milImageDark
                );

                MeasureImageInfo res = Z23A_API.ConvertMilImageToImageInfo(milImage);
                MeasureImageInfo res_dark = Z23A_API.ConvertMilImageToImageInfo(milImageDark);

                long time;
                int spend_time;

                Tool.ResetTimeCount(out time);
                ContrastResultInfo result = Z23A_API.GetSequentialContrast(res, res_dark, TestSide.Left, new PointF(6958, 4922), 28.01027058, 204.248366, nine_pos,
                                                                            22222, 6911042);
                spend_time = Tool.GetTime(time);
                Console.WriteLine($"GetSequentialContrast spend time: {spend_time} ms");
            }
            finally
            {
                if (milImage != MIL.M_NULL) MIL.MbufFree(milImage);
                if (milImageDark != MIL.M_NULL) MIL.MbufFree(milImageDark);
                if (milSys != MIL.M_NULL) MIL.MsysFree(milSys);
                if (milApp != MIL.M_NULL) MIL.MappFree(milApp);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void Btn_SequentialContrastTest_Click(object sender, EventArgs e)
        {
            MIL_ID milApp = MIL.M_NULL;
            MIL_ID milSys = MIL.M_NULL;
            MIL_ID milImage = MIL.M_NULL;
            List<PointF> result_9point = null;

            try
            {
                MIL.MappAlloc(MIL.M_NULL, MIL.M_DEFAULT, ref milApp);
                MIL.MsysAlloc(milApp, MIL.M_SYSTEM_HOST, MIL.M_DEFAULT, MIL.M_DEFAULT, ref milSys);

                MIL.MbufImport(
                    @"D:\0.桌面雜物\9Points_W.tiff",
                    MIL.M_DEFAULT,
                    MIL.M_RESTORE,
                    milSys,
                    ref milImage
                );

                MeasureImageInfo res = Z23A_API.ConvertMilImageToImageInfo(milImage);

                long time;
                int spend_time;

                Tool.ResetTimeCount(out time);
                result_9point = Z23A_API.GetNinePtsPosition(res, TestSide.Left, new PointF(6958, 4922), 28.01027058, 204.248366);
                spend_time = Tool.GetTime(time);
                Console.WriteLine($"GetNinePtsPosition spend time: {spend_time} ms");
            }
            finally
            {
                if (milImage != MIL.M_NULL) MIL.MbufFree(milImage);
                if (milSys != MIL.M_NULL) MIL.MsysFree(milSys);
                if (milApp != MIL.M_NULL) MIL.MappFree(milApp);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            if (result_9point != null)
                CalculateSequentialContrast(result_9point);

        }

        private void Btn_CallMemoryMonitor_Click(object sender, EventArgs e)
        {
            Tool.F_Monitor f_Monitor = new Tool.F_Monitor();
            f_Monitor.Show();
        }
    }
}
