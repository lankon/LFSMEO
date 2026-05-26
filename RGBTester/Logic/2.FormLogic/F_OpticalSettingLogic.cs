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

        public LinearCurveFitting BackgroundCalibration(out double standard)
        {
            int[] Step = new int[10];
            double[] dInttensity = new double[10];
            float[] intensity;
            float totalcount = 0;
            int index = 0;
            
            //重置分光卡係數
            Spectrometer.SetBackgroundCoef(0, 0, 0);

            //計算Standard背景亮度
            intensity = Spectrometer.GetSpectrumOneShot(ESpectrumName.SPECTRUM_1, 0, pass_mfactor:true);
            for (int j = 0; j < intensity.Length; j++)
            {
                totalcount += intensity[j];
            }
            standard = totalcount / intensity.Length;

            for (int i=100; i<=1000; i = i+100)
            {
                intensity = Spectrometer.GetSpectrumOneShot(ESpectrumName.SPECTRUM_1, (uint)i, pass_mfactor:true);
                totalcount = 0;

                for (int j=0; j<intensity.Length; j++)
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
            Spectrometer.SetBackgroundCoef(standard, linearCurveFitting.Slope, linearCurveFitting.Offset);

            return linearCurveFitting;
        }
    }
}
