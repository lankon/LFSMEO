using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolFunction;

namespace RGBTester.Logic
{
    public class F_FunctionTesterLogic
    {
        public F_FunctionTesterLogic(IRGBTesterMachine rGBTesterMachine, IServiceProvider serviceProvider,
                                     F_StartFormLogic f_StartFormLogic, RGBTesterFunction rGBTesterFunction)
        {
            RGBTesterMachine = rGBTesterMachine;
            ServiceProvider = serviceProvider;
            StartFormLogic = f_StartFormLogic;
            RGBfunc = rGBTesterFunction;
        }

        #region parameter define
        private IRGBTesterMachine RGBTesterMachine;
        private IServiceProvider ServiceProvider;
        private RGBTesterFunction RGBfunc;
        private F_StartFormLogic StartFormLogic;
        #endregion

        public void SetVirtual_IO_Rule()
        {
            RGBTesterMachine.DIOL.AddIORule(EIOName.SphereUp, true, (EIOName.SphereUpSensor, true));
            RGBTesterMachine.DIOL.AddIORule(EIOName.SphereUp, false, (EIOName.SphereUpSensor, false));

            RGBTesterMachine.DIOL.AddIORule(EIOName.SphereDown, true, (EIOName.SphereDownSensor, true));
            RGBTesterMachine.DIOL.AddIORule(EIOName.SphereDown, false, (EIOName.SphereDownSensor, false));

            RGBTesterMachine.DIOL.AddIORule(EIOName.Sphere_LR, true, (EIOName.SphereLeftSensor, true), (EIOName.SphereRightSensor, false));
            RGBTesterMachine.DIOL.AddIORule(EIOName.Sphere_LR, false, (EIOName.SphereLeftSensor, false), (EIOName.SphereRightSensor, true));

            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckUp, true, (EIOName.ChuckUpSensor, true));
            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckUp, false, (EIOName.ChuckUpSensor, false));

            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckDown, true, (EIOName.ChuckDownSensor, true));
            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckDown, false, (EIOName.ChuckDownSensor, false));

            RGBTesterMachine.DIOL.AddIORule(EIOName.Chuck_LR, true, (EIOName.ChuckLeftSensor, true), (EIOName.ChuckRightSensor, false));
            RGBTesterMachine.DIOL.AddIORule(EIOName.Chuck_LR, false, (EIOName.ChuckLeftSensor, false), (EIOName.ChuckRightSensor, true));
        }
        public int StartFunctionTest()
        {
            IFunction_LightEngine lea = ServiceProvider.GetRequiredService<IFunction_LightEngine>();
            lea.Open();

            RGBfunc.SerialNumber = ApplicationSetting.Get_String_Recipe<eF_FunctionTester>((int)eF_FunctionTester.TxtBx_SerialNumber);

            RGBTesterMachine.DIOL.Clear_AI_VirtualData();
            StartFormLogic.ReadVirtual_AI_Data();

            var MainTask = ServiceProvider.GetRequiredService<IBaseMainTask>();
            MainTask.SetTask<TaskFunctionTest>();
            MainTask.Run();

            return 0;
        }

    }
}
