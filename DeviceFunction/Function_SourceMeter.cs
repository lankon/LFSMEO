using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;
using DeviceCore;

namespace DeviceFunction
{
    public class Function_SourceMeter: IFunction_SourceMeter
    {
        // 測試用
        public Function_SourceMeter(IEnumerable<ISourceMeter> sourceMeter)
        {
            SMU = sourceMeter;
        }

        #region parameter
        private IEnumerable<ISourceMeter> SMU;
        private bool IsInitial = false;                 // 是否已經初始化過SMU
        private List<ISourceMeter> SourceMeterList = new List<ISourceMeter>();
        private Dictionary<string, SourceMeterData> SourceMeterListDict;        //存放UI SMU設定
        private SourceMeterConfiguration SMUConfig;
        #endregion

        #region private function
        private void BindingDeviceIndex()
        {
            foreach (KeyValuePair<string, SourceMeterData> entry in SourceMeterListDict)
            {
                string name = entry.Key;
                SourceMeterData smu = entry.Value;

                for (int j = 0; j < SourceMeterList.Count; j++)
                {
                    if (smu.Title_SMU_Type == SourceMeterList[j].GetSourceMeterType().ToString())
                    {
                        SourceMeterList[j].BindingDeviceIndex(smu.Title_CardID);
                        break;
                    }
                }
            }
        }
        #endregion

        public void LoadConfiguration(List<SourceMeterData> newSourceMeterDataList)
        {
            SourceMeterListDict = newSourceMeterDataList.GroupBy(x => x.Title_Name).ToDictionary(g => g.Key, g => g.First());
        }

        public int Initial_All_SMU()
        {
            SourceMeterList.Clear();

            //依照SMU型號以及ID名稱分組
            var hardwareConfigs = SourceMeterListDict.Values.GroupBy(x => new { x.Title_SMU_Type, x.Title_CardID }).Select(g => g.First());

            foreach (var config in hardwareConfigs)
            {
                var prototype = SMU.FirstOrDefault(p => p.GetSourceMeterType().ToString() == config.Title_SMU_Type);

                if (prototype != null)
                {
                    ISourceMeter newDevice = (ISourceMeter)Activator.CreateInstance(prototype.GetType());

                    int res = -1;
                    try
                    {
                        // for NI設定 先暫時這樣寫
                        res = newDevice.Open(new SourceMeterConnection()
                        {
                            ResourceName = config.Title_CardID,
                            ChannelName = "0",
                            ResetDevice = false,
                            DriverSetupOptions = "DriverSetup=SenseLeadErrorDetectionEnabled:0"
                        });
                    }
                    catch (Exception ex)
                    {
                        Tool.SaveLogToFile($"SMU {config.Title_SMU_Type} ({config.Title_CardID})連線失敗, Exception: {ex.Message}", level: "ERR");
                        res = -1;
                    }

                    if (res == 0)
                    {
                        Tool.SaveLogToFile($"SMU:{config.Title_SMU_Type}_{config.Title_CardID} Open Success");
                        SourceMeterList.Add(newDevice);
                    }
                    else
                        Tool.SaveLogToFile($"SMU {config.Title_SMU_Type} ({config.Title_CardID})連線失敗", level: "ERR");
                }
            }

            BindingDeviceIndex();

            int ret = -1;
            for (int i = 0; i < SourceMeterList.Count; i++)
            {
                ret = 0;
                IsInitial = true;

                if (SourceMeterList[i].GetSourceMeterType() != ESMUType.VIRTUAL)
                {
                    Tool.SaveLogToFile("實體分光卡 Initial Success");
                    return 0;
                }
            }

            return ret;
        }

        public bool SetSMUConfig(ESMUName name, SourceMeterConfiguration config)
        {
            SourceMeterListDict.TryGetValue(name.ToString(), out SourceMeterData smu_data);

            if (smu_data == null || IsInitial == false)
                return false;

            var targetDevice = SourceMeterList.FirstOrDefault(device => device.GetSourceMeterType().ToString() == smu_data.Title_SMU_Type);

            config.StartTriggerInputTerminal = TransferToTriggerLine(smu_data.Title_InputTrigger);
            config.StartTriggerOutputTerminal = TransferToTriggerLine(smu_data.Title_OutputTrigger);
            config.MeasurementSense = TransferToMeasurementSense(smu_data.Title_Sense);
            config.TransientResponse = TransferToTransientResponse(smu_data.Title_TransResp);

            int? res = targetDevice?.SetConfiguration(smu_data.Title_CardID, config);

            if(res != 0)
            {
                Tool.SaveLogToFile("SMU Config 設定失敗", level:"ERR");
                return false;
            }

            return true;
        }

