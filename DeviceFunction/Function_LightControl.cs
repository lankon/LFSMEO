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
    public class Function_LightControl :IFunction_LightControl
    {
        public Function_LightControl(IEnumerable<ILightControl> spec)
        {
            Light = spec;
        }

        #region parameter define
        private bool IsInitial = false;
        private int DeviceIndex = -1;
        private IEnumerable<ILightControl> Light;
        private List<ILightControl> LightList = new List<ILightControl>();
        private Dictionary<string, LightData> LightListDict;        //存放UI光源設定
        #endregion

        #region private function
        private bool CheckLightUse()
        {
            if(DeviceIndex != -1 && IsInitial == true)
                return true;
            else
                return false;
        }
        #endregion

        #region public function
        public int Initial_All_LightControl()
        {
            LightList.Clear();

            //依照光源型號以及COM名稱分組
            var hardwareConfigs = LightListDict.Values.GroupBy(x => new { x.Title_LightType, x.Title_Comport })
                                                      .Select(g => g.First());

            foreach (var config in hardwareConfigs)
            {
                var prototype = Light.FirstOrDefault(p => p.GetLightControlType().ToString() == config.Title_LightType);

                if (prototype != null)
                {
                    ILightControl newDevice = (ILightControl)Activator.CreateInstance(prototype.GetType());

                    LIGHT_INFO info = new LIGHT_INFO
                    {
                        Type = (ELightControlType)Enum.Parse(typeof(ELightControlType), config.Title_LightType),
                        PortName = config.Title_Comport,
                    };

                    newDevice.SetDeviceParameter(info);

                    int res = newDevice.Open();

                    if (res == 0)
                        LightList.Add(newDevice);
                    else
                        Tool.SaveLogToFile($"光源控制器 {config.Title_LightType} ({config.Title_Comport}) 開啟失敗, Error: {res}");
                }
            }

            int ret = -1;
            for (int i = 0; i < LightList.Count; i++)
            {
                ret = 0;

                if (LightList[i].GetLightControlType() != ELightControlType.VITUAL)
                {
                    Tool.SaveLogToFile("實體光源 Initial Success");
                    return 0;
                }
            }

            return ret;
        }
        public void LoadConfiguration(List<LightData> newLightDataList)
        {
            LightListDict = newLightDataList
                        .GroupBy(x => x.Title_Name)
                        .ToDictionary(g => g.Key, g => g.First());
        }
        public bool SetLightValue(ELightName name, int value)
        {
            LightListDict.TryGetValue(name.ToString(), out LightData light_data);

            if (light_data == null)
                return false;

            int res = -1;
            int light_value = value;
            int light_port = light_data.Title_OutPort;
            int light_station = light_data.Title_Station;

            var targetDevice = LightList.FirstOrDefault(device =>
                                                        device.GetLightControlType().ToString() == light_data.Title_LightType &&
                                                        device.GetPortName() == light_data.Title_Comport);

            if (targetDevice != null)
            {
                res = targetDevice.SetLightValue(light_value, light_port, light_station);
            }
            else
            {
                // 找不到對應硬體 (可能初始化失敗或 Port 設定錯誤)
                Tool.SaveLogToFile($"SetLightValue Fail: Cannot find hardware {light_data.Title_LightType} on {light_data.Title_Comport}");
                return false;
            }

            if (res != 0)
                return false;
            else
                return true;
        }
        public bool SetLightValue(ELightControlType type, string port_name, int station, int port, int value)
        {
            int res = -1;

            var targetDevice = LightList.FirstOrDefault(device =>
                                                        device.GetLightControlType() == type &&
                                                        device.GetPortName() == port_name);

            if (targetDevice != null)
            {
                res = targetDevice.SetLightValue(value, port, station);
            }
            else
            {
                // 找不到對應硬體 (可能初始化失敗或 Port 設定錯誤)
                Tool.SaveLogToFile($"SetLightValue Fail: Cannot find hardware {type} on {port_name}");
                return false;
            }

            if (res != 0)
                return false;
            else
                return true;
        }
        #endregion

    }
}
