using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

using ToolFunction;
using BurnInTester.Base;
using BurnInTester.Logic.TestSetting;

namespace BurnInTester.Logic
{
    #region Task
    public class Task_AgingBox : IBaseTask<Task_AgingBox.WORK>
    {
        public Task_AgingBox(IBaseTaskDependence dependencies,
            IF_StateControl f_StateControl,
            string set_state = "Default")
            : base(dependencies)
        {
            TaskName = this.GetType().Name;
            State = WORK.INITIAL;
            _BoxIndex = Tool.StringToInt(set_state);

            Tool.SaveLogToFile($"{TaskName} {_BoxIndex + 1} Start", level: "INF");

            F_StateControl = f_StateControl;
        }

        #region parameter
        private int _BoxIndex = 0;                          // 溫控箱編號
        private int _StageIndex;                            // 目前 Stage 在 _StagePlans 中的索引
        private int _ExecutionSubTaskIndex = 0;             // ExecutionSubTask執行索引
        private int _DurationTick = 0;                      // 記錄執行時間的tick
        private Queue<PlannedStep> _PendingSteps = new Queue<PlannedStep>();            // 待執行Preparation
        private Queue<PlannedStep> _ExecutionSteps = new Queue<PlannedStep>();      // 待執行Execution
        private Queue<MonitorPlan> _MonitorSteps = new Queue<MonitorPlan>();        // 尚未完成的監控步驟

        private TestSettingData _Setting;                   // 此溫控箱目前使用的測試設定
        private List<StagePlan> _StagePlans;

        private IF_BaseTask SubTask;                        //子流程
        private List<IF_BaseTask> MonitorSubTask = new List<IF_BaseTask>();     // 監控子流程
        private List<IF_BaseTask> ExecuteSubTask = new List<IF_BaseTask>();     // 執行子流程
        private IF_StateControl F_StateControl;
        private readonly object _SyncRoot = new object();   // UI 執行緒會送命令，排程執行緒會掃描狀態，因此存取執行資料時必須鎖定。
        public enum WORK
        {
            NONE,
            INITIAL,
            IDLE,

            LOAD_STAGE,

            START_PREPARATION_STEP,
            WAIT_PREPARATION_STEP,
            FINISH_PREPARATION_STEP,

            START_EXECUTION_STEP,
            WAIT_EXECUTION_STEP,

            WAIT_DURATION,

            FINISH_EXECUTION_STEP,

            INITIAL_SUBTASK,
            SUBTASK_PROCESS,
            SUBTASK_PROCESS_PAUSE,
            WAIT_SUBTASK_PROCESS,

            END,

            SUCCESS,
            FAIL,

            PAUSE,
            ABORT,
            CONTINUE,
        }

        private sealed class PreparationMonitorStep
        {
            public IndexedStep IndexedStep { get; set; }
            public TimeSpan Interval { get; set; }
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

        private void LoadPendingSteps(IEnumerable<PlannedStep> steps)
        {
            _PendingSteps.Clear();

            foreach (PlannedStep step in steps)
                _PendingSteps.Enqueue(step);
        }

        private bool IsRunningState(WORK state)
        {
            return state != WORK.NONE &&
                state != WORK.SUCCESS &&
                state != WORK.FAIL &&
                state != WORK.ABORT &&
                state != WORK.INITIAL &&
                state != WORK.IDLE;
        }

        private void ResetAllStatus()
        {
            _StageIndex = 0;


            _PendingSteps.Clear();
        }

        private void AddMonitorSubTask()
        {
            int count = _MonitorSteps.Count;
            for (int i = 0; i < count; i++)
            {
                MonitorPlan monitor_step = _MonitorSteps.Dequeue();
                if (monitor_step.PlannedStep.Step.TestProcess == TestProcess.LIV)
                {
                    SubTask_LIV subTask_LIV = new SubTask_LIV(Deps, F_StateControl);
                    subTask_LIV.SetMonitorPlannedStep(monitor_step);
                    MonitorSubTask.Add(subTask_LIV);
                }
                //else if (monitor_step.PlannedStep.Step.TestProcess == TestProcess.Temperature)
                //{
                //    SubTask_Temperature subTask_Temperature = new SubTask_Temperature(Deps, F_StateControl);
                //    MonitorSubTask.Add(subTask_Temperature);
                //}
            }
        }
        private bool ProcessMonitorSubTask()
        {
            bool all_success = true;
            for (int i = 0; i < MonitorSubTask.Count; i++)
            {
                TASK_STATUS monitor_check = MonitorSubTask[i].Run(GetStatusCommand());
                
                if (monitor_check != TASK_STATUS.SUCCESS)
                    all_success = false;
            }
            return all_success;
        }
        private void ResetMonitorSubTask()
        {
            for (int i = 0; i < MonitorSubTask.Count; i++)
            {
                if (MonitorSubTask[i] is SubTask_LIV liv)
                    liv.ResetProcess();
            }
        }

