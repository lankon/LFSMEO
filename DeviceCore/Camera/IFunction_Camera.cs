using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DeviceCore
{
    public class ImageReadyEventArgs : EventArgs
    {
        public IntPtr ImageData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public IMAGE_FORMAT Format { get; set; }
    }
    public enum IMAGE_FORMAT
    {
        MONO8,
        RGB8,
    }

    public interface IFunction_Camera
    {
        int Initial_All_Camera();
        bool StartGrab();
        bool StopGrab();
        bool SoftTrigger();
        event EventHandler<ImageReadyEventArgs> OnImageUpdated;
    }
}
