using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DeviceCore
{
    public enum ELightControlType
    {
        VITUAL,
        FT,
    }

    public struct LIGHT_INFO
    {
        public ELightControlType Type;
        public string Name;
        public string PortName;
        public int BaudRate;
        public Parity Parity;
        public int DataBits;
        public StopBits StopBits;
    }

    public interface ILightControl
    {
        int SetDeviceParameter(LIGHT_INFO info);
        int Open();
        int SetLightValue(int value, int port = 0, int station_number = 0);
        ELightControlType GetLightControlType();
    }
}
