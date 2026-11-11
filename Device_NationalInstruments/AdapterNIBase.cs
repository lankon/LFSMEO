using System;
using System.Collections.Generic;
using System.Linq;

using NationalInstruments;
using NationalInstruments.ModularInstruments.NIDCPower;

using DeviceCore;

namespace Device_NationalInstruments
{
    /// <summary>
    /// NI-DCPower SourceMeter 底層實作。
    /// </summary>
    public class AdapterNIBase : ISourceMeter
    {
        #region parameter
        private NIPower[] Powers = new NIPower[MaxCardNumber];  // NI Powers
        private const int MaxCardNumber = 10;                   // NI SourceMeter支援的卡數。
        private int InitialIndex = 0;                           // NI SourceMeter初始化的起始卡號。
        private Dictionary<string, int> DeviceIndex = new Dictionary<string, int>();
        
        public class NIPower
        {
            public NIDCPower Power;                         // NI-DCPower Driver Session。
            public SourceMeterConfiguration Configuration;  // NI-DCPower Session 使用的量測設定。
            public string ResourceName;                     // NI 儀器的 Resource Name，例如 PXI1Slot2。
            public string ChannelName = "0";                // Session 使用的 NI 實體通道，預設為通道 0。
            public string SequenceName;                     // 建立於 NI Driver 內的 Advanced Sequence 名稱。
            public int ExpectedMeasurementCount;            // Sequence Step 數乘以 LoopCount 的預期量測筆數。

            public bool IsOpen { get; set; } = false;                           // 是否完成初始化
            public string InstrumentModel { get; set; } = string.Empty;         // NI 儀器型號。
            public string IOResourceDescriptor { get; set; } = string.Empty;    // Driver 實際使用的 I/O Resource Descriptor。
            public string LastError { get; set; } = string.Empty;               // 最近一次呼叫的錯誤訊息。
        }
        
        #endregion
        public ESMUType GetSourceMeterType()
        {
            return ESMUType.NI;
        }

        public int Open(SourceMeterConnection connection)
        {
            CloseSession();

            try
            {
                if (string.IsNullOrWhiteSpace(connection.DriverSetupOptions))
                {
                    Powers[InitialIndex] = new NIPower
                    {
                        Power = new NIDCPower(connection.ResourceName, connection.ChannelName, connection.ResetDevice)
                    };
                }
                else
                {
                    Powers[InitialIndex] = new NIPower
                    {
                        Power = new NIDCPower(connection.ResourceName, connection.ChannelName, 
                                              connection.ResetDevice, connection.DriverSetupOptions)
                    };
                }

                Powers[InitialIndex].InstrumentModel = Powers[InitialIndex].Power.Identity.InstrumentModel;
                Powers[InitialIndex].IOResourceDescriptor = Powers[InitialIndex].Power.DriverOperation.IOResourceDescriptor;
                Powers[InitialIndex].SequenceName = "SequenceName_" + connection.ResourceName;
                Powers[InitialIndex].ExpectedMeasurementCount = 0;

                if (Powers[InitialIndex].IOResourceDescriptor == connection.ResourceName)
                {
                    Powers[InitialIndex].IsOpen = true;
                    InitialIndex++;
                    return Success();
                }
                else
                {
                    CloseSession();
                    return Fail(SourceMeterErrorCode.InvalidArgument, "開啟 NI-DCPower Session 失敗");
                }
            }
            catch (Exception ex)
            {
                CloseSession();
                return DriverFailure("開啟 NI-DCPower Session 失敗", ex);
            }
        }

        public void BindingDeviceIndex(string id)
        {
            for (int i = 0; i < Powers.Length; i++)
            {
                if (Powers[i].IOResourceDescriptor == id)
                {
                    DeviceIndex[id] = i;
                    break;
                }
            }
        }

