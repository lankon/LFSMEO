using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

using AAMachine.Base.Equipment_Setting;

namespace AAMachine
{
    public class AAMachineMachine : IAAMachineMachine
    {
        // 機台使用硬體集合
        public IFunction_IO_Card DIOL { get; }
        public IFunction_MotionCard DML { get; }
        public IFunction_LightControl Light { get; }
        public IFunction_Camera CCD { get; }

        public AAMachineMachine(IFunction_MotionCard motion, IFunction_IO_Card iOCard,
                               IFunction_LightControl light, IFunction_Camera ccd)
        {
            DIOL = iOCard;
            DML = motion;
            Light = light;
            CCD = ccd;
        }
    }
}
