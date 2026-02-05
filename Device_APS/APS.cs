using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;
using Device_APS.APS168_W64;
using Device_APS.APS_Define_W32;

namespace Device_APS
{
    public class APS : IMotionCard
    {
        public APS()
        {

        }

        #region parameter define 
        APS_Parameter APS_Param = new APS_Parameter();
        private bool Initial_Success = false;
        private AXIS_INFO AxisInfo = new AXIS_INFO();

        struct APS_Parameter
        {
            public Int32 CardType;
            public Int32 MAX_DI_NUM;
            public Int32 MAX_DO_NUM;
            public Int32 MAX_MOTION_DEV_NUM;
            public bool[,,] Input_Status;   //紀錄[LineNo,DevNo,Port]對應的Input訊號
            public bool[,,] Motion_Status;  //紀錄[LineNo,DevNo,State]對應的Motion訊號
        }
        enum APS_Motion_IO
        {
            ALM = (int)APS_Define.MIO_ALM,
            PEL = (int)APS_Define.MIO_PEL,
            MEL = (int)APS_Define.MIO_MEL,
            ORG = (int)APS_Define.MIO_ORG,
            SVON = (int)APS_Define.MIO_SVON,
            INP = (int)APS_Define.MIO_INP,
            RDY = (int)APS_Define.MIO_RDY,
        }
        #endregion

        #region private function
        private int TransferToPulse(double intput)
        {
            double resolution = AxisInfo.DRIVER_RESOLUTION;
            double pitch = AxisInfo.PITCH;

            double pulse = intput * resolution / pitch;
            
            return (int)pulse;
        }
        #endregion


        public bool Open()
        {
            if (Initial_Success == true)
                return true;

            Int32 boardID_InBits = 0;
            Int32 mode = 0;
            Int32 ret = 0;
            Int32 StartAxisID = 0;
            Int32 TotalAxisNum = 0;

            try
            {
                ret = APS168.APS_initial(ref boardID_InBits, mode);
            }
            catch (Exception ex)
            {
                ret = -1;
            }


            if (ret != 0)
                return false;

            for (int i = 0; i < 16; i++)
            {
                int temp = (boardID_InBits >> i) & 1;

                if (temp != 1)
                    continue;

                ret = APS168.APS_get_card_name(i, ref APS_Param.CardType);

                if (APS_Param.CardType == (Int32)APS_Define.DEVICE_NAME_PCI_825458 ||
                    APS_Param.CardType == (Int32)APS_Define.DEVICE_NAME_AMP_20408C ||
                    APS_Param.CardType == (Int32)APS_Define.DEVICE_NAME_PCIE_8332)
                {
                    ret = APS168.APS_get_first_axisId(i, ref StartAxisID, ref TotalAxisNum);

                    
                    APS_Param.MAX_DI_NUM = 24;
                    APS_Param.MAX_DO_NUM = 24;
                    APS_Param.MAX_MOTION_DEV_NUM = TotalAxisNum;
                    APS_Param.Input_Status = new bool[5, 5, APS_Param.MAX_DI_NUM];
                    APS_Param.Motion_Status = new bool[5, TotalAxisNum, Enum.GetValues(typeof(APS_Motion_IO)).Length];

                    Initial_Success = true;
                    break;
                }
            }

            return Initial_Success;
        }

        public string GetName()
        {
            return "AMP_204C";

            if (!Initial_Success)
                return "None";

            if (APS_Param.CardType == (Int32)APS_Define.DEVICE_NAME_AMP_20408C)
                return "AMP_204C";
            else if (APS_Param.CardType == (Int32)APS_Define.DEVICE_NAME_PCIE_8332)
                return "PCIE_8332";

            return "None";
        }

        public int GetDeviceNo()
        {
            return APS_Param.MAX_MOTION_DEV_NUM;
        }

        #region IO Function
        public bool GetInputStatus(byte lineNo, byte DevNo, byte port)
        {
            if (APS_Param.CardType == (Int32)APS_Define.DEVICE_NAME_AMP_20408C)
            {
                if (port < 0 || port >= APS_Param.MAX_DI_NUM)
                    return false;
                if (DevNo < 0 || DevNo >= 5)
                    return false;
                if (lineNo < 0 || lineNo >= 5)
                    return false;
            }

            return APS_Param.Input_Status[lineNo, DevNo, port];
        }

