using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace DETester
{
    public interface IDETesterMachine
    {
        IFunction_IO_Card DIOL { get; }
        IFunction_MotionCard DML { get; }
        IFunction_LightControl Light { get; }
        IFunction_Camera CCD { get; }
    }
}
