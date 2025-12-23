using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Logic
{
    public class RGBTesterFunction
    {
        public RGBTesterFunction(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        #region parameter define
        IServiceProvider ServiceProvider;
        #endregion

        public class AvgData
        {
            public double Avg_Vin;
            public double Avg_Iin;
            public double Avg_Vled;
            public double Avg_Vf;
            public double Avg_Iled;
        }

        public class TestHardwareParam
        {
            public int DAQ_SampleRate { get { return 40; } }            //單位us, DAQ取樣頻率25KHz
            public int DisplayFrequency { get { return 11; } }          //單位ms, 頻率90Hz
            public int OneTimeTestChannel { get { return 5; } }         //一次DAQ取樣通道數(硬體)
            public int Period_DAQ_Count { get { return DisplayFrequency * 1000/ DAQ_SampleRate / OneTimeTestChannel; } }//一個週期內DAQ取樣次數(硬體)  11ms(90Hz) / 40us(DAQ SP Rate) / 5(通道數)
            public int H_SigMag { get { return 20; } }                  //HCM訊號放大倍率(硬體)
            public int L_SigMag { get { return 200; } }                 //LCM訊號放大倍率(硬體)
            public int LED_SigMag { get { return 20; } }                //LED訊號放大倍率(硬體)
            public int CurrentMeasureBias { get { return 1; } }         //Bias(硬體)
            public double Rfb_HCM { get { return 0.53; } }              //High Current Mode阻抗(硬體)
            public double Rfb_LCM { get { return 5.1; } }               //Low Current Mode阻抗(硬體)
            public double Rin { get { return 0.5; } }                   //輸入阻抗(硬體)
            public double LED_R_Duty { get { return 0.4; } }            //Red LED Duty(硬體)
            public double LED_G_Duty { get { return 0.18; } }           // Green LED Duty(硬體)
            public double LED_B_Duty { get { return 0.12; } }           //Blue LED Duty(硬體)
        }

        public class DAQ_IO_Point
        {
            public EIOName DAQ_Vin;
            public EIOName DAQ_ILED;
            public EIOName DAQ_VLED;
            public EIOName DAQ_Vf;
            public EIOName DAQ_Iin_HCM;
            public EIOName DAQ_Iin_LCM;
        }

        public DAQ_IO_Point Get_DAQ_IO_Point(byte TestSide, byte TestColor)
        {
            DAQ_IO_Point dAQ_IO_Point = new DAQ_IO_Point();
            ILightEngineFunction lea = ServiceProvider.GetRequiredService<ILightEngineFunction>();

            bool isLeft = (TestSide == lea.LED_LeftSide);
            dAQ_IO_Point.DAQ_Vin = isLeft ? EIOName.Left_Vin : EIOName.Right_Vin;
            dAQ_IO_Point.DAQ_ILED = isLeft ? EIOName.Left_ILED : EIOName.Right_ILED;
            dAQ_IO_Point.DAQ_VLED = isLeft ? EIOName.Left_VLED : EIOName.Right_VLED;
            dAQ_IO_Point.DAQ_Iin_HCM = isLeft ? EIOName.Left_Iin_HCM : EIOName.Right_Iin_HCM;
            dAQ_IO_Point.DAQ_Iin_LCM = isLeft ? EIOName.Left_Iin_LCM : EIOName.Right_Iin_LCM;

            if (TestColor == lea.LED_R)
            {
                dAQ_IO_Point.DAQ_Vf = isLeft ? EIOName.Left_VLED_R : EIOName.Right_VLED_R;
            }
            else if (TestColor == lea.LED_G)
            {
                dAQ_IO_Point.DAQ_Vf = isLeft ? EIOName.Left_VLED_G : EIOName.Right_VLED_G;
            }
            else if (TestColor == lea.LED_B)
            {
                dAQ_IO_Point.DAQ_Vf = isLeft ? EIOName.Left_VLED_B : EIOName.Right_VLED_B;
            }

            return dAQ_IO_Point;
        }
    }
}
