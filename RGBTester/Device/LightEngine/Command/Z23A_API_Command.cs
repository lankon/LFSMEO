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
        private bool IsInitial = false;
        public string ProductName { get; private set; } = eLEAType.Z23A_API.ToString();

        // [使用API時不用定義]
        public byte LED_R_LSB { get; private set; } = (byte)Z23A_FW.Color.COLOR_R;

        public byte LED_G_LSB { get; private set; } = (byte)Z23A_FW.Color.COLOR_G;

        public byte LED_B_LSB { get; private set; } = (byte)Z23A_FW.Color.COLOR_B;
        
        public byte LED_B2_LSB { get; private set; } = (byte)Z23A_FW.Color.COLOR_B2;        //需要修改成API定義

        public byte LED_RGB_MSB { get; private set; } = 0x00;       //沒有用到

        public byte LED_RightSide{ get; private set; } = 0x79;      //Z23A沒有左右邊,但流程需要,所以給一個區分左右邊的值      

        public byte LED_LeftSide { get; private set; } = 0x80;      //Z23A沒有左右邊,但流程需要,所以給一個區分左右邊的值
        #endregion

        #region private function

        #endregion

        #region public function
        public bool Open()
        {
            if(IsInitial == true)
                return true;

            string res = "";
            
            try
            {
                api = new Z23A_FW();
                res = api.Initial_UART_Settings();
            }
            catch
            {
                return false;
            }
            
            if (res != "Initial UART success")
                return false;

            res = api.Get_FW_Version();

            if (res == "UART_ERR_COM")
                return false;

            IsInitial = true;
            return true;
        }
        public bool SetLed_DAC(byte rgb, byte side, int value)
        {
            if(IsInitial == false) return false;
            
            Z23A_FW.Color color = Z23A_FW.Color.COLOR_ALL;
            int value_R = 0, value_G = 0, value_B = 0, value_B2 = 0;

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
            else if(rgb == LED_B2_LSB)
            {
                value_B2 = value;
            }
                
            int[] set_value = new int[] { value_R, value_G, value_B, value_B2 };

             Z23A_FW.Error_Code res = api.RAA491901_Set_DAC_Value(Z23A_FW.Color.COLOR_ALL, set_value);

            if (res == Z23A_FW.Error_Code.STATUS_OK)
                return true;
            else
                return false;
        }

        public bool SetLed_AllColorDAC(byte side, params int[] values)
        {
            if (IsInitial == false) 
                return false;

            if(values == null || values.Length < 3)
                return false;

            int value_R = Math.Min(values[0], 1023);
            int value_G = Math.Min(values[1], 1023);
            int value_B = Math.Min(values[2], 1023);

            int[] set_value;

            if (values.Length >= 4)
            {
                int value_B2 = Math.Min(values[3], 1023);
                set_value = new int[] { value_R, value_G, value_B, value_B2 };
            }
            else
                set_value = new int[] { value_R, value_G, value_B };

            Z23A_FW.Error_Code res = api.RAA491901_Set_DAC_Value(Z23A_FW.Color.COLOR_ALL, set_value);

            if (res == Z23A_FW.Error_Code.STATUS_OK)
                return true;
            else
                return false;
        }
        public bool SetLed_CurrentMode(string mode)
        {
            if (IsInitial == false) return false;

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
            if (IsInitial == false) return "-99";

            Tuple<Z23A_FW.Error_Code, double> res = api.TMP_Get_Temperature();

            if (res.Item1 == Z23A_FW.Error_Code.STATUS_OK)
                return res.Item2.ToString("F2");
            else
                return "-99";
        }
        public int[] Get_DAC()
        {
            int[] error = new int[] { -99, -99, -99 };

            if (IsInitial == false) return error;

            //var res = api.RAA491901_Get_Current_Mode();
            Tuple<Z23A_FW.Error_Code, int[]> res = api.RAA491901_Get_DAC_Value(Z23A_FW.Color.COLOR_ALL);

            if (res.Item1 == Z23A_FW.Error_Code.STATUS_OK)
                return res.Item2;
            else
                return error;
        }
        public bool ResetLED()
        {
            Z23A_FW.Error_Code res = api.RAA491901_Set_Startup_State(Z23A_FW.RAA_State.DISABLE_STATE);

            if(res != Z23A_FW.Error_Code.STATUS_OK)
                return false;

            res = api.RAA491901_Set_Startup_State(Z23A_FW.RAA_State.ENABLE_STATE);

            if (res != Z23A_FW.Error_Code.STATUS_OK)
                return false;

            Tuple<Z23A_FW.Error_Code, Z23A_FW.RAA_State> state = api.RAA491901_Get_Startup_State();

            if (state.Item2 == Z23A_FW.RAA_State.ENABLE_STATE)
                return true;
            else
                return false;
        }

        public double Get_VoltageLimit(byte rgb, byte side)
        {
            throw new NotImplementedException();
        }

        public bool Set_RegisterValue(byte adr, byte len, byte[] value)
        {
            Z23A_FW.Error_Code res = api.RAA491901_Set_Register_Value(adr, len, value);

            if (res == Z23A_FW.Error_Code.STATUS_OK)
                return true;
            else
                return false;
        }

        public bool SetLed_AllColorVoltage(byte side, params double[] values)
        {
            if (IsInitial == false)
                return false;

            if (values == null || values.Length < 1)
                return false;

            double voltage = Math.Min(values[0], 5.5);

            //int value_R = Math.Min(values[0], 1023);
            //int value_G = Math.Min(values[1], 1023);
            //int value_B = Math.Min(values[2], 1023);

            //int[] set_value;

            //if (values.Length >= 4)
            //{
            //    int value_B2 = Math.Min(values[3], 1023);
            //    set_value = new int[] { value_R, value_G, value_B, value_B2 };
            //}
            //else
            //    set_value = new int[] { value_R, value_G, value_B };

            //目前需要全部設一樣
            double[]  set_value = new double[] { voltage, voltage, voltage};

            Z23A_FW.Error_Code res = api.MAX77675_Set_Target_Voltage(Z23A_FW.Color.COLOR_ALL, values);

            if (res == Z23A_FW.Error_Code.STATUS_OK)
                return true;
            else
                return false;
        }
        #endregion
    }
}
