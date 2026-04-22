using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DeviceCore
{
    public interface ITemperatureControl
    {
        int Open(string com, string baudrate, string data_bits, string stop_bits, string parity);   //開啟通訊阜
        int Close();                    //關閉通訊阜
        int Initialize();               //初始化溫控器
        int AskPV();                    //詢問目前溫度
        int Start(double sv);           //開始控溫
        int Stop();                     //停止控溫
        int GetAnswer(out string answer);       //取得回傳指令
    }
}