        public bool SetRelay(ESMUName name, bool connect)
        {
            SourceMeterListDict.TryGetValue(name.ToString(), out SourceMeterData smu_data);

            if (smu_data == null || IsInitial == false)
                return false;

            var targetDevice = SourceMeterList.FirstOrDefault(device => device.GetSourceMeterType().ToString() == smu_data.Title_SMU_Type);

            int? res = targetDevice?.SetOutputRelay(smu_data.Title_CardID, connect);

            if (res != 0)
            {
                Tool.SaveLogToFile("SMU SetRelay 失敗", level: "ERR");
                return false;
            }

            return true;
        }

        public bool Reset(ESMUName name)
        {
            SourceMeterListDict.TryGetValue(name.ToString(), out SourceMeterData smu_data);

            if (smu_data == null || IsInitial == false)
                return false;

            var targetDevice = SourceMeterList.FirstOrDefault(device => device.GetSourceMeterType().ToString() == smu_data.Title_SMU_Type);

            int? res = targetDevice?.Reset(smu_data.Title_CardID);

            if (res != 0)
            {
                Tool.SaveLogToFile("SMU Reset 失敗", level: "ERR");
                return false;
            }

            return true;
        }

        public bool SIMV(ESMUName name, 
                         double current, double voltage_limit, double delay_time,
                         out double res_i, out double res_v)
        {
            res_i = 0; res_v = 0;

            SourceMeterListDict.TryGetValue(name.ToString(), out SourceMeterData smu_data);

            if (smu_data == null || IsInitial == false)
                return false;

            ISourceMeter targetDevice = SourceMeterList.FirstOrDefault(device => device.GetSourceMeterType().ToString() == smu_data.Title_SMU_Type);

            // 取得所屬SMU目前設定
            SourceMeterConfiguration config = null;
            targetDevice?.GetConfiguration(smu_data.Title_CardID, out config);

            if (config == null)
                return false;

            config.SourceFunction = SourceMeterSourceFunction.Current;
            config.OutputMode = SourceMeterOutputMode.ContinuousWave;
            config.ComplianceLimit = voltage_limit;

            config.SourceRange = 0;
            config.ComplianceRange = 0;
            config.AutoRangePerStep = true;

            config.LoopCount = 1;

            config.Steps = new List<SourceMeterStep>
            {
                new SourceMeterStep
                {
                    Level = current,
                    SourceDelaySeconds = delay_time*0.2,    // 取20%當作爬升時間
                    ApertureTimeSeconds = delay_time*0.8    // 取80%當作量測
                }
            };

            return ExecuteSinglePoint(targetDevice, smu_data, config, "SIMV", out res_i, out res_v);
        }

        //public bool SIMV_Pulse(double current, double voltage_limit, double pulse_width, double duty,
        //                       out double res_i, out double res_v)
        //{
        //    // 單位: current(A), voltage_limit(V), pulse_width(s), Duty(%)
        //    res_i = 0;
        //    res_v = 0;

        //    if (pulse_width <= 0)
        //    {
        //        Tool.SaveLogToFile("SIMV_Pulse Fail: Pulse Width 必須是大於 0 的有限值");
        //        return false;
        //    }

        //    if (duty <= 0 || duty >= 100)
        //    {
        //        Tool.SaveLogToFile("SIMV_Pulse Fail: Duty 必須介於 0 與 100 之間");
        //        return false;
        //    }

        //    double pulse_off_time = pulse_width / (duty * 0.01) - pulse_width;
        //    double source_delay_time = pulse_width * 0.2;   // 上升緣區段,可能需要依照實際情況調整
        //    double aperture_time = pulse_width * 0.7;       // 避免取到下降緣,可能需要依照實際情況調整

        //    SMUConfig.SourceFunction = SourceMeterSourceFunction.Current;
        //    SMUConfig.OutputMode = SourceMeterOutputMode.Pulse;
        //    SMUConfig.ComplianceLimit = voltage_limit;

