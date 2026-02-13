using DeviceCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using ToolFunction;

namespace Device_Virtual
{
    public class Virtual_Motion : IMotionCard
    {
        public Virtual_Motion()
        {
        }

        #region parameter define
        private Dictionary<int, AXIS_INFO> AxisInfoMap = new Dictionary<int, AXIS_INFO>();
        private Dictionary<int, double> CurrentPosition = new Dictionary<int, double>();        //當前位置(mm)
        private Dictionary<int, double> MoveTime = new Dictionary<int, double>();               //模擬運動時間(ms)
        private Dictionary<int, long> MoveTimer = new Dictionary<int, long>();                  //模擬運動計時器(ms)
        private Dictionary<int, bool> CM_Stop = new Dictionary<int, bool>();                    //連續移動停止旗標

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

        #region private function
        private int TransferToPulse(double intput, int axis)
        {
            double resolution = AxisInfoMap[axis].DRIVER_RESOLUTION;
            double pitch = AxisInfoMap[axis].PITCH;

            if (pitch == 0)
                return 0;

            double pulse = intput * resolution / pitch;

            return (int)pulse;
        }
        private double TransferToMillimeter(double intput, int axis)
        {
            double resolution = AxisInfoMap[axis].DRIVER_RESOLUTION;
            double pitch = AxisInfoMap[axis].PITCH;

            if (resolution == 0)
                return 0;

            double pulse = intput / resolution * pitch;

            return pulse;
        }
        private double CalculateTCurveTime(double position, double velocity_max, double velocity_start, double Tacc, double Sacc, double Tdec, double Sdec)
        {
            //模擬T-Curve運動時間
            //T:運動時間
            //S:距離
            //V:速度
            //Sacc:加速度距離
            //Sdec:減速度距離
            //Acc:加速度
            //Dec:減速度
            //Vmax:最大速度

            position = Math.Abs(position);

            double T_acc = (velocity_max - velocity_start) / Tacc;
            double S_acc = velocity_start * T_acc + 0.5 * Tacc * Math.Pow(T_acc, 2);
            double T_dec = velocity_max / Tdec;
            double S_dec = Math.Pow(velocity_max, 2) / (2 * Tdec);
            double T_total = 0;

            if (S_acc + S_dec > position)
            {
                double velocity_top = (2 * position * Tacc * Tdec + Tdec * Math.Pow(velocity_start, 2)) / (Tacc + Tdec);
                velocity_top = Math.Sqrt(velocity_top);

                T_acc = (velocity_top - velocity_start) / Tacc;
                T_dec = velocity_top / Tdec;
                T_total = T_acc + T_dec;
            }
            else
            {
                double T_const = (position - S_acc - S_dec) / velocity_max;
                T_total = T_acc + T_const + T_dec;
            }

            if (T_total <= 0)
                return 100;
            else
                return T_total * 1000;  //ms
        }
        #endregion

        #region public function
        public bool Open()
        {
            for(int i = 0; i < GetDeviceNo(); i++)
            {
                CurrentPosition[i] = 0;
                MoveTime[i] = 0;
                MoveTimer[i] = 0;
                CM_Stop[i] = false;

                AxisInfoMap[i] = new AXIS_INFO()
                {
                    PEL_POS = 10000,
                    MEL_POS = -10000,
                };
            }

            return true;
        }


        public bool SetMotionConfig(AXIS_INFO axisInfo, int axis)
        {
            AxisInfoMap[axis] = axisInfo;
            return true;
        }
        public bool Servo_ONOff(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, bool flag = false)
        {
            return true;
        }
        public int SetPosition(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, double pos = 0)
        {
            CurrentPosition[devNo] = pos;
            return 0;
        }


        public string GetName()
        {
            return "Virtual";
        }
        public int GetDeviceNo()
        {
            return 25;
        }
        public short UpdateMotionStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            return 0;
        }
        public bool GetMotionComplete(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            if (Tool.GetTime(MoveTimer[devNo]) < MoveTime[devNo])
                return false;
            else
                return true;
        }
        public bool GetMotionStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, int state = 0)
        {
            if (devNo > GetDeviceNo())
                return false;
            
            if (state == (int)VIRTUAL_MOTION_IO.MEL && CurrentPosition[devNo] <= AxisInfoMap[devNo].MEL_POS)
            {
                CM_Stop[devNo] = true; //極限觸發後停止連續移動
                return true;
            }

            if (state == (int)VIRTUAL_MOTION_IO.PEL && CurrentPosition[devNo] >= AxisInfoMap[devNo].PEL_POS)
            {
                CM_Stop[devNo] = true; //極限觸發後停止連續移動
                return true;
            }

            if (state == (int)VIRTUAL_MOTION_IO.ORG || state == (int)VIRTUAL_MOTION_IO.INP ||
                state == (int)VIRTUAL_MOTION_IO.RDY || state == (int)VIRTUAL_MOTION_IO.SVON)
                return true;

            return false;
        }
        public double GetPosition(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            CurrentPosition.TryGetValue(devNo, out double pos);

            return pos;
        }


        public int GoHome(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, int count = 1)
        {
            return 0;
        }
        public int Stop(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, double Tdec = 0)
        {
            CM_Stop[devNo] = true;

            return 0;
        }
        public int RelativeSMove(int axis, double position, double velocity_max, double velocity_start, double Tacc, double Sacc, double Tdec, double Sdec)
        {
            double time = 0;

            if (Sacc == 0 && Sdec == 0)
                time = CalculateTCurveTime(position, velocity_max, velocity_start, Tacc, Sacc, Tdec, Sdec);
            else
                time = 0;

            CurrentPosition[axis] = CurrentPosition[axis] + position;
            MoveTime[axis] = time;
            Tool.ResetTimeCount(out long startTicks);
            MoveTimer[axis] = startTicks;

            return 0;
        }
        public int AbsoluteSMove(int axis, double position, double velocity_max, double velocity_start, double Tacc, double Sacc, double Tdec, double Sdec)
        {
            double time = 0;

            if (Sacc == 0 && Sdec == 0)
                time = CalculateTCurveTime(CurrentPosition[axis] - position, velocity_max, velocity_start, Tacc, Sacc, Tdec, Sdec);
            else
                time = 0;

            CurrentPosition[axis] = position;
            MoveTime[axis] = time;
            Tool.ResetTimeCount(out long startTicks);
            MoveTimer[axis] = startTicks;

            return 0;
        }
        public int ContinuousMove(int axis, int dir, double acc, double dec, double velocity_max)
        {
            //觸發後撞到極限才會停止,先給初始化流程使用
            
            int ret = 0;

            Tool.ResetTimeCount(out long startTicks);
            CM_Stop[axis] = false;

            Thread moveThread = new Thread(() =>
            {
                while (!CM_Stop[axis])
                {
                    if (Tool.GetTime(startTicks, time: "s") > 5)
                        break;

                    double time = Tool.GetTime(startTicks, time: "s");

                    if (dir == 1)    //負向
                        CurrentPosition[axis] = CurrentPosition[axis] - time * velocity_max;
                    else
                        CurrentPosition[axis] = CurrentPosition[axis] + time * velocity_max;

                    Thread.Sleep(100);
                }
            });

            moveThread.IsBackground = true;
            moveThread.Start();

            acc = TransferToPulse(acc, axis);
            dec = TransferToPulse(dec, axis);
            velocity_max = TransferToPulse(velocity_max, axis);

            return ret;
        }
        #endregion

    }
}
