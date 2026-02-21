using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;
using DeviceCore;

namespace Device_VirtualLight
{
    public class VirtualLight : ILightControl
    {
        public VirtualLight()
        {
            
        }

        #region parameter define
        private LIGHT_INFO LightInfo = new LIGHT_INFO();
        #endregion

        public ELightControlType GetLightControlType()
        {
            return ELightControlType.VITUAL;
        }

        public int Open()
        {
            return 0;
        }

        public int SetDeviceParameter(LIGHT_INFO info)
        {
            LightInfo = info;
            return 0;
        }

        public int SetLightValue(int value, int port = 0, int station_number = 0)
        {
            
            return 0;
        }

        public string GetPortName()
        {
            throw new NotImplementedException();
        }
    }
}
