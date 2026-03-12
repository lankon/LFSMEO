using DeviceCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ToolFunction;

namespace DeviceFunction
{
    public class Function_Camera: IFunction_Camera
    {
        public Function_Camera(IEnumerable<ICamera> ccd)
        {
            Camera = ccd;
        }

        public event EventHandler<ImageReadyEventArgs> OnImageUpdated;

        #region parameter define
        private IEnumerable<ICamera> Camera;
        private int[] CCD_Info2List;
        private List<ICamera> CameraList = new List<ICamera>();
        private List<CAMERA_INFO> CCD_INFO = new List<CAMERA_INFO>();
        #endregion

        #region private function
        private void LoadCameraConfig(string path)
        {
            XDocument doc = XDocument.Load(path);

            // 直接讀出所有Camera
            foreach (var ccd in doc.Descendants("Camera"))
            {
                string name = (string)ccd.Attribute("name");

                // 讀出每個 Parameter
                foreach (var param in ccd.Elements("Parameter"))
                {
                    string key = (string)param.Attribute("key");
                    string value = (string)param.Attribute("value");

                    Project2CameraInfo(int.Parse(name.Replace("Camera", "")), key, value);
                }  
            }
        }
        private void InitialCameraInfo()
        {
            CAMERA_INFO ccd_info = new CAMERA_INFO();
            
            int count = Enum.GetNames(typeof(CCD_NAME)).Length;

            for (int i = 0; i < count; i++)
            {
                ccd_info.CCD_USE = 0;
                CCD_INFO.Add(ccd_info);
            }
        }
        private void Project2CameraInfo(int camera, string item, string value)
        {
            //新增相機參數時需添加
            var info = CCD_INFO[camera];

            //[Connect Configuration]
            if (item == eF_CameraSetting.Cmbx_AxisType.ToString())
                info.CCD_TYPE = Tool.StringToInt(value);
            else if (item == eF_CameraSetting.TxtBx_ID_IP.ToString())
                info.IP_ID = value;
            else if (item == eF_CameraSetting.TxtBx_CCD_Name.ToString())
                info.CCD_NAME = value;
            else if (item == eF_CameraSetting.Cmbx_AxisUse.ToString())
                info.CCD_USE = Tool.StringToInt(value);

            CCD_INFO[camera] = info;
        }
        private string GetCameraType(int index)
        {
            if (index == 0)
                return "None";
            else if (index == 1)
                return "Virtual";
            else if (index == 2)
                return "Hikvision";
            else
                return "None";
        }
        private bool CheckCameraEnable(int ccd)
        {
            if (CCD_INFO[ccd].CCD_USE == 0 || CCD_Info2List[ccd] == -1)
                return false;
            else
                return true;
        }
        #endregion

        #region public function
        // [Initial]
        public int Initial_All_Camera()
        {
            //此函式程式開啟後只能呼叫一次
            InitialCameraInfo();

            foreach (ICamera camera in Camera)
            {
                if (camera.Connect() == 0)
                    CameraList.Add(camera);
            }

            return 0;
        }

        public void BindingCamera()
        {
            CCD_Info2List = new int[CCD_INFO.Count];

            Dictionary<string, int> nameToIndex = new Dictionary<string, int>();

            for (int j = 0; j < CameraList.Count; j++)
            {
                string name = CameraList[j].GetCameraType().ToString();

                nameToIndex[name] = j;
            }

            for (int i = 0; i < CCD_INFO.Count; i++)
            {
                CCD_Info2List[i] = -1;

                if (CCD_INFO[i].CCD_TYPE < 0)
                    continue;

                string ccd_type = GetCameraType(CCD_INFO[i].CCD_TYPE);

                if (nameToIndex.TryGetValue(ccd_type, out int idx))
                {
                    CCD_Info2List[i] = idx;
                }
            }
        }




        public bool StartGrab(int ccd)
        {
            if (!CheckCameraEnable(ccd))
                return false;

            string ip = CCD_INFO[ccd].IP_ID;
            int ret = CameraList[CCD_Info2List[ccd]].StartGrabbing(ip);

            if(ret == 0)
                return true;
            else
                return false;
        }

        public bool StopGrab(int ccd)
        {
            if (!CheckCameraEnable(ccd))
                return false;

            string ip = CCD_INFO[ccd].IP_ID;
            int ret = CameraList[CCD_Info2List[ccd]].StopGrabbing(ip);

            if(ret == 0)
                return true;
            else
                return false;
        }

        public bool SoftTrigger(int ccd)
        {
            if (!CheckCameraEnable(ccd))
                return false;

            string ip = CCD_INFO[ccd].IP_ID;
            int ret = CameraList[CCD_Info2List[ccd]].SoftwareTrigger(ip);

            if(ret == 0)
                return true;
            else
                return false;
        }

        public bool GetImageDisplay(int ccd)
        {
            if (!CheckCameraEnable(ccd))
                return false;

            string ip = CCD_INFO[ccd].IP_ID;
            IntPtr image = IntPtr.Zero;
            int width = 0;
            int height = 0;
            int ret = CameraList[CCD_Info2List[ccd]].GetImage(ip, ref image, ref width, ref height);

            if(ret == 0)
            {
                OnImageUpdated?.Invoke(this, new ImageReadyEventArgs
                {
                    ImageData = image,
                    Width = width,
                    Height = height,
                    Format = IMAGE_FORMAT.MONO8
                });

                return true;
            }

            return false;
        }

        public bool StopGrabAllCamera()
        {

            return false;
        }

        //[Read&Save Camera Information]
        public void SaveCameraConfig(string filePath, string axisName, Dictionary<string, string> parameters)
        {
            XDocument doc;

            if (File.Exists(filePath))
                doc = XDocument.Load(filePath);
            else
                doc = new XDocument(new XElement("CameraConfig"));

            XElement root = doc.Element("CameraConfig");

            // 找出 Axis 節點
            var existingAxis = root.Elements("Camera")
                                   .FirstOrDefault(x => (string)x.Attribute("name") == axisName);

            if (existingAxis != null)
            {
                // 清空舊的參數
                existingAxis.RemoveNodes();
            }
            else
            {
                existingAxis = new XElement("Camera", new XAttribute("name", axisName));
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
        public bool LoadCameraConfig()
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            string load_path = AppPath + @"\Setting\CameraConfig.xml";

            if (!File.Exists(load_path))
                return false;

            LoadCameraConfig(load_path);

            return true;
        }
        public IReadOnlyList<CAMERA_INFO> GetCameraConfig()
        {
            return CCD_INFO.AsReadOnly();
        }
        #endregion

    }
}
