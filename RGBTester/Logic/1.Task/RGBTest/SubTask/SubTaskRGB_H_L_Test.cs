using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
//using System.Windows.Forms;

using ToolFunction;
using RGBTester.Base;

namespace RGBTester.Logic
{
    #region Task
    public class SubTaskRGB_H_L_Test : IBaseTask<SubTaskRGB_H_L_Test.WORK>
    {
        public SubTaskRGB_H_L_Test(IBaseTaskDependence dependencies, 
                          IF_StateControl f_StateControl,  
                          string set_state = "Default") : base(dependencies)
        {
            TaskName = this.GetType().Name;
            State = WORK.INITIAL;

            string[] splits = set_state.Split('_');

            if(splits.Length == 3)
            {
                if (splits[2] == "H")
                    OnlyHeighMode = true;
                else if (splits[2] == "L")
                    OnlyLowMode = true;
            }

            switch (set_state)
            {
                default:
                    State = WORK.INITIAL;
                    break;
            }
            Tool.SaveLogToFile($"{TaskName} Start", level: "INF");

            F_StateControl = f_StateControl;

            Type = set_state;
        }

        #region parameter
        private string Type;
        private Queue<int> qDAC_L = new Queue<int>();
        private Queue<int> qDAC_H = new Queue<int>();
        private int DAC_Start = 0, DAC_End = 4, DAC_Step = 2;
        private RGBTesterData TesterData_H = new RGBTesterData();
        private RGBTesterData TesterData_L = new RGBTesterData();
        private LinearCurveFitting LinearCurveFitting_L;
        private LinearCurveFitting LinearCurveFitting_H;
        private int RepeatTime = 1;     //平均次數
        private int DAQ_Vf_R = 0;       //DAQ卡Vr點位
        private int DAQ_Vf_G = 0;       //DAQ卡Vg點位
        private int DAQ_Vf_B = 0;       //DAQ卡Vb點位
        private int DAQ_Vf = 0;         //DAQ卡Vf點位
        private int SigMag = 1;         //訊號放大倍率
        private byte Side;              //LED Side
        private byte Color;             //LED Color
        private double Rfb_HCM = 0;     //High Current Mode阻抗
        private double Rfb_LCM = 0;     //Low Current Mode阻抗
        private double Rfb = 1;         //阻抗
        private double Rin = 1;         //輸入阻抗
        private double LED_Duty;        //LED Duty
        private bool OnlyHeighMode = false;     //只跑Heigh Current Mode
        private bool OnlyLowMode = false;       //只跑Low Current Mode
        private IF_BaseTask SubTask;            //子流程
        private IF_StateControl F_StateControl;
        public enum WORK
        {
            NONE,
            INITIAL,

            SET_DAC_LOW,
            GET_ADC_LOW,
            CALCULATE_LOW,

            SET_DAC_HIGH,
            GET_ADC_HIGH,
            CALCULATE_HIGH,

            END,

            SUCCESS,
            FAIL,

            PAUSE,
            ABORT,
            CONTINUE,
        }
        #endregion

        #region private function
        private void Preset()
        {
            for(int i=DAC_Start; i<=DAC_End; i+=DAC_Step)
            {
                qDAC_L.Enqueue(i);
                qDAC_H.Enqueue(i);
            }

            string[] res = Type.Split('_');

            if (res[0] == "Left")
                Side = Deps.LightEngine.LED_LeftSide;
            else
                Side = Deps.LightEngine.LED_RightSide;

            if (res[1] == "R")
            {
                Color = Deps.LightEngine.LED_R_LSB;
                DAQ_Vf = DAQ_Vf_R;
            }
            else if (res[1] == "G")
            {
                Color = Deps.LightEngine.LED_G_LSB;
                DAQ_Vf = DAQ_Vf_G;
            }
            else
            {
                Color = Deps.LightEngine.LED_B_LSB;
                DAQ_Vf = DAQ_Vf_B;
            }
                
        }
        
        protected override void Transition(WORK target)
        {
            if (target != State && !(target == WORK.GET_ADC_LOW || target == WORK.SET_DAC_LOW || 
                                     target == WORK.GET_ADC_HIGH || target == WORK.SET_DAC_HIGH)) //狀態有變化時紀錄
            {
                Tool.SaveLogToFile($"[Task]({TaskName})" + target.ToString());
                F_StateControl.UpdateTask($"({TaskName})\n" + target.ToString());
            }

            State = target;
        }
        protected override WORK AbortState(WORK target)
        {
            WORK target_work = WORK.NONE;

            switch (target)
            {
                //case WORK.RUNNING_2: target_work = WORK.RUNNING_ABORT; break;
                default: target_work = WORK.ABORT; break;
            }

            return target_work;
        }
        private void CheckResult(TASK_STATUS check, WORK SUCCESS = WORK.SUCCESS, 
                                                    WORK PAUSE = WORK.PAUSE,
                                                    WORK ABORT = WORK.ABORT,
                                                    WORK FAIL = WORK.FAIL)
        {
            switch (check)
            {
                case TASK_STATUS.SUCCESS:
                    {
                        Transition(SUCCESS);
                    }
                    break;
                case TASK_STATUS.PAUSE:
                    {
                        SetStatus(TASK_STATUS.PAUSE);   //設定目前Task狀態,讓MainTask知道Task狀態
                        SetPauseState(State);           //記錄目前暫停的case
                        SetNextState(State);            //設定使用者按繼續後要回到的case
                        Transition(PAUSE);
                    }
                    break;
                case TASK_STATUS.ABORT:
                    {
                        Transition(ABORT);
                    }
                    break;
                case TASK_STATUS.CONTINUE:
                    {

                    }
                    break;
                case TASK_STATUS.FAIL:
                    {
                        Transition(FAIL);
                    }
                    break;
            }
        }
        
