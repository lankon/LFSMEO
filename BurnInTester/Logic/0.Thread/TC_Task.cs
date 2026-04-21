using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.Extensions.DependencyInjection;
using DeviceCore;
using BurnInTester.Device;

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
        AgingInformation _AgingInformation;
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
                            _AgingInformation = ServiceProvider.GetRequiredService<AgingInformation>();
                            CommManage = new TC_CommManage(Func_TC, _AgingInformation);
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
                            int box_num = count % HW_Param.TC_Box._CtrlBoxNum;
                            string command = HW_Param.TC_Box.BoxNum[box_num] + "," + HW_Param.TC_Box.ChNum[box_num];
                            
                            if(HW_Param.TC_Box.Use[box_num] == true)
                                CommManage.UpdateTemperature(ETemperatureControlName.TC_1, command, box_num);

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
