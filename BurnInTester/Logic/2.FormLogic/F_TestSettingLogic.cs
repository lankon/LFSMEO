using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BurnInTester.Device;
using BurnInTester.Logic.TestSetting;
using RecipeManage.Base;
using ToolFunction;

namespace BurnInTester.Logic
{
    public sealed class F_TestSettingLogic
    {
        public F_TestSettingLogic(HW_ParamSetting hW_ParamSetting, AgingInformation agingInformation)
        {
            _AgingInfo = agingInformation;
            _HW_ParamSetting = hW_ParamSetting;
            _Repository = new TestSettingRepository();
            Data = new TestSettingData();
        }

        #region parameter
        private readonly TestSettingRepository _Repository;
        private readonly HW_ParamSetting _HW_ParamSetting;
        private readonly AgingInformation _AgingInfo;

        public TestSettingData Data { get; private set; }
        #endregion

        #region public function
        public void Load(string filePath)
        {
            Data = _Repository.Load(filePath);
        }

        public void LoadAllAgingProcessSettings()
        {
            Tool.SaveLogToFile("Load All Box Aging Process Start");

            for (int i = 0; i < _HW_ParamSetting.TC_Box._CtrlBoxNum; i++)
            {
                string recipeName = ApplicationSetting.Get_String_Recipe<eF_Recipe>(
                    (int)eF_Recipe.TxtBx_RecipeName);
                string fileName = $"TestSettingBox{i + 1}.xml";
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Setting",
                    "Package",
                    recipeName,
                    fileName);

                // Reset memory before loading another recipe.
                _AgingInfo.AgingProcessSetting[i] = new TestSettingData();

                if (File.Exists(filePath))
                    _AgingInfo.AgingProcessSetting[i] = _Repository.Load(filePath);
            }

            Tool.SaveLogToFile("Load All Box Aging Process End");
        }

        public void Save(string filePath)
        {
            Validate();
            _Repository.Save(filePath, Data);
        }

        public TestStep AddTestStep(int insertIndex)
        {
            string stageId = GetNextStageId();
            TestStep step = new TestStep
            {
                No = "1",
                StageId = stageId,
                StageRole = StageStepRole.Execution,
                MonitorIntervalSeconds = "0",
                Skip = "FALSE",
                TestProcess = TestProcess.None,
                Content = TestProcess.None.ToString(),
                Id = Guid.NewGuid().ToString("N")
            };

            if (insertIndex < 0 || insertIndex > Data.TestSteps.Count)
                insertIndex = Data.TestSteps.Count;

            Data.Stages.Add(new AgingStage
            {
                Id = stageId,
                Duration = "0",
                ExecutionMode = ExecutionRepeatMode.Once,
                ExecutionIntervalSeconds = "0"
            });
            Data.TestSteps.Insert(insertIndex, step);
            return step;
        }

        public void SynchronizeTestSteps(IEnumerable<TestStep> testSteps)
        {
            if (testSteps == null)
                throw new ArgumentNullException(nameof(testSteps));

            List<TestStep> sourceSteps = testSteps.ToList();
            if (sourceSteps.Count != Data.TestSteps.Count)
                throw new InvalidOperationException("Grid row count does not match the data row count.");

            Dictionary<string, TestStep> currentSteps = Data.TestSteps.ToDictionary(step => step.Id);
            Dictionary<string, string> previousStageIds = Data.TestSteps.ToDictionary(
                step => step.Id,
                step => step.StageId);
            Dictionary<string, AgingStage> previousStages = Data.Stages.ToDictionary(
                stage => stage.Id,
                stage => stage);
            HashSet<string> sourceIds = new HashSet<string>();
            List<TestStep> reorderedSteps = new List<TestStep>();

            foreach (TestStep sourceStep in sourceSteps)
            {
                if (string.IsNullOrWhiteSpace(sourceStep.Id))
                    throw new InvalidOperationException("A test step has an empty ID.");

                if (!sourceIds.Add(sourceStep.Id))
                    throw new InvalidOperationException($"Duplicate test step ID: {sourceStep.Id}");

                TestStep currentStep;
                if (!currentSteps.TryGetValue(sourceStep.Id, out currentStep))
                    throw new InvalidOperationException($"Test step {sourceStep.Id} was not found.");

                if (currentStep.TestProcess != sourceStep.TestProcess)
                    ChangeTestProcess(sourceStep.Id, sourceStep.TestProcess);

                currentStep.No = sourceStep.No;
                currentStep.StageId = sourceStep.StageId;
                currentStep.StageRole = sourceStep.StageRole;
                currentStep.MonitorIntervalSeconds = sourceStep.MonitorIntervalSeconds;
                currentStep.Skip = sourceStep.Skip;
                currentStep.Content = sourceStep.Content;
                reorderedSteps.Add(currentStep);
            }

            Data.TestSteps.Clear();
            Data.TestSteps.AddRange(reorderedSteps);
            SynchronizeStages(previousStageIds, previousStages);
        }

