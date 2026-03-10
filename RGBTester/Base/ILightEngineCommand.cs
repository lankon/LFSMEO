using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public interface ILightEngineCommand
    {
        string ProductName { get; }

        byte LED_R_LSB { get; }
        byte LED_G_LSB { get; }
        byte LED_B_LSB { get; }
        byte LED_RGB_MSB { get; }
        byte LED_RightSide { get; }
        byte LED_LeftSide { get; }

        bool Open();
        bool ResetLED();

        bool SetLed_DAC(byte rgb, byte side, int value);
        bool SetLed_AllColorDAC(byte side, int value_r, int value_g, int value_b);
        bool SetLed_CurrentMode(string mode);
        bool Set_RegisterValue(byte adr, byte len, byte[] value);

        string GetTemperature();
        int[] Get_DAC();
        double Get_VoltageLimit(byte rgb, byte side);
        
        
    }
}
