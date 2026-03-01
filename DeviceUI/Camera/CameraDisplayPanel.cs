using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DeviceCore;

namespace DeviceUI.Camera
{
    public class CameraDisplayPanel:Panel, ICameraDisplayPanel
    {
        public CameraDisplayPanel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

            this.MouseEnter += (s, e) => { this.Focus(); };
            this.MouseWheel += CameraDisplayPanel_MouseWheel;
            this.MouseMove += CameraDisplayPanel_MouseMove;
            this.MouseUp += CameraDisplayPanel_MouseUp;
            this.MouseDown += CameraDisplayPanel_MouseDown;
        }

        #region parameter define
        private float _zoomScale = 1.0f;                //影像放大縮小比例
        private PointF _offset = new PointF(0, 0);      //影像放大縮小位移
        private Point _startPoint;                      // 滑鼠按下時的起點
        private Point _endPoint;                        // 滑鼠移動時的當前點
        private bool _isSelecting = false;              // 是否正在拖拉矩形
        public Rectangle _selectedRect { get; private set; } // 最終選取的矩形區域
        private readonly object _imageLock = new object();
        private Bitmap _currentImage;                   //傳入CCD原始圖像資訊
        public Bitmap CurrentImage                      //傳入CCD原始圖像資訊
        {
            get => _currentImage;
            set
            {
                bool isFirstImage = false;
                lock (_imageLock)
                {
                    if (_currentImage == null && value != null) isFirstImage = true;
                    _currentImage?.Dispose();
                    _currentImage = value;
                }

                if (isFirstImage)
                    FitWindow();
                else
                    this.Invalidate();
            }
        }
        #endregion

