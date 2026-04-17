using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;
using BurnInTester.Device;
using Microsoft.Extensions.DependencyInjection;

namespace BurnInTester.Logic
{
    public class TC_Task
    {
        public TC_Task(IServiceProvider serviceProvider, IFunction_TemperatureControl function_TemperatureControl)
        {
            Func_TC = function_TemperatureControl;
            ServiceProvider = serviceProvider;

            // 啟動背景處理迴圈
            Task.Run(() => ProcessLoop());
        }

        #region parameter define
        private TC_CommManage CommManage;
        private HW_ParamSetting HW_Param;
        private int count = 0;
        private IServiceProvider ServiceProvider;
        private IFunction_TemperatureControl Func_TC;
        private WORK State = WORK.INITIAL;
        private enum WORK
        {
            INITIAL,
            IDLE,

            ASK_PV,
            START,
            STOP,
        }
        #endregion

        #region private function
        private async Task ProcessLoop()
        {
            while (true)
            {
                switch(State)
                {
                    case WORK.INITIAL:
                        {
                            HW_Param = ServiceProvider.GetRequiredService<HW_ParamSetting>();
                            CommManage = new TC_CommManage(Func_TC);
                            State = WORK.IDLE;
                        }
                        break;
                    case WORK.IDLE:
                        {
                            await Task.Delay(100);
                            State = WORK.ASK_PV;
                        }
                        break;
                    case WORK.ASK_PV:
                        {
                            string command = HW_Param.TC_Box.BoxNum[count % HW_Param.TC_Box._CtrlBoxNum] + "," + HW_Param.TC_Box.ChNum[count % HW_Param.TC_Box._CtrlBoxNum];
                            CommManage.UpdateTemperature(ETemperatureControlName.TC_1, command);
                            count++;

                            if (count > 100000)
                                count = 0;

                            State = WORK.IDLE;
                        }
                        break;
                    case WORK.START:
                        // 在START狀態下的處理邏輯
                        break;
                    case WORK.STOP:
                        // 在STOP狀態下的處理邏輯
                        break;
                }
            }
        }
        #endregion



    }
}