        public int SetConfiguration(string id, SourceMeterConfiguration configuration)
        {
            int index = GetDeviceIndex(id);

            if (index == -1)
                return Fail(SourceMeterErrorCode.InvalidArgument, "未找到對應的設備索引");

            NIPower nPower = Powers[index];
            NIDCPower Power = Powers[index].Power;

            try
            {
                DCPowerSourceOutputFunction outputFunction = ToNIOutputFunction(configuration.SourceFunction, configuration.OutputMode);
                DCPowerAdvancedSequenceProperty[] sequenceProperties = BuildSequenceProperties(configuration);

                Power.Source.Mode = DCPowerSourceMode.Sequence;
                Power.Source.SequenceLoopCount = configuration.LoopCount;

                Power.Outputs[nPower.ChannelName].Source.AdvancedSequencing.CreateAdvancedSequence(nPower.SequenceName, sequenceProperties, true);
                Power.Outputs[nPower.ChannelName].Source.AdvancedSequencing.ActiveAdvancedSequence = nPower.SequenceName;

                double fixedSourceRange = ResolveFixedSourceRange(configuration);

                foreach (SourceMeterStep step in configuration.Steps)
                {
                    Power.Outputs[nPower.ChannelName].Source.AdvancedSequencing.CreateAdvancedSequenceStep(true);
                    ApplyStep(nPower, configuration, step, outputFunction, fixedSourceRange);
                }

                Power.Measurement.Configuration.MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete;
                Power.Triggers.SourceTrigger.Disable();
                ConfigureStartTrigger(nPower, configuration.StartTriggerInputTerminal);
                ConfigureExportSignal(nPower, DCPowerSignalSource.StartTrigger, configuration.StartTriggerOutputTerminal);
                ConfigureExportSignal(nPower, DCPowerSignalSource.SourceCompleteEvent, configuration.SourceCompleteEventOutputTerminal);
                ConfigureExportSignal(nPower, DCPowerSignalSource.PulseTrigger, configuration.PulseTriggerOutputTerminal);

                nPower.ExpectedMeasurementCount = checked(configuration.Steps.Count * configuration.LoopCount);

                nPower.Configuration = configuration;

                return Success();
            }
            catch (Exception ex)
            {
                nPower.ExpectedMeasurementCount = 0;
                Reset(id);
                return DriverFailure("設定 NI SourceMeter Sequence 失敗", ex);
            }
        }

        public int GetConfiguration(string id, out SourceMeterConfiguration configuration)
        {
            configuration = null;

            int index = GetDeviceIndex(id);

            if (index == -1)
                return Fail(SourceMeterErrorCode.InvalidArgument, "未找到對應的設備索引");

            configuration = Powers[index].Configuration;

            return Success();
        }

        public int OutputOn(string id)
        {
            int index = GetDeviceIndex(id);

            if (index == -1)
                return Fail(SourceMeterErrorCode.InvalidArgument, "未找到對應的設備索引");

            try
            {
                string channel_name = Powers[index].ChannelName;
                Powers[index].Power.Outputs[channel_name].Source.Output.Enabled = true;
                Powers[index].Power.Control.Commit();
                Powers[index].Power.Control.Initiate();
                return Success();
            }
            catch (Exception ex)
            {
                Reset(id);
                return DriverFailure("啟動 NI SourceMeter 輸出失敗", ex);
            }
        }

        public int WaitForCompletion(string id, double timeoutSeconds)
        {
            int index = GetDeviceIndex(id);

            if (index == -1 || Powers[index] == null)
                return Fail(SourceMeterErrorCode.InvalidArgument, "未找到對應的設備索引");

            if (!IsPositiveFinite(timeoutSeconds))
                return Fail(SourceMeterErrorCode.InvalidArgument, "timeoutSeconds 必須大於 0。", index);

            try
            {
                Powers[index].Power.Events.SequenceEngineDoneEvent.WaitForEvent(new PrecisionTimeSpan(timeoutSeconds));
                return Success(index);
            }
            catch (Exception ex)
            {
                return DriverFailure("等待 NI Sequence 完成失敗", ex, index);
            }
        }

