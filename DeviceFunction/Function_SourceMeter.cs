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
                    SourceDelaySeconds = delay_time*0.05,   // 取5%當作爬升時間
                    ApertureTimeSeconds = delay_time
                }
            };

            int result = SMU.Configure(SMUConfig);
            if (result != (int)SourceMeterErrorCode.Success)
            {
                Tool.SaveLogToFile($"SIMV Configure Fail, SourceMeterErrorCode = {result}");
                return false;
            }

            try
            {
                result = SMU.OutputOn();

                if (result != (int)SourceMeterErrorCode.Success)
                {
                    Tool.SaveLogToFile($"SIMV OutputOn Fail, SourceMeterErrorCode = {result}");
                    return false;
                }

                IList<SourceMeterMeasurement> measurements;

                result = SMU.Fetch(count: 1, timeoutSeconds: 5, out measurements);

                if (result != (int)SourceMeterErrorCode.Success)
                {
                    Tool.SaveLogToFile($"SIMV Fetch Fail, SourceMeterErrorCode = {result}");
                    return false;
                }

                res_v = measurements[0].Voltage;    // 量測電壓結果
                res_i = measurements[0].Current;    // 輸出電流結果

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
