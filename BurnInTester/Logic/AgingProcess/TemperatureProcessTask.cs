using System;
using System.Globalization;
using System.Linq;
using BurnInTester.Base;
using BurnInTester.Logic.TestSetting;
using ToolFunction;

namespace BurnInTester.Logic
{
    /// <summary>
    /// 驗證 TemperatureCondition，並建立 AgingBoxTask 使用的非阻塞溫控狀態機。
    /// </summary>
    public sealed class TemperatureProcessTaskFactory : IAgingProcessTaskFactory
    {
        public TemperatureProcessTaskFactory(
            IBaseTaskDependence dependencies,
            AgingInformation agingInformation,
            TC_Task temperatureTask)
        {
            if (dependencies == null)
                throw new ArgumentNullException(nameof(dependencies));
            if (agingInformation == null)
                throw new ArgumentNullException(nameof(agingInformation));
            if (temperatureTask == null)
                throw new ArgumentNullException(nameof(temperatureTask));

            _Dependencies = dependencies;
            _AgingInformation = agingInformation;
            _TemperatureTask = temperatureTask;
        }

        private readonly IBaseTaskDependence _Dependencies;
        private readonly AgingInformation _AgingInformation;
        private readonly TC_Task _TemperatureTask;

        public TestProcess Process
        {
            get { return TestProcess.Temperature; }
        }

        public void Validate(TestSettingData setting, TestStep step)
        {
            GetParameters(setting, step);
        }

        public IAgingProcessTask Create(
            int boxIndex,
            TestSettingData setting,
            TestStep step,
            AgingProcessStatus status,
            bool reportStatus)
        {
            return new TemperatureProcessTask(
                _Dependencies,
                boxIndex,
                step,
                GetParameters(setting, step),
                _AgingInformation,
                _TemperatureTask,
                status,
                reportStatus);
        }

