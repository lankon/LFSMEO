using System;
using BurnInTester.Base;
using BurnInTester.Logic.TestSetting;

namespace BurnInTester.Logic
{
    /// <summary>提供 UI 使用的老化排程執行結果。</summary>
    public enum AgingRunState
    {
        Idle,
        Running,
        Paused,
        Completed,
        Failed,
        Aborted
    }

    /// <summary>由目前 Process WORK 推導出的溫控步驟階段。</summary>
    public enum AgingStepPhase
    {
        None,
        SendingTemperatureCommand,
        WaitingTemperatureStable,
        StabilizationWait,
        StepCompleted
    }

    /// <summary>由目前 Box WORK 推導出的 Stage 階段。</summary>
    public enum AgingStagePhase
    {
        None,
        Preparing,
        Running,
        Completed
    }

    /// <summary>由 Box WORK 與 Process WORK 推導出的等待原因。</summary>
    public enum AgingWaitReason
    {
        None,
        ExecutingTestProcess,
        WaitingHardwareResult,
        WaitingFreshTemperature,
        WaitingTemperatureTolerance,
        WaitingTemperatureStableTime,
        PreparationMonitorInterval,
        ExecutionInterval,
        StageDuration
    }

    /// <summary>單一溫控箱在某個時間點的唯讀狀態。</summary>
    public sealed class AgingProcessStatusSnapshot
    {
        internal AgingProcessStatusSnapshot(
            int boxIndex,
            AgingRunState runState,
            string currentWorkState,
            string currentProcessState,
            string currentStageId,
            AgingStagePhase stagePhase,
            TimeSpan stageRemainingTime,
            AgingStepPhase stepPhase,
            int currentStepIndex,
            string currentStepNo,
            string currentStepId,
            StageStepRole currentStageRole,
            TestProcess currentProcess,
            DateTime stepStartTime,
            AgingWaitReason waitReason,
            DateTime waitStartedAt,
            DateTime waitUntil,
            TimeSpan remainingTime,
            DateTime lastProgressTime,
            string errorMessage)
        {
            BoxIndex = boxIndex;
            RunState = runState;
            CurrentWorkState = currentWorkState;
            CurrentProcessState = currentProcessState;
            CurrentStageId = currentStageId;
            StagePhase = stagePhase;
            StageRemainingTime = stageRemainingTime;
            StepPhase = stepPhase;
            CurrentStepIndex = currentStepIndex;
            CurrentStepNo = currentStepNo;
            CurrentStepId = currentStepId;
            CurrentStageRole = currentStageRole;
            CurrentProcess = currentProcess;
            StepStartTime = stepStartTime;
            WaitReason = waitReason;
            WaitStartedAt = waitStartedAt;
            WaitUntil = waitUntil;
            RemainingTime = remainingTime;
            LastProgressTime = lastProgressTime;
            ErrorMessage = errorMessage;
        }

        public int BoxIndex { get; private set; }
        public AgingRunState RunState { get; private set; }
        public string CurrentWorkState { get; private set; }
        public string CurrentProcessState { get; private set; }
        public string CurrentStageId { get; private set; }
        public AgingStagePhase StagePhase { get; private set; }
        public TimeSpan StageRemainingTime { get; private set; }
        public AgingStepPhase StepPhase { get; private set; }
        public int CurrentStepIndex { get; private set; }
        public string CurrentStepNo { get; private set; }
        public string CurrentStepId { get; private set; }
        public StageStepRole CurrentStageRole { get; private set; }
        public TestProcess CurrentProcess { get; private set; }
        public DateTime StepStartTime { get; private set; }
        public AgingWaitReason WaitReason { get; private set; }
        public DateTime WaitStartedAt { get; private set; }
        public DateTime WaitUntil { get; private set; }
        public TimeSpan RemainingTime { get; private set; }
        public DateTime LastProgressTime { get; private set; }
        public string ErrorMessage { get; private set; }

        public string DebugLocation
        {
            get
            {
                return $"Box {BoxIndex + 1} | State={RunState}/{CurrentWorkState} | " +
                    $"ProcessState={CurrentProcessState} | Stage={CurrentStageId} | " +
                    $"No={CurrentStepNo} | {CurrentStageRole}/{CurrentProcess}";
            }
        }

        public override string ToString()
        {
            return DebugLocation;
        }
    }

    /// <summary>
    /// 儲存一個溫控箱的顯示資訊。
    /// 這裡不控制流程；RunState、StagePhase、StepPhase、WaitReason 都由實際 WORK 推導。
    /// </summary>
    public sealed class AgingProcessStatus
    {
        public AgingProcessStatus(int boxIndex)
        {
            _BoxIndex = boxIndex;
            Reset("NONE");
        }

        private readonly object _SyncRoot = new object(); // 保護 UI 與排程執行緒共用的顯示資料
        private readonly int _BoxIndex;                   // 此狀態所屬的溫控箱索引（0-based）

