using DeviceCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        #region parameter define
        private IEnumerable<ICamera> Camera;
        private int[] CCD_Info2List;
        private bool[] PauseLiveFlag { get; set; }
        private List<ICamera> CameraList = new List<ICamera>();
        private List<CAMERA_INFO> CCD_INFO = new List<CAMERA_INFO>();
        private Dictionary<int, CancellationTokenSource> _activeLiveTasks = new Dictionary<int, CancellationTokenSource>();
        private readonly Dictionary<int, CameraFrameBufferPool> _framePools = new Dictionary<int, CameraFrameBufferPool>();
        private EventHandler<ImageReadyEventArgs>[] OnImageUpdates;
        
        enum WORK
        {
            INIT,
            START_GRAB,
            TRIGGER,
            GET_IMAGE,
            PAUSE,
        }
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

            OnImageUpdates = new EventHandler<ImageReadyEventArgs>[Enum.GetNames(typeof(CCD_NAME)).Length];
            PauseLiveFlag = new bool[Enum.GetNames(typeof(CCD_NAME)).Length];
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
            if (item == eF_CameraSetting.TxtBx_FPS.ToString())
                info.FPS = Tool.StringToInt(value);

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
        private void FireImageUpdate(int ccd, ImageReadyEventArgs e)
        {
            EventHandler<ImageReadyEventArgs> handler = OnImageUpdates[(int)ccd];
            if (handler == null)
                return;

            handler.Invoke(this, e);
        }
        private CameraFrameBufferPool GetFramePool(int ccd)
        {
            lock (_framePools)
            {
                if (!_framePools.TryGetValue(ccd, out CameraFrameBufferPool pool))
                {
                    pool = new CameraFrameBufferPool(ccd, 3);
                    _framePools[ccd] = pool;
                }

                return pool;
            }
        }
        private int GetBytesPerPixel(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.Format8bppIndexed:
                    return 1;
                case PixelFormat.Format24bppRgb:
                    return 3;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppPArgb:
                    return 4;
                default:
                    return Math.Max(1, Image.GetPixelFormatSize(format) / 8);
            }
        }
        private int GetAlignedStride(int width, PixelFormat format)
        {
            int bytesPerPixel = GetBytesPerPixel(format);
            return ((width * bytesPerPixel + 3) / 4) * 4;
        }
        private void CopyImageToFrame(IntPtr source, CameraFrame frame)
        {
            int bytesPerPixel = GetBytesPerPixel(frame.Format);
            int sourceStride = frame.Width * bytesPerPixel;
            int copyBytes = Math.Min(sourceStride, frame.Stride);

            for (int y = 0; y < frame.Height; y++)
            {
                IntPtr sourceRow = IntPtr.Add(source, y * sourceStride);
                IntPtr destRow = IntPtr.Add(frame.ImageData, y * frame.Stride);
                NativeMethods.CopyMemory(destRow, sourceRow, new UIntPtr((uint)copyBytes));
            }
        }
        private void CameraLiveLoop(int ccdIndex, CancellationToken token)
        {
            WORK state = WORK.INIT;
            WORK temp = WORK.INIT;
            int index = ccdIndex;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (PauseLiveFlag[ccdIndex] == true && state != WORK.PAUSE)
                    {
                        temp = state;
                        state = WORK.PAUSE;
                    }
                    
                    switch(state)
                    {
                        case WORK.INIT:
                            {
                                state = WORK.START_GRAB;
                            }
                            break;
                        case WORK.START_GRAB:
                            {
                                if (StartGrab(ccdIndex) == false)
                                {
                                    Tool.SaveLogToFile("CCD Live START_GRAB fail", level:"ERR");
                                    return;   //有可能是已經StartGrab了,先繼續執行
                                }

                                state = WORK.TRIGGER;
                            }
                            break;
                        case WORK.TRIGGER:
                            {
                                if (SoftTrigger(ccdIndex) == false)
                                {
                                    Tool.SaveLogToFile("CCD Live TRIGGER fail", level: "ERR");
                                    return;
                                }

                                state = WORK.GET_IMAGE;
                            }
                            break;
                        case WORK.GET_IMAGE:
                            {
                                if (index > 1)  //測試用
                                    index = 0;

                                string path = AppDomain.CurrentDomain.BaseDirectory + $@"\Setting\Virtual\CCD_LIVE\CCD{ccdIndex}_picture{index}.png";
                                if (GetImageDisplay(ccdIndex, path) == false)
                                {
                                    Tool.SaveLogToFile($"CCD{ccdIndex} Live GET_IMAGE fail", level: "ERR");
                                    return;
                                }
                                index++;
                                state = WORK.TRIGGER;
                            }
                            break;
                        case WORK.PAUSE:
                            {
                                Thread.Sleep(200);

                                if (PauseLiveFlag[ccdIndex] == false)
                                    state = temp;
                            }
                            break;
                    }

                    int fps = CCD_INFO[ccdIndex].FPS < 0 ? 10 : CCD_INFO[ccdIndex].FPS;
                    Thread.Sleep(1000/fps);
                }
            }
            catch (Exception ex) { /* 錯誤處理 */ }
        }
        #endregion

        private sealed class CameraFrameBufferPool
        {
            private readonly object _sync = new object();
            private readonly FrameSlot[] _slots;
            private readonly int _ccdIndex;

            public CameraFrameBufferPool(int ccdIndex, int count)
            {
                _ccdIndex = ccdIndex;
                _slots = new FrameSlot[count];

                for (int i = 0; i < _slots.Length; i++)
                    _slots[i] = new FrameSlot();
            }

            public CameraFrame TryRent(int width, int height, int stride, PixelFormat format)
            {
                lock (_sync)
                {
                    for (int i = 0; i < _slots.Length; i++)
                    {
                        FrameSlot slot = _slots[i];
                        if (slot.InUse)
                            continue;

                        int requiredBytes = stride * height;
                        if (slot.Buffer == IntPtr.Zero || slot.Capacity < requiredBytes)
                        {
                            if (slot.Buffer != IntPtr.Zero)
                                Marshal.FreeHGlobal(slot.Buffer);

                            slot.Buffer = Marshal.AllocHGlobal(requiredBytes);
                            slot.Capacity = requiredBytes;
                        }

                        slot.InUse = true;
                        slot.Frame = new CameraFrame(slot.Buffer, width, height, stride, format, _ccdIndex, Release);
                        return slot.Frame;
                    }
                }

                return null;
            }

            private void Release(CameraFrame frame)
            {
                lock (_sync)
                {
                    for (int i = 0; i < _slots.Length; i++)
                    {
                        FrameSlot slot = _slots[i];
                        if (!object.ReferenceEquals(slot.Frame, frame))
                            continue;

                        slot.Frame = null;
                        slot.InUse = false;
                        return;
                    }
                }
            }

            private sealed class FrameSlot
            {
                public IntPtr Buffer = IntPtr.Zero;
                public int Capacity = 0;
                public bool InUse = false;
                public CameraFrame Frame;
            }
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
            public static extern void CopyMemory(IntPtr dest, IntPtr src, UIntPtr count);
        }

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

        // [Camera Grab]
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
        public bool StopGrabAllCamera()
        {

            return false;
        }

        // [Camera Trigger && LIVE]
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
        public bool StartLive(int ccdIndex)
        {
            // 如果已經在跑了，就不要重複開
            if (_activeLiveTasks.ContainsKey(ccdIndex)) 
                return true;

            // 數量限制：保護硬體頻寬與記憶體
            if (_activeLiveTasks.Count >= 5)
            {
                Tool.SaveLogToFile("超出CCD Live數量", level: "WRN");
                return false;
            }

            // 建立取消權杖
            var cts = new CancellationTokenSource();
            _activeLiveTasks.Add(ccdIndex, cts);

            // 啟動非同步取像迴圈
            Task.Run(() => CameraLiveLoop(ccdIndex, cts.Token), cts.Token);

            return true;
        }
        public bool StopLive(int ccd)
        {
            if (!CheckCameraEnable(ccd))
                return false;

            string ip = CCD_INFO[ccd].IP_ID;
            int ret = CameraList[CCD_Info2List[ccd]].StopGrabbing(ip);

            if (_activeLiveTasks.TryGetValue(ccd, out var cts))
            {
                cts.Cancel(); // 停止迴圈
                _activeLiveTasks.Remove(ccd);
                return true;
            }

            return false;
        }
        public void PauseLive(int ccd, bool is_pause)
        {
            PauseLiveFlag[ccd] = is_pause;
        }

        // [Get Image]
        public bool GetImageDisplay(int ccd, string image_path)
        {
            if (!CheckCameraEnable(ccd))
                return false;

            string ip = CCD_INFO[ccd].IP_ID;
            IntPtr image = IntPtr.Zero;
            int width = 0;
            int height = 0;
            PixelFormat pixelFormat = PixelFormat.Format32bppArgb;

            CameraList[CCD_Info2List[ccd]].SetVirtualImagePath(ip, image_path);
            int ret = CameraList[CCD_Info2List[ccd]].GetImage(ip, ref image, ref width, ref height, ref pixelFormat);

            if (ret == 0)
            {
                int stride = GetAlignedStride(width, pixelFormat);
                CameraFrame frame = GetFramePool(ccd).TryRent(width, height, stride, pixelFormat);
                if (frame == null)
                    return true;

                try
                {
                    CopyImageToFrame(image, frame);

                    ImageReadyEventArgs e = new ImageReadyEventArgs
                    {
                        Frame = frame,
                        ImageData = frame.ImageData,
                        Width = frame.Width,
                        Height = frame.Height,
                        Format = frame.Format,
                        CCD_Index = ccd
                    };

                    FireImageUpdate(ccd, e);
                }
                finally
                {
                    frame.Dispose();
                }

                return true;
            }

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


        public void Subscribe(int ccd, EventHandler<ImageReadyEventArgs> handler)
        {
            OnImageUpdates[ccd] += handler;
        }
        #endregion 

    }
}
