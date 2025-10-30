using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public enum eMotionSetting
    {
        Cmbx_MachineType,
        Cmbx_ShowFormName,

        Cmbx_AxisType,
        TxtBx_AxisStation,
        Cmbx_AxisLimitLogic,
        Cmbx_AxisLimitStopMode,
        Cmbx_AxisUse,

        TxtBx_AxisName,
    }


    public interface IMotion_IO_Card
    {
        #region abstract
        bool Open();

        //[Motion Function]
        bool SetMotionConfig();
        short UpdateMotionStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0);
        bool GetMotionStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, int state = 0);
        bool GetMotionComplete(byte cardNo = 0, byte lineNo = 0, byte devNo = 0);
        bool Servo_ONOff(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, bool flag = false);
        bool GoHome(byte cardNo = 0, byte lineNo = 0, byte devNo = 0);
        double GetPosition(byte cardNo = 0, byte lineNo = 0, byte devNo = 0);
        int SetPosition(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, double pos = 0);
        int AbsoluteSMove(int axis, double position, double velocity_max, double velocity_start,
                                          double Tacc, double Sacc, double Tdec, double Sdec);
        int RelativeSMove(int axis, double position, double velocity_max, double velocity_start,
                                          double Tacc, double Sacc, double Tdec, double Sdec);


        //[IO Function]
        string GetName();
        void UpdateInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0);
        bool GetInputStatus(byte lineNo, byte DevNo, byte port);
        void UpdateOutput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0);
        bool GetOutputStatus(byte lineNo, byte DevNo, byte port);
        bool SetOutputStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false);
        #endregion
    }
}
