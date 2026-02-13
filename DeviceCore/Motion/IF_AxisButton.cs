using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public interface IF_AxisButton
    {
        int GetCurrentBtnNum();
        void StartUpdatePositionInvoke(bool start);
        void ShowFormName(bool show);
    }
}
