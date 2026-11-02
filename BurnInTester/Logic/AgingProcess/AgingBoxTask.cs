using System;
using System.Collections.Generic;
using System.Linq;
using BurnInTester.Base;
using BurnInTester.Logic.TestSetting;
using ToolFunction;

namespace BurnInTester.Logic
{
    /// <summary>
    /// 管理單一溫控箱的 Stage 與 Step 狀態機。
    /// AgingScheduler 每次掃描只呼叫一次 Run，本類別不可阻塞共用排程執行緒。
    /// </summary>
    internal sealed class AgingBoxTask : IBaseTask<AgingBoxTask.WORK>
    {
        /// <summary>單一溫控箱的實際流程狀態，也是排程控制的唯一狀態來源。</summary>
        public enum WORK
        {
            NONE,                       // 尚未開始或已重設
            LOAD_STAGE,                 // 載入下一個 Stage

            START_PREPARATION_STEP,     // 建立下一個 Preparation ProcessTask
            WAIT_PREPARATION_STEP,      // 掃描目前 Preparation ProcessTask
            FINISH_PREPARATION,         // 等待執行中的 Monitor 收尾

            START_EXECUTION_STEP,       // 建立下一個 Execution ProcessTask
            WAIT_EXECUTION_STEP,        // 掃描目前 Execution ProcessTask
            WAIT_EXECUTION_INTERVAL,    // 等待下一輪 Execution Sequence
            WAIT_STAGE_DURATION,        // Execution 完成後等待 Stage Duration
            COMPLETE_STAGE,             // 結束目前 Stage 並移到下一個 Stage

            SUCCESS,                    // 全部 Stage 完成
            FAIL,                       // 排程或子流程失敗
            PAUSE,                      // 使用者暫停
            ABORT,                      // 使用者放棄
            CONTINUE                    // 保留給原有狀態機風格；Resume 後直接回原 WORK
        }

        public AgingBoxTask(
            IBaseTaskDependence dependencies,
            int boxIndex,
            AgingProcessStatus status,
            IEnumerable<IAgingProcessTaskFactory> factories)
            : base(dependencies)
        {
            if (status == null)
                throw new ArgumentNullException(nameof(status));
            if (factories == null)
                throw new ArgumentNullException(nameof(factories));

            TaskName = $"AgingBoxTask[{boxIndex + 1}]";
            _BoxIndex = boxIndex;
            _Status = status;
            _Factories = factories.ToDictionary(
                factory => factory.Process,
                factory => factory);
            _PlanBuilder = new AgingStagePlanBuilder(_Factories);
            _StagePlans = new List<StageExecutionPlan>();
            _MonitorTasks = new List<MonitorRuntime>();
            _PendingSteps = new Queue<IndexedStep>();
            _PendingCommand = TASK_STATUS.NONE;
            State = WORK.NONE;
            SetStatus(TASK_STATUS.NONE);
        }

        // UI 執行緒會送命令，排程執行緒會掃描狀態，因此存取執行資料時必須鎖定。
        private readonly object _SyncRoot = new object();

        // 建立後不會改變的溫控箱與排程相依資料。
        private readonly int _BoxIndex;                                      // 溫控箱索引（0-based）
        private readonly AgingProcessStatus _Status;                         // 提供 UI 與 Debug 使用的狀態快照
        private readonly Dictionary<TestProcess, IAgingProcessTaskFactory> _Factories; // ProcessTask 建立工廠
        private readonly AgingStagePlanBuilder _PlanBuilder;                 // 將設定資料轉成可執行 Stage 計畫

        // Start 時建立，整個排程執行期間保留的資料。
        private TestSettingData _Setting;                         // 此溫控箱目前使用的測試設定
        private List<StageExecutionPlan> _StagePlans;             // 依設定順序建立的 Stage 執行計畫
        private readonly Queue<IndexedStep> _PendingSteps;        // 目前階段尚未完成的 Preparation 或 Execution
        private readonly List<MonitorRuntime> _MonitorTasks;      // Preparation 期間可同時重複執行的 Monitor

