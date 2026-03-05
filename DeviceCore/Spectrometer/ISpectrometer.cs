using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DeviceCore
{
    

    public interface ISpectrometer
    {
        ESpectrometerType GetSpectrometerType();
        
        int Open();
        void BindingDeviceIndex(string serialNumber);
        float[] GetWavelength(string sn);
        float[] GetSpectrumOneShot(string sn, uint integral_time, uint avg_time = 1);
        float[] GetSpectrum(string sn, uint integral_time, uint avg_time = 1);
    }
}
