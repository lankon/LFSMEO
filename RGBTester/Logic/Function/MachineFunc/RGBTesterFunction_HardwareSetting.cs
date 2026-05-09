using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Logic
{
    public partial class RGBTesterFunction
    {
        private double MaxCurrent_LCM = 30;
        private double MaxCurrent_HCM = 300;
        private double Rfb_LCM = 1;
        private double Rfb_HCM = 1;
        private int TestChannel = 5;

        public void SetRfb(double R_LCM, double R_HCM)
        {
            if (R_LCM > 0.00000001)
                Rfb_LCM = R_LCM;
            else
                Rfb_LCM = 1;

            if (R_HCM > 0.00000001)
                Rfb_HCM = R_HCM;
            else
                Rfb_HCM = 1;
        }
        public void SetTestChannel(int ch_count)
        {
            TestChannel = ch_count;
        }

        public class TestHardwareParam
        {
            private RGBTesterFunction _parent;
            public TestHardwareParam(RGBTesterFunction parent)
            {
                _parent = parent;
            }

            //[Phase1 Setting]
            public int DAQ_SampleRate { get { return 40; } }            //單位us, DAQ取樣頻率25KHz(硬體)
            public int DisplayFrequency { get { return 11; } }          //單位ms, 頻率90Hz(硬體)
            public int OneTimeTestChannel { get { return _parent.TestChannel; } }   //一次DAQ取樣通道數(硬體)
            public int Period_DAQ_Count { get { return DisplayFrequency * 1000 / DAQ_SampleRate / OneTimeTestChannel; } }//一個週期內DAQ取樣次數(硬體)  11ms(90Hz) / 40us(DAQ SP Rate) / 5(通道數)
            

            public int H_SigMag { get { return 20; } }                  //HCM訊號放大倍率(硬體)
            public int L_SigMag { get { return 200; } }                 //LCM訊號放大倍率(硬體)
            public int LED_SigMag { get { return 20; } }                //LED訊號放大倍率(硬體)
            public double CurrentMeasureBias { get { return 0.9; } }    //Bias(硬體)
            public double Rfb_HCM { get { return _parent.Rfb_HCM; } }           //High Current Mode阻抗(硬體)-會隨產品變動
            public double Rfb_LCM { get { return _parent.Rfb_LCM; } }           //Low Current Mode阻抗(硬體)-會隨產品變動
            public double Rin { get { return 0.5; } }                   //輸入阻抗(硬體)
            public double LED_R_Duty { get { return 0.4; } }            //Red LED Duty(硬體)
            public double LED_G_Duty { get { return 0.18; } }           //Green LED Duty(硬體)
            public double LED_B_Duty { get { return 0.12; } }           //Blue LED Duty(硬體)
            public double LED_B2_Duty { get { return 0.12; } }          //Blue2 LED Duty(硬體)
            public double HCM_MaxCurrent { get { return _parent.MaxCurrent_HCM; } }     //HCM理論最大電流
            public double LCM_MaxCurrent { get { return _parent.MaxCurrent_LCM; } }     //LCM理論最大電流
        }

    }
}