        #region private function
        public Bitmap CreateUniversalBitmap(int width, int height, IntPtr ptr, MyCamera.MvGvspPixelType pixelType)
        {
            PixelFormat format = PixelFormat.Format8bppIndexed;
            int bytesPerPixel = 1;
            bool isMono = false;

            // 1. 根據 PixelType 判定格式與位元組數
            switch (pixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                    format = PixelFormat.Format8bppIndexed;
                    bytesPerPixel = 1;
                    isMono = true;
                    break;

                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                    format = PixelFormat.Format24bppRgb;
                    bytesPerPixel = 3;
                    break;

                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                    // 注意：GDI+ 沒有原生 YUV 格式，直接顯示會是色偏的。
                    // 這裡設定 16bpp 是為了讓記憶體長度對齊 (2 bytes per pixel)
                    format = PixelFormat.Format16bppRgb565;
                    bytesPerPixel = 2;
                    break;

                default:
                    // 若遇到不支援的格式，預設走 Mono8 嘗試顯示
                    format = PixelFormat.Format8bppIndexed;
                    bytesPerPixel = 1;
                    break;
            }

            // 2. 計算符合 4 字節對齊的 Stride
            // 這是最穩健的寫法，確保不同寬度的 CCD 都不會發生畫面傾斜
            int stride = ((width * bytesPerPixel + 3) / 4) * 4;

            // 3. 建立 Bitmap 物件 (指向 Pinned 指標)
            Bitmap bmp = new Bitmap(width, height, stride, format, ptr);

            // 4. 如果是 Mono8，修正調色盤
            if (isMono)
            {
                ColorPalette palette = bmp.Palette;
                for (int i = 0; i < 256; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                }
                bmp.Palette = palette;
            }

            return bmp;
        }
        private Rectangle GetDisplayRect(Bitmap bmp)
        {
            if (bmp == null) return Rectangle.Empty;

            // 計算縮放比例
            float ratio = Math.Min((float)this.Width / bmp.Width, (float)this.Height / bmp.Height);
            int tw = (int)(bmp.Width * ratio);
            int th = (int)(bmp.Height * ratio);

            // 計算居中位移
            int tx = (this.Width - tw) / 2;
            int ty = (this.Height - th) / 2;

            return new Rectangle(tx, ty, tw, th);
        }
        private Rectangle GetNormalizedRect(Point p1, Point p2)
        {
            return new Rectangle(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X),
                Math.Abs(p1.Y - p2.Y)
            );
        }
        private void DrawSelectionRectangle(Graphics g)
        {
            if (!_isSelecting) return;

            Rectangle drawRect = GetNormalizedRect(_startPoint, _endPoint);

            using (Pen dashPen = new Pen(Color.Blue, 1))
            {
                dashPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                g.DrawRectangle(dashPen, drawRect);
            }

            // 如果之後想加填充，也可以寫在這裡
            // using (SolidBrush brush = new SolidBrush(Color.FromArgb(50, Color.Yellow))) {
            //     g.FillRectangle(brush, drawRect);
            // }
        }
        private void DrawCrosshair(Graphics g)
        {
            Color crosshairColor = Color.Red;
            float thickness = 1.0f;

            using (Pen pen = new Pen(crosshairColor, thickness))
            {
                // 畫水平線
                g.DrawLine(pen, 0, this.Height / 2, this.Width, this.Height / 2);
                // 畫垂直線
                g.DrawLine(pen, this.Width / 2, 0, this.Width / 2, this.Height);
            }
        }
        private Rectangle GetImageInternalRect()
        {
            //將框選的螢幕矩形範圍推回實際影像的範圍

            if (_currentImage == null) return Rectangle.Empty;

            int ix = (int)((_selectedRect.X - _offset.X) / _zoomScale);
            int iy = (int)((_selectedRect.Y - _offset.Y) / _zoomScale);
            int iw = (int)(_selectedRect.Width / _zoomScale);
            int ih = (int)(_selectedRect.Height / _zoomScale);

            return new Rectangle(ix, iy, iw, ih);
        }
        #endregion

        #region public function
        public void FitWindow()
        {
            if (_currentImage == null) return;

            // 計算符合視窗的比例
            float ratio = Math.Min((float)this.Width / _currentImage.Width, (float)this.Height / _currentImage.Height);
            _zoomScale = ratio;

            // 計算置中位移，並存入 _offset
            _offset.X = (this.Width - _currentImage.Width * ratio) / 2f;
            _offset.Y = (this.Height - _currentImage.Height * ratio) / 2f;

            this.Invalidate();
        }
        public void SaveFullImage(string filePath)
        {
            lock (_imageLock)
            {
                if (_currentImage != null)
                {
                    _currentImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
        }
        public void SaveSelectedROI(string filePath)
        {
            // 1. 先取得轉換後的影像真實座標
            Rectangle realRect = GetImageInternalRect();

            if (realRect.Width <= 0 || realRect.Height <= 0) return;

            lock (_imageLock)
            {
                if (_currentImage != null)
                {
                    // 2. 從原始影像中裁切 (Clone) 出該區域
                    using (Bitmap roiImg = _currentImage.Clone(realRect, _currentImage.PixelFormat))
                    {
                        roiImg.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                }
            }
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            lock (_imageLock)
            {
                if (_currentImage != null)
                {
                    g.TranslateTransform(_offset.X, _offset.Y);
                    g.ScaleTransform(_zoomScale, _zoomScale);

                    g.DrawImage(_currentImage, 0, 0);

                    // 重設轉換，避免影響後續畫線
                    g.ResetTransform();
                }
            }

            DrawCrosshair(g);
            DrawSelectionRectangle(g);

            base.OnPaint(e);
        }

        private void CameraDisplayPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_currentImage == null) return;

            float oldScale = _zoomScale;

            // 判斷滾輪向上或向下 (每次縮放 10%)
            if (e.Delta > 0) _zoomScale *= 1.05f;
            else _zoomScale /= 1.05f;

            // 限制最小縮放倍率，避免影像消失
            if (_zoomScale < 0.5f) _zoomScale = 0.5f;

            // 調整 Offset 讓滑鼠指向的像素點不動
            float ratio = _zoomScale / oldScale;
            _offset.X = e.X - (e.X - _offset.X) * ratio;
            _offset.Y = e.Y - (e.Y - _offset.Y) * ratio;

            this.Invalidate();
        }

        private void CameraDisplayPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _isSelecting)
            {
                _isSelecting = false;
                // 計算最終矩形
                _selectedRect = GetNormalizedRect(_startPoint, _endPoint);

                // 可以在這裡觸發一個事件，通知主程式「範圍選好了」
                //OnSelectionCompleted(_selectedRect);
            }
        }

        private void CameraDisplayPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isSelecting)
            {
                _endPoint = e.Location;
                this.Invalidate();
            }
        }

        private void CameraDisplayPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isSelecting = true;
                _startPoint = e.Location;
                _endPoint = e.Location; // 防止沒動滑鼠就放開
            }
        }
    }
}
