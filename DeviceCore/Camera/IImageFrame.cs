using System;
using System.Drawing.Imaging;

namespace DeviceCore
{
    public interface IImageFrame : IDisposable
    {
        IntPtr ImageData { get; }
        int Width { get; }
        int Height { get; }
        int Stride { get; }
        int CCD_Index { get; }
        PixelFormat Format { get; }

        bool TryAddRef();
    }
}
