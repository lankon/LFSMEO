using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;
using RGBTester.Base;

namespace RGBTester
{
    public class RGBTesterMachine:IRGBTesterMachine
    {
        public IFunction_IO_Card DIOL { get; }
        public IFunction_MotionCard DML { get; }
        public IFunction_Spectrometer Spectrometer { get; }
        public IFunction_LightControl Light { get; }
        public IFunction_LightEngine LightEngine { get; }

        public RGBTesterMachine(IFunction_MotionCard motion, IFunction_IO_Card iOCard, IFunction_Spectrometer spec,
                                IFunction_LightControl light, IFunction_LightEngine lightEngine)
        {
            DIOL = iOCard;
            DML = motion;
            Spectrometer = spec;
            Light = light;
            LightEngine = lightEngine;
        }
    }
}
