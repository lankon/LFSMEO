using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
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

        public string GetTemperature()
        {
            var lea = ServiceProvider.GetRequiredService<ILightEngineCommand>();
            return lea.GetTemperature();
        }

        public int StartTaskAction(string method = "")
        {
            int res = CheckTestCondition();

            if (res < 0)
                return res;

            var MainTask = ServiceProvider.GetRequiredService<IBaseMainTask>();
            
            if(method == "Left")
                MainTask.SetTask<TaskRGBTest>("Left");
            else if(method == "Right")
                MainTask.SetTask<TaskRGBTest>("Right");
            else
                MainTask.SetTask<TaskRGBTest>("Both");

            MainTask.Run();

            return 0;
        }
        #endregion

        #region private function
        private int CheckTestCondition()
        {
            int res = 0;
            
            int LeftAvg = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_AvgCount);
            int LeftStart = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_DAC_Start);
            int LeftEnd = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_DAC_End);
            int LeftStep = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Left_DAC_Step);
            int RightAvg = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_AvgCount);
            int RightStart = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_DAC_Start);
            int RightEnd = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_DAC_End);
            int RightStep = ApplicationSetting.Get_Int_Recipe<eF_StartFormRecipe>((int)eF_StartFormRecipe.TxtBx_Right_DAC_Step);

            if (LeftAvg <= 0 || LeftStart < 0 || LeftEnd <= 0 || LeftStep<= 0 ||
                RightAvg <= 0 || RightStart < 0 || RightEnd <= 0 || RightStep <= 0)
            {
                Tool.SaveLogToFile("測試條件輸入小於等於零");

                var box1 = ServiceProvider.GetRequiredService<IF_StatusBox>();
                box1.ShowMessage("Test Condition Setting Fail");
                
                res = -1;
                return res;
            }

            if(((LeftStart - LeftEnd) % LeftStep != 0) || ((RightStart - RightEnd) % RightStep != 0))
            {
                Tool.SaveLogToFile("Step無法整除");
                res = -2;
            }

            if(res < 0)
            {
                var box = ServiceProvider.GetRequiredService<IF_StatusBox>();
                box.ShowMessage("Test Condition Setting Fail");
            }

            return res;
        }

        #endregion
    }
}
