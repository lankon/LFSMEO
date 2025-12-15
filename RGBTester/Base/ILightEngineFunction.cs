using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public enum eLEAType
    {
        VIRTUAL,
        Z23A_API,
        GL18,
    }
    
    public interface ILightEngineFunction
    {
        byte LED_R { get; }
        byte LED_G { get; }
        byte LED_B { get; }
        byte LED_RightSide { get; }
        byte LED_LeftSide { get; }

        void Set_LEA_Type();
        bool Open();
        bool SetLed_DAC(byte rgb, byte side, int value);
        bool SetLed_CurrentMode(string mode);
        string GetTemperature();
        int[] Get_DAC();
    }
}
