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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace DeviceFunction
{
    public class Function_Camera: IFunction_Camera
    {
        public Function_Camera(IEnumerable<ICamera> spec)
        {
            Camera = spec;
        }

        #region parameter define
        private bool IsInitial = false;
        private int DeviceIndex = -1;
        private IEnumerable<ICamera> Camera;
        private List<ICamera> CameraList = new List<ICamera>();
        #endregion

        #region private function
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
        #endregion

    }
}
