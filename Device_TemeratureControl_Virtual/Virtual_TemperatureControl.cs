using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace Device_TemeratureControl_Virtual
{
    public class Virtual_TemperatureControl : ITemperatureControl, IVirtualTemperatureControl
    {

        #region parameter define
        private string lastChannel = "";
        private string Comport;
        private CMD_TYPE CMD = CMD_TYPE.None;
        private readonly object sync = new object();
        private readonly Dictionary<string, ChannelState> channels = new Dictionary<string, ChannelState>();
        private enum CMD_TYPE
        {
            None,
            AskPV,
            Start,
            Stop,
            Initialize
        }
        private class ChannelState
        {
            public double AutomaticTemperature = 25.0;
            public double ManualTemperature = 25.0;
            public bool Manual;
        }
        #endregion

        #region private function
        private ChannelState GetChannel(string cmd)
        {
            string key = (cmd ?? "").Trim();
            if (!channels.TryGetValue(key, out var state))
                channels[key] = state = new ChannelState();
            return state;
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
            lock (sync)
            {
                GetChannel(cmd).AutomaticTemperature = sv;
                CMD = CMD_TYPE.Start;
            }
            return 0;
        }
        public int Stop(string cmd = "")
        {
            lock (sync)
            {
                GetChannel(cmd).AutomaticTemperature = 25.0;
                CMD = CMD_TYPE.Stop;
            }
            return 0;
        }
        public int AskPV(string cmd = "")
        {
            lock (sync)
            {
                lastChannel = cmd;
                CMD = CMD_TYPE.AskPV;
            }
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
            lock (sync)
            {
                var state = GetChannel(string.IsNullOrEmpty(cmd) ? lastChannel : cmd);
                double pv = state.Manual ? state.ManualTemperature : state.AutomaticTemperature;
                if (CMD == CMD_TYPE.AskPV)
                    answer = new string[] { pv.ToString("F2"), "0", "0", "0", "0" };
                else
                    answer = new string[] { "" };
            }

            return 0;
        }
        public void SetSimulation(string channel, bool manual, double temperature)
        {
            if (double.IsNaN(temperature) || double.IsInfinity(temperature) || temperature < 0 || temperature > 150)
                return;

            lock (sync)
            {
                var state = GetChannel(channel);
                state.Manual = manual;
                state.ManualTemperature = temperature;
            }
        }
        public void GetSimulationSettings(string channel, out bool manual, out double temperature)
        {
            lock (sync)
            {
                var state = GetChannel(channel);
                manual = state.Manual;
                temperature = state.ManualTemperature;
            }
        }
        #endregion
    }
}