        public int Fetch(string id, int count, double timeoutSeconds, out IList<SourceMeterMeasurement> measurements)
        {
            measurements = new List<SourceMeterMeasurement>();

            int index = GetDeviceIndex(id);

            if (index == -1 || Powers[index] == null)
                return Fail(SourceMeterErrorCode.InvalidArgument, "未找到對應的設備索引");

            int fetchCount = count == 0 ? Powers[index].ExpectedMeasurementCount : count;

            if (Powers[index].ExpectedMeasurementCount > 0 && fetchCount > Powers[index].ExpectedMeasurementCount)
                return Fail(SourceMeterErrorCode.InvalidArgument, "要求的 Fetch 數量大於目前 Sequence 預期量測數量。", index);

            try
            {
                PrecisionTimeSpan timeout = new PrecisionTimeSpan(timeoutSeconds);
                Powers[index].Power.Events.SequenceEngineDoneEvent.WaitForEvent(timeout);

                // 取得量測數值
                DCPowerFetchResult result = Powers[index].Power.Measurement.Fetch(
                    Powers[index].ChannelName,
                    timeout,
                    fetchCount);

                List<SourceMeterMeasurement> values = new List<SourceMeterMeasurement>(fetchCount);
                for (int measurementIndex = 0; measurementIndex < fetchCount; measurementIndex++)
                {
                    values.Add(new SourceMeterMeasurement
                    {
                        Voltage = result.VoltageMeasurements[measurementIndex],
                        Current = result.CurrentMeasurements[measurementIndex]
                    });
                }

                measurements = values;
                return Success(index);
            }
            catch (Exception ex)
            {
                return DriverFailure("取得 NI SourceMeter 量測結果失敗", ex, index);
            }
        }

        public int OutputOff(string id)
        {
            int index = GetDeviceIndex(id);

            if (index == -1 || Powers[index] == null)
                return Fail(SourceMeterErrorCode.InvalidArgument, "未找到對應的設備索引");

            try
            {
                string channelName = Powers[index].ChannelName;
                Powers[index].Power.Control.Abort();
                Powers[index].Power.Outputs[channelName].Source.Output.Enabled = false;
                Powers[index].Power.Control.Commit();
                return Success(index);
            }
            catch (Exception ex)
            {
                return DriverFailure("關閉 NI SourceMeter 輸出失敗", ex, index);
            }
        }

        public int Reset(string id)
        {
            int index = GetDeviceIndex(id);

            if (index == -1 || Powers[index] == null)
                return Fail(SourceMeterErrorCode.InvalidArgument, "未找到對應的設備索引");

            return Reset(index);
        }

        public int SetOutputRelay(string id, bool connected)
        {
            int index = GetDeviceIndex(id);

            if (index == -1 || Powers[index] == null)
                return Fail(SourceMeterErrorCode.InvalidArgument, "未找到對應的設備索引");

            try
            {
                string channelName = Powers[index].ChannelName;
                Powers[index].Power.Outputs[channelName].Source.Output.Connected = connected;
                Powers[index].Power.Control.Commit();
                return Success(index);
            }
            catch (Exception ex)
            {
                return DriverFailure(
                    connected
                        ? "閉合 NI SourceMeter 輸出 Relay 失敗"
                        : "斷開 NI SourceMeter 輸出 Relay 失敗",
                    ex,
                    index);
            }
        }








        private int GetDeviceIndex(string serialNumber)
        {
            if (DeviceIndex.TryGetValue(serialNumber, out int index))
                return index;
            else
                return -1; // 未找到對應的設備索引
        }

        private int Reset(int index)
        {
            try
            {
                string channelName = Powers[index].ChannelName;
                Powers[index].Power.Control.Abort();
                Powers[index].Power.Outputs[channelName].Source.AdvancedSequencing.DeleteAdvancedSequence(
                    Powers[index].SequenceName);
                Powers[index].Power.Utility.Reset();
                Powers[index].ExpectedMeasurementCount = 0;
                return Success(index);
            }
            catch (Exception ex)
            {
                return DriverFailure("重設 NI SourceMeter 失敗", ex, index);
            }
        }












        

        

        public int Close()
        {
            return CloseSession();
        }

