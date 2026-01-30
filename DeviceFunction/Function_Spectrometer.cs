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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace DeviceFunction
{
    public class Function_Spectrometer : IFunction_Spectrometer
    {
        public Function_Spectrometer(IEnumerable<ISpectrometer> spec)
        {
            Spectrometer = spec;
        }

        #region parameter define
        private bool IsInitial = false;
        private int DeviceIndex = 0;
        private IEnumerable<ISpectrometer> Spectrometer;
        private List<ISpectrometer> SpectrometerList = new List<ISpectrometer>();
        #endregion

        #region private function
        #endregion

        #region public function
        public int Initial_All_Spectrometer()
        {
            int res = -1;
            foreach (ISpectrometer meter in Spectrometer)
            {
                res = meter.Open();
                
                if (res == 0)
                    SpectrometerList.Add(meter);
            }

            if(SpectrometerList.Count > 0)
            {
                IsInitial = true;
                Tool.SaveLogToFile("分光卡 Initial Success");
                return 0;
            }
            else
                return -1;
        }

        public void SetUseDeviceType(ESpectrometerType type)
        {
            if (IsInitial == false)
                return;

            for (int i=0; i<SpectrometerList.Count; i++)
            {
                if(SpectrometerList[i].GetSpectrometerType() == type)
                {
                    DeviceIndex = i;
                    break;
                }
            }
        }

        public float[] GetSpectrumOneShot(uint integral_time, uint avg_time = 1)
        {
            float[] spectrum = null;

            if (IsInitial == false)
                return spectrum;

            spectrum = SpectrometerList[DeviceIndex].GetSpectrumOneShot(integral_time, avg_time);

            return spectrum;
        }

        public float[] GetSpectrum(uint integral_time, uint avg_time = 1)
        {
            float[] spectrum = null;

            if (IsInitial == false)
                return spectrum;

            spectrum = SpectrometerList[DeviceIndex].GetSpectrum(integral_time, avg_time);

            return spectrum;
        }
        #endregion

    }
}