        public void ChangeTestProcess(string id, TestProcess process)
        {
            TestStep step = GetRequiredStep(id);

            if (!Enum.IsDefined(typeof(TestProcess), process))
                throw new InvalidOperationException($"Unsupported test process: {process}");

            RemoveConditionsExcept(id, process);
            step.TestProcess = process;
            step.StageRole = process == TestProcess.Temperature
                ? StageStepRole.Preparation
                : StageStepRole.Execution;
            step.MonitorIntervalSeconds = "0";

            if (process == TestProcess.None || HasCondition(id, process))
                return;

            Data.Conditions.Add(CreateCondition(id, process));
        }

        public void RemoveTestStep(string id)
        {
            TestStep step = Data.TestSteps.FirstOrDefault(item => item.Id == id);
            if (step == null)
                return;

            Data.TestSteps.Remove(step);
            RemoveAllConditions(id);
            RemoveUnusedStages();
        }

        public void UpdateTemperatureCondition(string id, TemperatureCondition condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            TestStep step = GetRequiredStep(id);
            if (step.TestProcess != TestProcess.Temperature)
                throw new InvalidOperationException("The selected test process is not Temperature.");

            int conditionIndex = Data.Conditions.FindIndex(
                item => item.TestStepId == id && item.Process == TestProcess.Temperature);
            if (conditionIndex < 0)
                throw new InvalidOperationException($"Temperature condition {id} was not found.");

            condition.TestStepId = id;
            Data.Conditions[conditionIndex] = condition;
            GetRequiredStage(step.StageId).Duration = condition.BurnInDuration;

            step.Content =
                $"Temperature : {condition.TargetTemperature} C , Duration : {condition.BurnInDuration} min";
        }

        public bool TryGetTemperatureCondition(string id, out TemperatureCondition condition)
        {
            condition = Data.Conditions
                .OfType<TemperatureCondition>()
                .FirstOrDefault(item => item.TestStepId == id);
            return condition != null;
        }

        public void UpdateLIVCondition(string id, LivCondition condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            TestStep step = GetRequiredStep(id);
            if (step.TestProcess != TestProcess.LIV)
                throw new InvalidOperationException("The selected test process is not LIV.");

            int conditionIndex = Data.Conditions.FindIndex(
                item => item.TestStepId == id && item.Process == TestProcess.LIV);
            if (conditionIndex < 0)
                throw new InvalidOperationException($"LIV condition {id} was not found.");

            condition.TestStepId = id;
            Data.Conditions[conditionIndex] = condition;
            step.Content =
                $"Start : {condition.StartI} , Step : {condition.StepI} , " +
                $"End : {condition.EndI} (mA) ; Delay : {condition.DelayTime} (ms)";
        }

        public bool TryGetLIVCondition(string id, out LivCondition condition)
        {
            condition = Data.Conditions
                .OfType<LivCondition>()
                .FirstOrDefault(item => item.TestStepId == id);
            return condition != null;
        }

