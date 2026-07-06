using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using DeviceCore;


namespace Device_VirtualCamera
{
    public class VirtualCamera : ICamera
    {
        #region parameter define
        private Dictionary<string, CameraInfo> DeviceId = new Dictionary<string, CameraInfo>();

        private class CameraInfo
        {
            public string VirtualImagePath = "";
            public IntPtr PackedBuffer = IntPtr.Zero;
            public int PackedCapacity = 0;

            public void Dispose()
            {
                if (PackedBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(PackedBuffer);
                    PackedBuffer = IntPtr.Zero;
                    PackedCapacity = 0;
                }
            }
        }
        #endregion

        #region private function
        private int GetBytesPerPixel(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.Format8bppIndexed:
                    return 1;
                case PixelFormat.Format24bppRgb:
                    return 3;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppPArgb:
                    return 4;
                default:
                    return Math.Max(1, Image.GetPixelFormatSize(format) / 8);
            }
        }

        private void EnsurePackedBuffer(CameraInfo cameraInfo, int requiredBytes)
        {
            if (cameraInfo.PackedCapacity >= requiredBytes)
                return;

            if (cameraInfo.PackedBuffer != IntPtr.Zero)
                Marshal.FreeHGlobal(cameraInfo.PackedBuffer);

            cameraInfo.PackedBuffer = Marshal.AllocHGlobal(requiredBytes);
            cameraInfo.PackedCapacity = requiredBytes;
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

        public int SetHardwareGain(string id, double gain)
        {
            return 0;
        }

        public int SetExposureTime(string id, double time)
        {
            return 0;
        }

        public int GetImage(string id, ref IntPtr image, ref int image_width, ref int image_height, ref PixelFormat pixelFormat)
        {
            if (!DeviceId.TryGetValue(id, out CameraInfo cameraInfo)) 
                return -1;
            
            if (!File.Exists(cameraInfo.VirtualImagePath)) 
                return -1;

            try
            {
                using (FileStream fs = new FileStream(cameraInfo.VirtualImagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (Bitmap temp = (Bitmap)Image.FromStream(fs))
                    {
                        image_width = temp.Width;
                        image_height = temp.Height;
                        pixelFormat = temp.PixelFormat;

                        int bytesPerPixel = GetBytesPerPixel(pixelFormat);
                        int packedStride = image_width * bytesPerPixel;
                        int requiredBytes = packedStride * image_height;
                        EnsurePackedBuffer(cameraInfo, requiredBytes);

                        BitmapData srcData = temp.LockBits(new Rectangle(0, 0, image_width, image_height), ImageLockMode.ReadOnly, pixelFormat);

                        try
                        {
                            int sourceStride = srcData.Stride;

                            for (int y = 0; y < image_height; y++)
                            {
                                IntPtr sourceRow = IntPtr.Add(srcData.Scan0, y * sourceStride);
                                IntPtr destRow = IntPtr.Add(cameraInfo.PackedBuffer, y * packedStride);
                                NativeMethods.CopyMemory(destRow, sourceRow, new UIntPtr((uint)packedStride));
                            }
                        }
                        finally
                        {
                            temp.UnlockBits(srcData);
                        }
                    }
                }

                image = cameraInfo.PackedBuffer;
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
            if (DeviceId.TryGetValue(id, out CameraInfo cameraInfo))
            {
                cameraInfo.Dispose();
                DeviceId.Remove(id);
            }

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

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
            public static extern void CopyMemory(IntPtr dest, IntPtr src, UIntPtr count);
        }
    }
}
