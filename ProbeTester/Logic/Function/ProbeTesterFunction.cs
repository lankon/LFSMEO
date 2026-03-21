using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


using ToolFunction;
using DeviceCore;
using ProbeTester.Base;

namespace ProbeTester.Logic
{
    public class ProbeTesterFunction
    {
        public ProbeTesterFunction(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Axis_HardwareParam = new AxisHardwareParam(this);
            Camera_HardwareParam = new CameraHardwareParam(this);
        }

        #region parameter define
        IServiceProvider ServiceProvider;
        public AxisHardwareParam Axis_HardwareParam { get; private set; }
        public CameraHardwareParam Camera_HardwareParam { get; private set; }
        #endregion

        public class AxisHardwareParam
        {
            private ProbeTesterFunction _parent;
            public AxisHardwareParam(ProbeTesterFunction parent)
            {
                _parent = parent;
            }

            public int AxisX = (int)AXIS_NAME.AXIS_X;
            public int AxisY = (int)AXIS_NAME.AXIS_Y;
            public int AxisZ = (int)AXIS_NAME.AXIS_Z;
            public int AxisA = (int)AXIS_NAME.AXIS_A;
        }

        public class CameraHardwareParam
        {
            private ProbeTesterFunction _parent;
            public CameraHardwareParam(ProbeTesterFunction parent)
            {
                _parent = parent;
            }

            public int Scan = (int)CCD_NAME.CCD_0;
            public int Needle = (int)CCD_NAME.CCD_1;
        }


    }
}
