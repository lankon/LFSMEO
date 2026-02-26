using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeviceUI.Camera
{
    public class CameraDisplayPanel:Panel
    {
        public CameraDisplayPanel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
        }

        // 存放目前的影像 (從海康拿到的 Bitmap)
        private Bitmap _currentImage;
        public Bitmap CurrentImage
        {
            get => _currentImage;
            set
            {
                // 1. 釋放舊的 Bitmap 資源，避免 GDI+ 記憶體洩漏
                _currentImage?.Dispose();

                // 2. 存入新圖
                _currentImage = value;

                // 3. 關鍵：告訴 Windows 這個元件「無效」了，請儘快呼叫 OnPaint
                // Invalidate 是執行緒安全的，可以在取像的 Background Thread 呼叫
                this.Invalidate();
            }
        }
        public float ZoomScale { get; set; } = 1.0f;

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // 設定繪圖品質 (如果是機台操作，InterpolationMode 選 NearestNeighbor 效能最好)
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            if (CurrentImage != null)
            {
                // 這裡實作縮放與平移邏輯
                // g.ScaleTransform(ZoomScale, ZoomScale);
                g.DrawImage(CurrentImage, 0, 0);

                // 畫輔助線 (例如十字絲)
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    g.DrawLine(pen, 0, this.Height / 2, this.Width, this.Height / 2);
                    g.DrawLine(pen, this.Width / 2, 0, this.Width / 2, this.Height);
                }
            }

            base.OnPaint(e);
        }
    }
}
