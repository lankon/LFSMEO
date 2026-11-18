using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace Device_VirtualSMU
{
    public class VirtualSMU : ISourceMeter, IVirtualSourceMeter
    {
        public ESMUType GetSourceMeterType()
        {
            return ESMUType.VIRTUAL;
        }

        public int Open(SourceMeterConnection connection)
        {
            return 0;
        }

        public void BindingDeviceIndex(string id)
        {
        }

        public int GetConfiguration(string id, out SourceMeterConfiguration configuration)
        {
            configuration = new SourceMeterConfiguration();
            return 0;
        }

        public int SetConfiguration(string id, SourceMeterConfiguration configuration)
        {
            return 0;
        }

        public int OutputOn(string id)
        {
            return 0;
        }

        public int WaitForCompletion(string id, double timeoutSeconds)
        {
            return 0;
        }

        public int OutputOff(string id)
        {
            return 0;
        }

        public bool LoadVirtualData(string filePath)
        {
            return true;
        }

        public int Fetch(string id, int count, double timeoutSeconds, out IList<SourceMeterMeasurement> measurements)
        {
            
            measurements = new List<SourceMeterMeasurement>();
            return 0;
        }

        public int Close()
        {
            return 0;
        }

        public int Reset(string id)
        {
            return 0;
        }

        public int SetOutputRelay(string id, bool connected)
        {
            return 0;
        }

        
    }
}
