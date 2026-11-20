using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public interface IF_StatusBox
    {
        event EventHandler Confirmed;
        void ShowMessage(string msg, string status = "ERR");
    }
}
