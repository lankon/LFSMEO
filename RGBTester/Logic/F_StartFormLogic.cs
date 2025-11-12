using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Microsoft.Extensions.DependencyInjection;

using DeviceCore;
using RGBTester.Base;

namespace RGBTester.Logic
{
    public class F_StartFormLogic
    {
        public F_StartFormLogic(IRGBTesterMachine machine, IServiceProvider serviceProvider)
        {
            Machine = machine;
            ServiceProvider = serviceProvider;
        }

        #region parameter define
        IRGBTesterMachine Machine;
        IServiceProvider ServiceProvider;
        #endregion

        #region public function
        public void OpenChillerComm()
        {
            Machine.Chiller.Open("COM8",9600, Parity.None, 8, StopBits.One);
        }

        public void GetChillerStatus()
        {
            Machine.Chiller.GetStatus();
        }

        public void StartTaskAction()
        {
            var MainTask = ServiceProvider.GetRequiredService<IBaseMainTask>();
            MainTask.SetTask<StdTask>();
            MainTask.Run();

            var MainTask2 = ServiceProvider.GetRequiredService<IBaseMainTaskMulti>();
            MainTask2.SetTask<StdTask>();
            MainTask2.Run();
        }
        #endregion
    }
}
