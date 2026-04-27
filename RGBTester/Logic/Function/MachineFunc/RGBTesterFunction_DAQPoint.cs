using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using DeviceCore;
using RGBTester.Base;

namespace RGBTester.Logic
{
    public partial class RGBTesterFunction
    {
        #region parameter 
        public class DAQ_IO_Point
        {
            public EIOName DAQ_Vin;
            public EIOName DAQ_ILED;
            public EIOName DAQ_VLED;
            public EIOName DAQ_Vf;
            public EIOName DAQ_Iin_HCM;
            public EIOName DAQ_Iin_LCM;
        }
        public class DAQ_Point_FunctionTester
        {
            public EIOName DAQ_6V0;
            public EIOName DAQ_1V2;
            public EIOName DAQ_Vin;
            public EIOName DAQ_VLED_R;
            public EIOName DAQ_VLED_G;
            public EIOName DAQ_VLED_B;
            public EIOName DAQ_V_R;
            public EIOName DAQ_V_G;
            public EIOName DAQ_V_B1;
            public EIOName DAQ_V_B2;
            public EIOName DAQ_V_FB1;
            public EIOName DAQ_V_FB2;
        }
        #endregion

        public DAQ_Point_FunctionTester Get_DAQ_PointFuncTester(byte TestSide)
        {
            DAQ_Point_FunctionTester daq = new DAQ_Point_FunctionTester();

            bool isLeft = (TestSide == Machine.LightEngine.LED_LeftSide);
            daq.DAQ_6V0 = isLeft? EIOName.Left_6V:EIOName.Right_6V;
            daq.DAQ_1V2 = isLeft ? EIOName.Left_1V2 : EIOName.Right_1V2;
            daq.DAQ_Vin = isLeft ? EIOName.Left_Vin : EIOName.Right_Vin;
            daq.DAQ_VLED_R = isLeft ? EIOName.Left_VLED_R : EIOName.Right_VLED_R;
            daq.DAQ_VLED_G = isLeft ? EIOName.Left_VLED_G : EIOName.Right_VLED_G;
            daq.DAQ_VLED_B = isLeft ? EIOName.Left_VLED_B : EIOName.Right_VLED_B;
            daq.DAQ_V_R = isLeft ? EIOName.Left_V_R : EIOName.Right_V_R;
            daq.DAQ_V_G = isLeft ? EIOName.Left_V_G : EIOName.Right_V_G;
            daq.DAQ_V_B1 = isLeft ? EIOName.Left_V_B1 : EIOName.Right_V_B1;
            daq.DAQ_V_B2 = isLeft ? EIOName.Left_V_B2 : EIOName.Right_V_B2;
            daq.DAQ_V_FB1 = isLeft ? EIOName.Left_V_FB1 : EIOName.Right_V_FB1;
            daq.DAQ_V_FB2 = isLeft ? EIOName.Left_V_FB2 : EIOName.Right_V_FB2;

            return daq;
        }

        public DAQ_IO_Point Get_DAQ_IO_Point(byte TestSide, byte TestColor)
        {
            DAQ_IO_Point dAQ_IO_Point = new DAQ_IO_Point();
            IFunction_LightEngine lea = Machine.LightEngine;

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