        private void ResetTimeCount(out int tick)
        {
            tick = Environment.TickCount;
        }
        private bool CheckTimeOverSec(int tick, int time)
        {
            var time_count = Environment.TickCount - tick;
            bool res = time_count > time * 1000;

            return res;
        }
        #endregion

        #region public function
        public override TASK_STATUS Run(TASK_STATUS command = TASK_STATUS.NONE)
        {
            if (GetStatus() == TASK_STATUS.CONTINUE || command != TASK_STATUS.NONE)
            {
                RunLoop(command);
            }

            TASK_STATUS res = GetStatus();

            return res;
        }
        public override void GoToPause()
        {
            //執行暫停後可根據State選擇操作者使用狀態
            //ABORT_CONTINUE
            //ABORT
            //CONTINUE
            //讓使用者選擇
            SetPauseState(State);

            switch (State)
            {
                default:
                    {
                        F_StateControl.SetPauseAbortContinue(TASK_STATUS.ABORT_CONTINUE);
                    }
                    break;
            }

            if (GetSubTaskProcessing()) //判斷是否有SubTask執行
                SubTask.GoToPause();

        }
        #endregion

        

        protected override void RunLoop(TASK_STATUS task_command)
        {
            if (task_command == TASK_STATUS.ABORT)   //人員傳入ABORT命令
                GoToCaseAbortState(GetPauseState());
            else if(task_command == TASK_STATUS.CONTINUE)    //人員傳入CONTINUE命令
                GoToCaseConitinueState();
                
            switch (State)
            {
                case WORK.INITIAL:
                    {
                        Preset();

                        if(OnlyHeighMode)
                            Transition(WORK.SET_DAC_HIGH);
                        else
                            Transition(WORK.SET_DAC_LOW);

                        Tool.SaveLogToFile($"[Task]({TaskName})" + WORK.SET_DAC_LOW.ToString());
                    }
                    break;

                #region Low
                case WORK.SET_DAC_LOW:
                    {
                        //傳送廣達指令
                        int val = qDAC_L.Dequeue();
                        TesterData_L.DACpoint.Add(val);

                        Deps.LightEngine.SetLed_DAC(Color, Side, val);

                        Transition(WORK.GET_ADC_LOW);
                    }
                    break;
                case WORK.GET_ADC_LOW:
                    {
                        double sum_Vin = 0, sum_VSYS = 0, sum_Vled = 0;
                        double sum_Vf = 0, sum_Vfb = 0;

                        //取得AI訊號
                        for (int i=0; i<RepeatTime; i++)
                        {
                            sum_Vin += 0;
                            sum_VSYS += 0;
                            sum_Vled += 0;
                            sum_Vf += 0;    //DAQ_Vf點位
                            sum_Vfb += 0;
                        }

                        TesterData_L.Vin.Add(sum_Vin / RepeatTime);
                        TesterData_L.Iin.Add(sum_VSYS / RepeatTime / Rin / SigMag);
                        TesterData_L.Vled.Add(sum_Vled / RepeatTime);
                        TesterData_L.Vf.Add((sum_Vled - sum_Vf) / RepeatTime);
                        TesterData_L.Iled.Add(sum_Vfb / RepeatTime / Rfb / SigMag);

                        //Transition(WORK.CALCULATE_LOW);
                        State = WORK.CALCULATE_LOW;
                        goto case WORK.CALCULATE_LOW;
                    }
                    break;
                case WORK.CALCULATE_LOW:
                    {
                        if (qDAC_L.Count == 0)
                        {
                            for (int i = 0; i < TesterData_L.Vin.Count; i++)
                            {
                                TesterData_L.Pin.Add(TesterData_L.Vin[i] * TesterData_L.Iin[i]);
                                TesterData_L.Pled.Add(TesterData_L.Vf[i] * TesterData_L.Iled[i] * LED_Duty);
                                TesterData_L.Eff.Add(TesterData_L.Pled[i] / TesterData_L.Pin[i]);
                            }

                            LinearCurveFitting_L = new LinearCurveFitting(TesterData_L.DACpoint.ToArray(), TesterData_L.Iled.ToArray());

                            string[] side = Type.Split('_');
                            for(int i=0; i< TesterData_L.Vin.Count; i++)
                            {
                                Deps.File.WriteFile($"{Type}_Low,{TesterData_L.Iin[i]},{TesterData_L.Vled[i]}", side[0]);
                            }

                            Tool.SaveLogToFile($"[Task]({TaskName})" + WORK.GET_ADC_LOW.ToString());
                            Tool.SaveLogToFile($"[Task]({TaskName})" + WORK.CALCULATE_LOW.ToString());
                            Tool.SaveLogToFile($"[Task]({TaskName})" + WORK.SET_DAC_HIGH.ToString());

                            if (OnlyLowMode)
                                Transition(WORK.SUCCESS);
                            else
                                Transition(WORK.SET_DAC_HIGH);
                        }
                        else
                        {
                            //Transition(WORK.SET_DAC_LOW);
                            State = WORK.SET_DAC_LOW;
                            goto case WORK.SET_DAC_LOW;
                        }
                    }
                    break;
                #endregion
                #region High
                case WORK.SET_DAC_HIGH:
                    {
                        //傳送廣達指令
                        int val = qDAC_H.Dequeue();
                        TesterData_H.DACpoint.Add(val);
                        Deps.LightEngine.SetLed_DAC(Color, Side, val);
                        Transition(WORK.GET_ADC_HIGH);
                    }
                    break;
                case WORK.GET_ADC_HIGH:
                    {
                        double sum_Vin = 0, sum_VSYS = 0, sum_Vled = 0;
                        double sum_Vf = 0, sum_Vfb = 0;

                        //取得AI訊號
                        for (int i = 0; i < RepeatTime; i++)
                        {
                            sum_Vin += 0;
                            sum_VSYS += 0;
                            sum_Vled += 0;
                            sum_Vf += 0;    //DAQ_Vf點位
                            sum_Vfb += 0;
                        }

                        TesterData_H.Vin.Add(sum_Vin / RepeatTime);
                        TesterData_H.Iin.Add(sum_VSYS / RepeatTime / Rin / SigMag);
                        TesterData_H.Vled.Add(sum_Vled / RepeatTime);
                        TesterData_H.Vf.Add((sum_Vled - sum_Vf) / RepeatTime);
                        TesterData_H.Iled.Add(sum_Vfb / RepeatTime / Rfb / SigMag);
                        //Transition(WORK.CALCULATE_HIGH);
                        State = WORK.CALCULATE_HIGH;
                        goto case WORK.CALCULATE_HIGH;
                    }
                    break;
                case WORK.CALCULATE_HIGH:
                    {
                        if (qDAC_H.Count == 0)
                        {
                            for (int i = 0; i < TesterData_H.Vin.Count; i++)
                            {
                                TesterData_H.Pin.Add(TesterData_H.Vin[i] * TesterData_H.Iin[i]);
                                TesterData_H.Pled.Add(TesterData_H.Vf[i] * TesterData_H.Iled[i] * LED_Duty);
                                TesterData_H.Eff.Add(TesterData_H.Pled[i] / TesterData_H.Pin[i]);
                            }

                            LinearCurveFitting_L = new LinearCurveFitting(TesterData_H.DACpoint.ToArray(), TesterData_H.Iled.ToArray());

                            string[] side = Type.Split('_');
                            for (int i = 0; i < TesterData_L.Vin.Count; i++)
                            {
                                Deps.File.WriteFile($"{Type}_Heigh,{TesterData_H.Iin[i]},{TesterData_H.Vled[i]}", side[0]);
                            }

                            Tool.SaveLogToFile($"[Task]({TaskName})" + WORK.GET_ADC_HIGH.ToString());
                            Tool.SaveLogToFile($"[Task]({TaskName})" + WORK.CALCULATE_HIGH.ToString());
                            Transition(WORK.SUCCESS);
                        }
                        else
                        {
                            State = WORK.SET_DAC_HIGH;
                            goto case WORK.SET_DAC_HIGH;
                        }
                    }
                    break;
                #endregion

                case WORK.SUCCESS:
                    {
                        SetStatus(TASK_STATUS.SUCCESS);
                        Tool.SaveLogToFile($"{TaskName} End", level:"INF");
                    }
                    break;
                case WORK.FAIL:
                    {
                        SetStatus(TASK_STATUS.FAIL);
                    }
                    break;
                case WORK.PAUSE:
                    {
                        SetStatus(TASK_STATUS.PAUSE);
                    }
                    break;
                case WORK.ABORT:
                    {
                        SetStatus(TASK_STATUS.ABORT);
                    }
                    break;
                case WORK.CONTINUE:
                    {
                        if (GetNextState() != WORK.NONE)
                            GoToNextState();

                        SetStatus(TASK_STATUS.CONTINUE);
                        SetStatusCommand(TASK_STATUS.CONTINUE);
                    }
                    break;
                case WORK.END:
                    {
                        SetStatus(TASK_STATUS.SUCCESS);
                    }
                    break;
            }
        }
    }
    #endregion
 
}
