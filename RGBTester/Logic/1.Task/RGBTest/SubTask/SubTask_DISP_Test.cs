using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using RGBTester.Base;
using RGBTester.Logic;


namespace SampleCode.Logic
{
    public class SubTask_DISP_Test : IBaseTask<SubTask_DISP_Test.WORK>
    {
        public SubTask_DISP_Test(IBaseTaskDependence dependencies, 
                          IF_StateControl f_StateControl,
                          RGBTesterData LCM, RGBTesterData HCM, string set_state = "Default") : base(dependencies)
        {
            TaskName = this.GetType().Name;
            State = WORK.INITIAL;

            switch (set_state)
            {
                default:
                    State = WORK.INITIAL;
                    break;
            }
            Tool.SaveLogToFile($"{TaskName} Start", level: "INF");

            F_StateControl = f_StateControl;

            Type = set_state;
            TestData_LCM = LCM;
            TestData_HCM = HCM;
        }

        #region parameter
        private int Period_DAQ_Count;               //取樣次數
        private string Type;
        private byte Side;                          //LED Board通訊指令(硬體)_Side
        private RGBTesterFunction.DAQ_Point_FunctionTester DAQPoint;
        private RGBTesterFunction RGBfunc;
        private RGBTesterData TestData_LCM;
        private RGBTesterData TestData_HCM;
        private IF_BaseTask SubTask;                  //子流程
        private IF_StateControl F_StateControl;
        //private F_StateControl TaskForm;
        public enum WORK
        {
            NONE,
            INITIAL,
            IDLE,

            TEST_DISP,


            END,

            SUCCESS,
            FAIL,

            PAUSE,
            ABORT,
            CONTINUE,
        }
        #endregion

        #region private function
        protected override void Transition(WORK target)
        {
            if (target != State) //狀態有變化時紀錄
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
        /// <summary>
        /// 確認Task狀態
        /// </summary>
        /// <param name="check"></param>
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
        private RGBTesterFunction.AvgData_FuncTester PeriodAvgValueCalculate(string current_mode)
        {
            double[] DISP_6V0 = new double[Period_DAQ_Count];
            double[] DISP_1V2 = new double[Period_DAQ_Count];

            for (int i = 0; i < Period_DAQ_Count; i++)  //要修改 計算一個週期多少點數
            {
                DISP_6V0[i] = Deps.DIOL.GetAInputStatus(DAQPoint.DAQ_6V0);
                DISP_1V2[i] = Deps.DIOL.GetAInputStatus(DAQPoint.DAQ_1V2);
            }

            double threshold = 0.95;
            DataFilter dataFilter = new DataFilter();
            RGBTesterFunction.AvgData_FuncTester avgData = new RGBTesterFunction.AvgData_FuncTester();
            avgData.Avg_DISP_6V0 = dataFilter.GetPreciseHighLevel(DISP_6V0.ToList(), threshold, 0.005);
            avgData.Avg_DISP_1V2 = dataFilter.GetPreciseHighLevel(DISP_1V2.ToList(), threshold, 0.005);

            return avgData;
        }
        private void Preset()
        {
            string[] res = Type.Split('_');
            bool isLeft = (res[0] == "Left");
            Side = isLeft ? Deps.LightEngine.LED_LeftSide : Deps.LightEngine.LED_RightSide;

            RGBfunc = Deps.ServiceProvider.GetRequiredService<RGBTesterFunction>();
            DAQPoint = RGBfunc.Get_DAQ_PointFuncTester(Side);
            Period_DAQ_Count = RGBfunc.HardwareParam.Period_DAQ_Count;
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
                        Transition(WORK.TEST_DISP);
                    }
                    break;
                case WORK.TEST_DISP:
                    {
                        double sum_DISP_6V0 = 0, sum_DISP_1V2 = 0;
                        int repeat = 1;
                        RGBTesterFunction.AvgData_FuncTester AvgData = new RGBTesterFunction.AvgData_FuncTester();
                        for (int i = 0; i < repeat; i++)
                        {
                            AvgData = PeriodAvgValueCalculate("");

                            sum_DISP_6V0 += AvgData.Avg_DISP_6V0;
                            sum_DISP_1V2 += AvgData.Avg_DISP_1V2;
                        }

                        sum_DISP_6V0 = sum_DISP_6V0 / repeat;
                        sum_DISP_1V2 = sum_DISP_1V2 / repeat;

                        //HCM,LCM沒有差異,兩個都加
                        TestData_HCM.DISP_6V0.Add(sum_DISP_6V0);
                        TestData_HCM.DISP_1V2.Add(sum_DISP_1V2);
                        TestData_LCM.DISP_6V0.Add(sum_DISP_6V0);
                        TestData_LCM.DISP_1V2.Add(sum_DISP_1V2);

                        Transition(WORK.SUCCESS);
                    }
                    break;

                case WORK.SUCCESS:
                    {
                        SetStatus(TASK_STATUS.SUCCESS);
                        Tool.SaveLogToFile($"{TaskName} End", level: "INF");
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
}
