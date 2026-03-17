using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using DeviceCore;


namespace Device_VirtualCamera
{
    public class VirtualCamera : ICamera
    {

        #region parameter define
        private Bitmap _currentBmp;     // 必須作為類別成員，防止被 GC
        private BitmapData _bmpData;    // 用於紀錄鎖定的狀態
        private Bitmap _bmpBuffer;
        private string VirtualImagePath = "";
        #endregion

        #region private function
        private void Cleanup()
        {
            if (_currentBmp != null && _bmpData != null)
            {
                _currentBmp.UnlockBits(_bmpData);
                _currentBmp.Dispose();
                _currentBmp = null;
                _bmpData = null;
            }
        }
        #endregion

        #region public function
        public int Connect()
        {
            return 0;
        }

        public CCD_TYPE GetCameraType()
        {
            return CCD_TYPE.Virtual;
        }

        public int GetImage(string id, ref IntPtr image, ref int image_width, ref int image_height, ref PixelFormat pixelFormat)
        {
            if(!File.Exists(VirtualImagePath))
                return -1;
            
            try
            {
                //釋放舊資源並讀取新圖
                Cleanup();
                _currentBmp = new Bitmap(VirtualImagePath);

                image_width = _currentBmp.Width;
                image_height = _currentBmp.Height;
                pixelFormat = _currentBmp.PixelFormat;

                //定義要鎖定的區域（整張圖）
                Rectangle rect = new Rectangle(0, 0, image_width, image_height);

                if (pixelFormat == PixelFormat.Format8bppIndexed)
                {
                    _bmpData = _currentBmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
                }
                else
                {
                    _bmpData = _currentBmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                }

                // Scan0 就是指向像素資料首位址的 IntPtr
                image = _bmpData.Scan0;

                return 0; // Success
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read Error: {ex.Message}");
                return -1;
            }
        }

        public int SoftwareTrigger(string id)
        {
            return 0;
        }

        public int StartGrabbing(string id)
        {
            return 0;
        }

        public int StopGrabbing(string id)
        {
            return 0;
        }


        //[Virtual Camera Function]
        public void SetVirtualImagePath(string path)
        {
            VirtualImagePath = path;
        }
        #endregion
    }
}
