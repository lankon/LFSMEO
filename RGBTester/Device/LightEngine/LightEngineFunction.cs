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

        #region public function
        public void Set_LEA_Type()
        {
            int type = ApplicationSetting.Get_Int_Recipe<eF_StartForm>((int)eF_StartForm.Cmbx_ProductType);
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
                    LED_RightSide = lea.LED_RightSide;
                    LED_LeftSide = lea.LED_LeftSide;
                }
            }
        }

        public bool Open()
        {
            bool res = LEA.Open();

            if (res == true)
                Tool.SaveLogToFile("Light Engine Initial Success");
            else
                Tool.SaveLogToFile("Light Engine Initial Fail", level:"ERR");

            return res;
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
        public bool SetLed_AllColorDAC(byte side, int value_r, int value_g, int value_b)
        {
            string s_side = "";

            if (side == LED_LeftSide)
                s_side = "Left";
            else
                s_side = "Right";

            Tool.SaveLogToFile($"Set {s_side} All Color DAC = R:{value_r} G:{value_g} B:{value_b}");
            if (LEA.SetLed_AllColorDAC(side, value_r, value_g, value_b))
                return true;
            else
                return false;
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

        #region private function


        #endregion
    }
}
