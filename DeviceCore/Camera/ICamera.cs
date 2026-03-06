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
        int StartGrabbing(uint id);
        int StopGrabbing(uint id);
        int SoftwareTrigger(uint id);
        int GetImage(uint id);
    }
}
