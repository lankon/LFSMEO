using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    
    public enum ETemperatureControlName
    {
        NONE,
        TC_1,
        TC_2,
        TC_3,
        TC_4,
        TC_5,
        TC_6,
    }
    public class TemperatureControlData
    {
        public string Title_TC_Type { get; set; }
        public string Title_Name { get; set; }
        public string Title_Description { get; set; }
        public string Title_Comport { get; set; }
        public string Title_BaudRate { get; set; }
        public string Title_DataBits { get; set; }
        public string Title_StopBits { get; set; }
        public string Title_Parity { get; set; }
    }

    public interface IFunction_TemperatureControl
    {
        void LoadConfiguration(List<TemperatureControlData> new_TC_DataList);   //載入溫控裝置設定
        bool Initial_All_TemperatureControl();                      //初始化所有溫控裝置
        bool Close_All_TemperatureControl();                        //關閉所有溫控裝置
        bool AskPV(ETemperatureControlName name, string cmd = "");  //詢問目前溫度
        bool Start(ETemperatureControlName name, double sv, string cmd = "");   //開始控溫
        bool Stop(ETemperatureControlName name, string cmd = "");               //停止控溫
        string[] GetAnswer(ETemperatureControlName name, string cmd = "");      //取得回覆訊息
    }
}
