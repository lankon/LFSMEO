using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DeviceCore
{
    public enum ETemperatureControlType
    {
        NONE,
        VIRTUAL,
        Guishan_3Ch_Independent_RTD,
        Guishan_AMIDA_WIN,
    }

    public struct TC_INFO
    {
        public ETemperatureControlType Type;
        public string Name;
        public string PortName;
        public int BaudRate;
        public Parity Parity;
        public int DataBits;
        public StopBits StopBits;
    }

    public interface ITemperatureControl
    {
        int Open(string com, string baudrate, string data_bits, string stop_bits, string parity);   //開啟通訊阜
        int Close();                    //關閉通訊阜
        string GetPortName();           //取得通訊阜名稱
        ETemperatureControlType Get_TC_Type();      //取得溫控器型號
        int Initialize(string cmd = "");            //初始化溫控器
        int AskPV(string cmd = "");                 //詢問目前溫度
        int Start(double sv, string cmd = "");      //開始控溫
        int Stop(string cmd = "");                  //停止控溫
        int GetAnswer(out string[] answer, string cmd = "");        //取得回傳指令
    }
}
