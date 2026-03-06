using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public interface IF_Spectrometer
    {
        void ShowFormName(bool show);
        void Update_Spectrum_List();
    }
}
