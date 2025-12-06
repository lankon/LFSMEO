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
        public LinearCurveFitting(int[] DACpoint, double[] voltage_point)
        {
            Mean_DAC_Point(DACpoint);
            MeanCurrent(voltage_point);
            CalculateSlope(DACpoint, voltage_point);
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

            //return mDAC;
        }

        private void MeanCurrent(double[] voltage)
        {
            double sum = 0;

            for (int i = 0; i < voltage.Length; i++)
            {
                sum += voltage[i];
            }

            // I = V / R
            mCurrent = sum / voltage.Length;

            //return mCurrent;
        }

        private double CalculateSlope(int[] DACpoint, double[] voltage_point)
        {
            double covariance = CalculateCovariance(DACpoint, voltage_point);
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
        private double CalculateCovariance(int[] DACpoint, double[] voltage_point)
        {
            if (DACpoint.Length != voltage_point.Length)
                return -99;

            double M1 = 0;

            for (int i = 0; i < DACpoint.Length; i++)
            {
                double current_i = voltage_point[i];

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
