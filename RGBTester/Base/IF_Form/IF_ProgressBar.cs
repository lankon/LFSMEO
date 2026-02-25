using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public interface IF_ProgressBar
    {
        void UpateProgress(int value);
        void HideForm();
    }
}