        // 排程掃描位置與目前子流程。
        private int _StageIndex;                                 // 目前 Stage 在 _StagePlans 中的索引
        private IAgingProcessTask _CurrentProcessTask;            // 目前依序執行的 ProcessTask
        private IndexedStep _CurrentStep;                         // _CurrentProcessTask 對應的 TestStep

        // UI 命令與非阻塞計時資料；這些是命令或時間，不是額外的流程 Flag。
        private TASK_STATUS _PendingCommand;                      // UI 傳給下一次掃描處理的命令信箱
        private DateTime _StageEndAt;                             // Stage Duration 預定結束時間
        private DateTime _NextExecutionAt;                        // RepeatSequence 下一輪開始時間
        private DateTime _PausedAt;                               // 計算暫停時間並補回所有計時器

        /// <summary>單一 PreparationMonitor 的執行資料。</summary>
        private sealed class MonitorRuntime
        {
            public PreparationMonitorStep Plan { get; set; }      // Monitor 對應步驟與執行間隔
            public IAgingProcessTask ProcessTask { get; set; }    // 目前執行中的 Monitor ProcessTask
            public DateTime NextRunAt { get; set; }               // 下一次允許建立 ProcessTask 的時間
        }

        public bool IsActive
        {
            get
            {
                lock (_SyncRoot)
                    return IsRunningState(State);
            }
        }

        public void Start(TestSettingData setting)
        {
            lock (_SyncRoot)
            {
                if (IsRunningState(State))
                    throw new InvalidOperationException($"Box {_BoxIndex + 1} is already running.");

                ResetRuntimeState();

                try
                {
                    List<StageExecutionPlan> previewStagePlans =
                        _PlanBuilder.BuildPreview(setting);
                    Tool.SaveLogToFile(
                        $"Box {_BoxIndex + 1} schedule preview created: " +
                        $"{previewStagePlans.Count} Stage(s)");

                    List<StageExecutionPlan> stagePlans =
                        _PlanBuilder.Build(setting);

                    _Setting = setting;
                    _StagePlans = stagePlans;
                    Transition(WORK.LOAD_STAGE);
                    Tool.SaveLogToFile($"Box {_BoxIndex + 1} schedule started");
                }
                catch (Exception ex)
                {
                    FailInternal(ex);
                    throw;
                }
            }
        }

        /// <summary>由 UI 執行緒放入命令，實際處理由下一次排程掃描執行。</summary>
        public void RequestCommand(TASK_STATUS command)
        {
            if (command == TASK_STATUS.NONE)
                return;
            if (command != TASK_STATUS.PAUSE &&
                command != TASK_STATUS.CONTINUE &&
                command != TASK_STATUS.ABORT)
            {
                throw new ArgumentOutOfRangeException(nameof(command));
            }

            lock (_SyncRoot)
            {
                if (!IsRunningState(State))
                    return;

                // ABORT 優先權最高，後來的 PAUSE/CONTINUE 不可覆蓋。
                if (_PendingCommand != TASK_STATUS.ABORT ||
                    command == TASK_STATUS.ABORT)
                {
                    _PendingCommand = command;
                }
            }
        }

        public override TASK_STATUS Run(TASK_STATUS command = TASK_STATUS.NONE)
        {
            lock (_SyncRoot)
            {
                if (command != TASK_STATUS.NONE)
                    _PendingCommand = command;

                if (!IsRunningState(State))
                    return GetStatus();

                TASK_STATUS scanCommand = _PendingCommand;
                _PendingCommand = TASK_STATUS.NONE;
                RunLoop(scanCommand);
                return GetStatus();
            }
        }

