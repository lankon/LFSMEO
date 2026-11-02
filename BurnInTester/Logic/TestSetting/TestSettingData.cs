using System.Collections.Generic;

namespace BurnInTester.Logic.TestSetting
{
    public enum TestProcess
    {
        None,
        Temperature,
        LIV,
        VF,
        IR
    }

    public enum StageStepRole
    {
        Preparation,
        PreparationMonitor,
        Execution
    }

    public enum ExecutionRepeatMode
    {
        Once,
        RepeatSequence
    }

    public sealed class TestSettingData
    {
        public TestSettingData()
        {
            Stages = new List<AgingStage>();
            TestSteps = new List<TestStep>();
            Conditions = new List<TestCondition>();
        }

        public List<AgingStage> Stages { get; private set; }
        public List<TestStep> TestSteps { get; private set; }
        public List<TestCondition> Conditions { get; private set; }
    }

    public sealed class AgingStage
    {
        public string Id { get; set; }
        public string Duration { get; set; }
        public ExecutionRepeatMode ExecutionMode { get; set; }
        public string ExecutionIntervalSeconds { get; set; }
    }

    public sealed class TestStep
    {
        public string No { get; set; }
        public string StageId { get; set; }
        public StageStepRole StageRole { get; set; }
        public string MonitorIntervalSeconds { get; set; }
        public string Skip { get; set; }
        public TestProcess TestProcess { get; set; }
        public string Content { get; set; }
        public string Id { get; set; }
    }

    public abstract class TestCondition
    {
        public string TestStepId { get; set; }
        public abstract TestProcess Process { get; }
    }

    public sealed class TemperatureCondition : TestCondition
    {
        public override TestProcess Process
        {
            get { return TestProcess.Temperature; }
        }

        public string TargetTemperature { get; set; }

        // Kept for old XML and UI compatibility. Runtime uses AgingStage.Duration.
        public string BurnInDuration { get; set; }
        public string StabilizationWaitTime { get; set; }
        public string TemperatureTolerance { get; set; }
        public string StabilizationTimeout { get; set; }
    }

    public sealed class LivCondition : TestCondition
    {
        public override TestProcess Process
        {
            get { return TestProcess.LIV; }
        }

        public string StartI { get; set; }
        public string StepI { get; set; }
        public string EndI { get; set; }
        public string DelayTime { get; set; }
    }

    public sealed class VfCondition : TestCondition
    {
        public override TestProcess Process
        {
            get { return TestProcess.VF; }
        }
    }

    public sealed class IrCondition : TestCondition
    {
        public override TestProcess Process
        {
            get { return TestProcess.IR; }
        }
    }
}
