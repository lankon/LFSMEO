using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace DeviceCore
{
    public interface IF_LightControl
    {
        void ShowFormName(bool show);
        void Update_Light_List();
    }
}
