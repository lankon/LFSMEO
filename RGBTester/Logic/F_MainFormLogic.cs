using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolFunction;

using RGBTester.Base;

namespace RGBTester.Logic
{
    public class F_MainFormLogic
    {
        public F_MainFormLogic()
        {
            
        }

        #region parameter define
        IF_MainForm MainForm;
        IBaseMainTask MainTask;
        #endregion

        public void ReadAllSetting()
        {
            ApplicationSetting.ReadAllRecipe<eOEMSetting>();
        }

        public void SetForm(IF_MainForm f_MainForm)
        {
            MainForm = f_MainForm;
        }
    }
}
