using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;
using AAMachine.Machine.BDSA.Base;

namespace AAMachine.Machine.BDSA
{
    internal class FN_BDSA
    {
        public double GetEquipmentOffset(string name)
        {
            double res = 0;

            double CCDToNiddleOffset_X = ApplicationSetting.Get_Double_Recipe<eF_BackSetting>((int)eF_BackSetting.DispensingNeedleX);
            double CCDToNiddelOffset_Y = ApplicationSetting.Get_Double_Recipe<eF_BackSetting>((int)eF_BackSetting.DispensingNeedleY);

            if (name == "CCD2NeedleX")
                res = CCDToNiddleOffset_X;
            else if (name == "CCD2NeedleY")
                res = CCDToNiddelOffset_Y;

            return res;
        }


    }
}
