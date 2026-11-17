using System;
using System.Collections.Generic;

namespace DeviceCore
{
    public interface ISourceMeter
    {
        ESMUType GetSourceMeterType();          // 取得儀器類型
        void BindingDeviceIndex(string id);     // 將id對應到配對的儀器

        int Open(SourceMeterConnection connection);     // 開啟儀器 Session
        
        int GetConfiguration(string id, out SourceMeterConfiguration configuration);    // 取得目前儀器的設定。
        int SetConfiguration(string id, SourceMeterConfiguration configuration);        // 設定輸出、量測、Sequence 與 Trigger。
        int OutputOn(string id);                        // 啟用輸出並開始執行 Sequence。
        int WaitForCompletion(string id, double timeoutSeconds);    // 等待 Sequence 執行完成。
        int Fetch(string id, int count, double timeoutSeconds, out IList<SourceMeterMeasurement> measurements);    // 取得電壓與電流量測結果。
        int OutputOff(string id);                       // 中止 Sequence 並停用輸出
        
        int Reset(string id);                           // 中止、刪除 Sequence 並重設儀器
        int SetOutputRelay(string id, bool connected);  // true 閉合 Relay；false 斷開 Relay, 物理斷開
        int Close();        // 關閉儀器 Session
    }

    public enum ESMUType
    {
        VIRTUAL,
        NI,
    }

    public enum SourceMeterErrorCode
    {
        Success = 0,
        InvalidArgument = 1,
        NotConnected = 2,
        NotConfigured = 3,
        DriverError = 4,
        Timeout = 5,
        NotSupported = 6
    }

    public enum SourceMeterSourceFunction
    {
        Current,
        Voltage
    }

    public enum SourceMeterOutputMode
    {
        ContinuousWave,
        Pulse
    }

    public enum SourceMeterMeasurementSense
    {
        Local,  // 二線制
        Remote  // 四線制
    }

    public enum SourceMeterMeasurementMode
    {
        AutomaticallyAfterSourceComplete,
        OnMeasureTrigger,
        OnDemand
    }

    public enum SourceMeterTransientResponse
    {
        Slow,
        Normal,
        Fast
    }

    public enum SourceMeterTriggerTerminal
    {
        None = -1,
        TriggerLine0 = 0,
        TriggerLine1 = 1,
        TriggerLine2 = 2,
        TriggerLine3 = 3,
        TriggerLine4 = 4,
        TriggerLine5 = 5,
        TriggerLine6 = 6,
        TriggerLine7 = 7
    }

    public sealed class SourceMeterConnection
    {
        public string ResourceName { get; set; }
        public string ChannelName { get; set; } = "0";
        public bool ResetDevice { get; set; }
        public string DriverSetupOptions { get; set; } = string.Empty;
    }

    /// <summary>
    /// A hardware sequence step. Level is expressed in amperes when sourcing
    /// current and volts when sourcing voltage. All time values are in seconds.
    /// </summary>
    public sealed class SourceMeterStep
    {
        public double Level { get; set; }
        public double SourceDelaySeconds { get; set; }
        public double ApertureTimeSeconds { get; set; }
    }

    /// <summary>
    /// Vendor-neutral source program. A single point is represented by one step
    /// and one loop; sweeps and trains are represented by their step list and
    /// loop count without introducing test-specific mode names.
    /// </summary>
    public sealed class SourceMeterConfiguration
    {
        public SourceMeterSourceFunction SourceFunction { get; set; }       // 輸出功能：定電流或定電壓
        public SourceMeterOutputMode OutputMode { get; set; } = SourceMeterOutputMode.ContinuousWave;               // 輸出模式：連續波或脈衝
        public SourceMeterMeasurementSense MeasurementSense { get; set; } = SourceMeterMeasurementSense.Remote;     // 量測接線：二線式或四線式
        public SourceMeterMeasurementMode MeasurementMode { get; set; } = SourceMeterMeasurementMode.AutomaticallyAfterSourceComplete; // 決定開始量測的時機
        public SourceMeterTransientResponse TransientResponse { get; set; } = SourceMeterTransientResponse.Normal;  // 輸出暫態響應速度

        public double ComplianceLimit { get; set; }                 // 定電流時限制電壓；定電壓時限制電流
        public double SourceRange { get; set; }                     // 固定輸出 Range；0 表示由程式自動選擇
        public double ComplianceRange { get; set; }                 // Compliance 硬體 Range；0 表示依限制值選擇
        public bool AutoRangePerStep { get; set; } = true;          // SourceRange 為 0 時，是否每個 Step 個別選 Range

        public SourceMeterTriggerTerminal StartTriggerInputTerminal { get; set; } = SourceMeterTriggerTerminal.None; // Sequence 啟動 Trigger 輸入端
        public SourceMeterTriggerTerminal StartTriggerOutputTerminal { get; set; } = SourceMeterTriggerTerminal.None; // Start Trigger 輸出端
        public SourceMeterTriggerTerminal MeasureTriggerInputTerminal { get; set; } = SourceMeterTriggerTerminal.None; // Measure Trigger 輸入端
        public SourceMeterTriggerTerminal SourceCompleteEventOutputTerminal { get; set; } = SourceMeterTriggerTerminal.None; // Source Complete 事件輸出端
        public SourceMeterTriggerTerminal PulseTriggerOutputTerminal { get; set; } = SourceMeterTriggerTerminal.None; // Pulse Trigger 輸出端

        public int LoopCount { get; set; } = 1;                     // Sequence 重複執行次數
        public IList<SourceMeterStep> Steps { get; set; } = new List<SourceMeterStep>(); // Sequence 的輸出步驟

        public int? MeasurementRecordLength { get; set; }           // 每次量測記錄的筆數；null 保留 Driver 預設值
        public bool? SequenceStepDeltaTimeEnabled { get; set; }     // 是否啟用 NI Sequence Step Delta Time；null 保留預設值

        public double PulseOnTimeSeconds { get; set; }              // Pulse 輸出維持時間（秒）
        public double PulseOffTimeSeconds { get; set; }             // Pulse 關閉／Bias 維持時間（秒）
        public double PulseBiasLevel { get; set; } = 0;             // Pulse Off 階段的輸出值
        public double PulseBiasDelaySeconds { get; set; }           // Pulse Bias 後的延遲時間（秒）
    }

    public sealed class SourceMeterMeasurement
    {
        public double Voltage { get; set; }
        public double Current { get; set; }
    }
}
