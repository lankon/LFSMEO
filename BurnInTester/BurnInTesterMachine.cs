using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;
using BurnInTester;
using BurnInTester.Base;

namespace BurnInTester
{
    public class BurnInTesterMachine : IBurnInTesterMachine
    {
        public IFunction_IO_Card DIOL { get; }
        public IFunction_TemperatureControl TC { get; }

        public BurnInTesterMachine(IFunction_IO_Card iOCard, IFunction_TemperatureControl function_TemperatureControl)
        {
            DIOL = iOCard;
            TC = function_TemperatureControl;
        }
    }
}
