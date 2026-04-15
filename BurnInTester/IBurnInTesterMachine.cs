using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace BurnInTester
{
    public interface IBurnInTesterMachine
    {
        IFunction_IO_Card DIOL { get; }
        IFunction_TemperatureControl TC { get; }
    }
}
