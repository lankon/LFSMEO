using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace Device_TemeratureControl_Virtual
{
    public class Virtual_TemperatureControl : ITemperatureControl
    {
        public int Open(string com, string baudrate, string data_bits, string stop_bits, string parity)
        {
            throw new NotImplementedException();
        }

        public int Start(double sv)
        {
            throw new NotImplementedException();
        }

        public int Stop()
        {
            throw new NotImplementedException();
        }

        public int AskPV()
        {
            throw new NotImplementedException();
        }

        public int Close()
        {
            throw new NotImplementedException();
        }

        public int GetAnswer(out string answer)
        {
            throw new NotImplementedException();
        }

        public int Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
