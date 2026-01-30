using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DeviceCore
{
    public enum ESpectrometerType
    {
        OTO,
    }

    public interface IFunction_Spectrometer
    {
        int Initial_All_Spectrometer();
        float[] GetSpectrumOneShot(uint integral_time, uint avg_time = 1);
        float[] GetSpectrum(uint integral_time, uint avg_time = 1);
    }
}