        protected override void RunLoop(TASK_STATUS taskCommand)
        {
            if (HandleCommand(taskCommand))
                return;

            try
            {
                DateTime now = DateTime.Now;
                if (IsPreparationState(State))
                    RunPreparationMonitors(now);

                switch (State)
                {
                    case WORK.LOAD_STAGE:
                        LoadStage(now);
                        break;

                    case WORK.START_PREPARATION_STEP:
                        StartPreparationStep();
                        break;

                    case WORK.WAIT_PREPARATION_STEP:
                        RunCurrentStep(WORK.START_PREPARATION_STEP);
                        break;

                    case WORK.FINISH_PREPARATION:
                        FinishPreparation(now);
                        break;

                    case WORK.START_EXECUTION_STEP:
                        StartExecutionStep(now);
                        break;

                    case WORK.WAIT_EXECUTION_STEP:
                        RunCurrentStep(WORK.START_EXECUTION_STEP);
                        break;

                    case WORK.WAIT_EXECUTION_INTERVAL:
                        ScanExecutionInterval(now);
                        break;

                    case WORK.WAIT_STAGE_DURATION:
                        ScanStageDuration(now);
                        break;

                    case WORK.COMPLETE_STAGE:
                        CompleteStage();
                        break;

                }
            }
            catch (Exception ex)
            {
                FailInternal(ex);
            }
        }

        public override void GoToPause()
        {
            lock (_SyncRoot)
            {
                if (!IsRunningState(State) || State == WORK.PAUSE)
                    return;

                SetPauseState(State);
                _PausedAt = DateTime.Now;
                SendCommandToProcessTasks(TASK_STATUS.PAUSE);
                Transition(WORK.PAUSE);
                Tool.SaveLogToFile($"Box {_BoxIndex + 1} schedule paused");
            }
        }

        protected override WORK AbortState(WORK target)
        {
            return WORK.ABORT;
        }

        protected override void Transition(WORK target)
        {
            WORK previous = State;
            if (previous != target)
            {
                Tool.SaveLogToFile(
                    $"[Task]({TaskName}) {previous} -> {target} " +
                    $"Stage={GetCurrentStageId()} No={GetCurrentStepNo()}");
            }

            State = target;
            SetStatus(GetTaskStatus(target));
            _Status.SetTaskState(GetStatus(), State.ToString());
        }

        public void FailFromScheduler(Exception exception)
        {
            lock (_SyncRoot)
                FailInternal(exception);
        }

        private void ResetRuntimeState()
        {
            _StageIndex = 0;
            ClearCurrentStep();
            _MonitorTasks.Clear();
            _PendingSteps.Clear();
            _PendingCommand = TASK_STATUS.NONE;
            ResetStageTiming();
            _Status.Reset(WORK.NONE.ToString());
            Transition(WORK.NONE);
        }

        /// <summary>處理 Pause、Continue、Abort；回傳 true 表示本次掃描不再執行一般流程。</summary>
        private bool HandleCommand(TASK_STATUS command)
        {
            if (command == TASK_STATUS.ABORT)
            {
                AbortInternal();
                return true;
            }

            if (command == TASK_STATUS.PAUSE && State != WORK.PAUSE)
            {
                GoToPause();
                return true;
            }

            if (State != WORK.PAUSE)
                return false;

            if (command == TASK_STATUS.CONTINUE)
                ResumeInternal();

            return true;
        }

        private void LoadStage(DateTime now)
        {
            if (_StageIndex >= _StagePlans.Count)
            {
                Transition(WORK.SUCCESS);
                Tool.SaveLogToFile($"Box {_BoxIndex + 1} schedule completed");
                return;
            }

            StageExecutionPlan plan = CurrentStagePlan;
            InitializeStageRuntime(plan, now);
            _Status.SetCurrentStage(plan.Stage.Id);
            Tool.SaveLogToFile(
                $"Box {_BoxIndex + 1} Stage {plan.Stage.Id} Preparation START");
            Transition(WORK.START_PREPARATION_STEP);
        }

        private void InitializeStageRuntime(StageExecutionPlan plan, DateTime now)
        {
            ClearCurrentStep();
            ResetStageTiming();
            InitializeMonitors(plan, now);
            LoadPendingSteps(plan.PreparationSteps);
        }

        private void ClearCurrentStep()
        {
            _CurrentProcessTask = null;
            _CurrentStep = null;
        }

        private void ResetStageTiming()
        {
            _StageEndAt = DateTime.MinValue;
            _NextExecutionAt = DateTime.MinValue;
        }

