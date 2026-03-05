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
        VIRTUAL,
        OTO,
    }

    public interface IFunction_Spectrometer
    {
        void LoadConfiguration(List<SpectrumData> newSpectrumDataList);
        int Initial_All_Spectrometer();
        float[] GetSpectrumOneShot(ESpectrumName name, uint integral_time, uint avg_time = 1);
        float[] GetSpectrum(ESpectrumName name, uint integral_time, uint avg_time = 1);
    }
}
