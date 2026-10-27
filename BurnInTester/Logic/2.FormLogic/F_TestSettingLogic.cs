using System;
using System.Collections.Generic;
using System.Linq;
using BurnInTester.Logic.TestSetting;

namespace BurnInTester.Logic
{
    public sealed class F_TestSettingLogic
    {
        public F_TestSettingLogic()
        {
            _Repository = new TestSettingRepository();
            Data = new TestSettingData();
        }

        #region parameter
        private readonly TestSettingRepository _Repository;

        public TestSettingData Data { get; private set; }
        #endregion

        #region public function
        public void Load(string filePath)
        {
            Data = _Repository.Load(filePath);
        }

        public void Save(string filePath)
        {
            Validate();
            _Repository.Save(filePath, Data);
        }

        public TestStep AddTestStep(int insertIndex)
        {
            TestStep step = new TestStep
            {
                No = "1",
                Skip = "FALSE",
                TestProcess = TestProcess.None,
                Content = TestProcess.None.ToString(),
                Id = Guid.NewGuid().ToString("N")
            };

            if (insertIndex < 0 || insertIndex > Data.TestSteps.Count)
                insertIndex = Data.TestSteps.Count;

            Data.TestSteps.Insert(insertIndex, step);
            return step;
        }

        public void SynchronizeTestSteps(IEnumerable<TestStep> testSteps)
        {
            if (testSteps == null)
                throw new ArgumentNullException(nameof(testSteps));

            List<TestStep> sourceSteps = testSteps.ToList();
            if (sourceSteps.Count != Data.TestSteps.Count)
                throw new InvalidOperationException("畫面測試步驟數量與資料數量不一致。");

            Dictionary<string, TestStep> currentSteps = Data.TestSteps.ToDictionary(step => step.Id);
            HashSet<string> sourceIds = new HashSet<string>();
            List<TestStep> reorderedSteps = new List<TestStep>();

            foreach (TestStep sourceStep in sourceSteps)
            {
                if (string.IsNullOrWhiteSpace(sourceStep.Id))
                    throw new InvalidOperationException("測試條件存在空白 ID。");

                if (!sourceIds.Add(sourceStep.Id))
                    throw new InvalidOperationException($"測試條件 ID 重複：{sourceStep.Id}");

                TestStep currentStep;
                if (!currentSteps.TryGetValue(sourceStep.Id, out currentStep))
                    throw new InvalidOperationException($"找不到 ID {sourceStep.Id} 的測試步驟資料。");

                if (currentStep.TestProcess != sourceStep.TestProcess)
                    ChangeTestProcess(sourceStep.Id, sourceStep.TestProcess);

                currentStep.No = sourceStep.No;
                currentStep.Skip = sourceStep.Skip;
                currentStep.Content = sourceStep.Content;
                reorderedSteps.Add(currentStep);
            }

            Data.TestSteps.Clear();
            Data.TestSteps.AddRange(reorderedSteps);
        }

        public void ChangeTestProcess(string id, TestProcess process)
        {
            TestStep step = GetRequiredStep(id);

            if (!Enum.IsDefined(typeof(TestProcess), process))
                throw new InvalidOperationException($"尚未建立 {process} 的測試條件資料。");

            RemoveConditionsExcept(id, process);
            step.TestProcess = process;

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
        }

        public void UpdateTemperatureCondition(string id, TemperatureCondition condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            TestStep step = GetRequiredStep(id);
            if (step.TestProcess != TestProcess.Temperature)
                throw new InvalidOperationException("目前選擇的項目不是 Temperature。");

            int conditionIndex = Data.Conditions.FindIndex(
                item => item.TestStepId == id && item.Process == TestProcess.Temperature);
            if (conditionIndex < 0)
                throw new InvalidOperationException($"找不到 ID {id} 的 Temperature 測試條件。");

            condition.TestStepId = id;
            Data.Conditions[conditionIndex] = condition;

            // Content
            int TestStepsIndex = Data.TestSteps.FindIndex(item => item.Id == id);
            if (TestStepsIndex < 0)
                throw new InvalidOperationException($"找不到 ID {id} 的 Temperature 測試條件。");

            Data.TestSteps[TestStepsIndex].Content = $"Temperature : {condition.TargetTemperature} °C , Duration: {condition.BurnInDuration} min";
        }

        public bool TryGetTemperatureCondition(string id, out TemperatureCondition condition)
        {
            condition = Data.Conditions
                .OfType<TemperatureCondition>()
                .FirstOrDefault(item => item.TestStepId == id);
            return condition != null;
        }

        public void Validate()
        {
            Dictionary<string, TestProcess> processesById = new Dictionary<string, TestProcess>();

            foreach (TestStep step in Data.TestSteps)
            {
                if (string.IsNullOrWhiteSpace(step.Id))
                    throw new InvalidOperationException("測試條件存在空白 ID，無法儲存。");

                if (processesById.ContainsKey(step.Id))
                    throw new InvalidOperationException($"測試條件 ID 重複：{step.Id}");

                TestProcess process = step.TestProcess;
                processesById.Add(step.Id, process);

                int detailCount = CountConditions(step.Id);
                if (process == TestProcess.None)
                {
                    if (detailCount != 0)
                        throw new InvalidOperationException($"ID {step.Id} 的測試項目為 None，但仍存在明細資料。");

                    continue;
                }

                if (!HasCondition(step.Id, process))
                    throw new InvalidOperationException($"找不到 ID {step.Id} 的 {process} 測試條件。");

                if (detailCount != 1)
                    throw new InvalidOperationException($"ID {step.Id} 的測試條件明細數量不正確。");
            }

            ValidateConditionOwners(Data.Conditions, processesById);
        }
        #endregion

        #region private function
        private TestStep GetRequiredStep(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidOperationException("目前測試條件沒有 ID。");

            TestStep step = Data.TestSteps.FirstOrDefault(item => item.Id == id);
            if (step == null)
                throw new InvalidOperationException($"找不到 ID {id} 的測試步驟。");

            return step;
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
                    throw new InvalidOperationException($"尚未建立 {process} 的測試條件資料。");
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
                    throw new InvalidOperationException(
                        $"{condition.Process} 明細存在無主表的 ID：{condition.TestStepId}");

                if (process != condition.Process)
                    throw new InvalidOperationException(
                        $"ID {condition.TestStepId} 的主表項目為 {process}，但明細類型為 {condition.Process}。");
            }
        }
        #endregion
    }

}
