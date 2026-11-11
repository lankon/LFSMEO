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
    }

    public interface IFunction_SourceMeter
    {
        void LoadConfiguration(List<SourceMeterData> newSourceMeterDataList);
        int Initial_All_SMU();
        bool SetSMUConfig(ESMUName name, SourceMeterConfiguration config);
        bool SetRelay(ESMUName name, bool connect);
        bool Reset(ESMUName name);

        // 量測
        bool SIMV(ESMUName name,
                    double current, double voltage_limit, double delay_time,
                    out double res_i, out double res_v);

        //void LoadConfiguration(List<SpectrumData> newSpectrumDataList);
        //int Initial_All_Spectrometer();
        //void SetBackgroundCoef(double standard, double slope, double offset);
        //void SetMFactor();
        //float[] GetWavelengthSpan(ESpectrumName name);
        //float[] GetSpectrumOneShot(ESpectrumName name, uint integral_time, uint avg_time = 1, bool pass_mfactor = false);
        //float[] GetSpectrumRelativelyOneShot(ESpectrumName name, uint integral_time, uint avg_time = 1);
        ////float[] GetSpectrum(ESpectrumName name, uint integral_time, uint avg_time = 1);
        //double GetIntensityPercent(ESpectrumName name);
    }
}
