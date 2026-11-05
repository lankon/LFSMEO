using System;
using System.Collections.Generic;
using System.Globalization;

using BurnInTester.Logic.TestSetting;

namespace BurnInTester.Logic
{
    public class StagePlan
    {
        public StagePlan()
        {
            PreparationSteps = new List<PlannedStep>();
            MonitorSteps = new List<MonitorPlan>();
            ExecutionSteps = new List<PlannedStep>();
        }

        public AgingStage Stage { get; set; }
        public TimeSpan Duration { get; set; }
        public ExecutionRepeatMode ExecutionMode { get; set; }
        public TimeSpan ExecutionInterval { get; set; }
        public List<PlannedStep> PreparationSteps { get; private set; }
        public List<MonitorPlan> MonitorSteps { get; private set; }
        public List<PlannedStep> ExecutionSteps { get; private set; }
    }

    public class PlannedStep
    {
        // 已完成分類的測試步驟
        public int OriginalIndex { get; set; }
        public TestStep Step { get; set; }
        public TestCondition Condition { get; set; }
    }

    public class MonitorPlan
    {
        // Preparation 執行期間，需要固定間隔觸發的監控步驟。
        public PlannedStep PlannedStep { get; set; }
        public TimeSpan Interval { get; set; }
    }

    public class AgingStagePlanBuilderNew
    {
        // 將 TestSettingData 依 Stage 與 StageRole 分類
        public AgingStagePlanBuilderNew()
        {
        }

        #region public function
        public List<StagePlan> Classify(TestSettingData setting)
        {
            ValidateSetting(setting);

            Dictionary<string, AgingStage> stagesById = CreateStageLookup(setting.Stages);
            Dictionary<string, TestCondition> conditionsByStepId = CreateConditionLookup(setting.Conditions);
            List<PlannedStep> steps = CreatePlannedSteps(setting.TestSteps, conditionsByStepId);

            ValidateStageOrder(steps);

            List<StagePlan> plans = CreateStagePlans(steps, stagesById);
            ValidateStagePlans(plans);
            return plans;
        }
        #endregion

