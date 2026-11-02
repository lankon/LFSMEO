using System;
using System.Collections.Generic;
using System.Globalization;
using BurnInTester.Logic.TestSetting;

namespace BurnInTester.Logic
{
    /// <summary>
    /// TestStep 與它在原始 TestSteps 中的索引。
    /// </summary>
    public class IndexedStep
    {
        public int Index { get; set; }
        public TestStep Step { get; set; }
    }

    /// <summary>
    /// 一個 PreparationMonitor 的步驟與重複間隔。
    /// </summary>
    public class PreparationMonitorStep
    {
        public IndexedStep IndexedStep { get; set; }
        public TimeSpan Interval { get; set; }
    }

    /// <summary>
    /// 單一 Stage 已驗證完成的步驟分類與執行時間設定。
    /// </summary>
    public class StageExecutionPlan
    {
        public StageExecutionPlan()
        {
            PreparationSteps = new List<IndexedStep>();
            PreparationMonitorSteps = new List<PreparationMonitorStep>();
            ExecutionSteps = new List<IndexedStep>();
        }

        public AgingStage Stage { get; set; }
        public TimeSpan Duration { get; set; }
        public ExecutionRepeatMode ExecutionMode { get; set; }
        public TimeSpan ExecutionInterval { get; set; }
        public List<IndexedStep> PreparationSteps { get; private set; }
        public List<PreparationMonitorStep> PreparationMonitorSteps { get; private set; }
        public List<IndexedStep> ExecutionSteps { get; private set; }
    }

    /// <summary>
    /// 驗證 TestSettingData，並轉換成依序執行的 Stage 計畫。
    /// 本類別只建立計畫，不執行排程，也不存取硬體。
    /// </summary>
    internal sealed class AgingStagePlanBuilder
    {
        public AgingStagePlanBuilder(
            IDictionary<TestProcess, IAgingProcessTaskFactory> factories)
        {
            if (factories == null)
                throw new ArgumentNullException(nameof(factories));

            _Factories = factories;
        }

        private readonly IDictionary<TestProcess, IAgingProcessTaskFactory> _Factories; // 驗證每個測試項目是否已有可執行工廠

        /// <summary>建立預覽計畫，不檢查各測試項目的 ProcessTask 是否已實作。</summary>
        public List<StageExecutionPlan> BuildPreview(TestSettingData setting)
        {
            return BuildPlan(setting, false);
        }

        /// <summary>建立正式執行計畫，所有測試項目都必須已有可執行 ProcessTask。</summary>
        public List<StageExecutionPlan> Build(TestSettingData setting)
        {
            return BuildPlan(setting, true);
        }

        private List<StageExecutionPlan> BuildPlan(
            TestSettingData setting,
            bool validateProcessTask)
        {
            ValidateSetting(setting);

            List<IndexedStep> steps = GetExecutableSteps(setting);
            ValidateStageOrder(steps);

            if (validateProcessTask)
                ValidateProcessConditions(setting, steps);

            List<StageExecutionPlan> plans = CreateStagePlans(setting, steps);
            ValidatePlans(plans);

            return plans;
        }