        public void UpdateInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            Int32 digital_input_value = 0;

            APS168.APS_read_d_input(lineNo, 0 , ref digital_input_value);

            //digital_input_value = digital_input_value >> 8; //??芭比主程式

            for (int i = 0; i < APS_Param.MAX_DI_NUM; i++)
            {
                int check = ((digital_input_value >> i) & 1);

                if (check == 1)
                    APS_Param.Input_Status[lineNo, devNo, i] = true;
                else
                    APS_Param.Input_Status[lineNo, devNo, i] = false;
            }
        }

        public bool SetOutputStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false)
        {
            Int32 digital_output_value = 0;

            Int32[] do_ch = new Int32[APS_Param.MAX_DO_NUM];

            //****** Read digital output channels *****************************
            APS168.APS_read_d_output(cardNo, 0 , ref digital_output_value);

            for (int i = 0; i < APS_Param.MAX_DO_NUM; i++)
                do_ch[i] = ((digital_output_value >> i) & 1);

            //************ Write digital output channels examples *************
            int LineNumber = port;  //??芭比主程式+8

            if (truefalse)
                digital_output_value |= (1 << LineNumber);
            else
                digital_output_value &= ~(1 << LineNumber);

            APS168.APS_write_d_output(cardNo
                , 0                     // I32 DO_Group
                , digital_output_value  // I32 DO_Data
            );
            //*******************************************************************

            return true;
        }

        public void UpdateOutput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            throw new NotImplementedException();
        }

        public bool GetOutputStatus(byte lineNo, byte DevNo, byte port)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Motion Function
        public bool SetMotionConfig(AXIS_INFO axisInfo)
        {
            AxisInfo = axisInfo;
            return true;
        }

        public short UpdateMotionStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            if (Initial_Success == false)
                return 0;

            Int32 st = APS168.APS_motion_io_status(devNo);

            int index = (int)MOTION_IO.ALM;

            foreach(APS_Motion_IO signal in Enum.GetValues(typeof(APS_Motion_IO)))
            {
                int bitIndex = (int)signal;
                bool isOn = ((st >> bitIndex & 1) == 1) ? true : false;

                APS_Param.Motion_Status[lineNo, devNo, index] = isOn;

                index++;
            }

            return 0;
        }

        public bool GetMotionStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, int state = 0)
        {
            if (Initial_Success == false)
                return false;

            bool res = APS_Param.Motion_Status[lineNo, devNo, state];

            return res;
        }

        public bool GetMotionComplete(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            if (Initial_Success == false)
                return false;

            Int32 axis_id = devNo;
            Int32 msts = 0;

            msts = APS168.APS_motion_status(axis_id);   // Get motion status
            msts = (msts >> (int)APS_Define.NSTP) & 1;  // Get motion done bit

            // Get stop code.
            //APS168.APS_get_stop_code(Axis_ID, ref Stop_Code);

            if (msts == 1)
            {
                // Check move success or not
                msts = APS168.APS_motion_status(axis_id);       // Get motion status
                msts = (msts >> (int)APS_Define.MTS_ASTP) & 1;  // Get abnormal stop bit

                if (msts == 1)
                { // Error handling ...

                    //APS168.APS_get_stop_code(axis_id, ref m_stop_code);
                    return false; //error
                }
                else
                {   // Motion success.
                    return true;
                }
            }

            return false; //Now are in motion
        }

        public bool Servo_ONOff(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, bool flag = false)
        {
            if (Initial_Success == false)
                return false;
            
            int res = APS168.APS_set_servo_on(devNo, flag?1:0);

            if (res == 0)
                return true;
            else
                return false;
        }

        //public override bool SetGoHomeParam(AXIS_INFO hOME_PARAM)
        //{
        //    AxisInfo = hOME_PARAM;

        //    return true;
        //}

        public bool GoHome(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            if (Initial_Success == false)
                return false;

            byte axis_id = devNo;

            int acc = TransferToPulse(AxisInfo.HOME_ACC);
            int max_v = TransferToPulse(AxisInfo.MAX_VELOCITY);
            int org_v = TransferToPulse(AxisInfo.HOEM_FIND_ORG_VELOCITY);

            APS168.APS_set_axis_param(axis_id, (int)APS_Define.PRA_HOME_MODE, AxisInfo.MODE);                   //Set home mode         
            APS168.APS_set_axis_param(axis_id, (int)APS_Define.PRA_HOME_DIR, AxisInfo.DIRECTION);               //Set home direction
            APS168.APS_set_axis_param(axis_id, (int)APS_Define.PRA_HOME_CURVE, 0);                              // Set acceleration pattern (T-curve)
            APS168.APS_set_axis_param(axis_id, (int)APS_Define.PRA_HOME_ACC, acc);                              // Set homing acceleration rate
            APS168.APS_set_axis_param(axis_id, (int)APS_Define.PRA_HOME_VM, max_v);                             // Set homing maximum velocity. pulse/s
            APS168.APS_set_axis_param(axis_id, (int)APS_Define.PRA_HOME_VO, org_v);                             // Set homing velocitu to ORG pulse/s
            APS168.APS_set_axis_param(axis_id, (int)APS_Define.PRA_HOME_EZA, 0);                                // Set homing
            APS168.APS_set_axis_param(axis_id, (int)APS_Define.PRA_HOME_SHIFT, AxisInfo.HOME_SHIFT);            // Set homing shift position
            APS168.APS_set_axis_param(axis_id, (int)APS_Define.PRA_HOME_POS, AxisInfo.HOME_POS);                // Set homing position

            int res = APS168.APS_home_move(axis_id);

            if (res == 0)
                return true;
            else
                return false;
        }

        public double GetPosition(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            if (Initial_Success == false)
                return -1;

            int axis = devNo;
            double pos = -1;
            
            APS168.APS_get_position_f(axis, ref pos);

            return pos;
        }

        public int SetPosition(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, double pos = 0)
        {
            if (Initial_Success == false)
                return -1;

            int axis = devNo;
            int res1, res2;

            res1 = APS168.APS_set_position_f(axis, pos);
            res2 = APS168.APS_set_command_f(axis, pos);

            if (res1 != 0 || res2 != 0)
                return -1;
            else
                return 0;
        }

        public int AbsoluteSMove(int axis, double position, double velocity_max, double velocity_start, double Tacc, double Sfac, double Tdec, double Sdec)
        {
            if (Initial_Success == false)
                return -1;

            Int32 ret = -1;
            ASYNCALL p = new ASYNCALL();

            velocity_start  = TransferToPulse(velocity_start);
            velocity_max    = TransferToPulse(velocity_max);
            Tacc            = TransferToPulse(Tacc);
            Tdec            = TransferToPulse(Tdec);

            ret = APS168.APS_ptp_all(axis,
                                 (Int32)APS_Define.OPT_ABSOLUTE,
                                 position,
                                 velocity_start,
                                 velocity_max,
                                 0,
                                 Tacc,
                                 Tdec,
                                 Sfac,
                                 ref p);

            return ret;
        }

        public int RelativeSMove(int axis, double position, double velocity_max, double velocity_start, double Tacc, double Sacc, double Tdec, double Sdec)
        {
            if (Initial_Success == false)
                return -1;

            Int32 ret = -1;
            ASYNCALL p = new ASYNCALL();

            ret = APS168.APS_ptp_all(axis,
                     (Int32)APS_Define.OPT_RELATIVE,
                     position,
                     velocity_start,
                     velocity_max,
                     0,
                     Tacc,
                     Tdec,
                     Sacc,
                     ref p);

            return ret;
        }

        public int JogMoveStart(int axis, string direction, double velocity)
        {
            if (Initial_Success == false)
                return -1;
            Int32 ret = -1;
            Int32 jog_dir = 1;
            if (direction == "Negative")
                jog_dir = -1;
            //ret = APS168.APS_jog_start(axis,
            //                    jog_dir,
            //                    velocity,
            //                    0);
            return ret;
        }
        #endregion


    }
}
