using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;
using DeviceCore;

namespace PhotonicTest.Logic
{
    public class BaseTest
    {
        // 基本測試功能
        public BaseTest(IFunction_SourceMeter function_SourceMeter)
        {
            SMU = function_SourceMeter;
        }

        #region parameter define
        IFunction_SourceMeter SMU;
        ESMUName Ch = ESMUName.SMU_1;
        #endregion

        public void SetChannel(ESMUName ch)
        {
            Ch = ch;
        }

        // CW Mode單通道送電流量電壓
        public virtual bool SIMV(double current, double voltage_limit, double delay_time,
                                 out double res_i, out double res_v)
        {
            res_i = 0; res_v = 0;

            // 參數設定
            SourceMeterConfiguration config = new SourceMeterConfiguration()
            {
                SourceFunction = SourceMeterSourceFunction.Current,
                OutputMode = SourceMeterOutputMode.ContinuousWave,
                ComplianceLimit = voltage_limit,

                SourceRange = 0,
                ComplianceRange = 0,
                AutoRangePerStep = true,

                LoopCount = 1,

                Steps = new List<SourceMeterStep>
                {
                    new SourceMeterStep
                    {
                        Level = current,
                        SourceDelaySeconds = delay_time*0.2,    // 取20%當作爬升時間
                        ApertureTimeSeconds = delay_time*0.8    // 取80%當作量測
                    }
                },
            };

            try
            {
                if (!SMU.SetConfig(Ch, config))
                    return false;

                if (!SMU.OutputOn(Ch))
                    return false;

                IList<SourceMeterMeasurement> measurements;
                if (!SMU.GetMeasureResult(Ch, 1, out measurements))
                    return false;

                if (measurements == null || measurements.Count == 0)
                {
                    Tool.SaveLogToFile($"沒有取得量測資料");
                    return false;
                }

                res_v = measurements[0].Voltage;
                res_i = measurements[0].Current;

                return true;
            }
            finally
            {
                SMU.OutputOff(Ch);
            }
        }

        // CW Mode單通道送電壓量電流
        public virtual bool SVMI(double voltage, double current_limit, double delay_time,
                                 out double res_i, out double res_v)
        {
            res_i = 0; res_v = 0;

            // 參數設定
            SourceMeterConfiguration config = new SourceMeterConfiguration()
            {
                SourceFunction = SourceMeterSourceFunction.Voltage,
                OutputMode = SourceMeterOutputMode.ContinuousWave,
                ComplianceLimit = current_limit,

                SourceRange = 0,
                ComplianceRange = 0,
                AutoRangePerStep = true,

                LoopCount = 1,

                Steps = new List<SourceMeterStep>
                {
                    new SourceMeterStep
                    {
                        Level = voltage,
                        SourceDelaySeconds = delay_time*0.2,    // 取20%當作爬升時間
                        ApertureTimeSeconds = delay_time*0.8    // 取80%當作量測
                    }
                },
            };

            try
            {
                if (!SMU.SetConfig(Ch, config))
                    return false;

                if (!SMU.OutputOn(Ch))
                    return false;

                IList<SourceMeterMeasurement> measurements;
                if (!SMU.GetMeasureResult(Ch, 1, out measurements))
                    return false;

                if (measurements == null || measurements.Count == 0)
                {
                    Tool.SaveLogToFile($"沒有取得量測資料");
                    return false;
                }

                res_v = measurements[0].Voltage;
                res_i = measurements[0].Current;

                return true;
            }
            finally
            {
                SMU.OutputOff(Ch);
            }
        }

    }
}