        private void ApplyStep(NIPower device, SourceMeterConfiguration configuration, SourceMeterStep step,
                               DCPowerSourceOutputFunction outputFunction, double fixedSourceRange)
        {
            double sourceRange = configuration.SourceRange > 0 ? Math.Abs(configuration.SourceRange) : configuration.AutoRangePerStep
                    ? RangeRequest(configuration.SourceFunction, step.Level) : fixedSourceRange;

            double complianceRange = configuration.ComplianceRange > 0
                ? Math.Abs(configuration.ComplianceRange)
                : RangeRequest(Opposite(configuration.SourceFunction), configuration.ComplianceLimit);

            DCPowerOutputSource source = device.Power.Outputs[device.ChannelName].Source;
            source.Output.Function = outputFunction;
            source.SourceDelay = new PrecisionTimeSpan(step.SourceDelaySeconds);
            source.TransientResponse = ToNiTransientResponse(configuration.TransientResponse);
            device.Power.Outputs[device.ChannelName].Measurement.Sense = ToNiMeasurementSense(configuration.MeasurementSense);
            device.Power.Outputs[device.ChannelName].Measurement.ConfigureApertureTime(
                step.ApertureTimeSeconds,
                DCPowerMeasureApertureTimeUnits.Seconds);

            if (configuration.OutputMode == SourceMeterOutputMode.ContinuousWave)
            {
                ApplyContinuousWaveStep(
                    configuration,
                    step,
                    sourceRange,
                    complianceRange,
                    source);
            }
            else
            {
                ApplyPulseStep(
                    configuration,
                    step,
                    sourceRange,
                    complianceRange,
                    source);
            }
        }

        private void ApplyContinuousWaveStep(
            SourceMeterConfiguration configuration,
            SourceMeterStep step,
            double sourceRange,
            double complianceRange,
            DCPowerOutputSource source)
        {
            if (configuration.SourceFunction == SourceMeterSourceFunction.Current)
            {
                source.Current.CurrentLevel = step.Level;
                source.Current.CurrentLevelRange = sourceRange;
                source.Current.VoltageLimit = Math.Abs(configuration.ComplianceLimit);
                source.Current.VoltageLimitRange = complianceRange;
            }
            else
            {
                source.Voltage.VoltageLevel = step.Level;
                source.Voltage.VoltageLevelRange = sourceRange;
                source.Voltage.CurrentLimit = Math.Abs(configuration.ComplianceLimit);
                source.Voltage.CurrentLimitRange = complianceRange;
            }
        }

        private void ApplyPulseStep(
            SourceMeterConfiguration configuration,
            SourceMeterStep step,
            double sourceRange,
            double complianceRange,
            DCPowerOutputSource source)
        {
            source.PulseOnTime = configuration.PulseOnTimeSeconds;
            source.PulseOffTime = configuration.PulseOffTimeSeconds;
            source.PulseBiasDelay = new PrecisionTimeSpan(configuration.PulseBiasDelaySeconds);

            if (configuration.SourceFunction == SourceMeterSourceFunction.Current)
            {
                source.PulseCurrent.CurrentLevel = step.Level;
                source.PulseCurrent.CurrentLevelRange = sourceRange;
                source.PulseCurrent.BiasCurrentLevel = configuration.PulseBiasLevel;
                source.PulseCurrent.VoltageLimit = Math.Abs(configuration.ComplianceLimit);
                source.PulseCurrent.VoltageLimitRange = complianceRange;
                source.PulseCurrent.BiasVoltageLimit = Math.Abs(configuration.ComplianceLimit);
            }
            else
            {
                source.PulseVoltage.VoltageLevel = step.Level;
                source.PulseVoltage.VoltageLevelRange = sourceRange;
                source.PulseVoltage.BiasVoltageLevel = configuration.PulseBiasLevel;
                source.PulseVoltage.CurrentLimit = Math.Abs(configuration.ComplianceLimit);
                source.PulseVoltage.CurrentLimitRange = complianceRange;
                source.PulseVoltage.BiasCurrentLimit = Math.Abs(configuration.ComplianceLimit);
            }
        }

