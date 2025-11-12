using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace Device_Klzx
{
    public class ChillerControl_Klzx:IChillerControl
    {
        #region parameter define
        public double Temperature { get; set; } = 0.0;
        public bool bRunning { get; set; } = false;
        private SerialPort _serialPort;
        private byte _deviceAddress = 0x01;
        private ASK_TYPE AskType = ASK_TYPE.STATUS;

        enum ASK_TYPE
        {
            START,
            STOP,
            STATUS,
        }
        #endregion

        #region private function
        /// <summary>
        /// 發送 Modbus 命令 (包含計算 CRC 並讀取回覆)
        /// </summary>
        private void SendModbusCommand(byte[] commandData)
        {
            if (!_serialPort.IsOpen)
            {
                throw new InvalidOperationException("通訊埠尚未開啟。");
            }
            
            // 1. 創建一個包含CRC的完整幀
            byte[] fullFrame = new byte[commandData.Length + 2];
            Array.Copy(commandData, fullFrame, commandData.Length);

            // 2. 計算 CRC
            byte[] crc = CalculateCRC(commandData);
            fullFrame[commandData.Length] = crc[0];     // CRC Low
            fullFrame[commandData.Length + 1] = crc[1]; // CRC High

            // 3. 清空緩衝區並發送數據
            _serialPort.DiscardInBuffer();
            _serialPort.DiscardOutBuffer();
            _serialPort.Write(fullFrame, 0, fullFrame.Length);
        }

        /// <summary>
        /// 計算 Modbus RTU CRC16
        /// </summary>
        private byte[] CalculateCRC(byte[] data)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }

            return new byte[] { (byte)(crc & 0xFF), (byte)(crc >> 8) };
        }

        /// <summary>
        /// 解封包
        /// </summary>
        /// <param name="receive"></param>
        private void UnPack(byte[] receive)
        {
            if(AskType == ASK_TYPE.STATUS)
            {
                //[運行狀態]
                short statusCode = (short)((receive[3] << 8) | receive[4]);
                switch(statusCode)
                {
                    case 2:
                        bRunning = true;    //設備運作中
                        break;
                    default:
                        bRunning = false;   //設備停止中
                        break;
                }
                
                //[溫度]
                short rawTemperature = (short)((receive[17] << 8) | receive[18]);
                Temperature = rawTemperature / 10.0;
            }
        }
        #endregion

        /// <summary>
        /// 開啟通訊指令 (開啟 Serial Port)
        /// </summary>
        public bool Open(string port, int baud_rate, Parity parity, int data_bits, StopBits stopBits)
        {
            _serialPort = new SerialPort(port);
            _serialPort.BaudRate = baud_rate;
            _serialPort.Parity = parity;
            _serialPort.DataBits = data_bits;
            _serialPort.StopBits = stopBits;
            
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 關閉通訊指令 (關閉 Serial Port)
        /// </summary>
        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        /// <summary>
        /// 取得回傳碼
        /// </summary>
        public byte[] GetResponse()
        {
            int bytesToRead = _serialPort.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            _serialPort.Read(buffer, 0, bytesToRead);

            UnPack(buffer);

            return buffer;
        }

        /// <summary>
        /// 開始送水
        /// </summary>
        public void StartChiller()
        {
            byte[] commandData = new byte[]
            {
                _deviceAddress, // 設備地址
                0x06,           // 功能碼 0x06
                0x02, 0x00,     // 數據地址 (Hi: 0x02, Lo: 0x00)
                0x00, 0x72      // 數據值 (Hi: 0x00, Lo: 0x72)
            };

            SendModbusCommand(commandData);
        }

        /// <summary>
        /// 停止送水
        /// </summary>
        public void StopChiller()
        {
            byte[] commandData = new byte[]
            {
                _deviceAddress, // 設備地址
                0x06,           // 功能碼 0x06
                0x02, 0x00,     // 數據地址 (Hi: 0x02, Lo: 0x00)
                0x00, 0x70      // 數據值 (Hi: 0x00, Lo: 0x72)
            };

            SendModbusCommand(commandData);
        }

        /// <summary>
        /// 設定溫度指令
        /// </summary>
        /// <param name="temperature">要設定的溫度 (例如 20.5)</param>
        public void SetTemperature(double temperature)
        {
            // 例如 20.5°C 應寫入 205 (即 0x00CD)
            short tempValue = (short)(temperature * 10);

            byte[] commandData = new byte[]
            {
                _deviceAddress, // 設備地址
                0x06,           // 功能碼 0x06
                0x04, 0x01,     // 數據地址 (Hi: 0x04, Lo: 0x01)
                (byte)(tempValue >> 8),   // 數據值 (Hi)
                (byte)(tempValue & 0xFF)  // 數據值 (Lo)
            };

            SendModbusCommand(commandData);
        }

        /// <summary>
        /// 取得溫度
        ///</summary>
        public void GetTemerature()
        {
            byte[] commandData = new byte[]
            {
                _deviceAddress, // 設備地址
                0x04,           // 功能碼 0x06
                0x00, 0x07,     // 數據地址 (Hi: 0x04, Lo: 0x01)
                0x00,           // 數據個數 (Hi)
                0x01            // 數據個數 (Lo)
            };

            SendModbusCommand(commandData);
        }

        public void GetStatus()
        {
            AskType = ASK_TYPE.STATUS;
            
            byte[] commandData = new byte[]
            {
                _deviceAddress, // 設備地址
                0x04,           // 功能碼
                0x00, 0x00,     // 數據地址 (Hi,Lo)
                0x00,           // 數據個數 (Hi)
                0x08            // 數據個數 (Lo)
            };

            SendModbusCommand(commandData);
        }
    }
}