        private static void ValidateSetting(TestSettingData setting)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));
            if (setting.Stages == null)
                throw new InvalidOperationException("The Stage collection was not found.");
            if (setting.TestSteps == null)
                throw new InvalidOperationException("The TestStep collection was not found.");
        }

        private static List<IndexedStep> GetExecutableSteps(TestSettingData setting)
        {
            List<IndexedStep> result = new List<IndexedStep>();

            for (int index = 0; index < setting.TestSteps.Count; index++)
            {
                TestStep step = setting.TestSteps[index];
                ValidateBasicStep(step, index);

                if (IsSkipped(step))
                    continue;

                result.Add(new IndexedStep
                {
                    Index = index,
                    Step = step
                });
            }

            if (result.Count == 0)
            {
                throw new InvalidOperationException(
                    "The schedule has no executable test step.");
            }

            return result;
        }

        private static void ValidateBasicStep(TestStep step, int index)
        {
            if (step == null)
                throw new InvalidOperationException($"Step {index + 1} has no data.");
            if (IsSkipped(step))
                return;
            if (step.TestProcess == TestProcess.None)
                throw new InvalidOperationException($"Step {index + 1} has no process.");
            if (string.IsNullOrWhiteSpace(step.StageId))
                throw new InvalidOperationException($"Step {index + 1} has no Stage ID.");
        }

        private static void ValidateStageOrder(IList<IndexedStep> steps)
        {
            HashSet<string> completedStageIds = new HashSet<string>();
            string currentStageId = null;
            bool executionSeen = false;

            foreach (IndexedStep indexedStep in steps)
            {
                TestStep step = indexedStep.Step;

                if (currentStageId != step.StageId)
                {
                    if (currentStageId != null)
                        completedStageIds.Add(currentStageId);
                    if (completedStageIds.Contains(step.StageId))
                    {
                        throw new InvalidOperationException(
                            $"Stage {step.StageId} steps must be contiguous.");
                    }

                    currentStageId = step.StageId;
                    executionSeen = false;
                }

                if (step.StageRole == StageStepRole.Execution)
                {
                    executionSeen = true;
                }
                else if (executionSeen)
                {
                    throw new InvalidOperationException(
                        $"Stage {step.StageId} preparation roles must precede Execution.");
                }
            }
        }

        private void ValidateProcessConditions(
            TestSettingData setting,
            IEnumerable<IndexedStep> steps)
        {
            foreach (IndexedStep indexedStep in steps)
            {
                TestStep step = indexedStep.Step;
                IAgingProcessTaskFactory factory = GetFactory(step, indexedStep.Index);

                try
                {
                    factory.Validate(setting, step);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Step {indexedStep.Index + 1} is invalid: {ex.Message}",
                        ex);
                }
            }
        }

        private IAgingProcessTaskFactory GetFactory(TestStep step, int index)
        {
            IAgingProcessTaskFactory factory;
            if (!_Factories.TryGetValue(step.TestProcess, out factory))
            {
                throw new NotSupportedException(
                    $"Step {index + 1} has no {step.TestProcess} process task.");
            }

            return factory;
        }

        private static List<StageExecutionPlan> CreateStagePlans(
            TestSettingData setting,
            IEnumerable<IndexedStep> steps)
        {
            Dictionary<string, AgingStage> stagesById = CreateStageLookup(setting.Stages);
            Dictionary<string, StageExecutionPlan> plansByStageId =
                new Dictionary<string, StageExecutionPlan>();
            List<StageExecutionPlan> plans = new List<StageExecutionPlan>();

            foreach (IndexedStep indexedStep in steps)
            {
                TestStep step = indexedStep.Step;
                AgingStage stage = GetStage(stagesById, step.StageId);
                StageExecutionPlan plan = GetOrCreatePlan(
                    stage,
                    plansByStageId,
                    plans);

                AddStep(plan, indexedStep);
            }

            return plans;
        }

        private static Dictionary<string, AgingStage> CreateStageLookup(
            IEnumerable<AgingStage> stages)
        {
            Dictionary<string, AgingStage> result =
                new Dictionary<string, AgingStage>();

            foreach (AgingStage stage in stages)
            {
                if (stage == null)
                    throw new InvalidOperationException("A Stage has no data.");
                if (string.IsNullOrWhiteSpace(stage.Id))
                    throw new InvalidOperationException("A Stage has no ID.");
                if (result.ContainsKey(stage.Id))
                    throw new InvalidOperationException($"Stage ID is duplicated: {stage.Id}");

                result.Add(stage.Id, stage);
            }

            return result;
        }

        private static AgingStage GetStage(
            IDictionary<string, AgingStage> stagesById,
            string stageId)
        {
            AgingStage stage;
            if (!stagesById.TryGetValue(stageId, out stage))
                throw new InvalidOperationException($"Stage {stageId} was not found.");

            return stage;
        }

        private static StageExecutionPlan GetOrCreatePlan(
            AgingStage stage,
            IDictionary<string, StageExecutionPlan> plansByStageId,
            ICollection<StageExecutionPlan> plans)
        {
            StageExecutionPlan plan;
            if (plansByStageId.TryGetValue(stage.Id, out plan))
                return plan;

            plan = CreateStagePlan(stage);
            plansByStageId.Add(stage.Id, plan);
            plans.Add(plan);
            return plan;
        }

        private static void AddStep(
            StageExecutionPlan plan,
            IndexedStep indexedStep)
        {
            switch (indexedStep.Step.StageRole)
            {
                case StageStepRole.Preparation:
                    plan.PreparationSteps.Add(indexedStep);
                    break;

                case StageStepRole.PreparationMonitor:
                    plan.PreparationMonitorSteps.Add(
                        CreateMonitorStep(indexedStep));
                    break;

                case StageStepRole.Execution:
                    plan.ExecutionSteps.Add(indexedStep);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Step {indexedStep.Index + 1} has an invalid StageRole.");
            }
        }

        private static PreparationMonitorStep CreateMonitorStep(
            IndexedStep indexedStep)
        {
            return new PreparationMonitorStep
            {
                IndexedStep = indexedStep,
                Interval = ParsePositiveSeconds(
                    indexedStep.Step.MonitorIntervalSeconds,
                    $"Step {indexedStep.Index + 1} monitor interval")
            };
        }

        private static void ValidatePlans(IEnumerable<StageExecutionPlan> plans)
        {
            foreach (StageExecutionPlan plan in plans)
                ValidatePlan(plan);
        }

        private static void ValidatePlan(StageExecutionPlan plan)
        {
            if (plan.PreparationMonitorSteps.Count > 0 &&
                plan.PreparationSteps.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Stage {plan.Stage.Id} has a monitor but no Preparation.");
            }

            if (plan.ExecutionMode == ExecutionRepeatMode.RepeatSequence &&
                plan.ExecutionSteps.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Stage {plan.Stage.Id} is RepeatSequence but has no Execution step.");
            }
        }

        private static StageExecutionPlan CreateStagePlan(AgingStage stage)
        {
            if (!Enum.IsDefined(typeof(ExecutionRepeatMode), stage.ExecutionMode))
            {
                throw new InvalidOperationException(
                    $"Stage {stage.Id} has an invalid execution mode.");
            }

            TimeSpan duration = ParseStageDuration(stage);
            TimeSpan executionInterval = TimeSpan.Zero;

            if (stage.ExecutionMode == ExecutionRepeatMode.RepeatSequence)
            {
                if (duration <= TimeSpan.Zero)
                    throw new InvalidOperationException($"Stage {stage.Id} duration must be positive.");

                executionInterval = ParsePositiveSeconds(
                    stage.ExecutionIntervalSeconds,
                    $"Stage {stage.Id} execution interval");
            }

            return new StageExecutionPlan
            {
                Stage = stage,
                Duration = duration,
                ExecutionMode = stage.ExecutionMode,
                ExecutionInterval = executionInterval
            };
        }

        private static TimeSpan ParseStageDuration(AgingStage stage)
        {
            double durationMinutes = ParseNumber(
                stage.Duration,
                $"Stage {stage.Id} duration");

            if (durationMinutes < 0)
                throw new InvalidOperationException($"Stage {stage.Id} duration cannot be negative.");

            return CreateTimeSpanFromMinutes(
                durationMinutes,
                $"Stage {stage.Id} duration");
        }

        private static TimeSpan ParsePositiveSeconds(string value, string parameterName)
        {
            double seconds = ParseNumber(value, parameterName);
            if (seconds <= 0)
                throw new InvalidOperationException($"{parameterName} must be greater than zero.");

            try
            {
                return TimeSpan.FromSeconds(seconds);
            }
            catch (OverflowException)
            {
                throw new InvalidOperationException($"{parameterName} is out of range.");
            }
        }

        private static double ParseNumber(string value, string parameterName)
        {
            double number;
            bool success = double.TryParse(
                value,
                NumberStyles.Float,
                CultureInfo.CurrentCulture,
                out number);

            if (!success)
            {
                success = double.TryParse(
                    value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out number);
            }

            if (!success || double.IsNaN(number) || double.IsInfinity(number))
                throw new InvalidOperationException($"Invalid {parameterName}: {value}");

            return number;
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

        private static bool IsSkipped(TestStep step)
        {
            bool skip;
            return bool.TryParse(step.Skip, out skip) && skip;
        }
    }
}
