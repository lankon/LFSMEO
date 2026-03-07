using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DeviceCore
{
    public interface IFunction_Camera
    {
        int Initial_All_Camera();
        bool StartGrab();
        bool StopGrab();
        bool SoftTrigger();
    }
}
