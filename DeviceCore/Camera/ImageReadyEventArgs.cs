using System;
using System.Drawing.Imaging;

namespace DeviceCore
{
    public class ImageReadyEventArgs : EventArgs
    {
        public IImageFrame Frame { get; set; }
        public IntPtr ImageData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int CCD_Index { get; set; }
        public PixelFormat Format { get; set; }
    }
}
