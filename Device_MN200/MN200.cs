using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using DeviceCore;

namespace Device_MN200
{
    public class MN200:IMotionCard,IIOCard
    {
        #region parameter define
        private Int16 nErrCode;
        private bool Initial_Success = false;
        private const Byte MaxNumDevicesPerLine = 64;
        private const Byte MaxNumLine = 5;
        private const Byte MaxNumStatus = 32;
        private MN200_Parameter MN200_Param = new MN200_Parameter();
        PISO_MN200.MOTION_DEV_IO MOTION_DEV_IO = new PISO_MN200.MOTION_DEV_IO();
        public List<byte> InputLineNo 
        {
            get { return MN200_Param.IO_LineNo; } 
        }
        public List<byte> InputDevNo
        {
            get { return MN200_Param.IO_DevNo; }
        }

        struct MN200_Parameter
        {
            public bool[] UseLine;          //紀錄使用的LineNo.
            public byte[,] DevNoType;       //紀錄[LineNo,DevNo]對應的Type
            public bool[,,] Input_Status;   //紀錄[LineNo,DevNo,Port]對應的Input訊號
            public bool[,,] Output_Status;  //紀錄[LineNo,DevNo,Port]對應的Output訊號
            public List<byte> IO_LineNo;    //紀錄IO Type Line No.
            public List<byte> IO_DevNo;     //紀錄IO Type Device No.
            public bool[,,] Motion_Status;  //紀錄[LineNo,DevNo,State]對應的Motion訊號
            public List<byte> Motion_LineNo;//紀錄Motion Type Line No.
            public List<byte> Motion_DevNo; //紀錄Motion Type Device No.
        }
        enum MN200_Motion_IO
        {
            ALM,
            PEL,
            MEL,
            ORG,
            SVON,
            INP,
            RDY,
            RESET_ALM,
            SDLTC,
            SDIN,
            EMG,
            EZ,
            ERC,
        }
        #endregion

        public MN200()
        {
            MN200_Param.DevNoType = new byte[MaxNumLine, MaxNumDevicesPerLine];
            MN200_Param.Input_Status = new bool[MaxNumLine, MaxNumDevicesPerLine, MaxNumStatus];
            MN200_Param.Output_Status = new bool[MaxNumLine, MaxNumDevicesPerLine, MaxNumStatus];
            MN200_Param.IO_DevNo = new List<byte>();
            MN200_Param.IO_LineNo = new List<byte>();
            MN200_Param.Motion_Status = new bool[MaxNumLine, MaxNumDevicesPerLine, 16];
            MN200_Param.Motion_DevNo = new List<byte>();
            MN200_Param.Motion_LineNo = new List<byte>();
        }

        public bool Open()
        {
            if (Initial_Success == true)
                return true;
            
            Byte m_NumLine = 0;
            Byte DefBaudRate = PISO_MN200.Param.COMMSPEED_10M;    //要開放設定(連線速度)
            Byte pNumDev = 0;
            int IO_DevNum = 0;
            int Motion_DevNum = 0;

            try
            {
                if ((nErrCode = PISO_MN200.Functions.mn_open_all(ref m_NumLine)) != PISO_MN200.ErrCode.SUCCESS)
                {
                    PISO_MN200.Functions.mn_close_all();
                    return false;
                }
            }
            catch
            {
                return false;
            }

            if(m_NumLine == 0)
                return false;

            for (byte lineNo = 0; lineNo < m_NumLine; lineNo++) // Loop through each line
            {
                // Set baud rate of current line
                if ((nErrCode = PISO_MN200.Functions.mn_set_comm_speed(lineNo, DefBaudRate)) != PISO_MN200.ErrCode.SUCCESS)
                    continue;

                if ((nErrCode = PISO_MN200.Functions.mn_start_line(lineNo, ref pNumDev)) != PISO_MN200.ErrCode.SUCCESS)
                    continue;

                for (byte bDevNo = 0; bDevNo < MaxNumDevicesPerLine; bDevNo++)
                {
                    Byte bDevType = 0;

                    PISO_MN200.Functions.mn_get_dev_info(lineNo, bDevNo, ref bDevType);

                    switch (bDevType)
                    {
                        case PISO_MN200.Param.DEV_INFO_NO_DEV:
                            {
                                MN200_Param.DevNoType[lineNo, bDevNo] = bDevType;
                            }
                            break;

                        case PISO_MN200.Param.DEV_INFO_MOTION_DEV:
                            {
                                MN200_Param.DevNoType[lineNo, bDevNo] = bDevType;
                                MN200_Param.Motion_LineNo.Add(lineNo);
                                MN200_Param.Motion_DevNo.Add(bDevNo);
                                Motion_DevNum++;
                            }
                            break;

                        case PISO_MN200.Param.DEV_INFO_IO_16IN_16OUT_DEV:
                        case PISO_MN200.Param.DEV_INFO_IO_32IN_DEV:
                            {
                                MN200_Param.DevNoType[lineNo, bDevNo] = bDevType;
                                MN200_Param.IO_LineNo.Add(lineNo);
                                MN200_Param.IO_DevNo.Add(bDevNo);
                                IO_DevNum++;
                            }
                            break;
                        case PISO_MN200.Param.DEV_INFO_IO_32OUT_DEV:
                            {
                                MN200_Param.DevNoType[lineNo, bDevNo] = bDevType;
                                IO_DevNum++;
                            }
                            break;
                    }
                }
            }

            if(IO_DevNum == 0 && Motion_DevNum == 0)
                return false;

            Initial_Success = true;

            return true;
        }
        