        private TASK_STATUS _TaskStatus;                  // AgingBoxTask 最近一次回傳的狀態
        private string _CurrentWorkState;                 // AgingBoxTask 目前的 WORK 名稱
        private string _CurrentProcessState;              // 目前 ProcessTask 的 WORK 名稱
        private string _CurrentStageId;                   // 目前執行的 Stage ID
        private DateTime _StageEndTime;                   // 目前 Stage 預定結束時間
        private int _CurrentStepIndex;                    // TestSteps 中的原始索引
        private string _CurrentStepNo;                    // UI 顯示用 No
        private string _CurrentStepId;                    // 測試條件對應 ID
        private StageStepRole _CurrentStageRole;          // 目前步驟的 Stage Role
        private TestProcess _CurrentProcess;              // 目前步驟的測試項目
        private DateTime _StepStartTime;                  // 目前步驟開始時間
        private DateTime _WaitStartedAt;                  // 目前 WORK 開始等待的時間
        private DateTime _WaitUntil;                      // 已知的下一次轉移或逾時時間
        private DateTime _LastProgressTime;               // 最後一次 WORK 變化時間
        private string _ErrorMessage;                     // FAIL 原因

        public AgingProcessStatusSnapshot GetSnapshot()
        {
            lock (_SyncRoot)
            {
                DateTime now = DateTime.Now;
                return new AgingProcessStatusSnapshot(
                    _BoxIndex,
                    GetRunState(),
                    _CurrentWorkState,
                    _CurrentProcessState,
                    _CurrentStageId,
                    GetStagePhase(),
                    GetRemainingTime(_StageEndTime, now),
                    GetStepPhase(),
                    _CurrentStepIndex,
                    _CurrentStepNo,
                    _CurrentStepId,
                    _CurrentStageRole,
                    _CurrentProcess,
                    _StepStartTime,
                    GetWaitReason(),
                    _WaitStartedAt,
                    _WaitUntil,
                    GetRemainingTime(_WaitUntil, now),
                    _LastProgressTime,
                    _ErrorMessage);
            }
        }

        internal void Reset(string initialWorkState)
        {
            lock (_SyncRoot)
            {
                _TaskStatus = TASK_STATUS.CONTINUE;
                _CurrentWorkState = initialWorkState ?? "NONE";
                _CurrentProcessState = "";
                _CurrentStageId = "";
                _StageEndTime = DateTime.MinValue;
                _CurrentStepIndex = -1;
                _CurrentStepNo = "";
                _CurrentStepId = "";
                _CurrentStageRole = StageStepRole.Execution;
                _CurrentProcess = TestProcess.None;
                _StepStartTime = DateTime.MinValue;
                _WaitStartedAt = DateTime.MinValue;
                _WaitUntil = DateTime.MinValue;
                _LastProgressTime = DateTime.Now;
                _ErrorMessage = "";
            }
        }

        internal void SetTaskState(TASK_STATUS taskStatus, string workState)
        {
            lock (_SyncRoot)
            {
                string newState = workState ?? "";
                if (_CurrentWorkState != newState)
                {
                    _CurrentWorkState = newState;
                    _WaitStartedAt = IsWaitingState(newState)
                        ? DateTime.Now
                        : DateTime.MinValue;
                    _LastProgressTime = DateTime.Now;
                }

                _TaskStatus = taskStatus;
            }
        }

        internal void SetCurrentStage(string stageId)
        {
            lock (_SyncRoot)
            {
                _CurrentStageId = stageId ?? "";
                _StageEndTime = DateTime.MinValue;
                ClearCurrentStepValues();
                ClearWaitValues();
                _LastProgressTime = DateTime.Now;
            }
        }

        internal void SetStageEndTime(DateTime stageEndTime)
        {
            lock (_SyncRoot)
            {
                _StageEndTime = stageEndTime;
                _LastProgressTime = DateTime.Now;
            }
        }

