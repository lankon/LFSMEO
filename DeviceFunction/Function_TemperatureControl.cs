using DeviceCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ToolFunction;

namespace DeviceFunction
{
    public class Function_TemperatureControl : IFunction_TemperatureControl
    {
        public Function_TemperatureControl(IEnumerable<ITemperatureControl> TC)
        {
            TemperatureControls = TC;
        }

        #region parameter define
        private IEnumerable<ITemperatureControl> TemperatureControls;
        private List<ITemperatureControl> TemperatureControlList = new List<ITemperatureControl>();
        private Dictionary<string, TemperatureControlData> TemperatureControlListDict;        //存放UI溫控設定
        #endregion

        #region private function
        private ITemperatureControl GetTargetDevice(ETemperatureControlName name, out TemperatureControlData TC_Data)
        {
            ITemperatureControl targetDevice = null;
            TemperatureControlListDict.TryGetValue(name.ToString(), out TemperatureControlData tc_data);
            TC_Data = tc_data;

            if (tc_data == null)
                return targetDevice;

            string comport = tc_data.Title_Comport;

            targetDevice = TemperatureControlList.FirstOrDefault(device =>
                                                        device.Get_TC_Type().ToString() == tc_data.Title_TC_Type &&
                                                        device.GetPortName() == tc_data.Title_Comport);

            return targetDevice;
        }
        #endregion

        #region public function
        public bool Initial_All_TemperatureControl()
        {
            TemperatureControlList.Clear();

            //依照溫控型號以及COM名稱分組
            var hardwareConfigs = TemperatureControlListDict.Values.GroupBy(x => new { x.Title_TC_Type, x.Title_Comport })
                                                                    .Select(g => g.First());

            foreach (var config in hardwareConfigs)
            {
                var prototype = TemperatureControls.FirstOrDefault(p => p.Get_TC_Type().ToString() == config.Title_TC_Type);

                if (prototype != null)
                {
                    ITemperatureControl newDevice = (ITemperatureControl)Activator.CreateInstance(prototype.GetType());

                    int res = newDevice.Open(config.Title_Comport, config.Title_BaudRate, 
                                                config.Title_DataBits, config.Title_StopBits, 
                                                config.Title_Parity);

                    if (res == 0)
                        TemperatureControlList.Add(newDevice);
                    else
                        Tool.SaveLogToFile($"溫度控制器 {config.Title_TC_Type} ({config.Title_Comport})連線失敗", level:"ERR");
                }
            }

            int ret = -1;
            for (int i = 0; i < TemperatureControlList.Count; i++)
            {
                ret = 0;

                if (TemperatureControlList[i].Get_TC_Type() != ETemperatureControlType.VIRTUAL)
                {
                    Tool.SaveLogToFile("實體溫控器 Initial Success");
                    return true;
                }
            }

            return false;
        }
        public bool Close_All_TemperatureControl()
        {
            int res = -1;
            foreach (var device in TemperatureControlList)
            {
                res = device.Close();
                if (res != 0)
                    Tool.SaveLogToFile($"溫度控制器 {device.Get_TC_Type()} 關閉失敗", level:"ERR");
            }
            TemperatureControlList.Clear();

            return res == 0;
        }
        public void LoadConfiguration(List<TemperatureControlData> new_TC_DataList)
        {
            TemperatureControlListDict = new_TC_DataList.GroupBy(x => x.Title_Name)
                                                        .ToDictionary(g => g.Key, g => g.First());

        }
        public bool AskPV(ETemperatureControlName name, string cmd = "")
        {
            ITemperatureControl targetDevice = GetTargetDevice(name, out TemperatureControlData tc_data);

            if (targetDevice == null || tc_data == null)
            {
                Tool.SaveLogToFile($"AskPV Fail: Cannot find hardware {tc_data?.Title_TC_Type} on {tc_data?.Title_Comport}", level: "ERR");
                return false;
            }

            int res = targetDevice.AskPV(cmd);

            return res == 0;
        }
        public bool Start(ETemperatureControlName name, double sv, string cmd = "")
        {
            ITemperatureControl targetDevice = GetTargetDevice(name, out TemperatureControlData tc_data);

            if(targetDevice == null || tc_data == null)
            {
                Tool.SaveLogToFile($"Start Fail: Cannot find hardware {tc_data?.Title_TC_Type} on {tc_data?.Title_Comport}", level: "ERR");
                return false;
            }

            int res = targetDevice.Start(sv, cmd);

            if(res == 0)
                Tool.SaveLogToFile($"Start Success: Device {tc_data?.Title_TC_Type} on {tc_data?.Title_Comport} set SV to {sv}", level: "DBG");

            return res == 0;
        }
        public bool Stop(ETemperatureControlName name, string cmd = "")
        {
            ITemperatureControl targetDevice = GetTargetDevice(name, out TemperatureControlData tc_data);

            if (targetDevice == null || tc_data == null)
            {
                Tool.SaveLogToFile($"Stop Fail: Cannot find hardware {tc_data?.Title_TC_Type} on {tc_data?.Title_Comport}", level: "ERR");
                return false;
            }

            int res = targetDevice.Stop(cmd);

            return res == 0;
        }
        public string[] GetAnswer(ETemperatureControlName name, string cmd = "")
        {
            ITemperatureControl targetDevice = GetTargetDevice(name, out TemperatureControlData tc_data);
            
            if (targetDevice == null || tc_data == null)
            {
                Tool.SaveLogToFile($"GetAnswer Fail: Cannot find hardware {tc_data?.Title_TC_Type} on {tc_data?.Title_Comport}", level: "ERR");
                return new string[] { "ERR" };
            }

            int res = targetDevice.GetAnswer(out string[] answer, cmd);
            if (res != 0)
            {
                Tool.SaveLogToFile($"GetAnswer Fail: Device {tc_data?.Title_TC_Type} on {tc_data?.Title_Comport} cannot answer", level: "ERR");
                return new string[] { "ERR" };
            }
            return answer;
        }
        #endregion

    }
}
