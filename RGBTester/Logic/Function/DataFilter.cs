using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Logic
{
    public class DataFilter
    {
        #region parameter define
        #endregion

        #region public function
        public double GetPreciseHighLevel(List<double> data, double Threshold_Ratio, double Resolution = 0.0)
        {
            if (data.Count == 0) return 0;

            double[] sortedData = data.OrderBy(x => x).ToArray();
            int index = (int)(sortedData.Length * 0.96);
            List<double> dataAboveThreshold = data.Where(v => v > sortedData[index] * Threshold_Ratio).ToList();

            if(dataAboveThreshold.Count == 0)
                return data.Max();

            // 1. 找出數據範圍
            double min = dataAboveThreshold.Min();
            double max = dataAboveThreshold.Max();

            // 2. 建立直方圖 (切成 50 個區間)
            int binCount = 50;
            int[] bins = new int[binCount];
            double binSize = (max - min) / binCount;
            //double tolerance = (max - min) * 0.1;

            if (binSize == 0) return min; // 數據全部一樣

            if (Math.Abs(Resolution-0.0) > 0.0000001)
            {
                if (binSize < Resolution)
                {
                    binSize = Resolution;
                    //tolerance = Resolution;
                }
            }

            foreach (var val in dataAboveThreshold)
            {
                int binIndex = (int)((val - min) / binSize);
                
                if (binIndex >= binCount) 
                    binIndex = binCount - 1;
                
                bins[binIndex]++;
            }

            // 3. 找出點數最多的區間 (眾數區間)
            int maxBinIndex = Array.IndexOf(bins, bins.Max());
            double mostFrequentValue = min + (maxBinIndex * binSize) + (binSize / 2);

            var filteredData = dataAboveThreshold.Where(v =>
                Math.Abs(v - mostFrequentValue) < mostFrequentValue*0.01
            ).ToList();

            if(filteredData.Count == 0)
                return mostFrequentValue;

            return filteredData.Average();
        }
        #endregion

        #region private function
        #endregion
    }
}
