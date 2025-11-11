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
        bool Open(string port, int baud_rate, StopBits stopBits, Parity parity);
        void Close();
        void StartChiller();
        void StopChiller();
        void SetTemperature(double temperature);
        void GetTemerature();
    }
}
