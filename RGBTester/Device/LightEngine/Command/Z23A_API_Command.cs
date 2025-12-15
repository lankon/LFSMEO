using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ToolFunction;
using RGBTester.Base;
using Z23A_API;

namespace RGBTester.Device
{
    public class Z23A_API_Command:ILightEngineCommand
    {
        public Z23A_API_Command()
        {
        }

        #region parameter define
        Z23A_FW api;
        public string ProductName { get; private set; } = eLEAType.Z23A_API.ToString();

        // [使用API時不用定義]
        public byte LED_R_LSB { get; private set; } = (byte)Z23A_FW.Color.COLOR_R;

        public byte LED_G_LSB { get; private set; } = (byte)Z23A_FW.Color.COLOR_G;

        public byte LED_B_LSB { get; private set; } = (byte)Z23A_FW.Color.COLOR_B;

        public byte LED_RGB_MSB { get; private set; } = 0x00;       //沒有用到

        public byte LED_RightSide{ get; private set; } = 0x00;      //沒有用到

        public byte LED_LeftSide { get; private set; } = 0x00;      //沒有用到
        #endregion

        #region public function
        public bool Open()
        {
            api = new Z23A_FW();
            string res = api.Initial_UART_Settings();

            if (res == "Initial UART success")
                return true;
            else
                return false;
        }
        public bool SetLed_DAC(byte rgb, byte side, int value)
        {
            Z23A_FW.Color color = Z23A_FW.Color.COLOR_ALL;
            int value_R = 0, value_G = 0, value_B = 0;

            if (value > 1023)   //硬體限制最大1023
                value = 1023;

            if (rgb == LED_R_LSB)
            {
                color = Z23A_FW.Color.COLOR_R;
                value_R = value;
            }
            else if (rgb == LED_G_LSB)
            {
                color = Z23A_FW.Color.COLOR_G;
                value_G = value;
            }
            else if (rgb == LED_B_LSB)
            {
                color = Z23A_FW.Color.COLOR_B;
                value_B = value;
            }
                
            int[] set_value = new int[] { value_R, value_G, value_B };

             Z23A_FW.Error_Code res = api.RAA491901_Set_DAC_Value(Z23A_FW.Color.COLOR_ALL, set_value);

            if (res == Z23A_FW.Error_Code.STATUS_OK)
                return true;
            else
                return false;
        }

        public bool SetLed_CurrentMode(string mode)
        {
            Z23A_FW.Current_Mode current_Mode = Z23A_FW.Current_Mode.LOW_MODE;

            if (mode == "HCM")   //High Current Mode
                current_Mode = Z23A_FW.Current_Mode.HIGH_MODE;
            else if (mode == "LCM")
                current_Mode = Z23A_FW.Current_Mode.LOW_MODE;

            Z23A_FW.Error_Code res = api.RAA491901_Set_Current_Mode(current_Mode);

            if (res == Z23A_FW.Error_Code.STATUS_OK)
                return true;
            else
                return false;
        }
        public string GetTemperature()
        {
            Tuple<Z23A_FW.Error_Code, double> res = api.TMP_Get_Temperature();

            if (res.Item1 == Z23A_FW.Error_Code.STATUS_OK)
                return res.Item2.ToString("F2");
            else
                return "-99";
        }
        public int[] Get_DAC()
        {
            int[] error = new int[] { -99, -99, -99 };

            //var res = api.RAA491901_Get_Current_Mode();
            Tuple<Z23A_FW.Error_Code, int[]> res = api.RAA491901_Get_DAC_Value(Z23A_FW.Color.COLOR_ALL);

            if (res.Item1 == Z23A_FW.Error_Code.STATUS_OK)
                return res.Item2;
            else
                return error;
        }
        #endregion

        #region private function

        #endregion
    }
}