        internal void SetCurrentStep(int stepIndex, TestStep step)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));

            lock (_SyncRoot)
            {
                _CurrentStepIndex = stepIndex;
                _CurrentStepNo = step.No ?? "";
                _CurrentStepId = step.Id ?? "";
                _CurrentStageRole = step.StageRole;
                _CurrentProcess = step.TestProcess;
                _StepStartTime = DateTime.Now;
                _CurrentProcessState = "";
                ClearWaitValues();
                _LastProgressTime = DateTime.Now;
            }
        }

        internal void SetProcessState(string processState, DateTime? waitUntil)
        {
            lock (_SyncRoot)
            {
                string newState = processState ?? "";
                if (_CurrentProcessState != newState)
                {
                    _CurrentProcessState = newState;
                    _WaitStartedAt = IsWaitingState(newState)
                        ? DateTime.Now
                        : DateTime.MinValue;
                    _LastProgressTime = DateTime.Now;
                }

                _WaitUntil = waitUntil ?? DateTime.MinValue;
            }
        }

        internal void SetWaitUntil(DateTime waitUntil)
        {
            lock (_SyncRoot)
            {
                _WaitUntil = waitUntil;
                _WaitStartedAt = waitUntil == DateTime.MinValue
                    ? DateTime.MinValue
                    : DateTime.Now;
                _LastProgressTime = DateTime.Now;
            }
        }

        internal void CompleteCurrentStep()
        {
            lock (_SyncRoot)
            {
                _CurrentProcessState = "SUCCESS";
                ClearWaitValues();
                _LastProgressTime = DateTime.Now;
            }
        }

        internal void CompleteStage()
        {
            lock (_SyncRoot)
            {
                _StageEndTime = DateTime.MinValue;
                ClearWaitValues();
                _LastProgressTime = DateTime.Now;
            }
        }

        internal void ShiftTime(TimeSpan duration)
        {
            if (duration <= TimeSpan.Zero)
                return;

            lock (_SyncRoot)
            {
                if (_StageEndTime != DateTime.MinValue)
                    _StageEndTime = _StageEndTime.Add(duration);
                if (_WaitUntil != DateTime.MinValue)
                    _WaitUntil = _WaitUntil.Add(duration);
                _LastProgressTime = DateTime.Now;
            }
        }

        internal void SetError(string errorMessage)
        {
            lock (_SyncRoot)
            {
                _ErrorMessage = errorMessage ?? "";
                _LastProgressTime = DateTime.Now;
            }
        }

        private AgingRunState GetRunState()
        {
            switch (_TaskStatus)
            {
                case TASK_STATUS.SUCCESS:
                    return AgingRunState.Completed;
                case TASK_STATUS.FAIL:
                    return AgingRunState.Failed;
                case TASK_STATUS.PAUSE:
                    return AgingRunState.Paused;
                case TASK_STATUS.ABORT:
                    return AgingRunState.Aborted;
                default:
                    return _CurrentWorkState == "NONE"
                        ? AgingRunState.Idle
                        : AgingRunState.Running;
            }
        }

        private AgingStagePhase GetStagePhase()
        {
            if (string.IsNullOrWhiteSpace(_CurrentStageId))
                return AgingStagePhase.None;
            if (_CurrentWorkState.Contains("PREPARATION"))
                return AgingStagePhase.Preparing;
            if (_CurrentWorkState == "COMPLETE_STAGE" ||
                _CurrentWorkState == "SUCCESS")
            {
                return AgingStagePhase.Completed;
            }

            return AgingStagePhase.Running;
        }

        private AgingStepPhase GetStepPhase()
        {
            switch (_CurrentProcessState)
            {
                case "START":
                    return AgingStepPhase.SendingTemperatureCommand;
                case "WAIT_FRESH_TEMPERATURE":
                case "WAIT_TEMPERATURE_TOLERANCE":
                    return AgingStepPhase.WaitingTemperatureStable;
                case "WAIT_STABLE_TIME":
                    return AgingStepPhase.StabilizationWait;
                case "SUCCESS":
                    return AgingStepPhase.StepCompleted;
                default:
                    return AgingStepPhase.None;
            }
        }

        private AgingWaitReason GetWaitReason()
        {
            if (_TaskStatus != TASK_STATUS.CONTINUE)
                return AgingWaitReason.None;

            switch (_CurrentProcessState)
            {
                case "WAIT_FRESH_TEMPERATURE":
                    return AgingWaitReason.WaitingFreshTemperature;
                case "WAIT_TEMPERATURE_TOLERANCE":
                    return AgingWaitReason.WaitingTemperatureTolerance;
                case "WAIT_STABLE_TIME":
                    return AgingWaitReason.WaitingTemperatureStableTime;
            }

            if (_CurrentWorkState == "WAIT_EXECUTION_INTERVAL")
                return AgingWaitReason.ExecutionInterval;
            if (_CurrentWorkState == "WAIT_STAGE_DURATION")
                return AgingWaitReason.StageDuration;
            if (!string.IsNullOrWhiteSpace(_CurrentProcessState) &&
                _CurrentProcessState != "SUCCESS")
            {
                return AgingWaitReason.ExecutingTestProcess;
            }

            return AgingWaitReason.None;
        }

        private void ClearCurrentStepValues()
        {
            _CurrentStepIndex = -1;
            _CurrentStepNo = "";
            _CurrentStepId = "";
            _CurrentStageRole = StageStepRole.Execution;
            _CurrentProcess = TestProcess.None;
            _CurrentProcessState = "";
            _StepStartTime = DateTime.MinValue;
        }

        private void ClearWaitValues()
        {
            _WaitStartedAt = DateTime.MinValue;
            _WaitUntil = DateTime.MinValue;
        }

        private static bool IsWaitingState(string state)
        {
            return !string.IsNullOrWhiteSpace(state) &&
                state.StartsWith("WAIT_", StringComparison.Ordinal);
        }

        private static TimeSpan GetRemainingTime(DateTime endTime, DateTime now)
        {
            if (endTime == DateTime.MinValue || endTime <= now)
                return TimeSpan.Zero;

            return endTime - now;
        }
    }
}
