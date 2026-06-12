using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using DeviceCore;
using ToolFunction;
using RGBTester.Base;

namespace RGBTester.Logic
{
    public class F_MFactorCalibrationLogic
    {
        public F_MFactorCalibrationLogic(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        #region parameter function
        private SpectrumData StdSpectrum;
        private SpectrumData MatchSpectrum;
        private SpectrumData MFactor = new SpectrumData();
        IFunction_Spectrometer Spectrometer;
        IServiceProvider ServiceProvider;
        public double OpticalPower = 0;
        public double WLD = 0;
        public double Luminous = 0;

        enum type
        {
            POWER,
            WAVELENGTH,
        }

        public class SpectrumData
        {
            public List<int> Wavelength;
            public List<double> Intensity;
        }
        public class dSpectrumData
        {
            public double[] Wavelength;
            public double[] Intensity;
        }
        #endregion

        #region private function
        private SpectrumData GetSpectrumData(List<double> rawWavelength, List<double> rawIntensity)
        {
            // 安全檢查：確保有資料且長度一致
            if (rawWavelength == null || rawIntensity == null || rawWavelength.Count < 2 || rawWavelength.Count != rawIntensity.Count)
            {
                return new SpectrumData
                {
                    Wavelength = new List<int>(),
                    Intensity = new List<double>()
                };
            }

            List<int> iStdWavelength = new List<int>();
            List<double> iStdIntensity = new List<double>();

            int startX = (int)Math.Ceiling(rawWavelength.First());
            int endX = (int)Math.Floor(rawWavelength.Last());

            int i = 0;

            // 開始對每一個整數點進行內插計算
            for (int targetX = startX; targetX <= endX; targetX++)
            {
                // 尋找 targetX 在原始資料中夾在哪兩個點之間
                while (i < rawWavelength.Count && rawWavelength[i] < targetX)
                {
                    i++;
                }

                // 確保找到的點在有效範圍內
                if (i > 0 && i < rawWavelength.Count)
                {
                    double x0 = rawWavelength[i - 1];
                    double y0 = rawIntensity[i - 1];
                    double x1 = rawWavelength[i];
                    double y1 = rawIntensity[i];

                    // 線性內插核心公式
                    double targetY = y0 + (targetX - x0) * (y1 - y0) / (x1 - x0);

                    iStdWavelength.Add(targetX);
                    iStdIntensity.Add(targetY);
                }
            }

            SpectrumData spec = new SpectrumData();
            spec.Wavelength = iStdWavelength;
            spec.Intensity = iStdIntensity;

            return spec;
        }
        private SpectrumData CompareSpectrum(SpectrumData specA, SpectrumData specB)
        {
            // 確保兩邊都有資料
            if (specA?.Wavelength == null || specB?.Wavelength == null ||
                specA.Wavelength.Count == 0 || specB.Wavelength.Count == 0)
            {
                return MFactor; // 回傳空清單
            }

            int startWavelength = Math.Max(specA.Wavelength.First(), specB.Wavelength.First());
            int endWavelength = Math.Min(specA.Wavelength.Last(), specB.Wavelength.Last());

            // 確認是否有重疊
            if (startWavelength > endWavelength)
            {
                //無重疊
                return MFactor;
            }

            SpectrumData temp = new SpectrumData();
            temp.Wavelength = new List<int>();
            temp.Intensity = new List<double>();

            // 開始在重疊區間內進行相除
            for (int wave = startWavelength; wave <= endWavelength; wave++)
            {
                // 找出 wave 在 specA 和 specB 中的索引位置
                int indexA = specA.Wavelength.IndexOf(wave);
                int indexB = specB.Wavelength.IndexOf(wave);

                // 確保兩邊都有這個波長點
                if (indexA != -1 && indexB != -1)
                {
                    double intensityA = specA.Intensity[indexA];
                    double intensityB = specB.Intensity[indexB];

                    // 分母不能為0
                    if (intensityB != 0)
                    {
                        double ratio = intensityA / intensityB; // 這裡是以 A 除以 B，你可以根據需求改成 B 除以 A

                        temp.Wavelength.Add(specB.Wavelength[indexB]);
                        temp.Intensity.Add(ratio);
                    }
                    else
                    {
                        temp.Wavelength.Add(specB.Wavelength[indexB]);
                        temp.Intensity.Add(0);
                    }
                }
            }

            MFactor.Wavelength = temp.Wavelength;
            MFactor.Intensity = temp.Intensity;

            return MFactor;
        }
        private void SaveData(double[] wl, double[] intensity)
        {
            StreamWriter file = Tool.CreateFile("Result\\Spectrum", ".csv", false);

            Tool.WriteFile(file, $"Wavelength,Intensity");

            for (int i = 0; i < wl.Length; i++)
            {
                Tool.WriteFile(file, $"{wl[i]},{intensity[i]}");
            }

            Tool.CloseFile(file);
        }
        private double GetColorPowerGain(double wl, type _type)
        {
            double ColorPowerGain = 0;
            double Wavelength = 0;

            if (wl > 580 && wl < 680)
            {
                ColorPowerGain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_RedPowerGain);
                Wavelength = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_RedWavelengthGain);
            }
            else if (wl > 470 && wl < 570)
            {
                ColorPowerGain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_GreenPowerGain);
                Wavelength = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_GreenWavelengthGain);
            }
            else if(wl > 400 && wl < 500)
            {
                ColorPowerGain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_BluePowerGain);
                Wavelength = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_BlueWavelengthGain);
            }

            if (_type == type.POWER)
                return ColorPowerGain;
            else if (_type == type.WAVELENGTH)
                return Wavelength;

            return 0;
        }
        #endregion

        #region public function
        public SpectrumData ReadStdSpectrumFile(string filePath)
        {
            List<double> StdWavelength = new List<double>();
            List<double> StdIntensity = new List<double>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                int shift = 0;
                bool start_shift = false;

                foreach (string line in lines)
                {
                    // 在這裡處理每一列的文字，例如：
                    if (line.Contains("[Data]"))
                        start_shift = true;

                    if(start_shift)
                        shift++;

                    if(shift > 10)
                    {
                        string[] splits = line.Split('\t');

                        if (splits.Length < 2)
                            continue;

                        StdWavelength.Add(Tool.StringToDouble(splits[0]));
                        StdIntensity.Add(Tool.StringToDouble(splits[1]));
                    }
                }

                StdSpectrum = GetSpectrumData(StdWavelength, StdIntensity);

                return StdSpectrum;
            }
            catch (Exception ex)
            {
                Tool.SaveLogToFile($"{ex}", level: "ERR");

                return StdSpectrum;
            }
        }
        public void CalMFactor(double[] wl, double[] intensity)
        {
            List<double> l_wl = new List<double>(wl);
            List<double> l_intensity = new List<double>(intensity);

            MatchSpectrum = GetSpectrumData(l_wl, l_intensity);

            MFactor = CompareSpectrum(StdSpectrum, MatchSpectrum);

            int index_600nm = MFactor.Wavelength.IndexOf(600);
            // 寫檔
            StreamWriter file = Tool.CreateFile("\\Setting\\MFactor", ".csv", false);
            for (int i = 0; i < MFactor.Wavelength.Count; i++)
            {
                Tool.WriteFile(file, $"{MFactor.Wavelength[i]},{MFactor.Intensity[i] / MFactor.Intensity[index_600nm]}");
            }
            Tool.CloseFile(file);
        }
        public dSpectrumData CalculateOptical()
        {
            Spectrometer = ServiceProvider.GetRequiredService<IFunction_Spectrometer>();

            int intgTime = ApplicationSetting.Get_Int_Recipe<eF_MFactorCalibration>((int)eF_MFactorCalibration.TxtBx_IntgralTime);
            float[] intensity = Spectrometer.GetSpectrumOneShot(ESpectrumName.SPECTRUM_1, (uint)intgTime);
            float[] wl = Spectrometer.GetWavelengthSpan(ESpectrumName.SPECTRUM_1);
            double[] intensityDouble = Array.ConvertAll(intensity, x => (double)x);
            double[] wlDouble = Array.ConvertAll(wl, x => (double)x);

            SaveData(wlDouble, intensityDouble);

            Wavelength CalWl = new Wavelength();
            LuminousFlux lm = new LuminousFlux();
            double k_value = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_OpticalKValue);

            OpticalPower = CalWl.Calculate_Power(wlDouble, intensityDouble, intgTime, k_value);
            Luminous = lm.CalculateTotalLumens(wlDouble, intensityDouble, intgTime, k_value);
            WLD = CalWl.Calculate_WLD(wlDouble, intensityDouble);

            dSpectrumData spectrum = new dSpectrumData();
            spectrum.Wavelength = wlDouble;
            spectrum.Intensity = intensityDouble;

            return spectrum;
        }
        public double GetWLD()
        {
            double offset = GetColorPowerGain(WLD, type.WAVELENGTH);

            WLD = WLD + offset;

            return WLD;
        }
        public double GetOpticalPower()
        {
            double gain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_PowerGain);
            double offset = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_PowerOffset);
            double color_gain = GetColorPowerGain(WLD, type.POWER);

            OpticalPower = OpticalPower * gain * color_gain + offset;

            return OpticalPower;
        }
        public double GetLuminous()
        {
            double gain = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_PowerGain);
            double offset = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_PowerOffset);
            double color_gain = GetColorPowerGain(WLD, type.POWER);

            Luminous = Luminous * gain * color_gain + offset;

            return Luminous;
        }
        #endregion




    }
}
