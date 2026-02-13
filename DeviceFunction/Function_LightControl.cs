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
    public class Function_LightControl 
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
                res = light.Open();
                
                if (res == 0)
                    LightList.Add(light);
            }

            if(LightList.Count > 1)
            {
                IsInitial = true;
                Tool.SaveLogToFile("光源 Initial Success");
                return 0;
            }
            else
                return -1;
        }
        public void SetUseDeviceType(ELightControlType type)
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