        #region private function
        // Check
        private void ValidateSetting(TestSettingData setting)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));
            if (setting.Stages == null)
                throw new InvalidOperationException("找不到 Stage 設定資料。");
            if (setting.TestSteps == null)
                throw new InvalidOperationException("找不到 TestStep 設定資料。");
            if (setting.Conditions == null)
                throw new InvalidOperationException("找不到 Condition 設定資料。");
        }

        private void ValidateStageOrder(IList<PlannedStep> steps)
        {
            HashSet<string> completedStageIds = new HashSet<string>();
            string currentStageId = null;
            bool executionSeen = false;

            foreach (PlannedStep plannedStep in steps)
            {
                TestStep step = plannedStep.Step;

                if (currentStageId != step.StageId)
                {
                    if (currentStageId != null)
                        completedStageIds.Add(currentStageId);
                    if (completedStageIds.Contains(step.StageId))
                    {
                        throw new InvalidOperationException(
                            $"Stage {step.StageId} 的 TestStep 必須連續排列。");
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
                        $"Stage {step.StageId} 的 Preparation 必須排在 Execution 前面。");
                }
            }
        }

        private void ValidateStep(TestStep step, int index)
        {
            if (step == null)
                throw new InvalidOperationException($"第 {index + 1} 筆 TestStep 沒有資料。");
            if (IsSkipped(step))
                return;
            if (string.IsNullOrWhiteSpace(step.Id))
                throw new InvalidOperationException($"第 {index + 1} 筆 TestStep 沒有 ID。");
            if (string.IsNullOrWhiteSpace(step.StageId))
                throw new InvalidOperationException($"第 {index + 1} 筆 TestStep 沒有 Stage ID。");
            if (step.TestProcess == TestProcess.None)
                throw new InvalidOperationException($"第 {index + 1} 筆 TestStep 沒有 TestProcess。");
            if (!Enum.IsDefined(typeof(StageStepRole), step.StageRole))
                throw new InvalidOperationException($"第 {index + 1} 筆 TestStep 的 StageRole 無效。");
        }

        private void ValidateStagePlans(IEnumerable<StagePlan> plans)
        {
            foreach (StagePlan plan in plans)
            {
                if (plan.MonitorSteps.Count > 0 && plan.PreparationSteps.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"Stage {plan.Stage.Id} 有 Monitor，但沒有 Preparation。");
                }

                if (plan.ExecutionMode == ExecutionRepeatMode.RepeatSequence &&
                    plan.ExecutionSteps.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"Stage {plan.Stage.Id} 設為 RepeatSequence，但沒有 Execution。");
                }
            }
        }

        // Sort
        private Dictionary<string, AgingStage> CreateStageLookup(
            IEnumerable<AgingStage> stages)
        {
            Dictionary<string, AgingStage> result =
                new Dictionary<string, AgingStage>();

            foreach (AgingStage stage in stages)
            {
                if (stage == null)
                    throw new InvalidOperationException("Stage 設定中存在空白資料。");
                if (string.IsNullOrWhiteSpace(stage.Id))
                    throw new InvalidOperationException("Stage 設定中存在空白 ID。");
                if (result.ContainsKey(stage.Id))
                    throw new InvalidOperationException($"Stage ID 重複：{stage.Id}");

                result.Add(stage.Id, stage);
            }

            return result;
        }

        private Dictionary<string, TestCondition> CreateConditionLookup(
            IEnumerable<TestCondition> conditions)
        {
            Dictionary<string, TestCondition> result =
                new Dictionary<string, TestCondition>();

            foreach (TestCondition condition in conditions)
            {
                if (condition == null)
                    throw new InvalidOperationException("Condition 設定中存在空白資料。");
                if (string.IsNullOrWhiteSpace(condition.TestStepId))
                    throw new InvalidOperationException("Condition 設定中存在空白 TestStepId。");
                if (result.ContainsKey(condition.TestStepId))
                    throw new InvalidOperationException($"TestStep ID {condition.TestStepId} 存在多筆 Condition。");

                result.Add(condition.TestStepId, condition);
            }

            return result;
        }

        private List<PlannedStep> CreatePlannedSteps(
            IList<TestStep> sourceSteps,
            IDictionary<string, TestCondition> conditionsByStepId)
        {
            List<PlannedStep> result = new List<PlannedStep>();
            HashSet<string> usedStepIds = new HashSet<string>();

            for (int index = 0; index < sourceSteps.Count; index++)
            {
                TestStep step = sourceSteps[index];
                ValidateStep(step, index);

                if (IsSkipped(step))
                    continue;

                if (!usedStepIds.Add(step.Id))
                    throw new InvalidOperationException($"TestStep ID 重複：{step.Id}");

                TestCondition condition = GetCondition(
                    step,
                    index,
                    conditionsByStepId);

                result.Add(new PlannedStep
                {
                    OriginalIndex = index,
                    Step = step,
                    Condition = condition
                });
            }

            if (result.Count == 0)
                throw new InvalidOperationException("排程中沒有可執行的 TestStep。");

            return result;
        }

        private List<StagePlan> CreateStagePlans(
            IEnumerable<PlannedStep> steps,
            IDictionary<string, AgingStage> stagesById)
        {
            List<StagePlan> result = new List<StagePlan>();
            Dictionary<string, StagePlan> plansByStageId =
                new Dictionary<string, StagePlan>();

            foreach (PlannedStep plannedStep in steps)
            {
                string stageId = plannedStep.Step.StageId;
                AgingStage stage = GetStage(stageId, stagesById);

                StagePlan plan;
                if (!plansByStageId.TryGetValue(stageId, out plan))
                {
                    plan = CreateStagePlan(stage);
                    plansByStageId.Add(stageId, plan);
                    result.Add(plan);
                }

                AddStep(plan, plannedStep);
            }

            return result;
        }

        private StagePlan CreateStagePlan(AgingStage stage)
        {
            StagePlan plan = new StagePlan
            {
                Stage = stage,
                Duration = ParseStageDuration(stage),
                ExecutionMode = stage.ExecutionMode,
                ExecutionInterval = TimeSpan.Zero
            };

            if (!Enum.IsDefined(typeof(ExecutionRepeatMode), stage.ExecutionMode))
                throw new InvalidOperationException($"Stage {stage.Id} 的 ExecutionMode 無效。");

            if (stage.ExecutionMode == ExecutionRepeatMode.RepeatSequence)
            {
                plan.ExecutionInterval = ParsePositiveSeconds(
                    stage.ExecutionIntervalSeconds,
                    $"Stage {stage.Id} ExecutionIntervalSeconds");
            }

            return plan;
        }


        private TestCondition GetCondition(
            TestStep step,
            int index,
            IDictionary<string, TestCondition> conditionsByStepId)
        {
            TestCondition condition;
            if (!conditionsByStepId.TryGetValue(step.Id, out condition))
            {
                throw new InvalidOperationException(
                    $"第 {index + 1} 筆 TestStep 找不到對應的 Condition，ID：{step.Id}");
            }

            if (condition.Process != step.TestProcess)
            {
                throw new InvalidOperationException(
                    $"第 {index + 1} 筆 TestStep 的 TestProcess 是 {step.TestProcess}，" +
                    $"但 Condition 是 {condition.Process}。");
            }

            return condition;
        }

        private AgingStage GetStage(
            string stageId,
            IDictionary<string, AgingStage> stagesById)
        {
            AgingStage stage;
            if (!stagesById.TryGetValue(stageId, out stage))
                throw new InvalidOperationException($"找不到 Stage {stageId} 的設定資料。");

            return stage;
        }
        
        private void AddStep(StagePlan plan, PlannedStep plannedStep)
        {
            switch (plannedStep.Step.StageRole)
            {
                case StageStepRole.Preparation:
                    plan.PreparationSteps.Add(plannedStep);
                    break;

                case StageStepRole.PreparationMonitor:
                    plan.MonitorSteps.Add(new MonitorPlan
                    {
                        PlannedStep = plannedStep,
                        Interval = ParsePositiveSeconds(
                            plannedStep.Step.MonitorIntervalSeconds,
                            $"TestStep {plannedStep.OriginalIndex + 1} MonitorIntervalSeconds")
                    });
                    break;

                case StageStepRole.Execution:
                    plan.ExecutionSteps.Add(plannedStep);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"TestStep {plannedStep.OriginalIndex + 1} 的 StageRole 無效。");
            }
        }

        private TimeSpan ParseStageDuration(AgingStage stage)
        {
            double durationMinutes = ParseNumber(
                stage.Duration,
                $"Stage {stage.Id} Duration");

            if (durationMinutes < 0)
                throw new InvalidOperationException($"Stage {stage.Id} Duration 不可小於零。");

            try
            {
                return TimeSpan.FromMinutes(durationMinutes);
            }
            catch (OverflowException)
            {
                throw new InvalidOperationException($"Stage {stage.Id} Duration 超出範圍。");
            }
        }

        private TimeSpan ParsePositiveSeconds(string value, string parameterName)
        {
            double seconds = ParseNumber(value, parameterName);
            if (seconds <= 0)
                throw new InvalidOperationException($"{parameterName} 必須大於零。");

            try
            {
                return TimeSpan.FromSeconds(seconds);
            }
            catch (OverflowException)
            {
                throw new InvalidOperationException($"{parameterName} 超出範圍。");
            }
        }

        private double ParseNumber(string value, string parameterName)
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
                throw new InvalidOperationException($"{parameterName} 格式錯誤：{value}");

            return number;
        }

        private bool IsSkipped(TestStep step)
        {
            bool skip;
            return bool.TryParse(step.Skip, out skip) && skip;
        }
        #endregion
    }
}
