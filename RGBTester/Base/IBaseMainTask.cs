using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace RGBTester.Base
{
    public interface IBaseMainTask
    {
        void GoToPause();
        void GoToAbort();
        void GoToContinue();
    }
    
}
