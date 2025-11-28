using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Microsoft.Extensions.DependencyInjection;

using DeviceCore;
using RGBTester.Base;
using System.Threading;

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
            //Machine.Chiller.Open("COM8",9600, Parity.None, 8, StopBits.One);
            Machine.Chiller.SetTemperature(25);
        }

        public void GetChillerStatus()
        {
            Thread.Sleep(100);
            
            while(true)
            {
                Machine.Chiller.GetStatus();

                Thread.Sleep(100);


            }
            
            
            
            
        }

        public void StartTaskAction(string method = "")
        {
            var MainTask = ServiceProvider.GetRequiredService<IBaseMainTask>();
            
            if(method == "Left")
                MainTask.SetTask<TaskRGBTest>("Left");
            else if(method == "Right")
                MainTask.SetTask<TaskRGBTest>("Right");
            else
                MainTask.SetTask<TaskRGBTest>("Both");

            MainTask.Run();

            //var MainTask2 = ServiceProvider.GetRequiredService<IBaseMainTaskMulti>();
            //MainTask2.SetTask<StdTask>();
            //MainTask2.Run();
        }
        #endregion
    }
}
