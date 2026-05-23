using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace RGBTester.Logic
{
    public class F_OpticalSettingLogic
    {
        public F_OpticalSettingLogic(IFunction_Spectrometer function_Spectrometer)
        {
            Spectrometer = function_Spectrometer;
        }

        #region parameter define
        IFunction_Spectrometer Spectrometer;
        #endregion

        public LinearCurveFitting BackgroundCalibration()
        {
            int[] Step = new int[10];
            double[] dInttensity = new double[10];
            int index = 0;

            for (int i=100; i<=1000; i = i+100)
            {
                float[] intensity = Spectrometer.GetSpectrumOneShot(ESpectrumName.SPECTRUM_1, (uint)i);
                float totalcount = 0;

                for(int j=0; j<intensity.Length; j++)
                {
                    totalcount += intensity[j];
                }

                totalcount = totalcount / intensity.Length;


                dInttensity[index] = totalcount;
                Step[index] = i;

                index++;
            }

            LinearCurveFitting linearCurveFitting = new LinearCurveFitting(Step, dInttensity);

            //設定至分光卡
            Spectrometer.SetBackgroundCoef(linearCurveFitting.Slope, linearCurveFitting.Offset);

            return linearCurveFitting;
        }

    }
}