        public bool TryGetStageForStep(string testStepId, out AgingStage stage)
        {
            stage = null;
            TestStep step = Data.TestSteps.FirstOrDefault(item => item.Id == testStepId);
            if (step == null)
                return false;

            stage = Data.Stages.FirstOrDefault(item => item.Id == step.StageId);
            return stage != null;
        }

        public void UpdateStageExecutionSettings(
            string testStepId,
            ExecutionRepeatMode mode,
            string intervalSeconds)
        {
            if (!Enum.IsDefined(typeof(ExecutionRepeatMode), mode))
                throw new InvalidOperationException($"Unsupported execution mode: {mode}");

            TestStep step = GetRequiredStep(testStepId);
            AgingStage stage = GetRequiredStage(step.StageId);

            if (mode == ExecutionRepeatMode.Once)
            {
                stage.ExecutionMode = mode;
                stage.ExecutionIntervalSeconds = "0";
                return;
            }

            double interval = ParseNumber(
                intervalSeconds,
                $"Stage {stage.Id} execution interval");
            if (interval <= 0)
                throw new InvalidOperationException("Execution interval must be greater than zero.");

            stage.ExecutionMode = mode;
            stage.ExecutionIntervalSeconds = intervalSeconds.Trim();
        }

        public void Validate()
        {
            ValidateStages();
            Dictionary<string, TestProcess> processesById = new Dictionary<string, TestProcess>();

            foreach (TestStep step in Data.TestSteps)
            {
                if (string.IsNullOrWhiteSpace(step.Id))
                    throw new InvalidOperationException("A test condition has an empty ID.");

                if (processesById.ContainsKey(step.Id))
                    throw new InvalidOperationException($"Duplicate test condition ID: {step.Id}");

                TestProcess process = step.TestProcess;
                processesById.Add(step.Id, process);

                int detailCount = CountConditions(step.Id);
                if (process == TestProcess.None)
                {
                    if (detailCount != 0)
                    {
                        throw new InvalidOperationException(
                            $"Test step {step.Id} is None but still has detail data.");
                    }

                    continue;
                }

                if (!HasCondition(step.Id, process))
                    throw new InvalidOperationException($"Condition {process}/{step.Id} was not found.");

                if (detailCount != 1)
                    throw new InvalidOperationException($"Test step {step.Id} has an invalid detail count.");
            }

            ValidateConditionOwners(Data.Conditions, processesById);
        }
        #endregion

        #region private function
        private TestStep GetRequiredStep(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidOperationException("The selected test step has no ID.");

            TestStep step = Data.TestSteps.FirstOrDefault(item => item.Id == id);
            if (step == null)
                throw new InvalidOperationException($"Test step {id} was not found.");

            return step;
        }

        private AgingStage GetRequiredStage(string stageId)
        {
            AgingStage stage = Data.Stages.FirstOrDefault(item => item.Id == stageId);
            if (stage == null)
                throw new InvalidOperationException($"Stage {stageId} was not found.");

            return stage;
        }

        private string GetNextStageId()
        {
            int stageNumber = 1;
            HashSet<string> stageIds = new HashSet<string>(
                Data.Stages.Select(stage => stage.Id));

            while (stageIds.Contains(stageNumber.ToString()))
                stageNumber++;

            return stageNumber.ToString();
        }

