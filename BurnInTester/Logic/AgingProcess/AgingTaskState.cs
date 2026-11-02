using BurnInTester.Base;
using BurnInTester.Logic.TestSetting;

namespace BurnInTester.Logic
{
    /// <summary>
    /// 老化測試子流程的共同介面。
    /// 每次 Run 只掃描一次狀態，不可在裡面 Sleep 或阻塞共用排程執行緒。
    /// </summary>
    public interface IAgingProcessTask : IF_BaseTask
    {
        /// <summary>目前子流程所在的 WORK，供狀態顯示與除錯使用。</summary>
        string StateName { get; }

        /// <summary>子流程失敗原因；沒有錯誤時為空字串。</summary>
        string ErrorMessage { get; }
    }

    /// <summary>
    /// 驗證測試條件並建立對應測試項目的狀態機。
    /// 未來增加 LIV、VF 或 IR 時，不需要修改 AgingBoxTask。
    /// </summary>
    public interface IAgingProcessTaskFactory
    {
        TestProcess Process { get; }

        void Validate(TestSettingData setting, TestStep step);

        IAgingProcessTask Create(
            int boxIndex,
            TestSettingData setting,
            TestStep step,
            AgingProcessStatus status,
            bool reportStatus);
    }
}
