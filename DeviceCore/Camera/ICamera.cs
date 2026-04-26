using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Drawing.Imaging;

namespace DeviceCore
{
    public struct CAMERA_INFO
    {
        //新增相機參數時需添加

        //[Connect]
        public int CCD_TYPE;        //相機種類
        public string IP_ID;        //相機ID
        public int CCD_USE;         //啟用/關閉
        public string CCD_NAME;     //名稱

        //[CCD Setting]
        public int FPS;       //Frames Per Second
        public int Gain;        //相機增益
        //public int ExporesTime


        //TxtBx_FPS,
        //TxtBx_Gain,
        //TxtBx_ExposureTime,
    }
    public interface ICamera
    {
        CCD_TYPE GetCameraType();
        int Connect();

        //[Set Parameter]
        int SetHardwareGain(string id, double gain);
        int SetExposureTime(string id, double time);

        //[]
        int StartGrabbing(string id);
        int StopGrabbing(string id);
        int SoftwareTrigger(string id);
        
        int GetImage(string id, ref IntPtr image, ref int image_width, ref int image_height, ref PixelFormat pixelFormat);

        //[Virtual Camera Function]
        void SetVirtualImagePath(string id, string path);
    }
}