        private void ResetExecuteSubTask()
        {
            for (int i = 0; i < _ExecutionSteps.Count; i++)
            {
                if (MonitorSubTask[i] is SubTask_LIV liv)
                    liv.ResetProcess();
            }
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

        public bool Start(TestSettingData setting)
        {
            if (IsRunningState(State))
            {
                Tool.SaveLogToFile($"Box {_BoxIndex + 1} is already running.", level: "WRN");
                return false;
            }

            ResetAllStatus();

            try
            {
                AgingStagePlanBuilderNew _PlanBuilder = new AgingStagePlanBuilderNew();
                _StagePlans = _PlanBuilder.Classify(setting);
                _Setting = setting;

                Transition(WORK.LOAD_STAGE);
                Tool.SaveLogToFile($"Box {_BoxIndex + 1} schedule started");
            }
            catch (Exception ex)
            {
                //FailInternal(ex);
                throw;
            }

            return true;
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
                        Transition(WORK.IDLE);
                    }
                    break;

                case WORK.IDLE:
                    {
                        int index = _BoxIndex;
                    }
                    break;

                case WORK.LOAD_STAGE:
                    {
                        foreach (PlannedStep step in _StagePlans[_StageIndex].PreparationSteps)
                            _PendingSteps.Enqueue(step);

                        foreach (MonitorPlan step in _StagePlans[_StageIndex].MonitorSteps)
                            _MonitorSteps.Enqueue(step);

                        foreach (PlannedStep step in _StagePlans[_StageIndex].ExecutionSteps)
                            _ExecutionSteps.Enqueue(step);

                        Transition(WORK.START_PREPARATION_STEP);
                    }
                    break;

                #region PREPARATION
                case WORK.START_PREPARATION_STEP:
                    {
                        if (_PendingSteps.Count == 0)
                        {
                            Transition(WORK.FINISH_PREPARATION_STEP);
                            return;
                        }

                        SetSubTaskProcessing(true); // 設定SubTask執行
                        PlannedStep step = _PendingSteps.Peek();
                        if(step.Step.TestProcess == TestProcess.Temperature)
                        {
                            SubTask = new SubTask_Temperature(Deps, F_StateControl);
                            if(SubTask is SubTask_Temperature temp)
                            {
                                temp.SetBoxNum(_BoxIndex);
                                temp.SetPlannedStep(step);
                            }
                        }

                        AddMonitorSubTask();

                        Transition(WORK.WAIT_PREPARATION_STEP);
                    }
                    break;
                case WORK.WAIT_PREPARATION_STEP:
                    {
                        TASK_STATUS check = SubTask.Run(GetStatusCommand());

                        bool finish = ProcessMonitorSubTask();  // 執行監控流程

                        if(SubTask.IsDone() && finish)
                            CheckResult(check, SUCCESS: WORK.FINISH_PREPARATION_STEP);
                        else if (finish)
                            ResetMonitorSubTask();
                    }
                    break;
                case WORK.FINISH_PREPARATION_STEP:
                    {
                        _PendingSteps.Dequeue();    // 移除已完成的Preparation Step

                        if (_ExecutionSteps.Count != 0)
                            Transition(WORK.START_EXECUTION_STEP);
                    }
                    break;
                #endregion

                #region EXECUTION
                case WORK.START_EXECUTION_STEP:
                    {
                        int count = _ExecutionSteps.Count;
                        for (int i = 0; i < count; i++)
                        {
                            PlannedStep execution_step = _ExecutionSteps.Dequeue();
                            if (execution_step.Step.TestProcess == TestProcess.LIV)
                            {
                                SubTask_LIV subTask_LIV = new SubTask_LIV(Deps, F_StateControl);
                                subTask_LIV.SetPlannedStep(execution_step);
                                ExecuteSubTask.Add(subTask_LIV);
                            }
                        }

                        _ExecutionSubTaskIndex = 0;     // 歸零
                        Transition(WORK.WAIT_EXECUTION_STEP);
                    }
                    break;
                case WORK.WAIT_EXECUTION_STEP:
                    {
                        if(_ExecutionSubTaskIndex >= ExecuteSubTask.Count)
                        {
                            _DurationTick = Tool.GetCurrentTickCount();
                            Transition(WORK.WAIT_DURATION);
                            break;
                        }
                        
                        TASK_STATUS check = ExecuteSubTask[_ExecutionSubTaskIndex].Run(GetStatusCommand());

                        if (check == TASK_STATUS.SUCCESS)
                            _ExecutionSubTaskIndex++;

                        CheckResult(check, SUCCESS: WORK.WAIT_EXECUTION_STEP);
                    }
                    break;
                #endregion

                #region DURATION
                case WORK.WAIT_DURATION:
                    {
                        int duration = (int)_StagePlans[_StageIndex].Duration.TotalSeconds;

                        if (Tool.CheckTimeOverSec(_DurationTick, duration)) // 到達設定的Duration時間
                        {
                            _StageIndex++;
                            if (_StageIndex >= _StagePlans.Count)
                                Transition(WORK.SUCCESS);
                            else
                                Transition(WORK.LOAD_STAGE);
                        }
                        else if(!Tool.CheckTimeOverSec(_DurationTick, duration) &&
                                _StagePlans[_StageIndex].ExecutionMode == ExecutionRepeatMode.RepeatSequence)
                        {
                            ResetExecuteSubTask();
                            Transition(WORK.WAIT_EXECUTION_STEP);
                        }
                    }
                    break;
                #endregion

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
                        //SaveHistoryCurrentState(WORK.ABORT);
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
