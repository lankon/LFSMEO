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
        private NIDCPower Power;                        // NI-DCPower Driver Session。
        private string ResourceName;                    // NI 儀器的 Resource Name，例如 PXI1Slot2。
        private string ChannelName = "0";               // Session 使用的 NI 實體通道，預設為通道 0。
        private string SequenceName;                    // 建立於 NI Driver 內的 Advanced Sequence 名稱。
        private int ExpectedMeasurementCount;           // Sequence Step 數乘以 LoopCount 的預期量測筆數。

        public bool IsOpen { get; private set; } = false;                           // 是否完成初始化
        public string InstrumentModel { get; private set; } = string.Empty;         // NI 儀器型號。
        public string IOResourceDescriptor { get; private set; } = string.Empty;    // Driver 實際使用的 I/O Resource Descriptor。
        public string LastError { get; private set; } = string.Empty;               // 最近一次呼叫的錯誤訊息。
        #endregion

        public int Open(SourceMeterConnection connection)
        {
            CloseSession();

            try
            {
                ResourceName = connection.ResourceName;
                ChannelName = connection.ChannelName;

                if (string.IsNullOrWhiteSpace(connection.DriverSetupOptions))
                {
                    Power = new NIDCPower(
                        connection.ResourceName,
                        ChannelName,
                        connection.ResetDevice);
                }
                else
                {
                    Power = new NIDCPower(
                        connection.ResourceName,
                        ChannelName,
                        connection.ResetDevice,
                        connection.DriverSetupOptions);
                }

                InstrumentModel = Power.Identity.InstrumentModel;
                IOResourceDescriptor = Power.DriverOperation.IOResourceDescriptor;
                SequenceName = "SequenceName_" + connection.ResourceName;
                ExpectedMeasurementCount = 0;
                IsOpen = true;

                return Success();
            }
            catch (Exception ex)
            {
                CloseSession();
                return DriverFailure("開啟 NI-DCPower Session 失敗", ex);
            }
        }

        // 要修改
        public int Configure(SourceMeterConfiguration configuration)
        {
            try
            {
                DCPowerSourceOutputFunction outputFunction = ToNIOutputFunction(configuration.SourceFunction, configuration.OutputMode);
                DCPowerAdvancedSequenceProperty[] sequenceProperties = BuildSequenceProperties(configuration);

                Power.Source.Mode = DCPowerSourceMode.Sequence;
                Power.Source.SequenceLoopCount = configuration.LoopCount;
                Power.Outputs[ChannelName].Source.AdvancedSequencing.CreateAdvancedSequence(SequenceName, sequenceProperties, true);
                Power.Outputs[ChannelName].Source.AdvancedSequencing.ActiveAdvancedSequence = SequenceName;

                double fixedSourceRange = ResolveFixedSourceRange(configuration);

                foreach (SourceMeterStep step in configuration.Steps)
                {
                    Power.Outputs[ChannelName].Source.AdvancedSequencing.CreateAdvancedSequenceStep(true);
                    ApplyStep(configuration, step, outputFunction, fixedSourceRange);
                }

                Power.Measurement.Configuration.MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete;
                Power.Triggers.SourceTrigger.Disable();
                ConfigureStartTrigger(configuration.StartTriggerInputTerminal);
                ConfigureExportSignal(DCPowerSignalSource.StartTrigger, configuration.StartTriggerOutputTerminal);
                ConfigureExportSignal(DCPowerSignalSource.SourceCompleteEvent, configuration.SourceCompleteEventOutputTerminal);
                ConfigureExportSignal(DCPowerSignalSource.PulseTrigger, configuration.PulseTriggerOutputTerminal);

                ExpectedMeasurementCount = checked(configuration.Steps.Count * configuration.LoopCount);

                return Success();
            }
            catch (Exception ex)
            {
                ExpectedMeasurementCount = 0;
                Reset();
                return DriverFailure("設定 NI SourceMeter Sequence 失敗", ex);
            }
        }

        public int OutputOn()
        {
            try
            {
                Power.Outputs[ChannelName].Source.Output.Enabled = true;
                Power.Control.Commit();
                Power.Control.Initiate();
                return Success();
            }
            catch (Exception ex)
            {
                Reset();
                return DriverFailure("啟動 NI SourceMeter 輸出失敗", ex);
            }
        }

        public int WaitForCompletion(double timeoutSeconds)
        {
            if (!IsPositiveFinite(timeoutSeconds))
                return Fail(SourceMeterErrorCode.InvalidArgument, "timeoutSeconds 必須大於 0。");

            try
            {
                Power.Events.SequenceEngineDoneEvent.WaitForEvent(new PrecisionTimeSpan(timeoutSeconds));
                return Success();
            }
            catch (Exception ex)
            {
                return DriverFailure("等待 NI Sequence 完成失敗", ex);
            }
        }

        public int Fetch(int count, double timeoutSeconds, out IList<SourceMeterMeasurement> measurements)
        {
            measurements = new List<SourceMeterMeasurement>();

            int fetchCount = count == 0 ? ExpectedMeasurementCount : count;

            if (ExpectedMeasurementCount > 0 && fetchCount > ExpectedMeasurementCount)
                return Fail(SourceMeterErrorCode.InvalidArgument, "要求的 Fetch 數量大於目前 Sequence 預期量測數量。");

            try
            {
                PrecisionTimeSpan timeout = new PrecisionTimeSpan(timeoutSeconds);
                Power.Events.SequenceEngineDoneEvent.WaitForEvent(timeout);

                // 取得量測數值
                DCPowerFetchResult result = Power.Measurement.Fetch(ChannelName, timeout, fetchCount);

                List<SourceMeterMeasurement> values = new List<SourceMeterMeasurement>(fetchCount);
                for (int index = 0; index < fetchCount; index++)
                {
                    values.Add(new SourceMeterMeasurement
                    {
                        Voltage = result.VoltageMeasurements[index],
                        Current = result.CurrentMeasurements[index]
                    });
                }

                measurements = values;
                return Success();
            }
            catch (Exception ex)
            {
                return DriverFailure("取得 NI SourceMeter 量測結果失敗", ex);
            }
        }

        public int OutputOff()
        {
            try
            {
                Power.Control.Abort();
                Power.Outputs[ChannelName].Source.Output.Enabled = false;
                Power.Control.Commit();
                return Success();
            }
            catch (Exception ex)
            {
                return DriverFailure("關閉 NI SourceMeter 輸出失敗", ex);
            }
        }

        public int Reset()
        {
            try
            {
                Power.Control.Abort();
                Power.Outputs[ChannelName].Source.AdvancedSequencing.DeleteAdvancedSequence(SequenceName);
                Power.Utility.Reset();
                ExpectedMeasurementCount = 0;
                return Success();
            }
            catch (Exception ex)
            {
                return DriverFailure("重設 NI SourceMeter 失敗", ex);
            }
        }

        public int SetOutputRelay(bool connected)
        {
            try
            {
                Power.Outputs[ChannelName].Source.Output.Connected = connected;
                Power.Control.Commit();
                return Success();
            }
            catch (Exception ex)
            {
                return DriverFailure(
                    connected
                        ? "閉合 NI SourceMeter 輸出 Relay 失敗"
                        : "斷開 NI SourceMeter 輸出 Relay 失敗",
                    ex);
            }
        }

        public int Close()
        {
            return CloseSession();
        }

        private void ApplyStep(SourceMeterConfiguration configuration, SourceMeterStep step,
                               DCPowerSourceOutputFunction outputFunction, double fixedSourceRange)
        {
            double sourceRange = configuration.SourceRange > 0 ? Math.Abs(configuration.SourceRange) : configuration.AutoRangePerStep
                    ? RangeRequest(configuration.SourceFunction, step.Level) : fixedSourceRange;

            double complianceRange = configuration.ComplianceRange > 0
                ? Math.Abs(configuration.ComplianceRange)
                : RangeRequest(Opposite(configuration.SourceFunction), configuration.ComplianceLimit);

            DCPowerOutputSource source = Power.Outputs[ChannelName].Source;
            source.Output.Function = outputFunction;
            source.SourceDelay = new PrecisionTimeSpan(step.SourceDelaySeconds);
            source.TransientResponse = ToNiTransientResponse(configuration.TransientResponse);
            Power.Outputs[ChannelName].Measurement.Sense = ToNiMeasurementSense(configuration.MeasurementSense);
            Power.Outputs[ChannelName].Measurement.ConfigureApertureTime(step.ApertureTimeSeconds, DCPowerMeasureApertureTimeUnits.Seconds);

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

        private void ConfigureStartTrigger(SourceMeterTriggerTerminal inputTerminal)
        {
            if (inputTerminal == SourceMeterTriggerTerminal.None)
            {
                Power.Triggers.StartTrigger.Disable();
                return;
            }

            Power.Triggers.StartTrigger.DigitalEdge.Configure(
                ToNIStartTriggerInputTerminal(inputTerminal),
                DCPowerTriggerEdge.Rising);
        }

        private void ConfigureExportSignal(DCPowerSignalSource signalSource,
                                           SourceMeterTriggerTerminal outputTerminal)
        {
            if (outputTerminal == SourceMeterTriggerTerminal.None)
                return;

            string terminalName = "/" + ResourceName + "/PXI_Trig" + (int)outputTerminal;
            Power.ExportSignal(signalSource, terminalName);
        }

        private DCPowerDigitalEdgeStartTriggerInputTerminal ToNIStartTriggerInputTerminal(
            SourceMeterTriggerTerminal inputTerminal)
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

            double maximumLevel = configuration.Steps.Max(step => Math.Abs(step.Level));
            return RangeRequest(configuration.SourceFunction, maximumLevel);
        }

        private int CloseSession()
        {
            try
            {
                if (Power != null)
                {
                    Power.Control.Abort();
                    Power.Outputs[ChannelName].Source.Output.Enabled = false;
                    Power.Control.Commit();
                    Power.Dispose();
                }

                return Success();
            }
            catch (Exception ex)
            {
                return DriverFailure("關閉 NI-DCPower Session 時發生錯誤", ex);
            }
            finally
            {
                Power = null;
                ResourceName = string.Empty;
                InstrumentModel = string.Empty;
                IOResourceDescriptor = string.Empty;
                ExpectedMeasurementCount = 0;
                IsOpen = false;
            }
        }

        private int Success()
        {
            LastError = string.Empty;
            return (int)SourceMeterErrorCode.Success;
        }

        private int Fail(SourceMeterErrorCode code, string message)
        {
            LastError = message ?? string.Empty;
            return (int)code;
        }

        private int DriverFailure(string operation, Exception exception)
        {
            LastError = operation + ": " + exception.Message;
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
