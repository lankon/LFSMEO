using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolFunction;

using RGBTester.Base;
using DeviceCore;

namespace RGBTester.Logic
{
    public class F_MainFormLogic
    {
        public F_MainFormLogic(IRGBTesterMachine rGBTesterMachine)
        {
            RGBTesterMachine = rGBTesterMachine;
        }

        #region parameter define
        IF_MainForm MainForm;
        IBaseMainTask MainTask;
        IRGBTesterMachine RGBTesterMachine;
        #endregion

        public void ReadAllSetting()
        {
            ApplicationSetting.ReadAllRecipe<eOEMSetting>();
        }

        public void Initial_IO_Function()
        {
            RGBTesterMachine.DIOL.Initial_All_IO();
        }

        

        public void SetForm(IF_MainForm f_MainForm)
        {
            MainForm = f_MainForm;
        }
    }
}
