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
        float[] GetSpectrumOneShot(uint integral_time, uint avg_time = 1);
        float[] GetSpectrum(uint integral_time, uint avg_time = 1);
    }
}
