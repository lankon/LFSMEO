using DeviceCore;
using System;
using System.Drawing.Imaging;
using System.Threading;

namespace DeviceFunction
{
    internal sealed class CameraFrame : IImageFrame
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
}
