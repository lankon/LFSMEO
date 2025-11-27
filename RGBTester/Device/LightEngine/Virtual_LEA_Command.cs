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
    public class Virtual_LEA_Command:ILightEngineCommand
    {
        public Virtual_LEA_Command()
        {
            string portName = "COM3";
            _serialPort = new SerialPort(portName);     //COM,
            _serialPort.BaudRate = 19200;               //胞率
            _serialPort.DataBits = 8;                   // 8 個資料位元
            _serialPort.Parity = Parity.None;           // 無奇偶校驗
            _serialPort.StopBits = StopBits.One;        // 1 個停止位元
            _serialPort.ReadTimeout = 500;              // 設定讀取超時時間
            _serialPort.WriteTimeout = 500;             // 設定寫入超時時間
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
            const byte COMMAND = 0x70;

            byte[] commandData = { COMMAND, index, registerAddress };
            byte[] packet = new byte[PACKAGE_LENGTH];

            packet[0] = PACKAGE_LENGTH;     // PL
            packet[1] = COMMAND;            // CMD
            packet[2] = index;              // Index (0x00 或 0x01)
            packet[3] = registerAddress;    // Data 1 (位址)

            packet[4] = CalculateChecksum(commandData);

            return SendCommand(packet);
        }
        public int GetResponse()
        {

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
            return true;
        }

        public bool CheckConnect()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
