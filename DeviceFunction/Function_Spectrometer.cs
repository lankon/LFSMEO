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
        private Dictionary<string, SpectrumData> SpectrumListDict;        //存放UI光譜設定
        #endregion

        #region private function
        #endregion

        #region public function
        public int Initial_All_Spectrometer()
        {
            SpectrometerList.Clear();

            //依照光譜型號以及ID名稱分組
            var hardwareConfigs = SpectrumListDict.Values.GroupBy(x => new { x.Title_SpectrumType, x.Title_ID })
                                                      .Select(g => g.First());

            foreach (var config in hardwareConfigs)
            {
                var prototype = Spectrometer.FirstOrDefault(p => p.GetSpectrometerType().ToString() == config.Title_SpectrumType);

                if (prototype != null)
                {
                    ISpectrometer newDevice = (ISpectrometer)Activator.CreateInstance(prototype.GetType());

                    int res = newDevice.Open();

                    if (res == 0)
                        SpectrometerList.Add(newDevice);
                    else
                        Tool.SaveLogToFile($"光譜儀 {config.Title_SpectrumType} ({config.Title_ID})連線失敗", level: "ERR");
                }
            }

            int ret = -1;
            for (int i = 0; i < SpectrometerList.Count; i++)
            {
                ret = 0;
                IsInitial = true;

                if (SpectrometerList[i].GetSpectrometerType() != ESpectrometerType.VIRTUAL)
                {
                    Tool.SaveLogToFile("實體光譜儀 Initial Success");
                    return 0;
                }
            }

            return ret;
        }

        public void LoadConfiguration(List<SpectrumData> newSpectrumDataList)
        {
            SpectrumListDict = newSpectrumDataList
                        .GroupBy(x => x.Title_Name)
                        .ToDictionary(g => g.Key, g => g.First());
        }

        public float[] GetSpectrumOneShot(ESpectrumName name, uint integral_time, uint avg_time = 1)
        {
            SpectrumListDict.TryGetValue(name.ToString(), out SpectrumData spectrum_data);

            if (spectrum_data == null)
                return null;

            float[] spectrum = null;

            if (IsInitial == false)
                return spectrum;

            var targetDevice = SpectrometerList.FirstOrDefault(device =>
                                                               device.GetSpectrometerType().ToString() == spectrum_data.Title_SpectrumType);

            spectrum = targetDevice?.GetSpectrumOneShot(spectrum_data.Title_ID, integral_time, avg_time);

            return spectrum;
        }

        public float[] GetSpectrum(ESpectrumName name, uint integral_time, uint avg_time = 1)
        {
            SpectrumListDict.TryGetValue(name.ToString(), out SpectrumData spectrum_data);

            if (spectrum_data == null)
                return null;

            float[] spectrum = null;

            if (IsInitial == false)
                return spectrum;

            spectrum = SpectrometerList[DeviceIndex].GetSpectrum(spectrum_data.Title_ID, integral_time, avg_time);

            return spectrum;
        }
        #endregion

    }
}