        private void SynchronizeStages(
            IDictionary<string, string> previousStageIds,
            IDictionary<string, AgingStage> previousStages)
        {
            List<AgingStage> synchronizedStages = new List<AgingStage>();
            HashSet<string> addedStageIds = new HashSet<string>();

            foreach (TestStep step in Data.TestSteps)
            {
                if (string.IsNullOrWhiteSpace(step.StageId))
                    throw new InvalidOperationException($"Test step No. {step.No} has no Stage.");

                if (!addedStageIds.Add(step.StageId))
                    continue;

                AgingStage sourceStage;
                if (!previousStages.TryGetValue(step.StageId, out sourceStage))
                {
                    string previousStageId;
                    if (!previousStageIds.TryGetValue(step.Id, out previousStageId) ||
                        !previousStages.TryGetValue(previousStageId, out sourceStage))
                    {
                        sourceStage = null;
                    }
                }

                synchronizedStages.Add(new AgingStage
                {
                    Id = step.StageId,
                    Duration = sourceStage == null ? "0" : sourceStage.Duration,
                    ExecutionMode = sourceStage == null
                        ? ExecutionRepeatMode.Once
                        : sourceStage.ExecutionMode,
                    ExecutionIntervalSeconds = sourceStage == null ||
                        string.IsNullOrWhiteSpace(sourceStage.ExecutionIntervalSeconds)
                            ? "0"
                            : sourceStage.ExecutionIntervalSeconds
                });
            }

            Data.Stages.Clear();
            Data.Stages.AddRange(synchronizedStages);

            foreach (TemperatureCondition condition in Data.Conditions.OfType<TemperatureCondition>())
            {
                TestStep step = Data.TestSteps.FirstOrDefault(item => item.Id == condition.TestStepId);
                if (step != null)
                    condition.BurnInDuration = GetRequiredStage(step.StageId).Duration;
            }
        }

        private void RemoveUnusedStages()
        {
            HashSet<string> usedStageIds = new HashSet<string>(
                Data.TestSteps.Select(step => step.StageId));
            Data.Stages.RemoveAll(stage => !usedStageIds.Contains(stage.Id));
        }

