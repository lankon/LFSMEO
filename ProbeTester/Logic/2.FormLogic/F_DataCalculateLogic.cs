using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbeTester.Logic
{
    public class F_DataCalculateLogic
    {
        #region parameter define
        private List<double> PositionError = new List<double>();
        private List<double> Current = new List<double>();
        private DataFilter Filter = new DataFilter();
        public CalculateData CalculateDataResult = new CalculateData();

        public class CalculateData
        {
            public int RisingStart = 0;
            public int RisingEnd = 0;
            public int StableStart = 0;
            public int StableEnd = 0;
            public int Fs_pos = 0;      //靜摩擦力位置
            public double RMS = 0;
            public double Fs = 0;       //靜摩擦力
        }
        #endregion

        #region private function
        //[計算靜摩擦力電流]
        private void FindStaticFrictionCur(int start, int end, List<double> current)
        {
            double max = 0; int position = 0; ;
            for (int i = end - 1500; i < end + 1500; i++)
            {
                if (current[i] > max)
                {
                    max = current[i];
                    position = i;
                }
            }

            CalculateDataResult.Fs = max;
            CalculateDataResult.Fs_pos = position;
        }
        //[計算爬升段電流]
        private void FindRising(List<double> data, List<double> current)
        {
            double startValue = 1;                                      //稍微高於零點平台，代表開始上升
            double endValue = Filter.GetPreciseHighLevel(data, 0.9);    //搜尋高位穩定運行區數值

            int startIndex = data.FindIndex(x => x >= startValue);
            int endIndex = data.FindIndex(startIndex, x => x >= endValue);

            CalculateDataResult.RisingStart = startIndex;
            CalculateDataResult.RisingEnd = endIndex;

            //找出靜摩擦力電流峰值
            FindStaticFrictionCur(startIndex, endIndex, current);

            int fallStartIndex = data.FindIndex(endIndex, x => x <= endValue * 0.99);

            if (fallStartIndex != -1)
            {
                CalculateStableRMS(endIndex, fallStartIndex, current);
            }
        }
        //[計算平穩段RMS]
        private void CalculateStableRMS(int start, int end, List<double> data)
        {
            if (data == null || data.Count == 0)
                return;

            //內縮1000個資料點(可變動)
            end = end - 1000;
            start = start + 1000;

            CalculateDataResult.StableStart = start;
            CalculateDataResult.StableEnd = end;

            int count = (end) - (start) + 1;

            //平方和
            double sumOfSquares = data.Skip(start)
                                      .Take(count)
                                      .Select(x => x * x)
                                      .Sum();

            //rms
            double rms = Math.Sqrt(sumOfSquares / count);

            CalculateDataResult.RMS = rms;

            return;
        }
        #endregion

        #region public function
        //[讀取檔案]
        public void ReadFile(string filePath)
        {
            PositionError.Clear();
            Current.Clear();

            string[] lines = File.ReadAllLines(filePath);
            bool start_read = false;

            foreach (string line in lines)
            {
                if (start_read == true)
                {
                    string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (double.TryParse(parts[1], out double value))
                    {
                        PositionError.Add(value);
                    }
                    if (double.TryParse(parts[2], out double value1))
                    {
                        Current.Add(value1);
                    }
                }

                if (line.Contains("CH1") && line.Contains("CH2"))
                {
                    start_read = true;
                }
            }
        }
        public void SelectRange(double region_start, double region_end)
        {
            //取得目前框選的 X 軸範圍 (Index)
            double minX = region_start;
            double maxX = region_end;

            //確保 min 小於 max (防止使用者反向拖拉)
            int SelectStart = (int)Math.Max(0, Math.Min(minX, maxX));
            int endIndex = (int)Math.Min(PositionError.Count - 1, Math.Max(minX, maxX));

            //篩選出該範圍內的數據
            PositionError = PositionError.Skip(SelectStart).Take(endIndex - SelectStart + 1).ToList();
            Current = Current.Skip(SelectStart).Take(endIndex - SelectStart + 1).ToList();
        }
        public void Calculate()
        {
            FindRising(PositionError, Current);
        }
        //[取得位置誤差資料]
        public IReadOnlyList<double> GetPositionErrorData()
        {
            return PositionError;
        }
        public IReadOnlyList<double> GetCurrentData()
        {
            return Current;
        }
        #endregion




        

    }
}