        private void InitializeMonitors(StageExecutionPlan plan, DateTime now)
        {
            _MonitorTasks.Clear();
            foreach (PreparationMonitorStep monitorPlan in plan.PreparationMonitorSteps)
            {
                _MonitorTasks.Add(new MonitorRuntime
                {
                    Plan = monitorPlan,
                    NextRunAt = now
                });
            }
        }

        private void LoadPendingSteps(IEnumerable<IndexedStep> steps)
        {
            _PendingSteps.Clear();
            foreach (IndexedStep step in steps)
                _PendingSteps.Enqueue(step);
        }

        private void StartPreparationStep()
        {
            if (_PendingSteps.Count == 0)
            {
                Transition(WORK.FINISH_PREPARATION);
                return;
            }

            StartMainStep(_PendingSteps.Peek(), WORK.WAIT_PREPARATION_STEP);
        }

        private void FinishPreparation(DateTime now)
        {
            // 不再啟動新 Monitor，但已開始的 Monitor 必須先完成。
            if (_MonitorTasks.Any(item => item.ProcessTask != null))
                return;

            StageExecutionPlan plan = CurrentStagePlan;
            Tool.SaveLogToFile(
                $"Box {_BoxIndex + 1} Stage {plan.Stage.Id} Preparation END");

            _StageEndAt = now.Add(plan.Duration);
            _Status.SetStageEndTime(_StageEndAt);
            LoadPendingSteps(plan.ExecutionSteps);

            if (_PendingSteps.Count > 0)
                Transition(WORK.START_EXECUTION_STEP);
            else
                EnterStageDurationWait(now);
        }

        private void StartExecutionStep(DateTime now)
        {
            StageExecutionPlan plan = CurrentStagePlan;
            if (_PendingSteps.Count > 0)
            {
                StartMainStep(_PendingSteps.Peek(), WORK.WAIT_EXECUTION_STEP);
                return;
            }

            if (plan.ExecutionMode == ExecutionRepeatMode.Once)
            {
                EnterStageDurationWait(now);
                return;
            }

            if (now >= _StageEndAt)
            {
                Transition(WORK.COMPLETE_STAGE);
                return;
            }

            TimeSpan remaining = _StageEndAt - now;
            TimeSpan delay = plan.ExecutionInterval < remaining
                ? plan.ExecutionInterval
                : remaining;

            _NextExecutionAt = now.Add(delay);
            _Status.SetWaitUntil(_NextExecutionAt);
            Tool.SaveLogToFile(
                $"Box {_BoxIndex + 1} Stage {plan.Stage.Id} " +
                $"ExecutionInterval WAIT {delay.TotalSeconds} sec");
            Transition(WORK.WAIT_EXECUTION_INTERVAL);
        }

        private void ScanExecutionInterval(DateTime now)
        {
            if (now < _NextExecutionAt)
                return;

            _Status.SetWaitUntil(DateTime.MinValue);
            if (now >= _StageEndAt)
            {
                Transition(WORK.COMPLETE_STAGE);
                return;
            }

            LoadPendingSteps(CurrentStagePlan.ExecutionSteps);
            Transition(WORK.START_EXECUTION_STEP);
        }

        private void EnterStageDurationWait(DateTime now)
        {
            if (now >= _StageEndAt)
            {
                Transition(WORK.COMPLETE_STAGE);
                return;
            }

            _Status.SetWaitUntil(_StageEndAt);
            Tool.SaveLogToFile(
                $"Box {_BoxIndex + 1} Stage {CurrentStagePlan.Stage.Id} " +
                $"StageDuration WAIT {(_StageEndAt - now).TotalSeconds} sec");
            Transition(WORK.WAIT_STAGE_DURATION);
        }

        private void ScanStageDuration(DateTime now)
        {
            if (now < _StageEndAt)
                return;

            _Status.SetWaitUntil(DateTime.MinValue);
            Transition(WORK.COMPLETE_STAGE);
        }