        private void ValidateStages()
        {
            Dictionary<string, AgingStage> stagesById = new Dictionary<string, AgingStage>();

            foreach (AgingStage stage in Data.Stages)
            {
                if (string.IsNullOrWhiteSpace(stage.Id))
                    throw new InvalidOperationException("A Stage has an empty ID.");
                if (stagesById.ContainsKey(stage.Id))
                    throw new InvalidOperationException($"Duplicate Stage ID: {stage.Id}");
                if (!Enum.IsDefined(typeof(ExecutionRepeatMode), stage.ExecutionMode))
                    throw new InvalidOperationException($"Stage {stage.Id} has an invalid execution mode.");

                double duration = ParseNumber(stage.Duration, $"Stage {stage.Id} duration");
                if (duration < 0)
                    throw new InvalidOperationException($"Stage {stage.Id} duration cannot be negative.");

                double executionInterval = ParseNumber(
                    string.IsNullOrWhiteSpace(stage.ExecutionIntervalSeconds)
                        ? "0"
                        : stage.ExecutionIntervalSeconds,
                    $"Stage {stage.Id} execution interval");
                if (executionInterval < 0)
                    throw new InvalidOperationException($"Stage {stage.Id} interval cannot be negative.");
                if (stage.ExecutionMode == ExecutionRepeatMode.RepeatSequence)
                {
                    if (duration <= 0)
                        throw new InvalidOperationException($"Stage {stage.Id} duration must be positive.");
                    if (executionInterval <= 0)
                        throw new InvalidOperationException($"Stage {stage.Id} interval must be positive.");
                }

                stagesById.Add(stage.Id, stage);
            }

            string currentStageId = null;
            bool executionStepSeen = false;
            HashSet<string> closedStageIds = new HashSet<string>();
            Dictionary<string, int> preparationCounts = new Dictionary<string, int>();
            Dictionary<string, int> monitorCounts = new Dictionary<string, int>();
            Dictionary<string, int> executionCounts = new Dictionary<string, int>();

            foreach (TestStep step in Data.TestSteps)
            {
                if (!stagesById.ContainsKey(step.StageId))
                    throw new InvalidOperationException($"Stage {step.StageId} was not found.");
                if (!Enum.IsDefined(typeof(StageStepRole), step.StageRole))
                    throw new InvalidOperationException($"Test step {step.Id} has an invalid StageRole.");

                if (currentStageId != step.StageId)
                {
                    if (currentStageId != null)
                        closedStageIds.Add(currentStageId);
                    if (closedStageIds.Contains(step.StageId))
                        throw new InvalidOperationException($"Stage {step.StageId} steps must be contiguous.");

                    currentStageId = step.StageId;
                    executionStepSeen = false;
                }

                if (step.StageRole == StageStepRole.Execution)
                {
                    executionStepSeen = true;
                    IncrementCount(executionCounts, step.StageId);
                    continue;
                }

                if (executionStepSeen)
                {
                    throw new InvalidOperationException(
                        $"Stage {step.StageId} preparation roles must precede Execution.");
                }

                if (step.StageRole == StageStepRole.Preparation)
                {
                    IncrementCount(preparationCounts, step.StageId);
                }
                else if (step.StageRole == StageStepRole.PreparationMonitor)
                {
                    double interval = ParseNumber(
                        step.MonitorIntervalSeconds,
                        $"Monitor interval for {step.Id}");
                    if (interval <= 0)
                        throw new InvalidOperationException($"Monitor interval for {step.Id} must be positive.");

                    IncrementCount(monitorCounts, step.StageId);
                }
            }

            foreach (KeyValuePair<string, int> monitor in monitorCounts)
            {
                if (!preparationCounts.ContainsKey(monitor.Key))
                {
                    throw new InvalidOperationException(
                        $"Stage {monitor.Key} has a PreparationMonitor but no Preparation.");
                }
            }

            foreach (AgingStage stage in Data.Stages)
            {
                if (stage.ExecutionMode == ExecutionRepeatMode.RepeatSequence &&
                    !executionCounts.ContainsKey(stage.Id))
                {
                    throw new InvalidOperationException(
                        $"Stage {stage.Id} is RepeatSequence but has no Execution step.");
                }
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
                throw new InvalidOperationException($"Invalid {parameterName}: {value}");

            return number;
        }

        private void IncrementCount(IDictionary<string, int> counts, string stageId)
        {
            int count;
            counts.TryGetValue(stageId, out count);
            counts[stageId] = count + 1;
        }

        private TestCondition CreateCondition(string id, TestProcess process)
        {
            switch (process)
            {
                case TestProcess.Temperature:
                    return new TemperatureCondition { TestStepId = id };
                case TestProcess.LIV:
                    return new LivCondition { TestStepId = id };
                case TestProcess.VF:
                    return new VfCondition { TestStepId = id };
                case TestProcess.IR:
                    return new IrCondition { TestStepId = id };
                default:
                    throw new InvalidOperationException($"Unsupported test process: {process}");
            }
        }

        private void RemoveConditionsExcept(string id, TestProcess process)
        {
            Data.Conditions.RemoveAll(
                item => item.TestStepId == id && item.Process != process);
        }

        private void RemoveAllConditions(string id)
        {
            Data.Conditions.RemoveAll(item => item.TestStepId == id);
        }

        private int CountConditions(string id)
        {
            return Data.Conditions.Count(item => item.TestStepId == id);
        }

        private bool HasCondition(string id, TestProcess process)
        {
            return Data.Conditions.Any(
                item => item.TestStepId == id && item.Process == process);
        }

        private void ValidateConditionOwners(
            IEnumerable<TestCondition> conditions,
            IDictionary<string, TestProcess> processesById)
        {
            foreach (TestCondition condition in conditions)
            {
                TestProcess process;
                if (!processesById.TryGetValue(condition.TestStepId, out process))
                {
                    throw new InvalidOperationException(
                        $"{condition.Process} detail uses missing ID {condition.TestStepId}.");
                }

                if (process != condition.Process)
                {
                    throw new InvalidOperationException(
                        $"ID {condition.TestStepId} is {process} but detail is {condition.Process}.");
                }
            }
        }
        #endregion
    }
}
