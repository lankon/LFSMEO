using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Concurrent;

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
        private AgingInformation _AgingInformation;
        private int count = 0;          // 用於計算溫控箱的輪詢次數
        private int MonitorBoxNum = 0;  // 用於監控溫控箱的編號
        private ConcurrentQueue<CommandInfo> _CommandQueue = new ConcurrentQueue<CommandInfo>();    // 儲存待處理的命令
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
        private class CommandInfo
        {
            public WORK CommandType { get; set; }
            public double SV { get; set; }
            public int BoxNum { get; set; }
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
                            
                            if (_CommandQueue.Count != 0)   // 命令
                            {
                                if(_CommandQueue.TryPeek(out CommandInfo cmd))
                                {
                                    State = cmd.CommandType;
                                    break;
                                }
                            }

                            if (HW_Param.TC_Box.Use[MonitorBoxNum] == true)
                                State = WORK.ASK_PV;
                            else
                                count++;

                            if(MonitorBoxNum == 0)
                                await Task.Delay(1);  // 輪巡完所有溫控箱後,休息一下,避免CPU過度負荷
                        }
                        break;
                    case WORK.ASK_PV:
                        {
                            string command = HW_Param.TC_Box.BoxNum[MonitorBoxNum] + "," + HW_Param.TC_Box.ChNum[MonitorBoxNum];
                            
                            if(HW_Param.TC_Box.Use[MonitorBoxNum] == true)
                            {
                                bool updated = await CommManage.UpdateTemperature(HW_Param.TC1, command, MonitorBoxNum);
                                if (updated)
                                    TemperatureUpdated?.Invoke(MonitorBoxNum);
                            }

                            count++;

                            if (count > 100000) //防止int過大爆炸
                                count = 0;

                            State = WORK.IDLE;
                        }
                        break;
                    case WORK.START:
                        {
                            if(_CommandQueue.TryDequeue(out CommandInfo cmd))
                            {
                                string command = HW_Param.TC_Box.BoxNum[cmd.BoxNum] + "," + HW_Param.TC_Box.ChNum[cmd.BoxNum];
                                _ = CommManage.Start(HW_Param.TC1, cmd.SV, command, cmd.BoxNum);
                            }

                            State = WORK.IDLE;
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
        public event Action<int> TemperatureUpdated; // 更新溫度,給外部UI訂閱事件. int:溫控箱陣列索引（0-based）

        public void Start(double sv, string cmd = "", int box_num = 0)
        {
            if (box_num < 0)
                return;

            _CommandQueue.Enqueue(new CommandInfo 
            { 
                CommandType = WORK.START, 
                SV = sv, 
                BoxNum = box_num 
            });

            Tool.SaveLogToFile("開始控溫");
        }
        #endregion
    }
}