        private static TemperatureParameters GetParameters(
            TestSettingData setting,
            TestStep step)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));
            if (step == null)
                throw new ArgumentNullException(nameof(step));

            TemperatureCondition condition = setting.Conditions
                .OfType<TemperatureCondition>()
                .FirstOrDefault(item => item.TestStepId == step.Id);

            if (condition == null)
            {
                throw new InvalidOperationException(
                    $"Temperature condition {step.Id} was not found.");
            }

            double targetTemperature = ParseNumber(
                condition.TargetTemperature,
                "TargetTemperature");
            double stabilizationWaitMinutes = ParseNumber(
                condition.StabilizationWaitTime,
                "StabilizationWaitTime");
            double temperatureTolerance = ParseNumber(
                condition.TemperatureTolerance,
                "TemperatureTolerance");
            double stabilizationTimeoutMinutes = ParseNumber(
                condition.StabilizationTimeout,
                "StabilizationTimeout");

            if (stabilizationWaitMinutes < 0)
                throw new InvalidOperationException("StabilizationWaitTime cannot be negative.");
            if (temperatureTolerance < 0)
                throw new InvalidOperationException("TemperatureTolerance cannot be negative.");
            if (stabilizationTimeoutMinutes <= 0)
                throw new InvalidOperationException("StabilizationTimeout must be positive.");
            if (stabilizationWaitMinutes > stabilizationTimeoutMinutes)
            {
                throw new InvalidOperationException(
                    "StabilizationWaitTime cannot exceed StabilizationTimeout.");
            }

            return new TemperatureParameters
            {
                TargetTemperature = targetTemperature,
                TemperatureTolerance = temperatureTolerance,
                StabilizationWaitTime = CreateTimeSpanFromMinutes(
                    stabilizationWaitMinutes,
                    "StabilizationWaitTime"),
                StabilizationTimeout = CreateTimeSpanFromMinutes(
                    stabilizationTimeoutMinutes,
                    "StabilizationTimeout")
            };
        }

        private static double ParseNumber(string value, string parameterName)
        {
            double result;
            bool success = double.TryParse(
                value,
                NumberStyles.Float,
                CultureInfo.CurrentCulture,
                out result);

            if (!success)
            {
                success = double.TryParse(
                    value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out result);
            }

            if (!success || double.IsNaN(result) || double.IsInfinity(result))
                throw new InvalidOperationException($"Invalid {parameterName}: {value}");

            return result;
        }

        private static TimeSpan CreateTimeSpanFromMinutes(
            double minutes,
            string parameterName)
        {
            try
            {
                return TimeSpan.FromMinutes(minutes);
            }
            catch (OverflowException)
            {
                throw new InvalidOperationException($"{parameterName} is out of range.");
            }
        }

        internal sealed class TemperatureParameters
        {
            public double TargetTemperature { get; set; }
            public double TemperatureTolerance { get; set; }
            public TimeSpan StabilizationWaitTime { get; set; }
            public TimeSpan StabilizationTimeout { get; set; }
        }
    }

    /// <summary>
    /// 發送一次溫度命令，並以非阻塞掃描方式等待最新 PV 達到穩態。
    /// 每次 Run 只檢查一次狀態，不會 Delay、Sleep 或等待硬體回傳。
    /// </summary>
    internal sealed class TemperatureProcessTask :
        IBaseTask<TemperatureProcessTask.WORK>, IAgingProcessTask
    {
        /// <summary>Temperature ProcessTask 的唯一流程狀態。</summary>
        public enum WORK
        {
            NONE,                           // 尚未初始化
            START,                          // 將目標溫度命令排入 TC_Task
            WAIT_FRESH_TEMPERATURE,         // 等待送出命令後的新 PV
            WAIT_TEMPERATURE_TOLERANCE,     // 等待 PV 進入容許誤差
            WAIT_STABLE_TIME,               // PV 在容許誤差內持續等待穩態時間
            SUCCESS,                        // 溫度已達穩態
            FAIL,                           // 等待逾時或資料錯誤
            PAUSE,                          // 使用者暫停
            ABORT,                          // 使用者放棄
            CONTINUE                        // 保留給原有狀態機風格；Resume 後直接回原 WORK
        }

        public TemperatureProcessTask(
            IBaseTaskDependence dependencies,
            int boxIndex,
            TestStep step,
            TemperatureProcessTaskFactory.TemperatureParameters parameters,
            AgingInformation agingInformation,
            TC_Task temperatureTask,
            AgingProcessStatus status,
            bool reportStatus)
            : base(dependencies)
        {
            if (step == null)
                throw new ArgumentNullException(nameof(step));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (agingInformation == null)
                throw new ArgumentNullException(nameof(agingInformation));
            if (temperatureTask == null)
                throw new ArgumentNullException(nameof(temperatureTask));
            if (status == null)
                throw new ArgumentNullException(nameof(status));

            TaskName = $"TemperatureProcessTask[{boxIndex + 1}:{step.No}]";
            _BoxIndex = boxIndex;
            _Step = step;
            _Parameters = parameters;
            _AgingInformation = agingInformation;
            _TemperatureTask = temperatureTask;
            _Status = status;
            _ReportStatus = reportStatus;
            _ErrorMessage = "";
            State = WORK.START;
            SetStatus(TASK_STATUS.CONTINUE);
        }

        private static readonly TimeSpan TemperatureCheckInterval =
            TimeSpan.FromMilliseconds(500);                     // PV 取樣檢查週期

        // 建立後不會改變的測試資料與相依物件。
        private readonly int _BoxIndex;                         // 溫控箱索引（0-based）
        private readonly TestStep _Step;                        // 此 ProcessTask 對應的測試步驟
        private readonly TemperatureProcessTaskFactory.TemperatureParameters _Parameters; // 溫控判定參數
        private readonly AgingInformation _AgingInformation;    // 讀取此 Box 最新 PV
        private readonly TC_Task _TemperatureTask;              // 將設定溫度命令排入硬體佇列
        private readonly AgingProcessStatus _Status;            // 主步驟才回報狀態；Monitor 不覆蓋主畫面
        private readonly bool _ReportStatus;                    // true 表示此 Task 是目前主步驟

        // 非阻塞等待必須跨掃描保留的時間資料。
        private DateTime _CommandQueuedTime;                    // 溫度命令進入 TC_Task 的時間
        private DateTime _TimeoutAt;                            // 等待溫度穩定的最後期限
        private DateTime _StableSince;                          // PV 連續位於容許誤差內的起始時間
        private DateTime _NextCheckAt;                          // 下一次允許檢查 PV 的時間
        private DateTime _PausedAt;                             // Resume 時計算暫停經過時間
        private string _ErrorMessage;                           // FAIL 原因

        public string StateName
        {
            get { return State.ToString(); }
        }

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
        }

        protected override void RunLoop(TASK_STATUS taskCommand)
        {
            if (taskCommand == TASK_STATUS.ABORT)
            {
                Transition(WORK.ABORT);
                return;
            }

            if (taskCommand == TASK_STATUS.PAUSE && State != WORK.PAUSE)
            {
                GoToPause();
                return;
            }

            if (State == WORK.PAUSE)
            {
                if (taskCommand == TASK_STATUS.CONTINUE)
                    ResumeInternal();
                return;
            }

            try
            {
                switch (State)
                {
                    case WORK.START:
                        StartTemperatureCommand();
                        break;

                    case WORK.WAIT_FRESH_TEMPERATURE:
                    case WORK.WAIT_TEMPERATURE_TOLERANCE:
                    case WORK.WAIT_STABLE_TIME:
                        ScanTemperature();
                        break;

                }
            }
            catch (Exception ex)
            {
                Fail(ex.Message);
            }
        }

        public override void GoToPause()
        {
            if (IsFinished(State) || State == WORK.PAUSE)
                return;

            SetPauseState(State);
            _PausedAt = DateTime.Now;
            Transition(WORK.PAUSE);
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
                    $"[Task]({TaskName}) {previous} -> {target}");
            }

            State = target;
            SetStatus(GetTaskStatus(target));
            UpdateStatus(GetWaitUntil(target));
        }

        private void StartTemperatureCommand()
        {
            _CommandQueuedTime = DateTime.Now;
            _TimeoutAt = _CommandQueuedTime.Add(_Parameters.StabilizationTimeout);
            _NextCheckAt = _CommandQueuedTime;
            _TemperatureTask.Start(
                _Parameters.TargetTemperature,
                box_num: _BoxIndex);

            Tool.SaveLogToFile(
                $"Box {_BoxIndex + 1} No {_Step.No} Temperature command queued: " +
                $"SV={_Parameters.TargetTemperature}");
            Transition(WORK.WAIT_FRESH_TEMPERATURE);
        }

        private void ScanTemperature()
        {
            DateTime now = DateTime.Now;
            if (now < _NextCheckAt)
                return;

            _NextCheckAt = now.Add(TemperatureCheckInterval);
            if (now >= _TimeoutAt)
            {
                Fail(
                    $"Temperature did not stabilize at " +
                    $"{_Parameters.TargetTemperature} C in time.");
                return;
            }

            double[] temperatures;
            DateTime recordTime;
            _AgingInformation.TemperatureInfos[_BoxIndex]
                .GetPVSnapshot(out temperatures, out recordTime);

            bool hasFreshTemperature = recordTime >= _CommandQueuedTime;
            if (!hasFreshTemperature || temperatures.Length == 0)
            {
                _StableSince = DateTime.MinValue;
                Transition(WORK.WAIT_FRESH_TEMPERATURE);
                return;
            }

            bool isInTolerance =
                Math.Abs(temperatures[0] - _Parameters.TargetTemperature) <=
                _Parameters.TemperatureTolerance;
            if (!isInTolerance)
            {
                _StableSince = DateTime.MinValue;
                Transition(WORK.WAIT_TEMPERATURE_TOLERANCE);
                return;
            }

            if (_StableSince == DateTime.MinValue)
                _StableSince = now;

            TimeSpan stableTime = now - _StableSince;
            if (stableTime >= _Parameters.StabilizationWaitTime)
            {
                Tool.SaveLogToFile(
                    $"Box {_BoxIndex + 1} No {_Step.No} Temperature stable");
                Transition(WORK.SUCCESS);
                return;
            }

            Transition(WORK.WAIT_STABLE_TIME);
        }

        private void ResumeInternal()
        {
            ResumeTiming(DateTime.Now - _PausedAt);
            Transition(GetPauseState());
        }

        private void Fail(string message)
        {
            _ErrorMessage = string.IsNullOrWhiteSpace(message)
                ? "Temperature task failed."
                : message;
            Transition(WORK.FAIL);
        }

        private void UpdateStatus(DateTime? waitUntil)
        {
            if (_ReportStatus)
                _Status.SetProcessState(State.ToString(), waitUntil);
        }

        private DateTime? GetWaitUntil(WORK state)
        {
            if (state == WORK.WAIT_FRESH_TEMPERATURE ||
                state == WORK.WAIT_TEMPERATURE_TOLERANCE)
            {
                return _TimeoutAt;
            }

            if (state == WORK.WAIT_STABLE_TIME)
            {
                TimeSpan stableTime = _StableSince == DateTime.MinValue
                    ? TimeSpan.Zero
                    : DateTime.Now - _StableSince;
                TimeSpan remaining = _Parameters.StabilizationWaitTime - stableTime;
                return DateTime.Now.Add(remaining > TimeSpan.Zero
                    ? remaining
                    : TimeSpan.Zero);
            }

            return null;
        }

        private void ResumeTiming(TimeSpan pausedDuration)
        {
            if (_TimeoutAt != DateTime.MinValue)
                _TimeoutAt = _TimeoutAt.Add(pausedDuration);
            if (_StableSince != DateTime.MinValue)
                _StableSince = _StableSince.Add(pausedDuration);
            if (_NextCheckAt != DateTime.MinValue)
                _NextCheckAt = _NextCheckAt.Add(pausedDuration);
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

        private static bool IsFinished(WORK state)
        {
            return state == WORK.SUCCESS ||
                state == WORK.FAIL ||
                state == WORK.ABORT;
        }
    }
}
