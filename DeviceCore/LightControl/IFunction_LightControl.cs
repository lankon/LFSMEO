using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    
    public enum ELightName
    {
        NONE,
        LIGHT_1,
        LIGHT_2,
        LIGHT_3,
        LIGHT_4,
        LIGHT_5,
        LIGHT_6,
    }
    public class LightData
    {
        public string Title_LightType { get; set; }
        public string Title_Name { get; set; }
        public string Title_Description { get; set; }
        public string Title_Comport { get; set; }
        public int Title_Station { get; set; }
        public int Title_OutPort { get; set; }
        public int Title_Value { get; set; }
    }

    public interface IFunction_LightControl
    {
        void LoadConfiguration(List<LightData> newLightDataList);
        int Initial_All_LightControl();
        bool SetLightValue(ELightName name, int value);
        bool SetLightValue(ELightControlType type, string port_name, int station, int port, int value);
    }
}
