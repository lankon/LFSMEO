using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public interface IIOCardVirtual
    {
        int Add_AI_VirtualData(byte port, double value);
        int Clear_AI_VirtualData();
        void AddIORule(int outputCardNo, int outputLineNo, int outputDevNo, int outputPort, bool outputValue,
                              params (int inputCardNo, int inputLineNo, int inputDevNo, int inputPort, bool inputValue)[] effects);
    }
}
