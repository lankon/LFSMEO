using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DeviceCore
{
    public interface IChillerControl
    {
        double Temperature { get; set; }
        bool bRunning { get; set; }
        bool Open(string port, int baud_rate, Parity parity, int data_bits, StopBits stopBits);
        void Close();
        void StartChiller();
        void StopChiller();
        void SetTemperature(double temperature);
        void GetTemerature();
        void GetStatus();
    }
}
