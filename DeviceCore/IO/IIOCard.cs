using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public interface IIOCard
    {
        bool Open();
        string GetName();
        void UpdateInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0);
        bool GetInputStatus(byte cardNo, byte lineNo, byte DevNo, byte port);
        void UpdateOutput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0);
        bool GetOutputStatus(byte cardNo, byte lineNo, byte DevNo, byte port);
        bool SetOutputStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false);
        double GetAInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, string range = "");
    }
}
