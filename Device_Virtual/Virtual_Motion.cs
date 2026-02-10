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
        enum VIRTUAL_MOTION_IO
        {
            ALM,
            PEL,
            MEL,
            ORG,
            SVON,
            INP,
            RDY,
        }
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
            /*模擬T-Curve運動時間
            T:運動時間
            S:距離
            V:速度
            Sacc:加速度距離
            Sdec:減速度距離
            Acc:加速度
            Dec:減速度
            Vmax:最大速度

            加速度段移動時間 T_acc = (V_max - V_0) / Acc
            加速度段移動距離 S_acc = V_0 * T_acc + 0.5 * Acc * T_acc^2
            
            減速度段移動時間 T_dec = V_max / Dec
            減速度段移動距離 S_dec = V_max^2 / (2 * Dec)

            均速度段移動時間 T_const = (S_total - Sacc - Sdec) / Vmax

            總運動時間 T = T_acc + T_const + T_dec
            */

            throw new NotImplementedException();
        }

        public bool GetMotionStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, int state = 0)
        {
            if (state == (int)VIRTUAL_MOTION_IO.ALM || state == (int)VIRTUAL_MOTION_IO.MEL ||
                state == (int)VIRTUAL_MOTION_IO.PEL)
                return false;
            else
                return true;
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

        public int ContinuousMove(int axis, int dir, double acc, double dec, double velocity_max)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
