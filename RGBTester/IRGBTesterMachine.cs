using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;
using RGBTester.Base;

namespace RGBTester
{
    public interface IRGBTesterMachine
    {
        IFunction_IO_Card DIOL { get; }
        IFunction_MotionCard DML { get; }
        IChillerControl Chiller { get; }
        IIOCard IOTest { get; }
    }
}
