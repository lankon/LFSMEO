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


using DeviceCore;

namespace DeviceFunction
{
    public class Function_Camera: IFunction_Camera
    {
        

        public Function_Camera(IEnumerable<ICamera> spec)
        {
            Camera = spec;
        }

        public event EventHandler<ImageReadyEventArgs> OnImageUpdated;

        #region parameter define
        private bool IsInitial = false;
        private int DeviceIndex = -1;
        private IEnumerable<ICamera> Camera;
        private List<ICamera> CameraList = new List<ICamera>();
        private List<CAMERA_INFO> CCD_INFO = new List<CAMERA_INFO>();
        #endregion

        #region private function
        private void LoadCameraConfig(string path)
        {
            XDocument doc = XDocument.Load(path);

            // 直接讀出所有 Axis
            foreach (var axis in doc.Descendants("Camera"))
            {
                string name = (string)axis.Attribute("name");

                // 讀出每個 Parameter
                foreach (var param in axis.Elements("Parameter"))
                {
                    string key = (string)param.Attribute("key");
                    string value = (string)param.Attribute("value");

                    Project2AxisInfo(int.Parse(name.Replace("Camera", "")), key, value);
                }
            }
        }
        private void Project2AxisInfo(int axis, string item, string value)
        {
            //新增相機參數時需添加

            var info = CCD_INFO[axis];

            //[Connect Configuration]
            if (item == eF_CameraSetting.Cmbx_AxisType.ToString())
                info.CCD_TYPE = Tool.StringToInt(value);
            else if (item == eF_CameraSetting.TxtBx_ID_IP.ToString())
                info.IP_ID = value;
            else if (item == eF_CameraSetting.TxtBx_CCD_Name.ToString())
                info.CCD_NAME = value;
            else if (item == eF_CameraSetting.Cmbx_AxisUse.ToString())
                info.CCD_USE = Tool.StringToInt(value);

            CCD_INFO[axis] = info;
        }
        #endregion

        #region public function
        public int Initial_All_Camera()
        {
            foreach (ICamera camera in Camera)
            {
                if (camera.Connect() == 0)
                    CameraList.Add(camera);
            }

            return 0;
        }


        
        




        string ID = "4.235.33.40";

        public bool StartGrab()
        {
            CameraList[0].StartGrabbing(ID);

            return false;
        }

        public bool StopGrab()
        {
            CameraList[0].StopGrabbing(ID);

            return false;
        }

        public bool SoftTrigger()
        {
            CameraList[0].SoftwareTrigger(ID);
            GetImageDisplay();
            //CameraList[0].GetImage(ID);
            return false;
        }

        public bool GetImageDisplay()
        {
            IntPtr image = IntPtr.Zero;
            int width = 0;
            int height = 0;
            int ret = CameraList[0].GetImage(ID, ref image, ref width, ref height);

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


        //[Read&Save Axis Information]
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
