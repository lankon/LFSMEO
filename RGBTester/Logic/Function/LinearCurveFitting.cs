using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Logic
{
    public class LinearCurveFitting
    {
        #region parameter define
        public double mDAC = 0;
        public double mCurrent = 0;
        public double Slope { get; private set; } = 0;
        public double Offset { get; private set; } = 0;
        #endregion

        #region public function
        public LinearCurveFitting(int[] DACpoint, double[] current_point)
        {
            Mean_DAC_Point(DACpoint);
            MeanCurrent(current_point);
            CalculateSlope(DACpoint, current_point);
            CalculateOffset();
        }
        #endregion

        #region private function
        private void Mean_DAC_Point(int[] DACpoint)
        {
            long sum = 0;

            for (int i = 0; i < DACpoint.Length; i++)
            {
                sum += DACpoint[i];
            }

            mDAC = (double)sum / DACpoint.Length;
        }

        private void MeanCurrent(double[] current)
        {
            double sum = 0;

            for (int i = 0; i < current.Length; i++)
            {
                sum += current[i];
            }

            mCurrent = sum / current.Length;
        }

        private double CalculateSlope(int[] DACpoint, double[] current_point)
        {
            double covariance = CalculateCovariance(DACpoint, current_point);
            double variance = CalculateVariance(DACpoint);

            if (variance == 0) return 0; // 防止除以零

            Slope = covariance / variance;

            return Slope;
        }

        private double CalculateOffset()
        {
            double slope = Slope;
            Offset = mCurrent - slope * mDAC;

            return Offset;
        }
        private double CalculateCovariance(int[] DACpoint, double[] current_point)
        {
            if (DACpoint.Length != current_point.Length)
                return -99;

            double M1 = 0;

            for (int i = 0; i < DACpoint.Length; i++)
            {
                double current_i = current_point[i];

                M1 += (DACpoint[i] - mDAC) * (current_i - mCurrent);
            }

            return M1;
        }

        private double CalculateVariance(int[] DACpoint)
        {
            double M2 = 0;

            for (int i = 0; i < DACpoint.Length; i++)
            {
                M2 += Math.Pow((DACpoint[i] - mDAC), 2);
            }

            return M2;
        }
        #endregion
    }
}
