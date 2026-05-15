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
        float[] Wavelength = new float[4096];
        float[] Intensity = new float[4096];
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
            return Intensity;
        }

        public float[] GetSpectrumOneShot(string sn, uint integral_time, uint avg_time = 1)
        {
            return Intensity;
        }

        public float[] GetWavelength(string sn)
        {
            return Wavelength;
        }

        public int Open()
        {
            for(int i=0; i<Wavelength.Length; i++)
            {
                Wavelength[i] = Wavelength.Length;
                Intensity[i] = 0;
            }

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Setting\VirtualData\Virtual_Spectrum_Data.csv";

            if (!File.Exists(path))
                return 0;

            string[] lines = File.ReadAllLines(path);
            int index = 0;

            IEnumerable<string> dataLines = lines.Skip(1);
            foreach (string line in dataLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] values = line.Split(',');

                if (values.Length < 2) continue; //確保有波長以及強度值

                if (float.TryParse(values[0], out float nm_value))
                    Wavelength[index] = nm_value;

                if (float.TryParse(values[1], out float intensity_value))
                    Intensity[index] = intensity_value;

                index++;
                 if (index >= Wavelength.Length)
                    break;
            }

            for(int i=index; i<Wavelength.Length; i++)
            {
                Wavelength[i] = Wavelength[index-1]+1;
                Intensity[i] = 0;
            }

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
            
            return IntensityPercent.Dequeue();
        }
    }
}
