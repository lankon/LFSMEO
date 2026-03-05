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

    public enum ESpectrumName
    {
        NONE,
        SPECTRUM_1,
        SPECTRUM_2,
        SPECTRUM_3,
        SPECTRUM_4,
        SPECTRUM_5,
        SPECTRUM_6,
    }

    public class SpectrumData
    {
        public string Title_SpectrumType { get; set; }
        public string Title_Name { get; set; }
        public string Title_IntegralTime { get; set; }
        public string Title_ID { get; set; }
    }

    public interface IFunction_Spectrometer
    {
        void LoadConfiguration(List<SpectrumData> newSpectrumDataList);
        int Initial_All_Spectrometer();
        float[] GetWavelengthSpan(ESpectrumName name);
        float[] GetSpectrumOneShot(ESpectrumName name, uint integral_time, uint avg_time = 1);
        float[] GetSpectrum(ESpectrumName name, uint integral_time, uint avg_time = 1);
    }
}
