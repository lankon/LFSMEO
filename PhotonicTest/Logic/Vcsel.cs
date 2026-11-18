using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;
using ToolFunction;

namespace PhotonicTest.Logic
{
    public sealed class LIVPoint
    {
        public double LDCurrentSet { get; set; }
        public double LDCurrentMeasured { get; set; }
        public double LDVoltage { get; set; }
        public double PDCurrent { get; set; }
        public double PDVoltage { get; set; }
    }

    public class Vcsel : BaseTest
    {
        public Vcsel(IFunction_SourceMeter function_SourceMeter) : base(function_SourceMeter)
        {
            SMU = function_SourceMeter;
        }

        #region parameter define
        private readonly IFunction_SourceMeter SMU;
        private ESMUName LD = ESMUName.SMU_1;
        private ESMUName PD = ESMUName.SMU_2;
        #endregion

        public void SetVcselChannel(ESMUName LD, ESMUName PD)
        {
            this.LD = LD;
            this.PD = PD;
        }


        // CW Mode 量測光功率及電壓值
        public bool LIV(IList<double> ld_current_steps,
                        double ld_voltage_limit,
                        double pd_bias_voltage,
                        double pd_current_limit,
                        double source_delay_seconds,
                        double aperture_time_seconds,
                        out IList<LIVPoint> results)
        {
            results = new List<LIVPoint>();

            if (ld_current_steps == null || ld_current_steps.Count == 0)
            {
                Tool.SaveLogToFile("LIV 沒有設定 LD 電流 Step", level: "ERR");
                return false;
            }

            int point_count = ld_current_steps.Count;

            SourceMeterConfiguration ld_config = new SourceMeterConfiguration
            {
                SourceFunction = SourceMeterSourceFunction.Current,
                OutputMode = SourceMeterOutputMode.ContinuousWave,
                MeasurementMode = SourceMeterMeasurementMode.AutomaticallyAfterSourceComplete,
                ComplianceLimit = ld_voltage_limit,
                SourceRange = ld_current_steps.Max(current => Math.Abs(current)),
                ComplianceRange = 0,
                AutoRangePerStep = false,


                Steps = ld_current_steps.Select(current => new SourceMeterStep
                {
                    Level = current,
                    SourceDelaySeconds = source_delay_seconds,
                    ApertureTimeSeconds = aperture_time_seconds
                }).ToList()
            };

            SourceMeterConfiguration pd_config = new SourceMeterConfiguration
            {
                SourceFunction = SourceMeterSourceFunction.Voltage,
                OutputMode = SourceMeterOutputMode.ContinuousWave,
                MeasurementMode = SourceMeterMeasurementMode.AutomaticallyAfterSourceComplete,
                ComplianceLimit = pd_current_limit,
                SourceRange = Math.Abs(pd_bias_voltage),
                ComplianceRange = 0,
                AutoRangePerStep = false,

                Steps = ld_current_steps.Select(current => new SourceMeterStep
                {
                    Level = pd_bias_voltage,
                    SourceDelaySeconds = source_delay_seconds,
                    ApertureTimeSeconds = aperture_time_seconds
                }).ToList()
            };

            bool ld_output_on = false;
            bool pd_output_on = false;

            try
            {
                if (!SMU.SetConfig(PD, pd_config))
                    return false;

                if (!SMU.SetConfig(LD, ld_config))
                    return false;

                // Slave 必須先進入 Running 狀態等待 TriggerLine0。
                if (!SMU.OutputOn(PD))
                    return false;

                pd_output_on = true;

                // Master 啟動時輸出 Start Trigger，讓 LD、PD 同時執行 Sequence。
                if (!SMU.OutputOn(LD))
                    return false;

                ld_output_on = true;

                double timeout_seconds = Math.Max(
                    5,
                    point_count * (source_delay_seconds + aperture_time_seconds) + 5);

                IList<SourceMeterMeasurement> ld_measurements;
                if (!SMU.GetMeasureResult(LD, point_count, out ld_measurements))
                    return false;

                IList<SourceMeterMeasurement> pd_measurements;
                if (!SMU.GetMeasureResult(PD, point_count, out pd_measurements))
                    return false;

                if (ld_measurements.Count < point_count || pd_measurements.Count < point_count)
                {
                    Tool.SaveLogToFile("LIV 取得的 LD 或 PD 資料筆數不足", level: "ERR");
                    return false;
                }

                List<LIVPoint> liv_results = new List<LIVPoint>(point_count);

                for (int index = 0; index < point_count; index++)
                {
                    liv_results.Add(new LIVPoint
                    {
                        LDCurrentSet = ld_current_steps[index],
                        LDCurrentMeasured = ld_measurements[index].Current,
                        LDVoltage = ld_measurements[index].Voltage,
                        PDCurrent = pd_measurements[index].Current,
                        PDVoltage = pd_measurements[index].Voltage
                    });
                }

                results = liv_results;
                return true;
            }
            finally
            {
                // 先關閉 LD，避免錯誤處理期間持續對雷射送電。
                if (ld_output_on)
                    SMU.OutputOff(LD);

                if (pd_output_on)
                    SMU.OutputOff(PD);
            }

        }

