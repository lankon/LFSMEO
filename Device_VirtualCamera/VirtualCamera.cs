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
            public bool IsLocked = false;           // 追蹤狀態

            public void Dispose()
            {
                UnlockCurrent();
                _currentBmp?.Dispose();
            }

            public void UnlockCurrent()
            {
                if (IsLocked && _currentBmp != null && _bmpData != null)
                {
                    _currentBmp.UnlockBits(_bmpData);
                    IsLocked = false;
                }
            }
        }
        #endregion

        #region private function
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
            if (!DeviceId.TryGetValue(id, out CameraInfo cameraInfo)) return -1;
            if (!File.Exists(cameraInfo.VirtualImagePath)) return -1;

            try
            {
                // 1. 先解鎖上一次的狀態 (避免 InvalidOperationException)
                cameraInfo.UnlockCurrent();

                using (FileStream fs = new FileStream(cameraInfo.VirtualImagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (Bitmap temp = (Bitmap)Image.FromStream(fs))
                    {
                        image_width = temp.Width;
                        image_height = temp.Height;
                        pixelFormat = temp.PixelFormat;

                        // 2. 只有在尺寸或格式變動時，才重新 new Bitmap (關鍵優化！)
                        if (cameraInfo._currentBmp == null ||
                            cameraInfo._currentBmp.Width != image_width ||
                            cameraInfo._currentBmp.Height != image_height ||
                            cameraInfo._currentBmp.PixelFormat != pixelFormat)
                        {
                            cameraInfo._currentBmp?.Dispose();
                            cameraInfo._currentBmp = new Bitmap(image_width, image_height, pixelFormat);

                            // 如果是灰階，初始化一次 Palette 即可
                            if (pixelFormat == PixelFormat.Format8bppIndexed)
                            {
                                ColorPalette pal = cameraInfo._currentBmp.Palette;
                                for (int i = 0; i < 256; i++) pal.Entries[i] = Color.FromArgb(i, i, i);
                                cameraInfo._currentBmp.Palette = pal;
                            }
                        }

                        // 3. 高速記憶體拷貝 (不產生新物件)
                        BitmapData srcData = temp.LockBits(new Rectangle(0, 0, image_width, image_height), ImageLockMode.ReadOnly, pixelFormat);
                        BitmapData dstData = cameraInfo._currentBmp.LockBits(new Rectangle(0, 0, image_width, image_height), ImageLockMode.WriteOnly, pixelFormat);

                        try
                        {
                            unsafe
                            {
                                Buffer.MemoryCopy((void*)srcData.Scan0, (void*)dstData.Scan0,
                                                  (long)srcData.Stride * image_height, (long)srcData.Stride * image_height);
                            }
                        }
                        finally
                        {
                            temp.UnlockBits(srcData);
                            cameraInfo._currentBmp.UnlockBits(dstData);
                        }
                    }
                }

                // 4. 最後鎖定一次以回傳 IntPtr 給外部使用 (例如給 C++ 或 UI 顯示)
                Rectangle rect = new Rectangle(0, 0, image_width, image_height);
                cameraInfo._bmpData = cameraInfo._currentBmp.LockBits(rect, ImageLockMode.ReadOnly, pixelFormat);
                cameraInfo.IsLocked = true;

                image = cameraInfo._bmpData.Scan0;
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetImage Error: {ex.Message}");
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
