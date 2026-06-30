using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;

namespace DeviceCore
{
    public sealed class CameraFrame : IDisposable
    {
        private readonly Action<CameraFrame> _releaseAction;
        private int _refCount;
        private int _disposed;

        public CameraFrame(IntPtr imageData, int width, int height, int stride,
                           PixelFormat format, int ccdIndex, Action<CameraFrame> releaseAction)
        {
            ImageData = imageData;
            Width = width;
            Height = height;
            Stride = stride;
            Format = format;
            CCD_Index = ccdIndex;
            _releaseAction = releaseAction;
            _refCount = 1;
        }

        public IntPtr ImageData { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Stride { get; private set; }
        public int CCD_Index { get; private set; }
        public PixelFormat Format { get; private set; }

        public bool TryAddRef()
        {
            while (true)
            {
                int current = Volatile.Read(ref _refCount);
                if (current <= 0 || Volatile.Read(ref _disposed) != 0)
                    return false;

                if (Interlocked.CompareExchange(ref _refCount, current + 1, current) == current)
                    return true;
            }
        }

        public Bitmap CreateBitmapView()
        {
            Bitmap bmp = new Bitmap(Width, Height, Stride, Format, ImageData);

            if (Format == PixelFormat.Format8bppIndexed)
            {
                ColorPalette palette = bmp.Palette;
                for (int i = 0; i < 256; i++)
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                bmp.Palette = palette;
            }

            return bmp;
        }

        public void Dispose()
        {
            while (true)
            {
                int current = Volatile.Read(ref _refCount);
                if (current <= 0)
                    return;

                if (Interlocked.CompareExchange(ref _refCount, current - 1, current) != current)
                    continue;

                if (current == 1)
                {
                    Interlocked.Exchange(ref _disposed, 1);
                    _releaseAction?.Invoke(this);
                }

                return;
            }
        }
    }

    public class ImageReadyEventArgs : EventArgs
    {
        public CameraFrame Frame { get; set; }
        public IntPtr ImageData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int CCD_Index { get; set; }
        public PixelFormat Format { get; set; }
    }
    public enum CCD_TYPE
    {         
        //Basler,
        Virtual,
        Hikvision,
        //Dalsa,
    }
    public enum CCD_NAME
    {
        CCD_0,
        CCD_1,
        CCD_2,
        CCD_3,
        CCD_4,
        CCD_5,
    }
    public enum IMAGE_FORMAT
    {
        MONO8,
        RGB8,
    }

    public interface IFunction_Camera
    {
        EventHandler<ImageReadyEventArgs>[] OnImageUpdates { get; } 

        //[Initial]
        int Initial_All_Camera();
        void BindingCamera();
        void Subscribe(int ccd, EventHandler<ImageReadyEventArgs> handler);

        // [Camera Grab]
        bool StartGrab(int ccd);
        bool StopGrab(int ccd);

        // [Camera Trigger && LIVE]
        bool SoftTrigger(int ccd);
        bool StartLive(int ccdIndex);
        bool StopLive(int ccdIndex);
        void PauseLive(int ccd, bool is_pause);

        // [Get Image]
        bool GetImageDisplay(int ccd, string image_path);

        //[Read&Save Axis Information]
        void SaveCameraConfig(string filePath, string axisName, Dictionary<string, string> parameters);
        bool LoadCameraConfig(); 
        IReadOnlyList<CAMERA_INFO> GetCameraConfig();

    }
}
