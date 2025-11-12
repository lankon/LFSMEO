using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace RGBTester
{
    public class RGBTesterMachine:IRGBTesterMachine
    {
        public IFunction_IO_Card DIOL { get; }
        public IFunction_MotionCard DML { get; }
        public IChillerControl Chiller { get; }

        public RGBTesterMachine(IFunction_MotionCard motion, IFunction_IO_Card iOCard, IChillerControl chillerControl)
        {
            DIOL = iOCard;
            Chiller = chillerControl;
            DML = motion;
        }
    }
}
