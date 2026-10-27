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

    public sealed class TestSettingData
    {
        public TestSettingData()
        {
            TestSteps = new List<TestStep>();
            Conditions = new List<TestCondition>();
        }

        public List<TestStep> TestSteps { get; private set; }
        public List<TestCondition> Conditions { get; private set; }
    }

    public sealed class TestStep
    {
        public string No { get; set; }
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
