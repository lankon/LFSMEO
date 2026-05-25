using DeviceCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private double BackgroundCoefSlope = 0;         //背景校正
        private double BackgroundCoefOffset = 0;        //背景校正
        private double BackgroundStandard = 0;          //背景校正
        private bool IsInitial = false;
        private bool ReadMFactorSuccess = false;        //MFactor
        private int DeviceIndex = 0;
        private int MFactorStartWavelength = 0;         //MFactor
        private int MFactorEndWavelength = 0;           //MFactor
        private IEnumerable<ISpectrometer> Spectrometer;
        private List<double> MFactorCoef = new List<double>();  //MFactor
        private List<ISpectrometer> SpectrometerList = new List<ISpectrometer>();
        private Dictionary<string, SpectrumData> SpectrumListDict;        //存放UI光譜設定
        #endregion

        #region private function
        private void BindingDeviceIndex()
        {
            foreach (KeyValuePair<string, SpectrumData> entry in SpectrumListDict)
            {
                string name = entry.Key;
                SpectrumData intensities = entry.Value;

                for (int j = 0; j < SpectrometerList.Count; j++)
                {
                    if (intensities.Title_SpectrumType == SpectrometerList[j].GetSpectrometerType().ToString())
                    {
                        SpectrometerList[j].BindingDeviceIndex(intensities.Title_ID);
                        break;
                    }
                }
            }
        }
        private double GetMFactorLambda(double wavelength)
        {
            //!!!!!波長間隔必須為1nm
            
            if (wavelength < MFactorStartWavelength || wavelength > MFactorEndWavelength) 
                return 0;

            // 計算陣列索引
            double indexPos = wavelength - MFactorStartWavelength;
            int lowerIndex = (int)Math.Floor(indexPos);
            int upperIndex = (int)Math.Ceiling(indexPos);

            if (lowerIndex == upperIndex) 
                return MFactorCoef[lowerIndex];

            // 線性內插計算
            double fraction = indexPos - lowerIndex;
            double lowerValue = MFactorCoef[lowerIndex];
            double upperValue = MFactorCoef[upperIndex];

            return lowerValue + (upperValue - lowerValue) * fraction;
        }
        #endregion

        #region public function
        public void LoadConfiguration(List<SpectrumData> newSpectrumDataList)
        {
            SpectrumListDict = newSpectrumDataList
                        .GroupBy(x => x.Title_Name)
                        .ToDictionary(g => g.Key, g => g.First());
        }

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

            BindingDeviceIndex();

            int ret = -1;
            for (int i = 0; i < SpectrometerList.Count; i++)
            {
                ret = 0;
                IsInitial = true;

                if (SpectrometerList[i].GetSpectrometerType() != ESpectrometerType.VIRTUAL)
                {
                    Tool.SaveLogToFile("實體分光卡 Initial Success");
                    return 0;
                }
            }

            return ret;
        }

        public void SetBackgroundCoef(double standard, double slope, double offset)
        {
            BackgroundCoefOffset = standard;
            BackgroundCoefOffset = offset;
            BackgroundCoefSlope = slope;
        }

        public void SetMFactor()
        {
            List<int> Wavelength = new List<int>();
            List<double> Intensity = new List<double>();

            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"Setting\MFactor.csv";

            if (!File.Exists(filePath))
            {
                Tool.SaveLogToFile($"讀取失敗：找不到光譜檔案 {filePath},頻譜未帶入MFactor係數", level: "WRN");
                return;
            }

            try
            {
                foreach (string line in File.ReadLines(filePath))
                {
                    if (string.IsNullOrWhiteSpace(line)) 
                        continue;

                    string[] tokens = line.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (tokens.Length >= 2)
                    {
                        if (Int32.TryParse(tokens[0], out int wl) &&
                            double.TryParse(tokens[1], out double its))
                        {
                            Wavelength.Add(wl);
                            Intensity.Add(its);
                        }
                    }
                }

                MFactorStartWavelength = Wavelength[0];
                MFactorEndWavelength = Wavelength[Wavelength.Count - 1];
                MFactorCoef = Intensity;
                ReadMFactorSuccess = true;
            }
            catch (Exception ex)
            {
                Tool.SaveLogToFile($"讀取失敗：找不到光譜檔案 {filePath},頻譜未帶入MFactor係數", level: "WRN");
            }
        }

        public float[] GetWavelengthSpan(ESpectrumName name)
        {
            SpectrumListDict.TryGetValue(name.ToString(), out SpectrumData spectrum_data);

            if (spectrum_data == null)
                return null;

            float[] wavelegth = null;

            if (IsInitial == false)
                return wavelegth;

            var targetDevice = SpectrometerList.FirstOrDefault(device =>
                                                               device.GetSpectrometerType().ToString() == spectrum_data.Title_SpectrumType);

            wavelegth = targetDevice?.GetWavelength(spectrum_data.Title_ID);

            return wavelegth;
        }

        public float[] GetSpectrumOneShot(ESpectrumName name, uint integral_time, uint avg_time = 1)
        {
            SpectrumListDict.TryGetValue(name.ToString(), out SpectrumData spectrum_data);

            float[] spectrum = null;

            if (spectrum_data == null || IsInitial == false)
                return null;

            var targetDevice = SpectrometerList.FirstOrDefault(device =>
                                                               device.GetSpectrometerType().ToString() == spectrum_data.Title_SpectrumType);

            spectrum = targetDevice?.GetSpectrumOneShot(spectrum_data.Title_ID, integral_time, avg_time);

            //扣除背景雜訊
            double background = integral_time * BackgroundCoefSlope + BackgroundCoefOffset;
            background = background > BackgroundStandard? background : BackgroundStandard;
            spectrum = spectrum.Select(x => (float)(x - background)).ToArray();

            //未讀取MFactor直接回傳頻譜
            if (ReadMFactorSuccess == false)
                return spectrum;

            //取得對應波長
            targetDevice = SpectrometerList.FirstOrDefault(device =>
                                                               device.GetSpectrometerType().ToString() == spectrum_data.Title_SpectrumType);
            float[] wavelegth = targetDevice?.GetWavelength(spectrum_data.Title_ID);

            //乘上MFactor校正頻譜
            for (int i=0; i<spectrum.Length; i++)
            {
                double mfactor = GetMFactorLambda(wavelegth[i]);
                spectrum[i] = (float)(spectrum[i] * mfactor);
            }

            return spectrum;
        }

        public float[] GetSpectrumRelativelyOneShot(ESpectrumName name, uint integral_time, uint avg_time = 1)
        {
            SpectrumListDict.TryGetValue(name.ToString(), out SpectrumData spectrum_data);

            if (spectrum_data == null)
                return null;

            float[] spectrum = null;

            if (IsInitial == false)
                return spectrum;

            var targetDevice = SpectrometerList.FirstOrDefault(device =>
                                                               device.GetSpectrometerType().ToString() == spectrum_data.Title_SpectrumType);

            spectrum = targetDevice?.GetSpectrumRelativelyOneShot(spectrum_data.Title_ID, integral_time, avg_time);

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

            //扣除背景雜訊
            double background = integral_time * BackgroundCoefSlope + BackgroundCoefOffset;
            background = background > BackgroundStandard ? background : BackgroundStandard;

            spectrum.Select(x => x - (integral_time * BackgroundCoefSlope + BackgroundCoefOffset));

            return spectrum;
        }

        public double GetIntensityPercent(ESpectrumName name)
        {
            SpectrumListDict.TryGetValue(name.ToString(), out SpectrumData spectrum_data);

            if (spectrum_data == null)
                return 0.0;

            if (IsInitial == false)
                return 0.0;

            double percent = SpectrometerList[DeviceIndex].GetIntensityPercent(spectrum_data.Title_ID);
        
            return percent;
        }
        #endregion

    }
}
