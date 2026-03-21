using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;
using ProbeTester;
using ProbeTester.Base;

namespace ProbeTester
{
    public class ProbeTesterMachine : IProbeTesterMachine
    {
        public IFunction_IO_Card DIOL { get; }
        public IFunction_MotionCard DML { get; }
        public IFunction_Spectrometer Spectrometer { get; }
        public IFunction_LightControl Light { get; }
        public IFunction_Camera CCD { get; }

        public ProbeTesterMachine(IFunction_MotionCard motion, IFunction_IO_Card iOCard, IFunction_Spectrometer spec,
                                    IFunction_LightControl light, IFunction_Camera ccd)
        {
            DIOL = iOCard;
            DML = motion;
            Spectrometer = spec;
            Light = light;
            CCD = ccd;
        }
    }
}
