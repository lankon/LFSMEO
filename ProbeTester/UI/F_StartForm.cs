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
using DeviceUI.Camera;
using DeviceCore;
using ProbeTester.Base;


namespace ProbeTester.UI
{
    public partial class F_StartForm : Form
    {
        public F_StartForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;

            InitialForm();
        }

        #region parameter define
        private CameraDisplayPanel CameraDisplay;
        IServiceProvider ServiceProvider;
        #endregion

        #region private function
        void InitialForm()
        {
            ReadAllEnumRecipe();
            ApplicationSetting.UpdataRecipeToForm<eF_Equipment_Setting>(this);

            CreateDynamicElemet();

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
            CameraDisplay = new CameraDisplayPanel();
            this.panel1.Controls.Add(CameraDisplay);
            CameraDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            CameraDisplay.Location = new System.Drawing.Point(0, 0);
            CameraDisplay.Name = "CameraDisplay_1";
            CameraDisplay.Size = new System.Drawing.Size(807, 523);
            CameraDisplay.TabIndex = 0;
        }
        #endregion

        #region public function
        #endregion

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            GC.Collect();

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                //ofd.Filter = "Image Files|*.bmp;*.jpg;*.png;*.tif";
                //if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // 讀取圖片檔案
                    // 注意：直接 new Bitmap(path) 會導致檔案被程式鎖住
                    // 建議用以下方式讀取，讀完後檔案就不會被佔用
                    string fileName = "C:\\Users\\lankon\\Desktop\\Desktop.PNG";
                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        Bitmap bmp = new Bitmap(fs);

                        // 丟進你的 Panel 測試
                        CameraDisplay.CurrentImage = (Bitmap)bmp.Clone();
                    }
                }
            }
            



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
        }

        private void button9_Click(object sender, EventArgs e)
        {
            IFunction_Camera function_Camera = ServiceProvider.GetRequiredService<IFunction_Camera>();

            function_Camera.StartGrab();
            function_Camera.SoftTrigger();
        }
    }
}
