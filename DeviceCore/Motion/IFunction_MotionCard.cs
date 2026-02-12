using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public enum MOVE_VELOCITY_MODE
    {
        FAST,
        SLOW,
        NORMAL,
    }
    public enum SINGLE_MOVE_MODE
    {
        INDEX,
        CONTINUOUS_SLOW,
        CONTINUOUS_FAST,
        CONTINUOUS_NORMAL,
    }

    public interface IFunction_MotionCard
    {
        // [Initial Function]
        bool Initial_All_Motion();
        void BindingAxis();
        
        // [Set Parameter & Status Function]
        bool SetServo(int axis, bool on_off);
        bool SetSingleMoveMode(SINGLE_MOVE_MODE mode);

        // [Status Function]
        double GetPosition(int axis);
        void GetMotionStatus(int axis, out bool[] status);
        SINGLE_MOVE_MODE GetSingleMoveMode();

        // [Home Function]
        Task<bool> GoHome(int axis);
        bool Get_Home_Complete(int axis);
        
        // [Move Function]
        bool Get_Motion_Complete(int axis);
        bool PTP_Move(int axis, double pos, string mode = "Abs", MOVE_VELOCITY_MODE velocityMode = MOVE_VELOCITY_MODE.NORMAL);
        bool SingleMove(int axis, int dir, double pos);
        bool StopAxisMove(int axis);
        bool Jog_Start(int axis, string direction, MOVE_VELOCITY_MODE velocityMode = MOVE_VELOCITY_MODE.NORMAL);
        bool Jog_Stop(int axis);

        // [Read&Save Axis Information]
        void SaveAxisConfig(string filePath, string axisName, Dictionary<string, string> parameters);
        bool LoadAxisConfig();
        IReadOnlyList<AXIS_INFO> GetAxisConfig();
    }
}
