using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DeviceCore
{
    public interface IF_CameraButton
    {
        int GetCurrentBtnNum();
        void ShowFormName(bool show);
    }
}
