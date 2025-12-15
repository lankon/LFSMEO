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
        public byte LED_RightSide { get; private set; } = 0x00;
        public byte LED_LeftSide { get; private set; } = 0x01;
        public byte LED_R_LSB { get; private set; } = 0x13;
        public byte LED_G_LSB { get; private set; } = 0x14;
        public byte LED_B_LSB { get; private set; } = 0x15;
        public byte LED_RGB_MSB { get; private set; } = 0x17;
        #endregion

        #region public function
        public bool Open()
        {
            return true;
        }
        public bool SetLed_DAC(byte rgb, byte side, int value)
        {
            byte high = (byte)(value >> 8);     // 高位元
            byte low = (byte)(value & 0xFF);    // 低位元

            long targetTicks = Stopwatch.Frequency / 1000;//ms
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedTicks < targetTicks*10) { }

            // ====== 發送 LSB ======
            SetLedDriverData(side, rgb, low);

            // ====== 模擬等待回傳時間 ======
            sw = Stopwatch.StartNew();
            while (sw.ElapsedTicks < targetTicks * 15 / 10) { }

            // ====== 每道指令需間隔的時間 ======
            sw = Stopwatch.StartNew();
            while (sw.ElapsedTicks < targetTicks*10) { }

            // ====== 發送 MSB ======
            SetLedDriverData(side, LED_RGB_MSB, high);

            // ====== 模擬等待回傳時間 ======
            sw = Stopwatch.StartNew();
            while (sw.ElapsedTicks < targetTicks * 15 / 10) { }

            return true;
        }
        public bool SetLed_CurrentMode(string mode)
        {
            Tool.SaveLogToFile($"Set LED Board {mode}", level: "DBG");
            return true;
        }

        public string GetTemperature()
        {
            return "25";
        }
        public int[] Get_DAC()
        {
            int[] res = new int[] { 255, 255, 255 };

            return res;
        }
        #endregion

        #region private function
        private bool SetLedDriverData(byte index, byte registerAddress, byte value)
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
        private bool GetLedDriverData(byte index, byte registerAddress)
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
        private int GetResponse()
        {

            return 0;
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
            string command = string.Join(" ", packet.Select(b => $"0x{b:X2}"));
            Tool.SaveLogToFile(command);

            return true;
        }
        #endregion
    }
}