        //    SMUConfig.SourceRange = 0;
        //    SMUConfig.ComplianceRange = 0;
        //    SMUConfig.AutoRangePerStep = true;

        //    SMUConfig.LoopCount = 1;
        //    SMUConfig.PulseOnTimeSeconds = pulse_width;
        //    SMUConfig.PulseOffTimeSeconds = pulse_off_time;
        //    SMUConfig.PulseBiasLevel = 0;
        //    SMUConfig.PulseBiasDelaySeconds = 0;

        //    SMUConfig.Steps = new List<SourceMeterStep>
        //    {
        //        new SourceMeterStep
        //        {
        //            Level = current,
        //            SourceDelaySeconds = source_delay_time,
        //            ApertureTimeSeconds = aperture_time
        //        }
        //    };

        //    return ExecuteSinglePoint("SIMV_Pulse", out res_i, out res_v);
        //}

        //public bool LIV()
        //{
        //    return true;
        //}

        #region private function
        private bool ExecuteSinglePoint(ISourceMeter smu, SourceMeterData smu_data, SourceMeterConfiguration config, string function_name, out double res_i, out double res_v)
        {
            res_i = 0;
            res_v = 0;

            int result = smu.SetConfiguration(smu_data.Title_CardID, config);
            if (result != (int)SourceMeterErrorCode.Success)
            {
                Tool.SaveLogToFile($"{function_name} Configure Fail, SourceMeterErrorCode = {result}");
                return false;
            }

            try
            {
                result = smu.OutputOn(smu_data.Title_CardID);

                if (result != (int)SourceMeterErrorCode.Success)
                {
                    Tool.SaveLogToFile($"{function_name} OutputOn Fail, SourceMeterErrorCode = {result}");
                    return false;
                }

                IList<SourceMeterMeasurement> measurements;
                result = smu.Fetch(smu_data.Title_CardID, count: 1, timeoutSeconds: 5, out measurements);

                if (result != (int)SourceMeterErrorCode.Success)
                {
                    Tool.SaveLogToFile($"{function_name} Fetch Fail, SourceMeterErrorCode = {result}");
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
                smu.OutputOff(smu_data.Title_CardID);
                smu.Reset(smu_data.Title_CardID);
            }
        }

        private SourceMeterTriggerTerminal TransferToTriggerLine(string line_name)
        {
            line_name = "Trigger" + line_name;

            if (line_name == SourceMeterTriggerTerminal.TriggerLine0.ToString())
                return SourceMeterTriggerTerminal.TriggerLine0;
            else if (line_name == SourceMeterTriggerTerminal.TriggerLine1.ToString())
                return SourceMeterTriggerTerminal.TriggerLine1;
            else if (line_name == SourceMeterTriggerTerminal.TriggerLine2.ToString())
                return SourceMeterTriggerTerminal.TriggerLine2;
            else if (line_name == SourceMeterTriggerTerminal.TriggerLine3.ToString())
                return SourceMeterTriggerTerminal.TriggerLine3;
            else if (line_name == SourceMeterTriggerTerminal.TriggerLine4.ToString())
                return SourceMeterTriggerTerminal.TriggerLine4;
            else if (line_name == SourceMeterTriggerTerminal.TriggerLine5.ToString())
                return SourceMeterTriggerTerminal.TriggerLine5;
            else if (line_name == SourceMeterTriggerTerminal.TriggerLine6.ToString())
                return SourceMeterTriggerTerminal.TriggerLine6;
            else if (line_name == SourceMeterTriggerTerminal.TriggerLine7.ToString())
                return SourceMeterTriggerTerminal.TriggerLine7;
            else
                return SourceMeterTriggerTerminal.None;
        }
        private SourceMeterMeasurementSense TransferToMeasurementSense(string sense_name)
        {
            if (sense_name == SourceMeterMeasurementSense.Local.ToString())
                return SourceMeterMeasurementSense.Local;
            else if (sense_name == SourceMeterMeasurementSense.Remote.ToString())
                return SourceMeterMeasurementSense.Remote;
            else
                return SourceMeterMeasurementSense.Local;
        }
        private SourceMeterTransientResponse TransferToTransientResponse(string transient_response)
        {
            SourceMeterTransientResponse response;

            if (Enum.TryParse(transient_response, true, out response))
                return response;

            return SourceMeterTransientResponse.Normal;
        }
        #endregion
    }
}
