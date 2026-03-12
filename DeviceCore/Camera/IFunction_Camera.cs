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
    public enum CCD_TYPE
    {         
        //Basler,
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
        //[Initial]
        int Initial_All_Camera();
        void BindingCamera();


        bool StartGrab(int ccd);
        bool StopGrab(int ccd);
        bool SoftTrigger(int ccd);
        event EventHandler<ImageReadyEventArgs> OnImageUpdated;


        //[Read&Save Axis Information]
        void SaveCameraConfig(string filePath, string axisName, Dictionary<string, string> parameters);
        bool LoadCameraConfig(); 
        IReadOnlyList<CAMERA_INFO> GetCameraConfig();

    }
}
