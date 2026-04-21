using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace Device_TemeratureControl_Virtual
{
    public class Virtual_TemperatureControl : ITemperatureControl
    {

        #region parameter define
        private double CurrentTemperature = 25.0; // 預設溫度
        private CMD_TYPE CMD = CMD_TYPE.None;
        private string Comport;
        private enum CMD_TYPE
        {
            None,
            AskPV,
            Start,
            Stop,
            Initialize
        }
        #endregion

        #region public function
        public int Open(string com, string baudrate, string data_bits, string stop_bits, string parity)
        {
            Comport = com;
            return 0;
        }
        public int Close()
        {
            return 0;
        }
        public int Initialize(string cmd = "")
        {
            return 0;
        }

        public int Start(double sv, string cmd = "")
        {
            CurrentTemperature = sv;
            CMD = CMD_TYPE.Start;
            return 0;
        }
        public int Stop(string cmd = "")
        {
            CurrentTemperature = 25.0; // 停止控溫後回到預設溫度
            CMD = CMD_TYPE.Stop;
            return 0;
        }
        public int AskPV(string cmd = "")
        {
            CMD = CMD_TYPE.AskPV;
            return 0;
        }

        public ETemperatureControlType Get_TC_Type()
        {
            return ETemperatureControlType.VIRTUAL;
        }
        public string GetPortName()
        {
            return Comport;
        }
        public int GetAnswer(out string[] answer, string cmd = "")
        {
            if (CMD == CMD_TYPE.AskPV)
                answer = new string[] { CurrentTemperature.ToString("F2"), "0", "0", "0", "0" };
            else
                answer = new string[] { "" };

            return 0;
        }
        #endregion
    }
}
