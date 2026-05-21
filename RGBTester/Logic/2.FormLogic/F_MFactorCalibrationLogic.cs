using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;

namespace RGBTester.Logic
{
    public class F_MFactorCalibrationLogic
    {

        #region parameter function
        private SpectrumData StdSpectrum;
        private SpectrumData MatchSpectrum;
        private SpectrumData MFactor = new SpectrumData();
    
        public class SpectrumData
        {
            public List<int> Wavelength;
            public List<double> Intensity;
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

            // 寫檔
            StreamWriter file = Tool.CreateFile("\\Setting\\MFactor", ".csv", false);
            for (int i = 0; i < MFactor.Wavelength.Count; i++)
            {
                Tool.WriteFile(file, $"{MFactor.Wavelength[i]},{MFactor.Intensity[i]}");
            }
            Tool.CloseFile(file);
        }
        #endregion




    }
}
