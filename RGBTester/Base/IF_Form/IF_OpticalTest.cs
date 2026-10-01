using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public interface IF_OpticalTest
    {
        void ShowWlLumenResult(string side, string color, double wl, double lum);
    }
}
