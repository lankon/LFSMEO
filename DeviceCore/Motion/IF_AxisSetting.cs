using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public interface IF_AxisSetting
    {
        void UpdateParmeter();
        void SaveAxisParameter();
        void ShowFormName(bool show);
    }
}
