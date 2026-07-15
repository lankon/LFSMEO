using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Device_Virtual
{
    public class GodotUdpTransmitter : IDisposable
    {
        private readonly UdpClient _udpClient;
        private readonly string _targetIp;
        private readonly int _targetPort;

        public GodotUdpTransmitter(string ip = "127.0.0.1", int port = 8080)
        {
            _targetIp = ip;
            _targetPort = port;

            // 發送端通常不需要 Bind 固定的本地 Port，讓系統自動指派即可
            _udpClient = new UdpClient();
        }

        public int SendAxisDataWithTime(string axisName, double targetValue, double time_s)
        {
            try
            {
                // 格式化為 "X,50.0,2.5"
                string message = $"{axisName.ToUpper().Trim()},{targetValue:F3},{time_s:F3}";
                byte[] data = Encoding.UTF8.GetBytes(message);

                // 非同步送出，完全不阻塞主程式流程
                _udpClient.SendAsync(data, data.Length, _targetIp, _targetPort);

                return 0; // 成功
            }
            catch (Exception ex)
            {
                return -1;  // 發送失敗
            }

        }

        public void Dispose()
        {
            _udpClient?.Close();
            _udpClient?.Dispose();
        }
    }
}
