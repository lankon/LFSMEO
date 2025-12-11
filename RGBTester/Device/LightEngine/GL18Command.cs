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
    public class GL18Command:ILightEngineCommand
    {
        public GL18Command()
        {
            string portName = "COM5";
            _serialPort = new SerialPort(portName);     // COM,
            _serialPort.BaudRate = 115200;              // 胞率
            _serialPort.DataBits = 8;                   // 8 個資料位元
            _serialPort.Parity = Parity.None;           // 無奇偶校驗
            _serialPort.StopBits = StopBits.One;        // 1 個停止位元
            _serialPort.ReadTimeout = 500;              // 設定讀取超時時間
            _serialPort.WriteTimeout = 500;             // 設定寫入超時時間
            _serialPort.ReadBufferSize = 8192;          // 設定讀取緩衝區大小
            _serialPort.DataReceived += SerialPort_DataReceived;    //取得回傳資料

        }

        #region parameter define
        private SerialPort _serialPort;
        private ASK_STATE State = ASK_STATE.NONE;
        public byte LED_RightSide { get; private set; } = 0x00;
        public byte LED_LeftSide { get; private set; } = 0x01;
        public byte LED_R_LSB { get; private set; } = 0x13;
        public byte LED_G_LSB { get; private set; } = 0x14;
        public byte LED_B_LSB { get; private set; } = 0x15;
        public byte LED_RGB_MSB { get; private set; } = 0x17;
        private ManualResetEventSlim responseEvent = new ManualResetEventSlim(false);
        private int lastResponse = -1;
        //public event Action<int> OnResponseReceived;

        enum ASK_STATE
        {
            NONE,
            SET_DATA,
            GET_DATA,
        }
        #endregion

        #region public function
        public bool Open()
        {
            try
            {
                _serialPort.Open();
            }
            catch
            {
                return false;
            }

            return true;
        }
        public bool SetLed_DAC(byte rgb, byte side, int value)
        {
            byte high = (byte)(value >> 8);     // 高位元
            byte low = (byte)(value & 0xFF);    // 低位元
            int res = 0;

            long targetTicks = Stopwatch.Frequency / 1000;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedTicks < targetTicks * 10) { }

            // ====== 發送 LSB ======
            SetLedDriverData(side, rgb, low);

            res = WaitResponse(300);   // 等 300ms 回覆
            if (res < 0)
                return false;

            sw = Stopwatch.StartNew();
            while (sw.ElapsedTicks < targetTicks * 10) { }

            // ====== 發送 MSB ======
            SetLedDriverData(side, LED_RGB_MSB, high);

            res = WaitResponse(300);
            if (res < 0)
                return false;

            return true;
        }

        #endregion

        #region private function
        private bool SetLedDriverData(byte index, byte registerAddress, byte value)
        {
            State = ASK_STATE.SET_DATA;

            const byte PACKAGE_LENGTH = 6;
            const byte COMMAND = 0x87;

            byte[] commandData = { PACKAGE_LENGTH, COMMAND, index, registerAddress, value };
            byte[] packet = new byte[PACKAGE_LENGTH];

            packet[0] = PACKAGE_LENGTH;     // PL
            packet[1] = COMMAND;            // CMD
            packet[2] = index;              // Index (0x00 或 0x01)
            packet[3] = registerAddress;    // Data 1 (位址)
            packet[4] = value;              // Data 2 (值)

            packet[5] = CalculateChecksum(commandData);

            return SendCommand(packet);
        }
        private bool GetLedDriverData(byte index, byte registerAddress)
        {
            State = ASK_STATE.GET_DATA;

            const byte PACKAGE_LENGTH = 5;
            const byte COMMAND = 0x07;

            byte[] commandData = { COMMAND, index, registerAddress };
            byte[] packet = new byte[PACKAGE_LENGTH];

            packet[0] = PACKAGE_LENGTH;     // PL
            packet[1] = COMMAND;            // CMD
            packet[2] = index;              // Index (0x00 或 0x01)
            packet[3] = registerAddress;    // Data 1 (位址)

            packet[4] = CalculateChecksum(commandData);

            return SendCommand(packet);
        }
        private bool CheckConnect()
        {
            if (_serialPort.IsOpen)
                return true;
            else
                return false;
        }
        private int GetResponse(object sender, SerialDataReceivedEventArgs e)
        {
            // 等到對方送回資料時才會觸發
            string data = _serialPort.ReadExisting();

            byte[] buffer = new byte[1];

            int bytes = _serialPort.BytesToRead;
            if (bytes > 0)
            {
                if (bytes > buffer.Length)
                    buffer = new byte[bytes];

                _serialPort.Read(buffer, 0, bytes);

                return CheckResponse(buffer);
            }

            return -1;
        }
        private int WaitResponse(int timeoutMs = 300)
        {
            // 等待設備回覆
            bool ok = responseEvent.Wait(timeoutMs);

            // 下次使用前必須 Reset
            responseEvent.Reset();

            if (!ok)
                return -999; // timeout

            return lastResponse;
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytes = _serialPort.BytesToRead;
            if (bytes <= 0)
                return;

            byte[] buffer = new byte[bytes];
            _serialPort.Read(buffer, 0, bytes);

            lastResponse = CheckResponse(buffer);

            // 通知等待中的執行緒：已收到回覆
            responseEvent.Set();
        }
        private int CheckResponse(byte[] buffer)
        {
            if(State == ASK_STATE.SET_DATA && buffer.Length == 7)
            {
                if (buffer[3] == 0x41 && buffer[4] == 0x63 && buffer[5] == 0x6B)
                    return 0;
                else
                    return -1;
            }
            else if(State == ASK_STATE.GET_DATA && buffer.Length == 6)
            {
                return 0;
            }

            return -1;
        }
        private byte CalculateChecksum(byte[] data)
        {
            int sum = 0;

            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i];
            }

            return (byte)(sum & 0xFF);
        }
        private bool SendCommand(byte[] packet)
        {
            if (!_serialPort.IsOpen)
            {
                return false;
            }

            try
            {
                // 清空接收緩衝區
                _serialPort.DiscardInBuffer();

                // 寫入命令封包
                _serialPort.Write(packet, 0, packet.Length);

                //// 等待回傳，接收回傳資料
                //// 韌體成功回傳 "Ack" (0x41, 0x63, 0x6b)
                //// 這裡可以根據實際回傳的長度進行調整。
                //string response = _serialPort.ReadTo("\r\n"); // 假設回應以換行結束
                //                                              // 或者使用 ReadExisting() 讀取所有資料
                //                                              // string response = _serialPort.ReadExisting(); 
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SetLed_CurrentMode(string mode)
        {
            throw new NotImplementedException();
        }

        public string GetTemperature()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
