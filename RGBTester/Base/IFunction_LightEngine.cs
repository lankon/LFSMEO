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
    
    public interface IFunction_LightEngine
    {
        byte LED_R { get; }
        byte LED_G { get; }
        byte LED_B { get; }
        byte LED_B2 { get; }
        byte LED_RightSide { get; }
        byte LED_LeftSide { get; }

        void Set_LEA_Type();
        bool Open();
        bool SetLed_DAC(byte rgb, byte side, int value);
        bool SetLed_AllColorDAC(byte side, params int[] colors);
        bool SetLed_AllColorVoltage(byte side, params double[] values);
        bool SetLed_CurrentMode(string mode);
        bool Set_RegisterValue(byte adr, byte len, byte[] value);

        string GetTemperature();
        int[] Get_DAC();
        double Get_VoltageLimit(byte rgb, byte side);

        bool ResetLED();
    }
}
