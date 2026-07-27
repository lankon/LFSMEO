using System;

namespace DeviceCore
{
    public enum EConfocalTriggerSource
    {
        Continuous = 0,
        Software = 1,
    }

    public interface IConfocal
    {
        int Connect();

        void Disconnect();

        int SetTriggerMode(EConfocalTriggerSource mode);

        int GetValue(ref double Value);
    }
}
