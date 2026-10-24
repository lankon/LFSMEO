using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Drawing.Imaging;

namespace DeviceCore
{
    public class ImageReadyEventArgs : EventArgs
    {
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
        MLO,
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

        bool StartGrab(int ccd);
        bool StopGrab(int ccd);
        bool SoftTrigger(int ccd);
        bool GetImageDisplay(int ccd, string image_path);


        //[Read&Save Axis Information]
        void SaveCameraConfig(string filePath, string axisName, Dictionary<string, string> parameters);
        bool LoadCameraConfig(); 
        IReadOnlyList<CAMERA_INFO> GetCameraConfig();

    }
}
