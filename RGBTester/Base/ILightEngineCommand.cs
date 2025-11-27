using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public interface ILightEngineCommand
    {
        bool SetLedDriverData(byte index, byte registerAddress, byte value);
        bool GetLedDriverData(byte index, byte registerAddress);
        bool CheckConnect();
    }
}
