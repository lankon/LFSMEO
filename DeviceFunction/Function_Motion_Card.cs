using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Xml.Linq;

using ToolFunction;
using DeviceCore;

namespace DeviceFunction
{
    public class Function_Motion_Card:IFunction_MotionCard
    {
        public Function_Motion_Card(IEnumerable<IMotionCard> cards)
        {
            Cards = cards;
        }

        #region parameter define
        private List<IMotionCard> DML = new List<IMotionCard>();
        private List<AXIS_INFO> DML_INFO = new List<AXIS_INFO>();
        private int[] DML2Axis;
        private long CycleTime = 0;
        private bool[] DML_Home_Complete;       //判斷各軸是否完成Home
        private bool[] DML_Homing;              //判斷各軸是否正在執行Home
        private readonly IEnumerable<IMotionCard> Cards;
        private SINGLE_MOVE_MODE SingleMoveMode = SINGLE_MOVE_MODE.INDEX;

        
        public enum WORK
        {
            INITIAL,
            SERVO_ON,
            GO_HOME_FIRST,
            GO_HOME_FIRST_SHIFT,
            GO_HOME_SECOND,
            GO_HOME_SECOND_SHIFT,
            WAIT_GO_HOME_SECOND_SHIFT,
        }
        #endregion

        #region Threading
        private void Process()
        {
            while (true)
            {
                //Thread持續讀取Input訊號
                for (int k = 0; k < DML.Count; k++)
                {
                    if (DML[k].GetName() != "AMP_204C")
                        continue;

                    byte DevNo = (byte)DML[k].GetDeviceNo();

                    for(byte i=0;i<DevNo;i++)
                    {
                        DML[k].UpdateMotionStatus(devNo: i);
                    }
                }

                Thread.Sleep(100);
            }
        }
        #endregion

        #region private function
        private bool AchieveLimit(int axis)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            //DML[DML2Axis[axis]].UpdateMotionStatus(lineNo: line, devNo: dev_no);  //已經由Thread更新

            bool PEL = DML[DML2Axis[axis]].GetMotionStatus(lineNo: line, devNo: dev_no, state: (int)MOTION_IO.PEL);
            bool MEL = DML[DML2Axis[axis]].GetMotionStatus(lineNo: line, devNo: dev_no, state: (int)MOTION_IO.MEL);

            return PEL || MEL;
        }
        private bool AlarmStatus(int axis)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            bool ALM = DML[DML2Axis[axis]].GetMotionStatus(lineNo: line, devNo: dev_no, state: (int)MOTION_IO.ALM);

