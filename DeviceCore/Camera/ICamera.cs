using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

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
    }
    public interface ICamera
    {
        int Connect();
        int StartGrabbing(string id);
        int StopGrabbing(string id);
        int SoftwareTrigger(string id);
        int GetImage(string id, ref IntPtr image, ref int image_width, ref int image_height);
    }
}