        // Pulse Mode 同步量測 LD 電流／電壓與 PD 光電流
        public bool LIV_Pulse(IList<double> ld_current_steps,
                              double ld_voltage_limit,
                              double pd_bias_voltage,
                              double pd_current_limit,
                              double pulse_width,
                              double duty,
                              out IList<LIVPoint> results)
        {
            results = new List<LIVPoint>();

            if (ld_current_steps == null || ld_current_steps.Count == 0)
            {
                Tool.SaveLogToFile("LIV_Pulse 沒有設定 LD 電流 Step", level: "ERR");
                return false;
            }

            int point_count = ld_current_steps.Count;
            double pulse_off_time = pulse_width / (duty * 0.01) - pulse_width;

            // Pulse On 期間保留 10% 裕量，避免 Aperture Time 超出 Pulse On Time。
            double source_delay_time = pulse_width * 0.2;
            double aperture_time = pulse_width * 0.7;

            SourceMeterConfiguration ld_config = new SourceMeterConfiguration
            {
                SourceFunction = SourceMeterSourceFunction.Current,
                OutputMode = SourceMeterOutputMode.Pulse,
                MeasurementMode = SourceMeterMeasurementMode.AutomaticallyAfterSourceComplete,
                ComplianceLimit = ld_voltage_limit,
                SourceRange = ld_current_steps.Max(current => Math.Abs(current)),
                ComplianceRange = 0,
                AutoRangePerStep = false,

                PulseOnTimeSeconds = pulse_width,
                PulseOffTimeSeconds = pulse_off_time,
                PulseBiasLevel = 0,

                Steps = ld_current_steps.Select(current => new SourceMeterStep
                {
                    Level = current,
                    SourceDelaySeconds = source_delay_time,
                    ApertureTimeSeconds = aperture_time
                }).ToList()
            };

            SourceMeterConfiguration pd_config = new SourceMeterConfiguration
            {
                SourceFunction = SourceMeterSourceFunction.Voltage,
                OutputMode = SourceMeterOutputMode.Pulse,
                MeasurementMode = SourceMeterMeasurementMode.AutomaticallyAfterSourceComplete,
                ComplianceLimit = pd_current_limit,
                SourceRange = Math.Abs(pd_bias_voltage),
                ComplianceRange = 0,
                AutoRangePerStep = false,

                PulseOnTimeSeconds = pulse_width,
                PulseOffTimeSeconds = pulse_off_time,
                PulseBiasLevel = 0,

                Steps = ld_current_steps.Select(current => new SourceMeterStep
                {
                    Level = pd_bias_voltage,
                    SourceDelaySeconds = source_delay_time,
                    ApertureTimeSeconds = aperture_time
                }).ToList()
            };

            bool ld_output_on = false;
            bool pd_output_on = false;

            try
            {
                if (!SMU.SetConfig(PD, pd_config))
                    return false;

                if (!SMU.SetConfig(LD, ld_config))
                    return false;

                // PD 先進入 Running 狀態，等待 LD 的 Start Trigger。
                if (!SMU.OutputOn(PD))
                    return false;

                pd_output_on = true;

                // LD 啟動後輸出 Start Trigger，使 LD、PD 的 Pulse Sequence 同時開始。
                if (!SMU.OutputOn(LD))
                    return false;

                ld_output_on = true;

                IList<SourceMeterMeasurement> ld_measurements;
                if (!SMU.GetMeasureResult(LD, point_count, out ld_measurements))
                    return false;

                IList<SourceMeterMeasurement> pd_measurements;
                if (!SMU.GetMeasureResult(PD, point_count, out pd_measurements))
                    return false;

                if (ld_measurements.Count < point_count || pd_measurements.Count < point_count)
                {
                    Tool.SaveLogToFile("LIV_Pulse 取得的 LD 或 PD 資料筆數不足", level: "ERR");
                    return false;
                }

                List<LIVPoint> liv_results = new List<LIVPoint>(point_count);

                for (int index = 0; index < point_count; index++)
                {
                    liv_results.Add(new LIVPoint
                    {
                        LDCurrentSet = ld_current_steps[index],
                        LDCurrentMeasured = ld_measurements[index].Current,
                        LDVoltage = ld_measurements[index].Voltage,
                        PDCurrent = pd_measurements[index].Current,
                        PDVoltage = pd_measurements[index].Voltage
                    });
                }

                results = liv_results;
                return true;
            }
            finally
            {
                // 先關閉 LD，避免錯誤處理期間持續對雷射送電。
                if (ld_output_on)
                    SMU.OutputOff(LD);

                if (pd_output_on)
                    SMU.OutputOff(PD);
            }
        }



    }
}