        public string GetName()
        {
            if (Initial_Success)
                return "MN200";
            else
                return "None";
        }
        
        public bool SetMotionConfig()
        {
            throw new NotImplementedException();
        }
        
        public short UpdateMotionStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            PISO_MN200.Functions.mn_get_mdio_status(lineNo, devNo, ref MOTION_DEV_IO);

            Type ioType = typeof(PISO_MN200.MOTION_DEV_IO);

            foreach (MN200_Motion_IO io in Enum.GetValues(typeof(MN200_Motion_IO)))
            {
                var prop = ioType.GetField(io.ToString());
                if (prop != null)
                {
                    object value = prop.GetValue(MOTION_DEV_IO);  // 從 struct 中取值
                    bool isOn = (Convert.ToInt32(value) == PISO_MN200.Param.TURN_ON);

                    MN200_Param.Motion_Status[lineNo, devNo, (int)io] = isOn;
                }
            }

            return 0;
        }
        
        public bool Servo_ONOff(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, bool flag = false)
        {
            try
            {
                if (flag)
                    PISO_MN200.Functions.mn_servo_on(lineNo, devNo, PISO_MN200.Param.TURN_ON);
                else
                    PISO_MN200.Functions.mn_servo_on(lineNo, devNo, PISO_MN200.Param.TURN_OFF);

                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool GoHome(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            throw new NotImplementedException();
        }
        
        public double GetPosition(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            throw new NotImplementedException();
        }
        
        public bool GetMotionStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, int state = 0)
        {
            throw new NotImplementedException();
        }
        
        public bool GetMotionComplete(byte cardNo = 0, byte lineNo = 0, byte devNo = 0)
        {
            throw new NotImplementedException();
        }
        
        public int SetPosition(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, double pos = 0)
        {
            throw new NotImplementedException();
        }
        
        public int AbsoluteSMove(int axis, double position, double velocity_max, double velocity_start, double Tacc, double Sacc, double Tdec, double Sdec)
        {
            throw new NotImplementedException();
        }
        
        public int RelativeSMove(int axis, double position, double velocity_max, double velocity_start, double Tacc, double Sacc, double Tdec, double Sdec)
        {
            throw new NotImplementedException();
        }
        
        public void UpdateInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            throw new NotImplementedException();
        }
        
        public bool GetInputStatus(byte lineNo, byte DevNo, byte port)
        {
            throw new NotImplementedException();
        }
        
        public void UpdateOutput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            throw new NotImplementedException();
        }
        
        public bool GetOutputStatus(byte lineNo, byte DevNo, byte port)
        {
            throw new NotImplementedException();
        }

        public bool SetOutputStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false)
        {
            throw new NotImplementedException();
        }

        public bool GetInputStatus(byte cardNo, byte lineNo, byte DevNo, byte port)
        {
            throw new NotImplementedException();
        }

        public bool GetOutputStatus(byte cardNo, byte lineNo, byte DevNo, byte port)
        {
            throw new NotImplementedException();
        }

        public double GetAInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            throw new NotImplementedException();
        }

        public double GetAInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, string range = "")
        {
            throw new NotImplementedException();
        }

        public int Add_AI_VirtualData(byte port, double value)
        {
            throw new NotImplementedException();
        }
    }
}

