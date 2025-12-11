using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public interface ILightEngineCommand
    {
        byte LED_R_LSB { get; }
        byte LED_G_LSB { get; }
        byte LED_B_LSB { get; }
        byte LED_RGB_MSB { get; }
        byte LED_RightSide { get; }
        byte LED_LeftSide { get; }

        bool Open();
        bool SetLed_DAC(byte rgb, byte side, int value);
        bool SetLed_CurrentMode(string mode);
        string GetTemperature();
    }
}
