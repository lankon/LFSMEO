using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public interface IF_SourceMeter
    {
        void ShowFormName(bool show);
        void Update_SMU_List();
    }
}
