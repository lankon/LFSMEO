using System;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using BurnInTester.Base;
using BurnInTester.Logic.TestSetting;

namespace BurnInTester.Logic
{
    public class SubTask_Temperature : IBaseTask<SubTask_Temperature.WORK>
    {
        public SubTask_Temperature(IBaseTaskDependence dependencies, 
                                   IF_StateControl f_StateControl,  
                                   string set_state = "Default") : base(dependencies)
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
        }

        #region parameter
        private int BoxNum = 0;                     //箱號
        private int time_out;                       //逾時時間
        private int _Stable_time;                   //穩態時間
        private TemperatureCondition TestCondition; //溫控器測試條件
        private TC_Task Func_TC;                    //溫控器功能
        private PlannedStep PlannedStep;            //計畫步驟
        private AgingInformation _AgingInformation; //老化資訊
        private IF_BaseTask SubTask;                //子流程
        private IF_StateControl F_StateControl;
        //private F_StateControl TaskForm;
        public enum WORK
        {
            NONE,
            INITIAL,
            IDLE,

            START,                          // 將目標溫度命令排入 TC_Task
            WAIT_STABLE_TEMPERATURE,        // 等待 PV 進入穩態
            WAIT_STABLE_TIME,               // PV 在容許誤差內持續等待穩態時間

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
        private bool Preset()
        {
            try
            {
                Func_TC = Deps.ServiceProvider.GetRequiredService<TC_Task>();
                _AgingInformation = Deps.ServiceProvider.GetRequiredService<AgingInformation>();

                return true;
            }
            catch (Exception ex)
            {
                Tool.SaveLogToFile($"[Task]({TaskName}) Preset Error: {ex.Message}", level: "ERR");
                return false;
            }
        }
        private bool IsStableTemperature()
        {
            double[] pv = _AgingInformation.TemperatureInfos[BoxNum].GetPV();
            double target = Tool.StringToDouble(TestCondition.TargetTemperature.Trim());
            double tolerance = Tool.StringToDouble(TestCondition.TemperatureTolerance.Trim());

            if (Math.Abs(pv[0] - target) < tolerance)
                return true;
            else
                return false;
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
        public void SetPlannedStep(PlannedStep plan)
        {
            PlannedStep = plan;
        }
        public void SetBoxNum(int box)
        {
            Tool.SaveLogToFile($"Box {box/4+1}-{box%4+1} Start", level: "INF");
            BoxNum = box;
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
                        Transition(WORK.START);
                    }
                    break;
                case WORK.START:
                    {
                        TestCondition = PlannedStep.Condition as TemperatureCondition;
                        double tmperature = Tool.StringToDouble(TestCondition.TargetTemperature.Trim());
                        Func_TC.Start(tmperature, box_num:BoxNum);

                        time_out = Tool.GetCurrentTickCount();

                        Transition(WORK.WAIT_STABLE_TEMPERATURE);
                    }
                    break;
                case WORK.WAIT_STABLE_TEMPERATURE:  // 判斷穩態
                    {
                        if (IsStableTemperature())
                        {
                            _Stable_time = Tool.GetCurrentTickCount();
                            Transition(WORK.WAIT_STABLE_TIME);
                        }
                        else if (CheckTimeOverSec(time_out, Tool.StringToInt(TestCondition.StabilizationTimeout)*60))
                        {
                            Transition(WORK.FAIL);
                        }
                    }
                    break;
                case WORK.WAIT_STABLE_TIME:
                    {
                        if(!IsStableTemperature())
                            Transition(WORK.WAIT_STABLE_TEMPERATURE);
                        else if(CheckTimeOverSec(_Stable_time, Tool.StringToInt(TestCondition.StabilizationWaitTime)*60))
                            Transition(WORK.SUCCESS);
                    }
                    break;
                
                case WORK.SUCCESS:
                    {
                        _IsDone = true;
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
