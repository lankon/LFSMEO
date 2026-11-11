using System;
using System.Collections.Generic;

namespace DeviceCore
{
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

    public enum SourceMeterTransientResponse
    {
        Slow,
        Normal,
        Fast
    }

    public enum SourceMeterTriggerTerminal
    {
        None = -1,
        PxiTriggerLine0 = 0,
        PxiTriggerLine1 = 1,
        PxiTriggerLine2 = 2,
        PxiTriggerLine3 = 3,
        PxiTriggerLine4 = 4,
        PxiTriggerLine5 = 5,
        PxiTriggerLine6 = 6,
        PxiTriggerLine7 = 7
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
        public SourceMeterSourceFunction SourceFunction { get; set; }
        public SourceMeterOutputMode OutputMode { get; set; } = SourceMeterOutputMode.ContinuousWave;
        public SourceMeterMeasurementSense MeasurementSense { get; set; } = SourceMeterMeasurementSense.Remote;
        public SourceMeterTransientResponse TransientResponse { get; set; } = SourceMeterTransientResponse.Normal;

        public double ComplianceLimit { get; set; }
        public double SourceRange { get; set; }
        public double ComplianceRange { get; set; }
        public bool AutoRangePerStep { get; set; } = true;

        public SourceMeterTriggerTerminal StartTriggerInputTerminal { get; set; } = SourceMeterTriggerTerminal.None;
        public SourceMeterTriggerTerminal StartTriggerOutputTerminal { get; set; } = SourceMeterTriggerTerminal.None;
        public SourceMeterTriggerTerminal SourceCompleteEventOutputTerminal { get; set; } = SourceMeterTriggerTerminal.None;
        public SourceMeterTriggerTerminal PulseTriggerOutputTerminal { get; set; } = SourceMeterTriggerTerminal.None;

        public int LoopCount { get; set; } = 1;
        public IList<SourceMeterStep> Steps { get; set; } = new List<SourceMeterStep>();

        public double PulseOnTimeSeconds { get; set; }
        public double PulseOffTimeSeconds { get; set; }
        public double PulseBiasLevel { get; set; } = 0;             // Pulse off時的輸出
        public double PulseBiasDelaySeconds { get; set; }
    }

    public sealed class SourceMeterMeasurement
    {
        public double Voltage { get; set; }
        public double Current { get; set; }
    }

    public interface ISourceMeter
    {
        //bool IsOpen { get; }                    // Session 是否已開啟。
        //string InstrumentModel { get; }         // 目前連線的儀器型號。
        //string LastError { get; }               // 最近一次呼叫的錯誤訊息。

        ESMUType GetSourceMeterType();                          // 取得儀器類型
        int SetConfiguration(string id, SourceMeterConfiguration configuration);        // 設定輸出、量測、Sequence 與 Trigger。
        int GetConfiguration(string id, out SourceMeterConfiguration configuration);    // 取得目前儀器的設定。
        void BindingDeviceIndex(string id); 
        

        int Open(SourceMeterConnection connection);     // 開啟儀器 Session
        int OutputOn(string id);                        // 啟用輸出並開始執行 Sequence。
        int WaitForCompletion(string id, double timeoutSeconds);    // 等待 Sequence 執行完成。
        int Fetch(string id, int count, double timeoutSeconds, out IList<SourceMeterMeasurement> measurements);    // 取得電壓與電流量測結果。
        int OutputOff(string id);                       // 中止 Sequence 並停用輸出
        int Reset(string id);                           // 中止、刪除 Sequence 並重設儀器
        int SetOutputRelay(string id, bool connected);  // true 閉合 Relay；false 斷開 Relay
        int Close();        // 關閉儀器 Session
    }
}
