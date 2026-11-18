using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DeviceCore
{
    public enum ESMUName
    {
        SMU_1,
        SMU_2,
        SMU_3,
        SMU_4,
        SMU_5,
        SMU_6,
    }

    public class SourceMeterData
    {
        public string Title_SMU_Type { get; set; }
        public string Title_Name { get; set; }
        public string Title_CardID { get; set; }
        public string Title_Description { get; set; }
        public string Title_Sense { get; set; }
        public string Title_TransResp { get; set; }
        public string Title_InputTrigger { get; set; }
        public string Title_OutputTrigger { get; set; }
    }

    public interface IFunction_SourceMeter
    {
        void LoadConfiguration(List<SourceMeterData> newSourceMeterDataList);
        int Initial_All_SMU();
        bool SetConfig(ESMUName name, SourceMeterConfiguration config);
        bool OutputOn(ESMUName name);
        bool GetMeasureResult(ESMUName name, int data_len, out IList<SourceMeterMeasurement> measurements);
        bool OutputOff(ESMUName name);
        bool SetRelay(ESMUName name, bool connect);
        bool Reset(ESMUName name);
    }
}
