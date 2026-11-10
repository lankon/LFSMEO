using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;
using DeviceCore;

namespace DeviceFunction
{
    public class Function_SourceMeter
    {
        // 測試用
        public Function_SourceMeter(ISourceMeter sourceMeter)
        {
            SMU = sourceMeter;
        }

        #region parameter
        ISourceMeter SMU;
        SourceMeterConfiguration SMUConfig;
        #endregion

        public bool Open()
        {
            try
            {
                // 先寫死之後再開放設定
                int result = SMU.Open(new SourceMeterConnection()
                {
                    ResourceName = "PXI1Slot2",
                    ChannelName = "0",
                    ResetDevice = false,
                    DriverSetupOptions = "DriverSetup=SenseLeadErrorDetectionEnabled:0"
                });

                if(result != (int)SourceMeterErrorCode.Success)
                {
                    Tool.SaveLogToFile($"SMU Open Fail, SourceMeterErrorCode = {result}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Tool.SaveLogToFile($"SMU Open Fail:{ex}");
                return false;
            }

            Tool.SaveLogToFile($"SMU:{SMU.InstrumentModel} Open Success");
            return true;
        }

        public bool SetSMUConfig(SourceMeterConfiguration config)
        {
            // 設定SMU參數
            SMUConfig = config;
            return true;
        }

        public bool SIMV(double current, double voltage_limit, double delay_time, 
                         out double res_i, out double res_v)
        {
            res_i = 0;
            res_v = 0;
            SMUConfig.SourceFunction = SourceMeterSourceFunction.Current;
            SMUConfig.OutputMode = SourceMeterOutputMode.ContinuousWave;
            SMUConfig.ComplianceLimit = voltage_limit;

            SMUConfig.SourceRange = 0;
            SMUConfig.ComplianceRange = 0;
            SMUConfig.AutoRangePerStep = true;

            SMUConfig.LoopCount = 1;
            
            SMUConfig.Steps = new List<SourceMeterStep>
            {
                new SourceMeterStep
                {
                    Level = current,
                    SourceDelaySeconds = delay_time*0.2,    // 取20%當作爬升時間
                    ApertureTimeSeconds = delay_time*0.8    // 取80%當作量測
                }
            };

            return ExecuteSinglePoint("SIMV", out res_i, out res_v);
        }

        public bool SIMV_Pulse(double current, double voltage_limit, double pulse_width, double duty,
                               out double res_i, out double res_v)
        {
            // 單位: current(A), voltage_limit(V), pulse_width(s), Duty(%)
            res_i = 0;
            res_v = 0;

            if (pulse_width <= 0)
            {
                Tool.SaveLogToFile("SIMV_Pulse Fail: Pulse Width 必須是大於 0 的有限值");
                return false;
            }

            if (duty <= 0 || duty >= 100)
            {
                Tool.SaveLogToFile("SIMV_Pulse Fail: Duty 必須介於 0 與 100 之間");
                return false;
            }

            double pulse_off_time = pulse_width / (duty * 0.01) - pulse_width;
            double source_delay_time = pulse_width * 0.2;   // 上升緣區段,可能需要依照實際情況調整
            double aperture_time = pulse_width * 0.7;       // 避免取到下降緣,可能需要依照實際情況調整

            SMUConfig.SourceFunction = SourceMeterSourceFunction.Current;
            SMUConfig.OutputMode = SourceMeterOutputMode.Pulse;
            SMUConfig.ComplianceLimit = voltage_limit;

            SMUConfig.SourceRange = 0;
            SMUConfig.ComplianceRange = 0;
            SMUConfig.AutoRangePerStep = true;

            SMUConfig.LoopCount = 1;
            SMUConfig.PulseOnTimeSeconds = pulse_width;
            SMUConfig.PulseOffTimeSeconds = pulse_off_time;
            SMUConfig.PulseBiasLevel = 0;
            SMUConfig.PulseBiasDelaySeconds = 0;

            SMUConfig.Steps = new List<SourceMeterStep>
            {
                new SourceMeterStep
                {
                    Level = current,
                    SourceDelaySeconds = source_delay_time,
                    ApertureTimeSeconds = aperture_time
                }
            };

            return ExecuteSinglePoint("SIMV_Pulse", out res_i, out res_v);
        }

        public bool LIV()
        {
            return true;
        }




        private bool ExecuteSinglePoint(string function_name, out double res_i, out double res_v)
        {
            res_i = 0;
            res_v = 0;

            int result = SMU.Configure(SMUConfig);
            if (result != (int)SourceMeterErrorCode.Success)
            {
                Tool.SaveLogToFile($"{function_name} Configure Fail, SourceMeterErrorCode = {result}, LastError = {SMU.LastError}");
                return false;
            }

            try
            {
                result = SMU.OutputOn();

                if (result != (int)SourceMeterErrorCode.Success)
                {
                    Tool.SaveLogToFile($"{function_name} OutputOn Fail, SourceMeterErrorCode = {result}, LastError = {SMU.LastError}");
                    return false;
                }

                IList<SourceMeterMeasurement> measurements;
                result = SMU.Fetch(count: 1, timeoutSeconds: 5, out measurements);

                if (result != (int)SourceMeterErrorCode.Success)
                {
                    Tool.SaveLogToFile($"{function_name} Fetch Fail, SourceMeterErrorCode = {result}, LastError = {SMU.LastError}");
                    return false;
                }

                if (measurements == null || measurements.Count == 0)
                {
                    Tool.SaveLogToFile($"{function_name} Fetch Fail: 沒有取得量測資料");
                    return false;
                }

                res_v = measurements[0].Voltage;
                res_i = measurements[0].Current;

                return true;
            }
            finally
            {
                SMU.OutputOff();
                SMU.Reset();
            }
        }
    }
}
