using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;
using AAMachine.Base;
using AAMachine.Base.Equipment_Setting;

namespace AAMachine.MachineFunction
{
    public partial class MachineFunc
    {
        public class AxisDefine
        {
            public int Axis_X { get; set; } = 0;
            public int Axis_Y { get; set; } = 1;
            public int Axis_Z { get; set; } = 2;
            public int Axis_A { get; set; } = 3;
            public int Axis_TX { get; set; } = 4;
            public int Axis_TY { get; set; } = 5;

            private MachineFunc _MachineFunc;

            public void Initialize(MachineFunc func)
            {
                _MachineFunc = func;
            }

            public void SetAxisDefine()
            {
                // 要分辨機型

                eModuleType type = _MachineFunc.GetModuleType();

                if(type == eModuleType.MIRROR_AA)
                {
                    Axis_X = (int)AXIS_NAME.AXIS_X;
                    Axis_Y = (int)AXIS_NAME.AXIS_Y;
                    Axis_Z = (int)AXIS_NAME.AXIS_Z;
                    Axis_A = (int)AXIS_NAME.AXIS_A;
                    Axis_TX = (int)AXIS_NAME.AXIS_AX;
                    Axis_TY = (int)AXIS_NAME.AXIS_AY;
                }
            }
        }
    }
}
