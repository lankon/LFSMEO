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

namespace RGBTester.Device
{
    public class LightEngineFunction:IFunction_LightEngine
    {
        public LightEngineFunction(IEnumerable<ILightEngineCommand> lea)
        {
            LEAs = lea;
        }

        #region parameter define
        private IEnumerable<ILightEngineCommand> LEAs;
        private ILightEngineCommand LEA;
        private ASK_STATE State = ASK_STATE.NONE;
        public byte LED_RightSide { get; private set; } = 0x00;
        public byte LED_LeftSide { get; private set; } = 0x00;
        public byte LED_R { get; private set; } = 0x00;
        public byte LED_G { get; private set; } = 0x00;
        public byte LED_B { get; private set; } = 0x00;
        public byte LED_B2 { get; private set; } = 0x00;
        private ManualResetEventSlim responseEvent = new ManualResetEventSlim(false);
        private int lastResponse = -1;
        private int SelectType = -1;
        //public event Action<int> OnResponseReceived;

        enum ASK_STATE
        {
            NONE,
            SET_DATA,
            GET_DATA,
        }
        #endregion

        #region private function
        #endregion

        #region public function
        public void Set_LEA_Type()
        {
            int type = ApplicationSetting.Get_Int_Recipe<eF_StartForm>((int)eF_StartForm.Cmbx_ProductType); //有問題 不應該寫在這裡吧
            eLEAType select_type = eLEAType.VIRTUAL;

            if (type == (int)eLEAType.VIRTUAL)
                select_type = eLEAType.VIRTUAL;
            else if (type == (int)eLEAType.GL18)
                select_type = eLEAType.GL18;
            else if (type == (int)eLEAType.Z23A_API)
                select_type = eLEAType.Z23A_API;

            foreach (ILightEngineCommand lea in LEAs)
            {
                if (lea.ProductName == select_type.ToString())
                {
                    LEA = lea;
                    LED_R = lea.LED_R_LSB;
                    LED_G = lea.LED_G_LSB;
                    LED_B = lea.LED_B_LSB;
                    LED_B2 = lea.LED_B2_LSB;
                    LED_RightSide = lea.LED_RightSide;
                    LED_LeftSide = lea.LED_LeftSide;
                }
            }
        }

        public bool Open()
        {
            try
            {
                bool res = LEA.Open();

                if (res == true)
                    Tool.SaveLogToFile("Light Engine Initial Success");
                else
                    Tool.SaveLogToFile("Light Engine Initial Fail", level: "ERR");

                return res;
            }
            catch
            {
                return false;
            }
        }
        public bool ResetLED()
        {
            return LEA.ResetLED();
        }

        //[Set Function]
        public bool SetLed_DAC(byte rgb, byte side, int value)
        {
            string color = "";
            string s_side = "";

            if (rgb == LED_R)
                color = "Red";
            else if (rgb == LED_G)
                color = "Green";
            else if (rgb == LED_B)
                color = "Blue";
            else if (rgb == LED_B2)
                color = "Blue2";

            if (side == LED_LeftSide)
                s_side = "Left";
            else
                s_side = "Right";

            Tool.SaveLogToFile($"Set {s_side} {color} DAC = {value}");
            if (LEA.SetLed_DAC(rgb, side, value))

                return true;
            else
                return false;
        }
        public bool SetLed_AllColorDAC(byte side, params int[] colors)
        {
            string s_side = "";

            if (side == LED_LeftSide)
                s_side = "Left";
            else
                s_side = "Right";

            if (LEA.SetLed_AllColorDAC(side, colors))
            {
                if(colors.Length == 4)
                    Tool.SaveLogToFile($"Set {s_side} All Color DAC = R:{colors[0]} G:{colors[1]} B:{colors[2]} B2:{colors[3]}");
                else
                    Tool.SaveLogToFile($"Set {s_side} All Color DAC = R:{colors[0]} G:{colors[1]} B:{colors[2]}");
                return true;
            }
            else
            {
                Tool.SaveLogToFile($"Set {s_side} All Color DAC fail");
                return false;
            }
        }
        public bool SetLed_AllColorVoltage(byte side, params double[] values)
        {
            string s_side = "";

            if (side == LED_LeftSide)
                s_side = "Left";
            else
                s_side = "Right";

            if (LEA.SetLed_AllColorVoltage(side, values))
            {
                if (values.Length == 4)
                    Tool.SaveLogToFile($"Set {s_side} All Color Voltage = R:{values[0]} G:{values[1]} B:{values[2]} B2:{values[3]}");
                else
                    Tool.SaveLogToFile($"Set {s_side} All Color Voltage = R:{values[0]} G:{values[1]} B:{values[2]}");
                return true;
            }
            else
            {
                Tool.SaveLogToFile($"Set {s_side} All Color Voltage fail");
                return false;
            }
        }
        public bool SetLed_CurrentMode(string mode)
        {
            Tool.SaveLogToFile($"Set LED {mode}");

            if (LEA.SetLed_CurrentMode(mode))
                return true;
            else
                return false;
        }
        public bool Set_RegisterValue(byte adr, byte len, byte[] value)
        {
            if (LEA.Set_RegisterValue(adr, len, value))
                return true;
            else
                return false;
        }

        //[Get Function]
        public string GetTemperature()
        {
            return LEA.GetTemperature();
        }
        public int[] Get_DAC()
        {
            return LEA.Get_DAC();
        }
        public double Get_VoltageLimit(byte rgb, byte side)
        {
            return LEA.Get_VoltageLimit(rgb, side);
        }
        
        #endregion

    }
}
