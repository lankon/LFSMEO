using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace Device_Spectrum_Virtual
{
    public class Virtual_Spectrum : ISpectrometer
    {
        #region parameter define
        float MaxIntensity = 0;
        List<float> Wavelength = new List<float>();
        List<float> Intensity = new List<float>();
        Queue<double> IntensityPercent = new Queue<double>();
        #endregion

        public void BindingDeviceIndex(string serialNumber)
        {
            
        }

        public ESpectrometerType GetSpectrometerType()
        {
            return ESpectrometerType.VIRTUAL;
        }

        public float[] GetSpectrum(string sn, uint integral_time, uint avg_time = 1)
        {
            float[] f_intensity = Intensity.Select(x => x).ToArray();
            return f_intensity;
        }

        public float[] GetSpectrumOneShot(string sn, uint integral_time, uint avg_time = 1)
        {
            float[] f_intensity = Intensity.Select(x => x).ToArray();
            return f_intensity;
        }

        public float[] GetSpectrumRelativelyOneShot(string sn, uint integral_time, uint avg_time = 1)
        {
            return Intensity.Select(x => x / MaxIntensity * 100).ToArray();
        }

        public float[] GetWavelength(string sn)
        {
            float[] f_wl = Wavelength.Select(x => x).ToArray();
            return f_wl;
        }

        public int Open()
        {
            //for(int i=0; i<Wavelength.Length; i++)
            //{
            //    Wavelength[i] = Wavelength.Length;
            //    Intensity[i] = 0;
            //}

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Setting\VirtualData\Virtual_Spectrum_Data.csv";

            if (!File.Exists(path))
                return 0;

            string[] lines = File.ReadAllLines(path);

            IEnumerable<string> dataLines = lines.Skip(1);
            foreach (string line in dataLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] values = line.Split(',');

                if (values.Length < 2) continue; //確保有波長以及強度值

                if (float.TryParse(values[0], out float nm_value))
                    Wavelength.Add(nm_value);

                if (float.TryParse(values[1], out float intensity_value))
                    Intensity.Add(intensity_value);
            }

            MaxIntensity = Intensity.Max();

            return 0;    
        }

        public double GetIntensityPercent(string sn)
        {
            if(IntensityPercent.Count == 0)
            {
                IntensityPercent.Enqueue(75);
                //IntensityPercent.Enqueue(40);
                //IntensityPercent.Enqueue(40);
                //IntensityPercent.Enqueue(40);
            }

            return Intensity.Max() / MaxIntensity *0.7 * 100;
            //return IntensityPercent.Dequeue();
        }
    }
}
