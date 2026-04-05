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
                // 釋放舊資源並讀取新圖
                Cleanup();

                // 使用 FileStream 讀取，確保讀完就能關閉檔案
                using (FileStream fs = new FileStream(VirtualImagePath, FileMode.Open, FileAccess.Read))
                {
                    using (Bitmap temp = (Bitmap)Image.FromStream(fs))
                    {
                        image_width = temp.Width;
                        image_height = temp.Height;
                        pixelFormat = temp.PixelFormat;

                        // 建立一個全新的、位於記憶體的 Bitmap (與檔案完全脫離)
                        _currentBmp = new Bitmap(image_width, image_height, pixelFormat);

                        // 如果是灰階圖，必須手動複製調色盤 (Palette)
                        if (pixelFormat == PixelFormat.Format8bppIndexed)
                        {
                            ColorPalette pal = _currentBmp.Palette;
                            for (int i = 0; i < 256; i++) pal.Entries[i] = Color.FromArgb(i, i, i);
                            _currentBmp.Palette = pal;
                        }

                        // 使用 LockBits 同時鎖定「來源」與「目標」進行高速拷貝
                        BitmapData srcData = temp.LockBits(new Rectangle(0, 0, image_width, image_height), ImageLockMode.ReadOnly, pixelFormat);
                        BitmapData dstData = _currentBmp.LockBits(new Rectangle(0, 0, image_width, image_height), ImageLockMode.WriteOnly, pixelFormat);

                        try
                        {
                            unsafe
                            {
                                long bytesToCopy = (long)srcData.Stride * image_height;

                                System.Buffer.MemoryCopy(
                                    (void*)srcData.Scan0,
                                    (void*)dstData.Scan0,
                                    bytesToCopy,
                                    bytesToCopy);
                            }
                        }
                        finally
                        {
                            temp.UnlockBits(srcData);
                            _currentBmp.UnlockBits(dstData);
                        }
                    }
                } 

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
