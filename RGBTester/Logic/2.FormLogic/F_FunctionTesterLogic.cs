using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Logic
{
    public class F_FunctionTesterLogic
    {
        public F_FunctionTesterLogic(IRGBTesterMachine rGBTesterMachine, IServiceProvider serviceProvider)
        {
            RGBTesterMachine = rGBTesterMachine;
            ServiceProvider = serviceProvider;
        }

        #region parameter define
        private IRGBTesterMachine RGBTesterMachine;
        private IServiceProvider ServiceProvider;
        #endregion

        public void SetVirtual_IO_Rule()
        {
            RGBTesterMachine.DIOL.AddIORule(EIOName.SphereUp, true, (EIOName.SphereUpSensor, true));
            //RGBTesterMachine.DIOL.AddIORule(EIOName.SphereUp, false, (EIOName.SphereUpSensor, false));

            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckUp, true, (EIOName.ChuckUpSensor, true));
            //RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckUp, false, (EIOName.ChuckUpSensor, false));

            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckDown, true, (EIOName.ChuckDownSensor, true));
            //RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckDown, false, (EIOName.ChuckDownSensor, false));

            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckRight, true, (EIOName.ChuckRightSensor, true));
            //RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckRight, false, (EIOName.ChuckRightSensor, false));
        }


    }
}
