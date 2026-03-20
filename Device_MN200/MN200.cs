using DeviceCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Device_MN200
{
    public class MN200:IMotionCard,IIOCard
    {
        #region parameter define
        private Int16 nErrCode;
        private bool Initial_Success = false;
        private bool Initial_IO_Success = false;
        private bool Initial_Motion_Success = false;
        private const Byte MaxNumLine = 5;
        private const Byte MaxNumDevicesPerLine = 64;
        private const Byte MaxNumStatus = 32;
        private MN200_Parameter MN200_Param = new MN200_Parameter();
        private IO_PARAMETER MN200_IO_Param = new IO_PARAMETER();
        PISO_MN200.MOTION_DEV_IO MOTION_DEV_IO = new PISO_MN200.MOTION_DEV_IO();
        

        struct MN200_Parameter
        {
            public byte[,] DevNoType;       //紀錄[LineNo,DevNo]對應的Type
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
            // [Motion Parameter Initial]
            MN200_Param.DevNoType = new byte[MaxNumLine, MaxNumDevicesPerLine];
            MN200_Param.Motion_Status = new bool[MaxNumLine, MaxNumDevicesPerLine, 16];
            MN200_Param.Motion_DevNo = new List<byte>();
            MN200_Param.Motion_LineNo = new List<byte>();

            // [IO Parameter Initial]
            MN200_IO_Param.Input_Status = new bool[MaxNumLine, MaxNumDevicesPerLine, MaxNumStatus];
            MN200_IO_Param.Output_Status = new bool[MaxNumLine, MaxNumDevicesPerLine, MaxNumStatus];
            MN200_IO_Param.IO_DevNo = new List<byte>();
            MN200_IO_Param.IO_LineNo = new List<byte>();
        }

        #region private function
        private bool Check_IO_DeviceUse(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            bool res = true;

            if(Initial_IO_Success == false)
                res = false;

            if (lineNo >= MaxNumLine || devNo >= MaxNumDevicesPerLine || port >= MaxNumStatus)
                res = false;

            if(lineNo < 0 || devNo < 0 || port < 0)
                res = false;

            return res;
        }
        #endregion


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
                                MN200_IO_Param.DevNoType[lineNo, bDevNo] = bDevType;
                                MN200_IO_Param.IO_LineNo.Add(lineNo);
                                MN200_IO_Param.IO_DevNo.Add(bDevNo);
                                IO_DevNum++;
                            }
                            break;
                        case PISO_MN200.Param.DEV_INFO_IO_32OUT_DEV:
                            {
                                //MN200_IO_Param.DevNoType[lineNo, bDevNo] = bDevType;
                                //IO_DevNum++;
                            }
                            break;
                    }
                }
            }

            if(IO_DevNum == 0 && Motion_DevNum == 0)
                return false;
            
            if(IO_DevNum > 0)
                Initial_IO_Success = true;

            if(Motion_DevNum > 0)
                Initial_Motion_Success = true;

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

        #region Motion function
        public int GetDeviceNo()
        {
            return 1;
        }

        public bool SetMotionConfig(AXIS_INFO axisInfo, int UI_AxisId)
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

        public int GoHome(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, int count = 1)
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

        public int ContinuousMove(int axis, int dir, double acc, double dec, double velocity_max)
        {
            throw new NotImplementedException();
        }

        public int Stop(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, double Tdec = 0)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IO Funciton
        public IO_PARAMETER Get_IO_Info()
        {
            return MN200_IO_Param;
        }

        public void UpdateInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            if(!Check_IO_DeviceUse(cardNo, lineNo, devNo, port))
                return;

            UInt16 wData = 0;
            byte m_CurDevType = MN200_IO_Param.DevNoType[lineNo, devNo];

            if (m_CurDevType == PISO_MN200.Param.DEV_INFO_IO_32IN_DEV)
            {
                //讀取 DI 資料
                PISO_MN200.Functions.mn_get_di_word(lineNo, devNo, 0, ref wData);

                for (byte status = 0; status < 16; status++)
                {
                    MN200_IO_Param.Input_Status[lineNo, devNo, status] = (wData & (1 << status)) != 0;
                }

                PISO_MN200.Functions.mn_get_di_word(lineNo, devNo, 1, ref wData);

                for (byte status = 16; status < 32; status++)
                {
                    MN200_IO_Param.Input_Status[lineNo, devNo, status] = (wData & (1 << (status - 16))) != 0;
                }
            }
        }

        public bool GetInputStatus(byte cardNo, byte lineNo, byte DevNo, byte port)
        {
            if (!Check_IO_DeviceUse(cardNo, lineNo, DevNo, port))
                return false;

            return MN200_IO_Param.Input_Status[lineNo, DevNo, port];
        }

        public void UpdateOutput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            if (!Check_IO_DeviceUse(cardNo, lineNo, devNo, port))
                return;

            byte dev = MN200_Param.DevNoType[lineNo, devNo];

            if (dev == PISO_MN200.Param.DEV_INFO_IO_32OUT_DEV)
            {
                ushort uData_0 = 0;
                ushort uData_1 = 0;

                PISO_MN200.Functions.mn_get_do_word(lineNo, devNo, 0, ref uData_0);
                Thread.Sleep(1);
                PISO_MN200.Functions.mn_get_do_word(lineNo, devNo, 1, ref uData_1);

                for (int i = 0; i < 16; i++)
                {
                    if (((uData_0 >> i) & 1) == 1)
                        MN200_IO_Param.Output_Status[lineNo, devNo, i] = true;
                    else
                        MN200_IO_Param.Output_Status[lineNo, devNo, i] = false;

                    if (((uData_1 >> i) & 1) == 1)
                        MN200_IO_Param.Output_Status[lineNo, devNo, i + 16] = true;
                    else
                        MN200_IO_Param.Output_Status[lineNo, devNo, i + 16] = false;
                }
            }
        }

        public bool GetOutputStatus(byte cardNo, byte lineNo, byte DevNo, byte port)
        {
            if (!Check_IO_DeviceUse(cardNo, lineNo, DevNo, port))
                return false;

            UpdateOutput(0, lineNo, DevNo, port);

            bool res = MN200_IO_Param.Output_Status[lineNo, DevNo, port];

            return res;
        }

        public bool SetOutputStatus(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, bool truefalse = false)
        {
            if (!Check_IO_DeviceUse(cardNo, lineNo, devNo, port))
                return false;

            byte dev = MN200_Param.DevNoType[lineNo, devNo];

            if (dev == PISO_MN200.Param.DEV_INFO_IO_32OUT_DEV)
            {
                ushort uData = 0;
                short res = -1;
                byte word_no = 0;

                if (port < 16)
                    word_no = 0;
                else
                {
                    word_no = 1;
                    port -= 16;
                }

                //取得目前DO狀態
                PISO_MN200.Functions.mn_get_do_word(lineNo, devNo, word_no, ref uData);

                if (truefalse)
                    uData |= (ushort)(1 << port);    // 設為 1
                else
                    uData &= (ushort)~(1 << port);    // 設為 0

                res = PISO_MN200.Functions.mn_set_do_word(lineNo, devNo, word_no, uData);
            }

            return true;
        }

        //[未開發]
        public double GetAInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0)
        {
            throw new NotImplementedException();
        }

        public double GetAInput(byte cardNo = 0, byte lineNo = 0, byte devNo = 0, byte port = 0, string range = "")
        {
            throw new NotImplementedException();
        }
        #endregion

        // [Virtual IO Function]
        public int Add_AI_VirtualData(byte port, double value)
        {
            return -1;
        }

        public int Clear_AI_VirtualData()
        {
            return 0;
        }
    }
}