        private void CompleteStage()
        {
            StageExecutionPlan plan = CurrentStagePlan;
            _Status.CompleteStage();
            Tool.SaveLogToFile(
                $"Box {_BoxIndex + 1} Stage {plan.Stage.Id} completed");
            _StageIndex++;
            Transition(WORK.LOAD_STAGE);
        }

        private void StartMainStep(IndexedStep indexedStep, WORK waitingState)
        {
            _CurrentStep = indexedStep;
            _Status.SetCurrentStep(indexedStep.Index, indexedStep.Step);
            _CurrentProcessTask = CreateProcessTask(indexedStep, true);
            _Status.SetProcessState(_CurrentProcessTask.StateName, null);
            Tool.SaveLogToFile(TaskDescription(indexedStep) + " START");
            Transition(waitingState);
        }

        private void RunCurrentStep(WORK successState)
        {
            if (_CurrentProcessTask == null || _CurrentStep == null)
                throw new InvalidOperationException("The current process task was not created.");

            TASK_STATUS result = _CurrentProcessTask.Run(TASK_STATUS.NONE);
            if (result == TASK_STATUS.CONTINUE || result == TASK_STATUS.PAUSE)
                return;

            if (result == TASK_STATUS.FAIL)
            {
                throw new InvalidOperationException(
                    TaskDescription(_CurrentStep) + " failed: " +
                    _CurrentProcessTask.ErrorMessage);
            }

            if (result == TASK_STATUS.ABORT)
            {
                AbortInternal();
                return;
            }

            if (result != TASK_STATUS.SUCCESS)
                throw new InvalidOperationException("The process task returned an invalid status.");

            _Status.CompleteCurrentStep();
            Tool.SaveLogToFile(TaskDescription(_CurrentStep) + " END");
            _PendingSteps.Dequeue();

            if (State == WORK.WAIT_PREPARATION_STEP && _PendingSteps.Count == 0)
                successState = WORK.FINISH_PREPARATION;

            ClearCurrentStep();
            Transition(successState);
        }

        private void RunPreparationMonitors(DateTime now)
        {
            bool canStartNewMonitor = State != WORK.FINISH_PREPARATION;

            foreach (MonitorRuntime runtime in _MonitorTasks)
            {
                if (runtime.ProcessTask == null)
                {
                    if (!canStartNewMonitor || now < runtime.NextRunAt)
                        continue;

                    runtime.ProcessTask = CreateProcessTask(
                        runtime.Plan.IndexedStep,
                        false);
                    Tool.SaveLogToFile(
                        TaskDescription(runtime.Plan.IndexedStep) + " START");
                }

                TASK_STATUS result = runtime.ProcessTask.Run(TASK_STATUS.NONE);
                if (result == TASK_STATUS.CONTINUE || result == TASK_STATUS.PAUSE)
                    continue;

                if (result == TASK_STATUS.FAIL)
                {
                    throw new InvalidOperationException(
                        TaskDescription(runtime.Plan.IndexedStep) + " failed: " +
                        runtime.ProcessTask.ErrorMessage);
                }

                if (result == TASK_STATUS.ABORT)
                    throw new InvalidOperationException("Preparation monitor was aborted.");

                if (result != TASK_STATUS.SUCCESS)
                    throw new InvalidOperationException("Monitor returned an invalid status.");

                Tool.SaveLogToFile(
                    TaskDescription(runtime.Plan.IndexedStep) + " END");
                runtime.ProcessTask = null;
                runtime.NextRunAt = now.Add(runtime.Plan.Interval);

                if (canStartNewMonitor)
                {
                    Tool.SaveLogToFile(
                        TaskDescription(runtime.Plan.IndexedStep) +
                        $" WAIT {runtime.Plan.Interval.TotalSeconds} sec");
                }
            }
        }

        private IAgingProcessTask CreateProcessTask(
            IndexedStep indexedStep,
            bool reportStatus)
        {
            IAgingProcessTaskFactory factory;
            if (!_Factories.TryGetValue(indexedStep.Step.TestProcess, out factory))
            {
                throw new NotSupportedException(
                    $"No {indexedStep.Step.TestProcess} process task factory is registered.");
            }

            return factory.Create(
                _BoxIndex,
                _Setting,
                indexedStep.Step,
                _Status,
                reportStatus);
        }