        private DCPowerAdvancedSequenceProperty[] BuildSequenceProperties(SourceMeterConfiguration configuration)
        {
            List<DCPowerAdvancedSequenceProperty> properties =new List<DCPowerAdvancedSequenceProperty>
                                                                {
                                                                    DCPowerAdvancedSequenceProperty.SourceDelay,
                                                                    DCPowerAdvancedSequenceProperty.ApertureTime,
                                                                    DCPowerAdvancedSequenceProperty.TransientResponse
                                                                };

            if (configuration.OutputMode == SourceMeterOutputMode.ContinuousWave)
            {
                if (configuration.SourceFunction == SourceMeterSourceFunction.Current)
                {
                    properties.Add(DCPowerAdvancedSequenceProperty.CurrentLevel);
                    properties.Add(DCPowerAdvancedSequenceProperty.CurrentLevelRange);
                }
                else
                {
                    properties.Add(DCPowerAdvancedSequenceProperty.VoltageLevel);
                    properties.Add(DCPowerAdvancedSequenceProperty.VoltageLevelRange);
                }
            }
            else
            {
                properties.Add(DCPowerAdvancedSequenceProperty.PulseOnTime);
                properties.Add(DCPowerAdvancedSequenceProperty.PulseOffTime);
                properties.Add(DCPowerAdvancedSequenceProperty.PulseBiasDelay);

                if (configuration.SourceFunction == SourceMeterSourceFunction.Current)
                {
                    properties.Add(DCPowerAdvancedSequenceProperty.PulseCurrentLevel);
                    properties.Add(DCPowerAdvancedSequenceProperty.PulseCurrentLevelRange);
                    properties.Add(DCPowerAdvancedSequenceProperty.PulseBiasCurrentLevel);
                }
                else
                {
                    properties.Add(DCPowerAdvancedSequenceProperty.PulseVoltageLevel);
                    properties.Add(DCPowerAdvancedSequenceProperty.PulseVoltageLevelRange);
                    properties.Add(DCPowerAdvancedSequenceProperty.PulseBiasVoltageLevel);
                }
            }

            return properties.ToArray();
        }

        private void ConfigureStartTrigger(NIPower device, SourceMeterTriggerTerminal inputTerminal)
        {
            if (inputTerminal == SourceMeterTriggerTerminal.None)
            {
                device.Power.Triggers.StartTrigger.Disable();
                return;
            }

            device.Power.Triggers.StartTrigger.DigitalEdge.Configure(
                ToNIStartTriggerInputTerminal(inputTerminal),
                DCPowerTriggerEdge.Rising);
        }

        private void ConfigureExportSignal(NIPower device,
                                           DCPowerSignalSource signalSource,
                                           SourceMeterTriggerTerminal outputTerminal)
        {
            if (outputTerminal == SourceMeterTriggerTerminal.None)
                return;

            string terminalName = "/" + device.ResourceName + "/PXI_Trig" + (int)outputTerminal;
            device.Power.ExportSignal(signalSource, terminalName);
        }

        private DCPowerDigitalEdgeStartTriggerInputTerminal ToNIStartTriggerInputTerminal(SourceMeterTriggerTerminal inputTerminal)
        {
            switch (inputTerminal)
            {
                case SourceMeterTriggerTerminal.PxiTriggerLine0:
                    return DCPowerDigitalEdgeStartTriggerInputTerminal.PxiTriggerLine0;
                case SourceMeterTriggerTerminal.PxiTriggerLine1:
                    return DCPowerDigitalEdgeStartTriggerInputTerminal.PxiTriggerLine1;
                case SourceMeterTriggerTerminal.PxiTriggerLine2:
                    return DCPowerDigitalEdgeStartTriggerInputTerminal.PxiTriggerLine2;
                case SourceMeterTriggerTerminal.PxiTriggerLine3:
                    return DCPowerDigitalEdgeStartTriggerInputTerminal.PxiTriggerLine3;
                case SourceMeterTriggerTerminal.PxiTriggerLine4:
                    return DCPowerDigitalEdgeStartTriggerInputTerminal.PxiTriggerLine4;
                case SourceMeterTriggerTerminal.PxiTriggerLine5:
                    return DCPowerDigitalEdgeStartTriggerInputTerminal.PxiTriggerLine5;
                case SourceMeterTriggerTerminal.PxiTriggerLine6:
                    return DCPowerDigitalEdgeStartTriggerInputTerminal.PxiTriggerLine6;
                case SourceMeterTriggerTerminal.PxiTriggerLine7:
                    return DCPowerDigitalEdgeStartTriggerInputTerminal.PxiTriggerLine7;
                default:
                    return DCPowerDigitalEdgeStartTriggerInputTerminal.None;
            }
        }

