using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace Device_Virtual
{
    class VirtualDevice : IIOCard
    {
        public double GetAInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            throw new NotImplementedException();
        }

        public bool GetInputStatus(byte cardNo, byte lineNo, byte DevNo, byte port)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return "Virtual";
        }

        public bool GetOutputStatus(byte cardNo, byte lineNo, byte DevNo, byte port)
        {
            throw new NotImplementedException();
        }

        public bool Open()
        {
            return true;
        }

        public bool SetOutputStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false)
        {
            throw new NotImplementedException();
        }

        public void UpdateInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            
        }

        public void UpdateOutput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            
        }
    }
}