        private void ResumeInternal()
        {
            DateTime now = DateTime.Now;
            TimeSpan pausedDuration = now - _PausedAt;

            if (_StageEndAt != DateTime.MinValue)
                _StageEndAt = _StageEndAt.Add(pausedDuration);
            if (_NextExecutionAt != DateTime.MinValue)
                _NextExecutionAt = _NextExecutionAt.Add(pausedDuration);

            foreach (MonitorRuntime monitor in _MonitorTasks)
            {
                if (monitor.NextRunAt != DateTime.MinValue)
                    monitor.NextRunAt = monitor.NextRunAt.Add(pausedDuration);
            }

            _Status.ShiftTime(pausedDuration);
            SendCommandToProcessTasks(TASK_STATUS.CONTINUE);
            Transition(GetPauseState());
            Tool.SaveLogToFile($"Box {_BoxIndex + 1} schedule resumed");
        }

        private void AbortInternal()
        {
            SendCommandToProcessTasks(TASK_STATUS.ABORT);
            ClearCurrentStep();
            _PendingSteps.Clear();
            _MonitorTasks.Clear();
            Transition(WORK.ABORT);
            Tool.SaveLogToFile($"Box {_BoxIndex + 1} schedule aborted");
        }

        private void SendCommandToProcessTasks(TASK_STATUS command)
        {
            if (_CurrentProcessTask != null)
                _CurrentProcessTask.Run(command);

            foreach (MonitorRuntime monitor in _MonitorTasks)
            {
                if (monitor.ProcessTask != null)
                    monitor.ProcessTask.Run(command);
            }
        }

        private void FailInternal(Exception exception)
        {
            if (State == WORK.FAIL || State == WORK.SUCCESS || State == WORK.ABORT)
                return;

            SendCommandToProcessTasks(TASK_STATUS.ABORT);
            string message = exception == null
                ? "Unknown aging schedule error."
                : exception.Message;
            _Status.SetError(message);
            Transition(WORK.FAIL);
            Tool.SaveLogToFile(
                $"Box {_BoxIndex + 1} schedule failed: {exception}",
                level: "ERR");
        }

        private StageExecutionPlan CurrentStagePlan
        {
            get { return _StagePlans[_StageIndex]; }
        }

        private string TaskDescription(IndexedStep indexedStep)
        {
            TestStep step = indexedStep.Step;
            return $"Box {_BoxIndex + 1} Stage {step.StageId} No {step.No} " +
                $"{step.StageRole} {step.TestProcess}";
        }

        private string GetCurrentStageId()
        {
            return _StageIndex >= 0 && _StageIndex < _StagePlans.Count
                ? _StagePlans[_StageIndex].Stage.Id
                : "-";
        }

        private string GetCurrentStepNo()
        {
            return _CurrentStep == null ? "-" : _CurrentStep.Step.No;
        }

        private static TASK_STATUS GetTaskStatus(WORK state)
        {
            switch (state)
            {
                case WORK.NONE:
                    return TASK_STATUS.NONE;
                case WORK.SUCCESS:
                    return TASK_STATUS.SUCCESS;
                case WORK.FAIL:
                    return TASK_STATUS.FAIL;
                case WORK.PAUSE:
                    return TASK_STATUS.PAUSE;
                case WORK.ABORT:
                    return TASK_STATUS.ABORT;
                default:
                    return TASK_STATUS.CONTINUE;
            }
        }

        private static bool IsPreparationState(WORK state)
        {
            return state == WORK.START_PREPARATION_STEP ||
                state == WORK.WAIT_PREPARATION_STEP ||
                state == WORK.FINISH_PREPARATION;
        }

        private static bool IsRunningState(WORK state)
        {
            return state != WORK.NONE &&
                state != WORK.SUCCESS &&
                state != WORK.FAIL &&
                state != WORK.ABORT;
        }
    }
}
