using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace BurnInTester.Logic
{
    public class TC_Task
    {
        public TC_Task(IFunction_TemperatureControl function_TemperatureControl)
        {
            Func_TC = function_TemperatureControl;

            // 啟動背景處理迴圈
            Task.Run(() => ProcessLoop());
        }

        #region parameter define
        private TC_CommManage CommManage;
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
                            //CommManage.UpdateTemperature();
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
