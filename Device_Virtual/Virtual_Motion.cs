using DeviceCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Device_Virtual
{
    public class Virtual_Motion : IMotionCard
    {
        public Virtual_Motion()
        {
        }

        #region parameter define
        #endregion

        #region public function
        public bool Open()
        {
            return true;
        }

        public string GetName()
        {
            return "Virtual";
        }

        public int AbsoluteSMove(int axis, double position, double velocity_max, double velocity_start, double Tacc, double Sacc, double Tdec, double Sdec)
        {
            throw new NotImplementedException();
        }

        public bool GetMotionComplete(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            throw new NotImplementedException();
        }

        public bool GetMotionStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, int state = 0)
        {
            return false;
        }

        public double GetPosition(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            throw new NotImplementedException();
        }

        public int GoHome(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, int count = 1)
        {
            throw new NotImplementedException();
        }

        public int RelativeSMove(int axis, double position, double velocity_max, double velocity_start, double Tacc, double Sacc, double Tdec, double Sdec)
        {
            throw new NotImplementedException();
        }

        public bool Servo_ONOff(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, bool flag = false)
        {
            throw new NotImplementedException();
        }

        public bool SetMotionConfig(AXIS_INFO axisInfo)
        {
            throw new NotImplementedException();
        }

        public int SetPosition(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, double pos = 0)
        {
            throw new NotImplementedException();
        }

        public short UpdateMotionStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            throw new NotImplementedException();
        }

        public int GetDeviceNo()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
