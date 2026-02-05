using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public struct AXIS_INFO
    {
        //新增軸參數時需添加

        //[Axis Configuration]
        public string AXIS_TYPE;    //軸卡名稱
        public int LINE_NO;         //軸卡線程
        public int DEV_NO;          //軸卡軸編號
        public int AXIS_USE;        //軸卡使用Y/N
        public int LIMIT_LOGIC;     //硬體極限觸發邏輯
        public int STOP_MODE;       //停止模式
        public int DRIVER_RESOLUTION;       //驅動器解析度

        //[Hardware Configuration]
        public double PITCH;        //軸卡螺距

        //[Software Configuration]
        public string AXIS_NANE;    //軸名稱
        public int SW_LIMIT;        //軟體極限Y/N
        public double PEL_POS;      //軟體正極限位置
        public double MEL_POS;      //軟體負極限位置
        public int REVERSE_MOVE;    //運動方向相反Y/N

        //[Speed Configuration]
        public double FAST_MAX_SPEED;       //Fast最大速度
        public double FAST_INIT_SPEED;      //Fast起始速度
        public double FAST_ACC;             //Fast加速度
        public double FAST_DEC;             //Fast減速度
        public double FAST_Sfac;            //Fast Sfac
        public double SLOW_MAX_SPEED;       //Slow最大速度
        public double SLOW_INIT_SPEED;      //Slow起始速度
        public double SLOW_ACC;             //Slow加速度
        public double SLOW_DEC;             //Slow減速度
        public double SLOW_Sfac;            //Slow Sfac
        public double NORMAL_MAX_SPEED;     //Normal最大速度
        public double NORMAL_INIT_SPEED;    //Normal起始速度
        public double NORMAL_ACC;           //Normal加速度
        public double NORMAL_DEC;           //Normal減速度
        public double NORMAL_Sfac;          //NormalSfac

        //[Home Configuration]
        public int MODE;            //歸Home模式
        public int DIRECTION;       //方向
        public int HOME_POS;        //原點位置
        public int HOME_SHIFT;      //到原點後位移距離
        public int MAX_VELOCITY;    //最大速度
        public int HOEM_FIND_ORG_VELOCITY;  //搜尋原點速度
        public int HOME_ACC;        //Home加速度
    }
    public enum MOTION_IO
    {
        ALM,
        PEL,
        MEL,
        ORG,
        SVON,
        INP,
        RDY,
    }

    public interface IMotionCard
    {
        bool Open();
        string GetName();
        bool SetMotionConfig(AXIS_INFO axisInfo);
        short UpdateMotionStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0);
        bool GetMotionStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, int state = 0);
        bool GetMotionComplete(byte cardNo = 0, byte lineNo = 0, byte devNo = 0);
        bool Servo_ONOff(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, bool flag = false);
        //bool SetGoHomeParam(AXIS_INFO hOME_PARAM);
        bool GoHome(byte cardNo = 0, byte lineNo = 0, byte devNo = 0);
        double GetPosition(byte cardNo = 0, byte lineNo = 0, byte devNo = 0);
        int SetPosition(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, double pos = 0);
        int AbsoluteSMove(int axis, double position, double velocity_max, double velocity_start,
                                          double Tacc, double Sfac, double Tdec, double Sdec);
        int RelativeSMove(int axis, double position, double velocity_max, double velocity_start,
                                          double Tacc, double Sacc, double Tdec, double Sdec);
        
    }
}
