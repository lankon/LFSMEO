using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public interface IFunction_MotionCard
    {
        bool Initial_All_Motion();
        void SetAxis(AXIS_INFO MF);
        void ClearAxis();
        void BindingAxis();
        
        // Set Parameter & Status Function
        bool SetServo(int axis, bool on_off);
        int SetSpeedConfig();
        
        // Position Function
        double GetPosition(int axis);

        // Home Function
        Task<bool> GoHome(int axis);
        bool Get_Home_Complete(int axis);
        
        // Move Function
        bool Get_Motion_Complete(int axis);
        bool PTP_Move(int axis, double pos, string mode = "Abs");
        
        //[Read&Save Axis Information]
        void SaveAxisConfig(string filePath, string axisName, Dictionary<string, string> parameters);
        bool LoadAxisConfig();
        IReadOnlyList<AXIS_INFO> GetAxisConfig();
    }
}
