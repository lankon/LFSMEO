using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

            //先註解掉要再解決Open不了時的情況
            //_serialPort.Open();

            Task task = Task.Run(() => Process());
        }

        #region parameter define
        private SerialPort _serialPort;
        #endregion

        #region public function
        public bool SetLedDriverData(byte index, byte registerAddress, byte value)
        {
            const byte PACKAGE_LENGTH = 6;
            const byte COMMAND = 0xF0;

            byte[] commandData = { COMMAND, index, registerAddress, value };
            byte[] packet = new byte[PACKAGE_LENGTH];

            packet[0] = PACKAGE_LENGTH;     // PL
            packet[1] = COMMAND;            // CMD
            packet[2] = index;              // Index (0x00 或 0x01)
            packet[3] = registerAddress;    // Data 1 (位址)
            packet[4] = value;              // Data 2 (值)

            packet[5] = CalculateChecksum(commandData);

            return SendCommand(packet);
        }
        public bool GetLedDriverData(byte index, byte registerAddress)
        {
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
        public bool CheckConnect()
        {
            if (_serialPort.IsOpen)
                return true;
            else
                return false;
        }
        public int GetResponse()
        {
            byte[] buffer = new byte[9000];

            int bytes = _serialPort.BytesToRead;
            if (bytes > 0)
            {
                if (bytes > buffer.Length)
                    buffer = new byte[bytes];

                _serialPort.Read(buffer, 0, bytes);
            }

            return 0;
        }
        #endregion

        #region private function
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

                // 等待回傳，接收回傳資料
                // 韌體成功回傳 "Ack" (0x41, 0x63, 0x6b)
                // 這裡可以根據實際回傳的長度進行調整。
                string response = _serialPort.ReadTo("\r\n"); // 假設回應以換行結束
                                                              // 或者使用 ReadExisting() 讀取所有資料
                                                              // string response = _serialPort.ReadExisting(); 
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void Process()
        {
            byte[] buffer = new byte[9000];

            while (_serialPort.IsOpen)
            {
                try
                {
                    int bytes = _serialPort.BytesToRead;
                    if (bytes > 0)
                    {
                        if (bytes > buffer.Length)
                            buffer = new byte[bytes];

                        _serialPort.Read(buffer, 0, bytes);

                        //OnDataReceived(buffer, bytes);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Read error: " + ex.Message);
                }
            }
        }
        #endregion
    }
}
