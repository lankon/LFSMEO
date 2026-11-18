using System;
using System.Collections.Generic;

namespace DeviceCore
{
    public interface IVirtualSourceMeter
    {
        bool LoadVirtualData(string filePath);
    }
}