            return ALM;
        }
        private bool SetOrigin(int axis, double pos)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            int res = DML[DML2Axis[axis]].SetPosition(lineNo: line, devNo: dev_no, pos: pos);

            if (res == 0)
                return true;
            else
                return false;
        }
        private async Task<bool> WaitForMotionCompleteAsync(int axis, int timeoutMs = 60000 * 5)
        {
            int elapsed = 0;
            const int interval = 20;

            while (elapsed < timeoutMs)
            {
                if (Get_Motion_Complete(axis))
                    return true;

                await Task.Delay(interval);
                elapsed += interval;
            }

            return false;
        }
        private async Task<bool> WaitAchieveLimitAsync(int axis, int timeoutMs = 60000 * 5)
        {
            int elapsed = 0;
            const int interval = 20;

            while (elapsed < timeoutMs)
            {
                if (AchieveLimit(axis))
                    return true;

                byte line_no = (byte)DML_INFO[axis].LINE_NO;
                byte dev_no = (byte)DML_INFO[axis].DEV_NO;
                if (DML[DML2Axis[axis]].GetMotionStatus(lineNo: line_no,devNo: dev_no,state:(int)MOTION_IO.ALM))
                {
                    return false;
                }

                await Task.Delay(interval);
                elapsed += interval;
            }

            return false;
        }
        private async Task<bool> GoHome_FSM(int axis, int timeoutMs = 60000 * 5)
        {
            int elapsed = 0;
            int res = 0;
            int delay = 0;
            int enter_cpunt = 0;
            const int interval = 20;

            WORK state = WORK.INITIAL;

            while (elapsed < timeoutMs)
            {
                if (AlarmStatus(axis) == true)
                    return false;
                
                switch(state)
                {
                    case WORK.INITIAL:
                        {
                            DML[DML2Axis[axis]].SetMotionConfig(DML_INFO[axis], DML_INFO[axis].DEV_NO);
                            state = WORK.SERVO_ON;
                        }
                        break;
                    case WORK.SERVO_ON:
                        {
                            if (SetServo(axis, true))
                                state = WORK.GO_HOME_FIRST;
                            else
                                return false;
                        }
                        break;
                    case WORK.GO_HOME_FIRST:
                        {
                            res = DML[DML2Axis[axis]].ContinuousMove(DML_INFO[axis].DEV_NO, 
                                                                        DML_INFO[axis].DIRECTION, 
                                                                        DML_INFO[axis].HOME_ACC_1ST, 
                                                                        DML_INFO[axis].HOME_DEC_1ST, 
                                                                        DML_INFO[axis].MAX_VELOCITY_1ST);

                            if (res != 0)
                            {
                                Tool.SaveLogToFile("FAIL:GO_HOME_FIRST");
                                return false;
                            }
                            else
                                state = WORK.GO_HOME_FIRST_SHIFT;
                        }
                        break;
                    case WORK.GO_HOME_FIRST_SHIFT:
                        {
                            if (!AchieveLimit(axis) || enter_cpunt == 0)
                            {
                                delay = Tool.GetCurrentTickCount();
                                enter_cpunt++;
                                break;
                            }
                                
                            if(Tool.CheckTimeOverSec(delay, 1)/* && Get_Motion_Complete(axis) ==true*/)
                            {
                                res = DML[DML2Axis[axis]].RelativeSMove(DML_INFO[axis].DEV_NO,
                                                                            DML_INFO[axis].HOME_OFFSET_1ST,
                                                                            DML_INFO[axis].SLOW_MAX_SPEED,
                                                                            DML_INFO[axis].SLOW_INIT_SPEED,
                                                                            DML_INFO[axis].SLOW_ACC,
                                                                            DML_INFO[axis].SLOW_Sfac,
                                                                            DML_INFO[axis].SLOW_DEC,
                                                                            DML_INFO[axis].SLOW_Sfac);

                                if (res != 0)
                                {
                                    Tool.SaveLogToFile("FAIL:GO_HOME_FIRST_SHIFT");
                                    return false;
                                }
                                else
                                    state = WORK.GO_HOME_SECOND;
                            }
                        }
                        break;
                    case WORK.GO_HOME_SECOND:
                        {
                            if (Get_Motion_Complete(axis) == false)
                                break;
                            
                            //res = DML[DML2Axis[axis]].GoHome(lineNo: (byte)DML_INFO[axis].LINE_NO, devNo: (byte)DML_INFO[axis].DEV_NO, count:2);
                            res = DML[DML2Axis[axis]].ContinuousMove(DML_INFO[axis].DEV_NO,
                                                                        DML_INFO[axis].DIRECTION,
                                                                        DML_INFO[axis].HOME_ACC_2ND,
                                                                        DML_INFO[axis].HOME_DEC_2ND,
                                                                        DML_INFO[axis].MAX_VELOCITY_2ND);

                            enter_cpunt = 0;

                            if (res != 0)
                            {
                                Tool.SaveLogToFile("FAIL:GO_HOME_SECOND");
                                return false;
                            }
                            else
                                state = WORK.GO_HOME_SECOND_SHIFT;
                        }
                        break;
                    case WORK.GO_HOME_SECOND_SHIFT:
                        {
                            if (!AchieveLimit(axis) || enter_cpunt == 0)
                            {
                                delay = Tool.GetCurrentTickCount();
                                enter_cpunt++;
                                break;
                            }

                            if (Tool.CheckTimeOverSec(delay, 1)/* && Get_Motion_Complete(axis) == true*/)
                            {
                                SetOrigin(axis, DML_INFO[axis].HOME_POS);

                                //Tool.ResetTimeCount(out CycleTime);

                                res = DML[DML2Axis[axis]].RelativeSMove(DML_INFO[axis].DEV_NO,
                                                                            DML_INFO[axis].HOME_OFFSET_2ND,
                                                                            DML_INFO[axis].HOME_OFFSET_VELOCITY_2ND,
                                                                            DML_INFO[axis].FAST_INIT_SPEED,
                                                                            DML_INFO[axis].HOME_ACC_2ND,
                                                                            DML_INFO[axis].FAST_Sfac,
                                                                            DML_INFO[axis].HOME_DEC_2ND,
                                                                            DML_INFO[axis].FAST_Sfac);

                                if (res != 0)
                                {
                                    Tool.SaveLogToFile("FAIL:GO_HOME_SECOND_SHIFT");
                                    return false;
                                }
                                else
                                    state = WORK.WAIT_GO_HOME_SECOND_SHIFT;
                            }
                        }
                        break;
                    case WORK.WAIT_GO_HOME_SECOND_SHIFT:
                        {
                            if (Get_Motion_Complete(axis) == false)
                                break;

                            //Tool.SaveLogToFile($"AxisMoveTime:{Tool.GetTime(CycleTime)}");

                            return true;
                        }
                        break;
                }

                await Task.Delay(interval);
                elapsed += interval;
            }

            return true;
        }
        private void LoadMotionConfig(string path)
        {
            XDocument doc = XDocument.Load(path);

            // 直接讀出所有 Axis
            foreach (var axis in doc.Descendants("Axis"))
            {
                string name = (string)axis.Attribute("name");

                // 讀出每個 Parameter
                foreach (var param in axis.Elements("Parameter"))
                {
                    string key = (string)param.Attribute("key");
                    string value = (string)param.Attribute("value");

                    Project2AxisInfo(int.Parse(name.Replace("Axis", "")), key, value);
                }
            }
        }
        private void InitialAxsInfo()
        {
            AXIS_INFO aXIS_INFO = new AXIS_INFO();
            
            int count = Enum.GetNames(typeof(AXIS_NAME)).Length;
            
            for (int i=0; i< count; i++)
            {
                aXIS_INFO.AXIS_USE = 0;
                DML_INFO.Add(aXIS_INFO);
            }
        }
        private void Project2AxisInfo(int axis, string item, string value)
        {
            //新增軸參數時需添加

            var info = DML_INFO[axis];

            //[Axis Configuration]
            if (item == eF_AxisSetting.Cmbx_AxisType.ToString())
                info.AXIS_TYPE = value;
            else if (item == eF_AxisSetting.TxtBx_LineNo.ToString())
                info.LINE_NO = Tool.StringToInt(value);
            else if (item == eF_AxisSetting.TxtBx_AxisStation.ToString())
                info.DEV_NO = Tool.StringToInt(value);
            else if (item == eF_AxisSetting.Cmbx_AxisUse.ToString())
                info.AXIS_USE = Tool.StringToInt(value);
            else if (item == eF_AxisSetting.Cmbx_AxisLimitLogic.ToString())
                info.LIMIT_LOGIC = Tool.StringToInt(value);
            else if (item == eF_AxisSetting.Cmbx_AxisLimitStopMode.ToString())
                info.STOP_MODE = Tool.StringToInt(value);
            else if (item == eF_AxisSetting.TxtBx_DriverResolution.ToString())
                info.DRIVER_RESOLUTION = Tool.StringToInt(value);

            //[Hardware Configuration]
            else if (item == eF_AxisSetting.TxtBx_AxisPitch.ToString())
                info.PITCH = Tool.StringToDouble(value);

            //[Software Configuration]
            else if (item == eF_AxisSetting.TxtBx_AxisName.ToString())
                info.AXIS_NANE = value;
            else if (item == eF_AxisSetting.Cmbx_UseSoftLimit.ToString())
                info.SW_LIMIT = Tool.StringToInt(value);
            else if (item == eF_AxisSetting.TxtBx_SoftPEL.ToString())
                info.PEL_POS = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_SoftMEL.ToString())
                info.MEL_POS = Tool.StringToDouble(value);

            //[Speed Configuration]
            else if (item == eF_AxisSetting.TxtBx_FastMaxVelocity.ToString())
                info.FAST_MAX_SPEED = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_FastInitVelocity.ToString())
                info.FAST_INIT_SPEED = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_Fast_ACC.ToString())
                info.FAST_ACC = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_Fast_DEC.ToString())
                info.FAST_DEC = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_FastSfac.ToString())
                info.FAST_Sfac = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_SlowMaxVelocity.ToString())
                info.SLOW_MAX_SPEED = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_SlowInitVelocity.ToString())
                info.SLOW_INIT_SPEED = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_Slow_ACC.ToString())
                info.SLOW_ACC = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_Slow_DEC.ToString())
                info.SLOW_DEC = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_SlowSfac.ToString())
                info.SLOW_Sfac = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_NormalMaxVelocity.ToString())
                info.NORMAL_MAX_SPEED = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_NormalInitVelocity.ToString())
                info.NORMAL_INIT_SPEED = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_Normal_ACC.ToString())
                info.NORMAL_ACC = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_Normal_DEC.ToString())
                info.NORMAL_DEC = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_NormalSfac.ToString())
                info.NORMAL_Sfac = Tool.StringToDouble(value);

            //[Home Configuration]
            else if(item == eF_AxisSetting.Cmbx_HomeDirection.ToString())
                info.DIRECTION = Tool.StringToInt(value);
            else if (item == eF_AxisSetting.TxtBx_ORGPosition.ToString())
                info.HOME_POS = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_1stHomeVelocity.ToString())
                info.MAX_VELOCITY_1ST = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_1stHomeAcc.ToString())
                info.HOME_ACC_1ST = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_1stHomeDec.ToString())
                info.HOME_DEC_1ST = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_1stORGOffset.ToString())
                info.HOME_OFFSET_1ST = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_2ndHomeVelocity.ToString())
                info.MAX_VELOCITY_2ND = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_2ndHomeAcc.ToString())
                info.HOME_ACC_2ND = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_2ndHomeDec.ToString())
                info.HOME_DEC_2ND = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_2ndHomeOffsetVelocity.ToString())
                info.HOME_OFFSET_VELOCITY_2ND = Tool.StringToDouble(value);
            else if (item == eF_AxisSetting.TxtBx_2ndORGOffset.ToString())
                info.HOME_OFFSET_2ND = Tool.StringToDouble(value);

            DML_INFO[axis] = info;
        }
        private string GetAxisType(int index)
        {
            if (index == 0)
                return "None";
            else if (index == 1)
                return "Virtual";
            else if (index == 2)
                return "APS";
            else
                return "None";
        }
        private bool CheckAxisEnable(int axis)
        {             
            if (DML_INFO[axis].AXIS_USE == 0 || DML2Axis[axis] == -1)
                return false;
            else
                return true;
        }
        #endregion

        #region public function
        // Initial Function
        public bool Initial_All_Motion()
        {
            //此函式程式開啟後只能呼叫一次
            InitialAxsInfo();

            foreach (IMotionCard card in Cards)
            {
                if (card.Open() == true)
                    DML.Add(card);
            }

            Task task = Task.Run(() => Process());

            return true;
        }
        public void BindingAxis()
        {
            DML2Axis = new int[DML_INFO.Count];
            DML_Home_Complete = new bool[DML_INFO.Count];
            DML_Homing = new bool[DML_INFO.Count];
            //GO_HOME_PARAM = new HOME_INFO[DML_INFO.Count];

            Dictionary<string, int> nameToIndex = new Dictionary<string, int>();

            for (int j = 0; j < DML.Count; j++)
            {
                string name = "";

                if (DML[j].GetName() == "AMP_204C" || DML[j].GetName() == "PCIE_8332")
                    name = "APS";
                else
                    name = DML[j].GetName();

                nameToIndex[name] = j;
            }

            for (int i = 0; i < DML_INFO.Count; i++)
            {
                DML2Axis[i] = -1;

                if (DML_INFO[i].AXIS_TYPE == null)
                    continue;

                string axis_type = GetAxisType(int.Parse(DML_INFO[i].AXIS_TYPE));

                if (nameToIndex.TryGetValue(axis_type, out int idx))
                {
                    DML2Axis[i] = idx;
                }
            }
        }

        //[Set Parameter & Status Function]
        public bool SetServo(int axis, bool on_off)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            bool res = DML[DML2Axis[axis]].Servo_ONOff(lineNo: line, devNo: dev_no, flag: on_off);

            return res;
        }
        public bool SetSingleMoveMode(SINGLE_MOVE_MODE mode)
        {
            SingleMoveMode = mode;
            return true;
        }

        //[Status Function]
        public double GetPosition(int axis)
        {
            if (!CheckAxisEnable(axis))
                return 0.000;

            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            double res = DML[DML2Axis[axis]].GetPosition(lineNo: line, devNo: dev_no);

            return res;
        }
        public void GetMotionStatus(int axis, out bool[] status)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            int start = (int)MOTION_IO.ALM;
            int end = (int)MOTION_IO.RDY;

            bool [] bstatus = new bool[end - start + 1];

            if (!CheckAxisEnable(axis))
            {
                status = bstatus;
                return;
            }

            for (int i = start; i<=end; i++)
            {
                bstatus[i - start] = DML[DML2Axis[axis]].GetMotionStatus(lineNo: line, devNo: dev_no, state: i);
            }

            status = bstatus;
        }
        public SINGLE_MOVE_MODE GetSingleMoveMode()
        {
            return SingleMoveMode;
        }

        //[Home Function]
        public async Task<bool> GoHome(int axis)
        {
            if (!CheckAxisEnable(axis))
            {
                Tool.SaveLogToFile($"軸:{axis}({DML_INFO[axis].AXIS_NANE}) 未使用，無法執行初始化", level:"WRN");
                return false;
            }
            
            if(DML_Homing[axis] == true)
            {
                Tool.SaveLogToFile($"軸:{axis}({DML_INFO[axis].AXIS_NANE}) 正在執行初始化，請勿重複執行", level: "WRN");
                return false;
            }

            DML_Home_Complete[axis] = false;
            DML_Homing[axis] = true;

            Tool.SaveLogToFile($"軸:{axis}({DML_INFO[axis].AXIS_NANE}) 開始初始化");
            bool ok = await GoHome_FSM(axis, 15000);

            if (!ok)
            {
                Tool.SaveLogToFile($"軸:{axis}({DML_INFO[axis].AXIS_NANE}) 初始化未完成", level: "ERR");
                DML_Homing[axis] = false;
                return false;
            }

            DML_Home_Complete[axis] = true;
            DML_Homing[axis] = false;

            Tool.SaveLogToFile($"軸:{axis}({DML_INFO[axis].AXIS_NANE}) 初始化完成");

            return true;
        }
        public bool Get_Home_Complete(int axis)
        {
            return DML_Home_Complete[axis];
        }

        //[Move Function]
        public bool Get_Motion_Complete(int axis)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            bool res = DML[DML2Axis[axis]].GetMotionComplete(lineNo: line, devNo: dev_no);

            if(res == true)
            {
                int ret = DML[DML2Axis[axis]].UpdateMotionStatus(lineNo: line, devNo: dev_no);

                if (ret != 0)
                    return false;

                bool check_INP = DML[DML2Axis[axis]].GetMotionStatus(lineNo: line, devNo: dev_no, state: (int)MOTION_IO.INP);

                return check_INP;
            }

            return res;
        }
        public bool PTP_Move(int axis , double pos, string mode = "Abs", MOVE_VELOCITY_MODE velocityMode = MOVE_VELOCITY_MODE.NORMAL)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            double velocity_max = 0, velocity_start = 0, acc = 0, dec = 0, sfac = 0;

            if (velocityMode == MOVE_VELOCITY_MODE.SLOW)
            {
                velocity_max = DML_INFO[axis].SLOW_MAX_SPEED;
                velocity_start = DML_INFO[axis].SLOW_INIT_SPEED;
                acc = DML_INFO[axis].SLOW_ACC;
                dec = DML_INFO[axis].SLOW_DEC;
                sfac = DML_INFO[axis].SLOW_Sfac;
            }
            else if (velocityMode == MOVE_VELOCITY_MODE.NORMAL)
            {
                velocity_max = DML_INFO[axis].NORMAL_MAX_SPEED;
                velocity_start = DML_INFO[axis].NORMAL_INIT_SPEED;
                acc = DML_INFO[axis].NORMAL_ACC;
                dec = DML_INFO[axis].NORMAL_DEC;
                sfac = DML_INFO[axis].NORMAL_Sfac;
            }
            else if (velocityMode == MOVE_VELOCITY_MODE.FAST)
            {
                velocity_max = DML_INFO[axis].FAST_MAX_SPEED;
                velocity_start = DML_INFO[axis].FAST_INIT_SPEED;
                acc = DML_INFO[axis].FAST_ACC;
                dec = DML_INFO[axis].FAST_DEC;
                sfac = DML_INFO[axis].FAST_Sfac;
            }

            //DML[DML2Axis[axis]].SetMotionConfig(DML_INFO[axis], axis);

            int res = 0;
            if (mode == "Abs")
            {
                res = DML[DML2Axis[axis]].AbsoluteSMove(dev_no, pos, velocity_max, velocity_start, acc, sfac, dec, 0);
                Tool.SaveLogToFile($"軸:{axis}({DML_INFO[axis].AXIS_NANE})=>{pos}");
            }
            else if(mode == "Rel")
            {
                res = DML[DML2Axis[axis]].RelativeSMove(dev_no, pos, velocity_max, velocity_start, acc, sfac, dec, 0);

                double new_pos = DML[DML2Axis[axis]].GetPosition(devNo: dev_no) + pos;
                Tool.SaveLogToFile($"軸:{axis}({DML_INFO[axis].AXIS_NANE})=>{new_pos}");
            }

            if(res != 0)
            {
                Tool.SaveLogToFile($"軸:{axis} PTP_Move失敗", level: "ERR");
                return false;
            }

            return true;
        }
        public bool SingleMove(int axis, int dir, double pos)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            double velocity_max = 0, velocity_start = 0, acc = 0, dec = 0, sfac = 0;

            if (SingleMoveMode == SINGLE_MOVE_MODE.CONTINUOUS_SLOW)
            {
                velocity_max = DML_INFO[axis].SLOW_MAX_SPEED;
                acc = DML_INFO[axis].SLOW_ACC;
                dec = DML_INFO[axis].SLOW_DEC;
            }
            else if (SingleMoveMode == SINGLE_MOVE_MODE.CONTINUOUS_NORMAL || 
                     SingleMoveMode == SINGLE_MOVE_MODE.INDEX)
            {
                velocity_max = DML_INFO[axis].NORMAL_MAX_SPEED;
                acc = DML_INFO[axis].NORMAL_ACC;
                dec = DML_INFO[axis].NORMAL_DEC;
            }
            else if (SingleMoveMode == SINGLE_MOVE_MODE.CONTINUOUS_FAST)
            {
                velocity_max = DML_INFO[axis].FAST_MAX_SPEED;
                acc = DML_INFO[axis].FAST_ACC;
                dec = DML_INFO[axis].FAST_DEC;
            }

            int res = 0;
            if (SingleMoveMode == SINGLE_MOVE_MODE.INDEX)
            {
                res = DML[DML2Axis[axis]].RelativeSMove(dev_no, pos, velocity_max, velocity_start, acc, sfac, dec, sfac);

                double new_pos = DML[DML2Axis[axis]].GetPosition(devNo: dev_no) + pos;
                Tool.SaveLogToFile($"軸:{axis}({DML_INFO[axis].AXIS_NANE})=>{new_pos}");
            }
            else
            {
                res = DML[DML2Axis[axis]].ContinuousMove(dev_no, dir, acc, dec, velocity_max);
            }

            if (res != 0)
            {
                Tool.SaveLogToFile($"軸:{axis} Single_Move失敗", level: "ERR");
                return false;
            }

            return true;
        }
        public bool StopAxisMove(int axis)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            double dec = 0;

            if (SingleMoveMode == SINGLE_MOVE_MODE.CONTINUOUS_SLOW)
                dec = DML_INFO[axis].SLOW_DEC;
            else if (SingleMoveMode == SINGLE_MOVE_MODE.CONTINUOUS_NORMAL)
                dec = DML_INFO[axis].NORMAL_DEC;
            else if (SingleMoveMode == SINGLE_MOVE_MODE.CONTINUOUS_FAST)
                dec = DML_INFO[axis].FAST_DEC;

            int res = 0;
            res = DML[DML2Axis[axis]].Stop(devNo: dev_no, Tdec: dec);

            if (res != 0)
            {
                Tool.SaveLogToFile($"軸:{axis} Stop_Move失敗", level: "ERR");
                return false;
            }

            return true;
        }

        //[Read&Save Axis Information]
        public void SaveAxisConfig(string filePath, string axisName, Dictionary<string, string> parameters)
        {
            XDocument doc;

            if (File.Exists(filePath))
                doc = XDocument.Load(filePath);
            else
                doc = new XDocument(new XElement("MachineConfig"));

            XElement root = doc.Element("MachineConfig");

            // 找出 Axis 節點
            var existingAxis = root.Elements("Axis")
                                   .FirstOrDefault(x => (string)x.Attribute("name") == axisName);

            if (existingAxis != null)
            {
                // 清空舊的參數
                existingAxis.RemoveNodes();
            }
            else
            {
                existingAxis = new XElement("Axis", new XAttribute("name", axisName));
                root.Add(existingAxis);
            }

            // 新增參數
            foreach (var kvp in parameters)
            {
                existingAxis.Add(new XElement("Parameter",
                    new XAttribute("key", kvp.Key),
                    new XAttribute("value", kvp.Value)
                ));
            }

            doc.Save(filePath);
        }
        public bool LoadAxisConfig()
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            string load_path = AppPath + @"\Setting\AxisConfig.xml";

            if (!File.Exists(load_path))
                return false;

            LoadMotionConfig(load_path);

            return true;
        }
        public IReadOnlyList<AXIS_INFO> GetAxisConfig()
        {
            return DML_INFO.AsReadOnly();
        }
        #endregion
    }
}
