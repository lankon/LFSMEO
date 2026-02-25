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
        private List<LightData> LightListData = new List<LightData>();
        private Dictionary<string, LightData> LightListDict;
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
            int res = -1;
            foreach (ILightControl light in Light)
            {
                foreach (var item in LightListDict)
                {
                    if (item.Value.Title_LightType == light.GetLightControlType().ToString())
                    {

                    }


                    // item.Key 是鍵
                    // item.Value 是值（內容）
                        Console.WriteLine($"Key: {item.Key}, Value: {item.Value}");
                }

                res = light.Open();
                
                if (res == 0)
                    LightList.Add(light);
            }

            if(LightList.Count > 0)
            {
                IsInitial = true;

                if(LightList.Count > 1)
                    Tool.SaveLogToFile("實體光源 Initial Success");

                return 0;
            }
            else
                return -1;
        }
        public void LoadConfiguration(List<LightData> newLightDataList)
        {
            LightListData.Clear();
            LightListData.AddRange(newLightDataList);
            LightListDict = LightListData
                        .GroupBy(x => x.Title_Name)
                        .ToDictionary(g => g.Key, g => g.First());
        }
        public bool SetLightValue(ELightName name, int value)
        {
            LightListDict.TryGetValue(name.ToString(), out LightData light_data);

            int res = -1;
            int light_value = light_data.Title_Value;
            int light_port = light_data.Title_OutPort;
            int light_station = light_data.Title_Station;

            for (int j = 0; j < LightList.Count; j++)
            {
                string sLightCtrl = LightList[j].GetLightControlType().ToString();

                if (sLightCtrl != light_data.Title_LightType)
                    continue;

                res = LightList[j].SetLightValue(light_value, light_port, light_station);
            }

            if (res != 0)
                return false;
            else
                return true;
        }
        public bool SetLightValue(ELightControlType type, int station, int port, int value)
        {
            int res = -1;

            for (int j = 0; j < LightList.Count; j++)
            {
                if (LightList[j].GetLightControlType() != type)
                    continue;

                res = LightList[j].SetLightValue(value, port, station);
            }

            if (res != 0)
                return false;
            else
                return true;
        }



        public void SetCtrlDeviceType(ELightControlType type)
        {
            if (IsInitial == false)
                return;

            for (int i = 0; i < LightList.Count; i++)
            {
                if (LightList[i].GetLightControlType() == type)
                {
                    DeviceIndex = i;
                    break;
                }
            }
        }
        public bool SetLightValue(int value, int port = 0, int station_number = 0)
        {
            if (CheckLightUse() == false)
                return false;

            int res = -1;

            res = LightList[DeviceIndex].SetLightValue(value, port, station_number);

            if (res == 0)
            {
                Tool.SaveLogToFile($"VirtualLight Station:{station_number} Port:{port} SetValue:{value}");
                return true;
            }
            
            return false;
        }
        #endregion

    }
}
