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
        #region parameter define
        private List<IMotionCard> DML = new List<IMotionCard>();
        private List<AXIS_INFO> DML_INFO = new List<AXIS_INFO>();
        private int[] DML2Axis;
        private bool[] DML_Home_Complete;
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
                    if (DML[k].GetName() == "MN200")
                    {
                        //List<byte> LineNo = DML[k].Get_Motion_LineNo();
                        //List<byte> DevNo = DML[k].Get_Motion_DevNo();

                        //for (byte i = 0; i < LineNo.Count; i++)
                        //{
                        //    DML[k].UpdateMotionStatus(lineNo: LineNo[i], devNo: DevNo[i]);
                        //}
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

                await Task.Delay(interval);
                elapsed += interval;
            }

            return false;
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
            var info = DML_INFO[axis];

            //[Axis Configuration]
            //if (item == eOEMSetting.TxtBx_AxisType.ToString())
            //    info.AXIS_TYPE = value;
            //else if(item == eOEMSetting.TxtBx_LineNo.ToString())
            //    info.LINE_NO = Tool.StringToInt(value);
            //else if (item == eOEMSetting.TxtBx_AxisStation.ToString())
            //    info.DEV_NO = Tool.StringToInt(value);
            //else if (item == eOEMSetting.Cmbx_AxisUse.ToString())
            //    info.AXIS_USE = Tool.StringToInt(value);
            //else if (item == eOEMSetting.Cmbx_AxisLimitLogic.ToString())
            //    info.LIMIT_LOGIC = Tool.StringToInt(value);
            //else if (item == eOEMSetting.Cmbx_AxisLimitStopMode.ToString())
            //    info.STOP_MODE = Tool.StringToInt(value);
            ////[Software Configuration]
            //else if (item == eOEMSetting.TxtBx_AxisName.ToString())
            //    info.AXIS_NANE = value;
            //else if (item == eOEMSetting.Cmbx_SW_Limit.ToString())
            //    info.SW_LIMIT = Tool.StringToInt(value);
            //else if (item == eOEMSetting.TxtBx_SW_PEL_Pos.ToString())
            //    info.PEL_POS = Tool.StringToInt(value);
            //else if (item == eOEMSetting.TxtBx_SW_MEL_Pos.ToString())
            //    info.MEL_POS = Tool.StringToInt(value);
            //else if (item == eOEMSetting.Cmbx_ReverseMode.ToString())
            //    info.REVERSE_MOVE = Tool.StringToInt(value);
            ////[Home Configuration]
            //else if (item == eOEMSetting.Cmbx_HomeMode.ToString())
            //    info.MODE = Tool.StringToInt(value);
            //else if (item == eOEMSetting.Cmbx_HomeDirection.ToString())
            //    info.DIRECTION = Tool.StringToInt(value);
            //else if (item == eOEMSetting.TxtBx_ORGPosition.ToString())
            //    info.HOME_POS = Tool.StringToInt(value);
            //else if (item == eOEMSetting.TxtBx_ORGShiftPosition.ToString())
            //    info.HOME_SHIFT = Tool.StringToInt(value);
            //else if (item == eOEMSetting.TxtBx_HomeVelocity.ToString())
            //    info.MAX_VELOCITY = Tool.StringToInt(value);
            //else if (item == eOEMSetting.TxtBx_ORGVelocity.ToString())
            //    info.HOEM_FIND_ORG_VELOCITY = Tool.StringToInt(value);
            //else if (item == eOEMSetting.TxtBx_HomeAcc.ToString())
            //    info.ACC = Tool.StringToInt(value);
            
            DML_INFO[axis] = info;
        }
        #endregion

        Function_Motion_Card(IEnumerable<IMotionCard> cards)
        {
            Cards = cards;
        }

        #region public function
        // Initial Function
        public bool Initial_All_Motion()
        {
            bool Use_MN200, Use_APS;

            IMotionCard mN200 = null;//= new MN200();
            IMotionCard APS = null;// = new APS();

            Use_MN200 = OpenMotionCard(mN200, "MN200");
            Use_APS = OpenMotionCard(APS, "APS");

            if (!Use_MN200 && !Use_APS)    //沒有任何一張Motion卡
            {
                Tool.SaveLogToFile("Motion卡Initial失敗");
                return false;
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
            //GO_HOME_PARAM = new HOME_INFO[DML_INFO.Count];

            Dictionary<string, int> nameToIndex = new Dictionary<string, int>();

            for (int j = 0; j < DML.Count; j++)
            {
                nameToIndex[DML[j].GetName()] = j;
            }

            for (int i = 0; i < DML_INFO.Count; i++)
            {
                if (DML_INFO[i].AXIS_TYPE == null)
                    continue;

                if (nameToIndex.TryGetValue(DML_INFO[i].AXIS_TYPE, out int idx))
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


        // Home Function
        public async Task<bool> GoHome(int axis)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            DML_Home_Complete[axis] = false;
            DML[DML2Axis[axis]].GoHome(lineNo:line, devNo:dev_no);

            bool ok = await WaitAchieveLimitAsync(axis);

            if(!ok)
            {
                Tool.SaveLogToFile($"軸 = {axis} 初始化未完成");
            }

            //設定原點位置
            SetOrigin(axis, DML_INFO[axis].HOME_POS);

            //到達原點後位移

            return true;
        }
        public bool Get_Home_Complete(int axis)
        {
            return DML_Home_Complete[axis];
        }


        // Move Function
        public bool Get_Motion_Complete(int axis)
        {
            byte line = (byte)DML_INFO[axis].LINE_NO;
            byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            bool res = DML[DML2Axis[axis]].GetMotionComplete(lineNo: line, devNo: dev_no);

            return res;
        }
        public bool PTP_Move(int axis , double pos, string mode = "Abs")
        {
            //byte line = (byte)DML_INFO[axis].LINE_NO;
            //byte dev_no = (byte)DML_INFO[axis].DEV_NO;

            //if(mode == "Abs")
            //    DML[DML2Axis[axis]].AbsoluteSMove

            //bool res = DML[DML2Axis[axis]].AbsoluteSMove();




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
        public bool LoadAxisConfig(string load_path)
        {
            //string load_path = Application.StartupPath + @"\Setting\AxisConfig.xml";

            if (!File.Exists(load_path))
                return false;

            InitialAxsInfo();
            LoadMotionConfig(load_path);

            return true;
        }
        public List<AXIS_INFO> GetAxisConfig()
        {
            return DML_INFO;
        }
        #endregion
    }
}
