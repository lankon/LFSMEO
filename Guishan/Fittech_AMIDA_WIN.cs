using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace Device_Guishan
{
    public class Fittech_AMIDA_WIN : ITemperatureControl
    {
        public Fittech_AMIDA_WIN()
        {

        }

        #region parameter define
        private SerialPort Comport = new SerialPort();
        private string CtrlBox = "00";
        private string Channel = "0";
        private string SendCommand = "";
        private CMD_TYPE CurCmdType = CMD_TYPE.AskPV;
        private enum CMD_TYPE
        {
            None,
            AskPV,
            Start,
            Stop,
            Initialize
        }
        private enum ERROR_CODE
        {
            Success = 0,
            CMD_FAIL = -1,
            Fuse_Broken = -10,
        }
        #endregion

        #region private function
        private void DiscardOutBuffer()
        {
            if (Comport.IsOpen)
            {
                try
                {
                    if (Comport.BytesToWrite != 0)
                    {
                        Comport.DiscardOutBuffer();
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        private void DiscarInBuffer()
        {
            if (Comport.IsOpen)
            {
                try
                {
                    if (Comport.BytesToRead != 0)
                    {
                        Comport.DiscardInBuffer();
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        private void ClearBuffer()
        {
            DiscardOutBuffer();
            DiscarInBuffer();
        }
        #endregion

        #region public function
        public int Open(string com, string baudrate, string data_bits, string stop_bits, string parity)
        {
            Comport.PortName = com;
            Comport.BaudRate = int.Parse(baudrate);
            Comport.DataBits = int.Parse(data_bits);
            Comport.StopBits = (StopBits)Enum.Parse(typeof(StopBits), stop_bits);
            Comport.Parity = (Parity)Enum.Parse(typeof(Parity), parity);
            Comport.ReadTimeout = 2000;

            if (Comport.PortName == "None")
                return -1;

            if (!Comport.IsOpen)
            {
                try
                {
                    Comport.Open();
                }
                catch (Exception ex)
                {

                    return -1;
                }
            }
            return 0;
        }
        public int Close()
        {
            if (Comport.IsOpen)
            {
                try
                {
                    Comport.DiscardOutBuffer();
                    Comport.DiscardInBuffer();
                    Comport.Close();

                }
                catch (Exception ex)
                {
                    return -1;
                }
            }

            Comport.Dispose();

            return 0;
        }


        public ETemperatureControlType Get_TC_Type()
        {
            return ETemperatureControlType.Guishan_AMIDA_WIN;
        }
        public string GetPortName()
        {
            return Comport.PortName;
        }


        public int Initialize(string cmd = "")
        {
            string[] sp = cmd.Split(',');
            string command = $"B{sp[0]},INIT\r\n";
            SendCommand = command;

            try
            {
                ClearBuffer();
                Comport.Write(command);
                CurCmdType = CMD_TYPE.Initialize;
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public int AskPV(string cmd = "")
        {
            string[] sp = cmd.Split(',');
            string command = $"B{sp[0]},GTEMP,{sp[1]}\r\n";
            SendCommand = command;

            try
            {
                ClearBuffer();
                Comport.Write(command);
                CurCmdType = CMD_TYPE.AskPV;

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public int Start(double sv, string cmd = "")
        {
            string[] sp = cmd.Split(',');
            string SV_Value = sv.ToString("F2");
            string command = $"B{sp[0]},STEMP,1,{SV_Value},1,{sp[1]}\r\n";
            SendCommand = command;

            try
            {
                ClearBuffer();
                Comport.Write(command);
                CurCmdType = CMD_TYPE.Start;

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public int Stop(string cmd = "")
        {
            string[] sp = cmd.Split(',');
            string command = $"B{sp[0]},STEMP,0,25,1,{sp[1]}\r\n";
            SendCommand = command;

            try
            {
                ClearBuffer();
                Comport.Write(command);
                CurCmdType = CMD_TYPE.Stop;

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public int GetAnswer(out string[] answer, string cmd = "")
        {
            answer = new string[] { "" };

            try
            {
                Comport.ReadTimeout = 1000;
                string res = Comport.ReadLine();

                if(CurCmdType == CMD_TYPE.AskPV)
                {
                    string[] split_str = res.Split(',');
                    if (split_str.Length != 4)
                        return (int)ERROR_CODE.CMD_FAIL;

                    if(split_str[3] == "1") //保險絲斷路
                        return (int)ERROR_CODE.Fuse_Broken;

                    if (split_str[0] != $"B{CtrlBox}" || split_str[1] != "GTEMP")
                        return (int)ERROR_CODE.CMD_FAIL;

                    answer = new string[] { split_str[2], split_str[3], split_str[4], split_str[5], split_str[6] };  //溫度值
                }
                else
                {
                    if(res != SendCommand.Replace("\r\n", ""))
                        return (int)ERROR_CODE.CMD_FAIL;
                }

                return (int)ERROR_CODE.Success;
            }
            catch (Exception)
            {
                return (int)ERROR_CODE.CMD_FAIL;
            }
        }
        #endregion
    }
}
