using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCore
{
    public interface IF_IO_Card
    {
        void Update_IO_List();
        void UpdateOutputStatus_UI();
    }
}
