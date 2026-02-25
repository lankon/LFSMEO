using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public interface IF_StartForm
    {
        void ShowSlopeOffsetResult(string side, string color, string mode, double value, double value1, bool clamping = false);
    }
}
