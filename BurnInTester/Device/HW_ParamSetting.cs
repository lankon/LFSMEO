using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BurnInTester.Device
{
    public class HW_ParamSetting
    {
        public TemperatureControlBox TC_Box = new TemperatureControlBox();

        public class TemperatureControlBox
        {
            private const int CtrlBoxNum = 40;

            public int _CtrlBoxNum { get; set; } = CtrlBoxNum;                  //溫控箱數量
            public string[] BoxNum { get; set; } = new string[CtrlBoxNum];      //溫控箱編號
            public string[] ChNum { get; set; } = new string[CtrlBoxNum];       //溫控箱通道編號
            public bool[] Use { get; set; } = new bool[CtrlBoxNum];             //是否使用
        }
    }
}
