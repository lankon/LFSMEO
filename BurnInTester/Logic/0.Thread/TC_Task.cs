using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.Extensions.DependencyInjection;
using ToolFunction;
using DeviceCore;
using BurnInTester.Device;

namespace BurnInTester.Logic
{
    public partial class TC_Task
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
        private int MonitorBoxNum = 0;  // 用於監控溫控箱的編號
        private int CommandBoxNum = 0;  // 用於發送命令的溫控箱編號
        private double SV = 0;
        private bool StartHeating = false;
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
                            MonitorBoxNum = count % HW_Param.TC_Box._CtrlBoxNum;
                            if (HW_Param.TC_Box.Use[MonitorBoxNum] == true)
                                await Task.Delay(1);  //Thread休息用,溫度更新太慢可縮短

                            if (StartHeating == true)
                                State = WORK.START;
                            else
                                State = WORK.ASK_PV;
                        }
                        break;
                    case WORK.ASK_PV:
                        {
                            string command = HW_Param.TC_Box.BoxNum[MonitorBoxNum] + "," + HW_Param.TC_Box.ChNum[MonitorBoxNum];
                            
                            if(HW_Param.TC_Box.Use[MonitorBoxNum] == true)
                                _ = CommManage.UpdateTemperature(HW_Param.TC1, command, MonitorBoxNum);

                            count++;

                            if (count > 100000) //防止int過大爆炸
                                count = 0;

                            State = WORK.IDLE;
                        }
                        break;
                    case WORK.START:
                        {
                            StartHeating = false;
                            string command = HW_Param.TC_Box.BoxNum[CommandBoxNum] + "," + HW_Param.TC_Box.ChNum[CommandBoxNum];
                            _ = CommManage.Start(HW_Param.TC1, SV, command, CommandBoxNum);
                        }
                        break;
                    case WORK.STOP:
                        // 在STOP狀態下的處理邏輯
                        break;
                }
            }
        }
        #endregion

        #region public function
        public void Start(double sv, string cmd = "", int box_num = 0)
        {
            if (box_num == 0)
                return;
            
            SV = sv;
            CommandBoxNum = box_num;
            StartHeating = true;
            Tool.SaveLogToFile("開始控溫");
        }
        #endregion
    }
}
