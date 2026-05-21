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

        private List<double> StdWavelength = new List<double>();
        private List<double> StdIntensity = new List<double>();
        private List<int> iStdWavelength = new List<int>();
        private List<double> iStdIntensity = new List<double>();

        public class SpectrumData
        {
            public List<int> Wavelength;
            public List<double> Intensity;
        }

        public void ReadStdSpectrumFile(string filePath)
        {
            StdWavelength.Clear();
            StdIntensity.Clear();
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
            }
            catch (Exception ex)
            {
                Tool.SaveLogToFile($"{ex}", level:"ERR");
            }
        }
        public SpectrumData GetStdSpectrum()
        {
            iStdWavelength.Clear();
            iStdIntensity.Clear();
            
            int startX = (int)Math.Ceiling(StdWavelength.First());
            int endX = (int)Math.Floor(StdWavelength.Last());

            List<string> resultLines = new List<string>();

            // 開始對每一個整數點進行內插計算
            for (int targetX = startX; targetX <= endX; targetX++)
            {
                // 尋找 targetX 在原始資料中夾在哪兩個點之間
                int i = 0;
                while (i < StdWavelength.Count && StdWavelength[i] < targetX)
                {
                    i++;
                }

                // 確保找到的點在有效範圍內
                if (i > 0 && i < StdWavelength.Count)
                {
                    double x0 = StdWavelength[i - 1];
                    double y0 = StdIntensity[i - 1];
                    double x1 = StdWavelength[i];
                    double y1 = StdIntensity[i];

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
    }
}