        private DCPowerSourceOutputFunction ToNIOutputFunction(SourceMeterSourceFunction sourceFunction,
                                                               SourceMeterOutputMode outputMode)
        {
            if (sourceFunction == SourceMeterSourceFunction.Current)
            {
                return outputMode == SourceMeterOutputMode.Pulse? DCPowerSourceOutputFunction.PulseCurrent
                                                                : DCPowerSourceOutputFunction.DCCurrent;
            }

            // default:Voltage
            return outputMode == SourceMeterOutputMode.Pulse ? DCPowerSourceOutputFunction.PulseVoltage
                                                             : DCPowerSourceOutputFunction.DCVoltage;
        }

        private DCPowerMeasurementSense ToNiMeasurementSense(SourceMeterMeasurementSense sense)
        {
            return sense == SourceMeterMeasurementSense.Local? DCPowerMeasurementSense.Local : DCPowerMeasurementSense.Remote;
        }

        private DCPowerSourceTransientResponse ToNiTransientResponse(SourceMeterTransientResponse response)
        {
            switch (response)
            {
                case SourceMeterTransientResponse.Slow:
                    return DCPowerSourceTransientResponse.Slow;
        case SourceMeterTransientResponse.Fast:
            return DCPowerSourceTransientResponse.Fast;
        default:
            return DCPowerSourceTransientResponse.Normal;
    }
}

        private SourceMeterSourceFunction Opposite(SourceMeterSourceFunction sourceFunction)
        {
            return sourceFunction == SourceMeterSourceFunction.Current
                ? SourceMeterSourceFunction.Voltage
                : SourceMeterSourceFunction.Current;
        }

        private double RangeRequest(SourceMeterSourceFunction function, double value)
        {
            double absoluteValue = Math.Abs(value);
            if (absoluteValue > 0)
                return absoluteValue;

            return function == SourceMeterSourceFunction.Current ? 1e-9 : 1e-3;
        }

        private double ResolveFixedSourceRange(SourceMeterConfiguration configuration)
        {
            if (configuration.SourceRange > 0)
                return Math.Abs(configuration.SourceRange);

            if (configuration.Steps == null || configuration.Steps.Count == 0)
                return RangeRequest(configuration.SourceFunction, 0);

            double maximumLevel = configuration.Steps.Max(step => Math.Abs(step.Level));
            return RangeRequest(configuration.SourceFunction, maximumLevel);
        }

        private int CloseSession()
        {
            int result = (int)SourceMeterErrorCode.Success;

            for (int index = 0; index < Powers.Length; index++)
            {
                if (Powers[index] == null)
                    continue;

                try
                {
                    string channelName = Powers[index].ChannelName;
                    Powers[index].Power.Control.Abort();
                    Powers[index].Power.Outputs[channelName].Source.Output.Enabled = false;
                    Powers[index].Power.Control.Commit();
                    Powers[index].Power.Dispose();
                }
                catch (Exception ex)
                {
                    result = DriverFailure("關閉 NI-DCPower Session 時發生錯誤", ex, index);
                }
                finally
                {
                    Powers[index].Power = null;
                    Powers[index].ResourceName = string.Empty;
                    Powers[index].InstrumentModel = string.Empty;
                    Powers[index].IOResourceDescriptor = string.Empty;
                    Powers[index].ExpectedMeasurementCount = 0;
                    Powers[index].IsOpen = false;
                    Powers[index] = null;
                }
            }

            DeviceIndex.Clear();
            InitialIndex = 0;
            return result;
        }

        private int Success(int index = -1)
        {
            if (index >= 0 && index < Powers.Length && Powers[index] != null)
                Powers[index].LastError = string.Empty;

            return (int)SourceMeterErrorCode.Success;
        }

        private int Fail(SourceMeterErrorCode code, string message, int index = -1)
        {
            if (index >= 0 && index < Powers.Length && Powers[index] != null)
                Powers[index].LastError = message ?? string.Empty;

            return (int)code;
        }

        private int DriverFailure(string operation, Exception exception, int index = -1)
        {
            if (index >= 0 && index < Powers.Length && Powers[index] != null)
                Powers[index].LastError = operation + ": " + exception.Message;

            return (int)SourceMeterErrorCode.DriverError;
        }

        private bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private bool IsPositiveFinite(double value)
        {
            return IsFinite(value) && value > 0;
        }
    }
}
