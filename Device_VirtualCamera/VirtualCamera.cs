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
        private Dictionary<string, CameraInfo> DeviceId = new Dictionary<string, CameraInfo>();

        private class CameraInfo
        {
            public Bitmap _currentBmp;             // 必須作為類別成員，防止被 GC
            public BitmapData _bmpData;            // 用於紀錄鎖定的狀態
            public string VirtualImagePath = "";
        }
        #endregion

        #region private function
        private void Cleanup(string id)
        {
            DeviceId.TryGetValue(id, out CameraInfo cameraInfo);

            if (cameraInfo == null)
                return;

            if (cameraInfo._currentBmp != null && cameraInfo._bmpData != null)
            {
                cameraInfo._currentBmp.UnlockBits(cameraInfo._bmpData);
                cameraInfo._currentBmp.Dispose();
                cameraInfo._currentBmp = null;
                cameraInfo._bmpData = null;
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
            DeviceId.TryGetValue(id, out CameraInfo cameraInfo);

            if (cameraInfo == null)
                return -1;

            if (!File.Exists(cameraInfo.VirtualImagePath))
                return -1;
            
            try
            {
                // 釋放舊資源並讀取新圖
                Cleanup(id);

                // 使用 FileStream 讀取，確保讀完就能關閉檔案
                using (FileStream fs = new FileStream(cameraInfo.VirtualImagePath, FileMode.Open, FileAccess.Read))
                {
                    using (Bitmap temp = (Bitmap)Image.FromStream(fs))
                    {
                        image_width = temp.Width;
                        image_height = temp.Height;
                        pixelFormat = temp.PixelFormat;

                        // 建立一個全新的、位於記憶體的 Bitmap (與檔案完全脫離)
                        cameraInfo._currentBmp = new Bitmap(image_width, image_height, pixelFormat);

                        // 如果是灰階圖，必須手動複製調色盤 (Palette)
                        if (pixelFormat == PixelFormat.Format8bppIndexed)
                        {
                            ColorPalette pal = cameraInfo._currentBmp.Palette;
                            for (int i = 0; i < 256; i++) pal.Entries[i] = Color.FromArgb(i, i, i);
                            cameraInfo._currentBmp.Palette = pal;
                        }

                        // 使用 LockBits 同時鎖定「來源」與「目標」進行高速拷貝
                        BitmapData srcData = temp.LockBits(new Rectangle(0, 0, image_width, image_height), ImageLockMode.ReadOnly, pixelFormat);
                        BitmapData dstData = cameraInfo._currentBmp.LockBits(new Rectangle(0, 0, image_width, image_height), ImageLockMode.WriteOnly, pixelFormat);

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
                            cameraInfo._currentBmp.UnlockBits(dstData);
                        }
                    }
                } 

                image_width = cameraInfo._currentBmp.Width;
                image_height = cameraInfo._currentBmp.Height;
                pixelFormat = cameraInfo._currentBmp.PixelFormat;

                //定義要鎖定的區域（整張圖）
                Rectangle rect = new Rectangle(0, 0, image_width, image_height);

                if (pixelFormat == PixelFormat.Format8bppIndexed)
                {
                    cameraInfo._bmpData = cameraInfo._currentBmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
                }
                else
                {
                    cameraInfo._bmpData = cameraInfo._currentBmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                }

                // Scan0 就是指向像素資料首位址的 IntPtr
                image = cameraInfo._bmpData.Scan0;

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
            DeviceId.TryGetValue(id, out CameraInfo cameraInfo);

            if (cameraInfo != null)
                return 0;

            CameraInfo new_cameraInfo = new CameraInfo();
            DeviceId.Add(id, new_cameraInfo);

            return 0;
        }

        public int StopGrabbing(string id)
        {
            return 0;
        }


        //[Virtual Camera Function]
        public void SetVirtualImagePath(string id, string path)
        {
            DeviceId.TryGetValue(id, out CameraInfo cameraInfo);

            if (cameraInfo == null)
                return;

            cameraInfo.VirtualImagePath = path;
        }
        #endregion
    }
}
