using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DeviceCore
{
    public interface ICamera
    {
        int Connect();
        int StartGrabbing(string id);
        int StopGrabbing(string id);
        int SoftwareTrigger(string id);
        int GetImage(string id);
    }
}
