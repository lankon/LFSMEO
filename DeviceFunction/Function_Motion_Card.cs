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
        private bool[] DML_Home_Complete;       //判斷各軸是否完成Home
        private bool[] DML_Homing;              //判斷各軸是否正在執行Home
        private readonly IEnumerable<IMotionCard> Cards;

        public enum AXIS_NAME
        {
            AXIS_X,
            AXIS_Y,
            AXIS_Z,
            AXIS_A,
            AXIS_AX,
            AXIS_AY,
            AXIS_AZ,
            AXIS_AA,
            AXIS_EX,
            AXIS_EY,
            AXIS_EZ,
            AXIS_EA,
            AXIS_IX,
            AXIS_IY,
            AXIS_IZ,
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

                Thread.Sleep(200);
            }
        }
        #endregion

        #region private function
        private bool OpenMotionCard(IMotionCard motion, string name)
        {
            if (motion.Open() == true)
            {
                DML.Add(motion);
                return true;
            }
            else
            {
                Tool.SaveLogToFile($"{name}開卡失敗");
                return false;
            }
        }
        private bool AchieveLimit(int axis)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            DML[DML2Axis[axis]].UpdateMotionStatus(lineNo: line, devNo: dev_no);

            bool PEL = DML[DML2Axis[axis]].GetMotionStatus(lineNo: line, devNo: dev_no, state: (int)MOTION_IO.PEL);
            bool MEL = DML[DML2Axis[axis]].GetMotionStatus(lineNo: line, devNo: dev_no, state: (int)MOTION_IO.MEL);

            return PEL || MEL;
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

            //[Software Configuration]
            else if (item == eF_AxisSetting.TxtBx_AxisName.ToString())
                info.AXIS_NANE = value;

            //[Home Configuration]
            else if (item == eF_AxisSetting.Cmbx_HomeMode.ToString())
                info.MODE = Tool.StringToInt(value);
            else if (item == eF_AxisSetting.Cmbx_HomeDirection.ToString())
                info.DIRECTION = Tool.StringToInt(value);
            else if (item == eF_AxisSetting.TxtBx_ORGPosition.ToString())
                info.HOME_POS = Tool.StringToInt(value);
            else if (item == eF_AxisSetting.TxtBx_ORGShiftPosition.ToString())
                info.HOME_SHIFT = Tool.StringToInt(value);
            else if (item == eF_AxisSetting.TxtBx_HomeVelocity.ToString())
                info.MAX_VELOCITY = Tool.StringToInt(value);
            else if (item == eF_AxisSetting.TxtBx_ORGVelocity.ToString())
                info.HOEM_FIND_ORG_VELOCITY = Tool.StringToInt(value);
            else if (item == eF_AxisSetting.TxtBx_HomeAcc.ToString())
                info.HOME_ACC = Tool.StringToInt(value);

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
        public void SetAxis(AXIS_INFO MF)
        {
            DML_INFO.Add(MF);
        }
        public void ClearAxis()
        {
            DML_INFO.Clear();
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
                if (DML_INFO[i].AXIS_TYPE == null)
                    continue;

                string axis_type = GetAxisType(int.Parse(DML_INFO[i].AXIS_TYPE));

                if (nameToIndex.TryGetValue(axis_type, out int idx))
                {
                    DML2Axis[i] = idx;
                }

                
            }
        }

        // Set Parameter & Status Function
        public bool SetServo(int axis, bool on_off)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            bool res = DML[DML2Axis[axis]].Servo_ONOff(lineNo: line, devNo: dev_no, flag: on_off);

            return res;
        }
        public int SetSpeedConfig()
        {



            return 0;
        }
        //public int SetHomeConfig(int axis, HOME_INFO info)
        //{
        //    DML[DML2Axis[axis]].SetGoHomeParam()

        //    return 0;
        //}


        // Position Function
        public double GetPosition(int axis)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            double res = DML[DML2Axis[axis]].GetPosition(lineNo: line, devNo: dev_no);

            return res;
        }


        //[Home Function]
        public async Task<bool> GoHome(int axis)
        {
            if (DML_INFO[axis].AXIS_USE == 0)
            {
                Tool.SaveLogToFile($"軸 = {axis}({DML_INFO[axis].AXIS_NANE}) 未使用，無法執行初始化", level:"WRN");
                return false;
            }
            
            if(DML_Homing[axis] == true)
            {
                Tool.SaveLogToFile($"軸 = {axis}({DML_INFO[axis].AXIS_NANE}) 正在執行初始化，請勿重複執行", level: "WRN");
                return false;
            }

            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            DML_Home_Complete[axis] = false;
            DML_Homing[axis] = true;

            DML[DML2Axis[axis]].SetMotionConfig(DML_INFO[axis]);
            DML[DML2Axis[axis]].Servo_ONOff(lineNo: line, devNo: dev_no, flag: true);
            DML[DML2Axis[axis]].GoHome(lineNo:line, devNo:dev_no);

            bool ok = await WaitAchieveLimitAsync(axis, 15000);

            if(!ok)
            {
                Tool.SaveLogToFile($"軸 = {axis}({DML_INFO[axis].AXIS_NANE}) 初始化未完成", level: "ERR");
                DML_Homing[axis] = false;
                return false;
            }

            //設定原點位置
            SetOrigin(axis, DML_INFO[axis].HOME_POS);

            //到達原點後位移

            DML_Home_Complete[axis] = true;
            DML_Homing[axis] = false;

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

            return res;
        }
        public bool PTP_Move(int axis , double pos, string mode = "Abs", MOVE_VELOCITY_MODE velocityMode = MOVE_VELOCITY_MODE.NORMAL)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            double velocity_max = 0, velocity_start = 0, acc = 0, dec = 0, sfac = 0;
            if (velocityMode == MOVE_VELOCITY_MODE.NORMAL)
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

            DML[DML2Axis[axis]].SetMotionConfig(DML_INFO[axis]);

            int res = 0;
            if (mode == "Abs")
                res = DML[DML2Axis[axis]].AbsoluteSMove(axis, pos, velocity_max, velocity_start, acc, sfac, dec, 0);
            else if(mode == "Rel")
                res = DML[DML2Axis[axis]].RelativeSMove(axis, pos, velocity_max, velocity_start, acc, sfac, dec, 0);

            if(res != 0)
            {
                Tool.SaveLogToFile($"軸:{axis} PTP_Move失敗", level: "ERR");
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

        public bool Jog_Start(int axis, string direction, MOVE_VELOCITY_MODE velocityMode = MOVE_VELOCITY_MODE.NORMAL)
        {
            throw new NotImplementedException();
        }

        public bool Jog_Stop(int axis)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
