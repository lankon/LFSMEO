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
                ISourceMeter newDevice = SMU.FirstOrDefault(p => p.GetSourceMeterType().ToString() == config.Title_SMU_Type);

                if (newDevice != null)
                {
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
                    Tool.SaveLogToFile("實體電流源 Initial Success");
                    return 0;
                }
            }

            return ret;
        }

        public bool SetConfig(ESMUName name, SourceMeterConfiguration config)
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

        public bool OutputOn(ESMUName name)
        {
            SourceMeterListDict.TryGetValue(name.ToString(), out SourceMeterData smu_data);

            if (smu_data == null || IsInitial == false)
                return false;

            ISourceMeter targetDevice = SourceMeterList.FirstOrDefault(device => device.GetSourceMeterType().ToString() == smu_data.Title_SMU_Type);

            int res = targetDevice.OutputOn(smu_data.Title_CardID);

            if (res != 0)
            {
                Tool.SaveLogToFile("SMU OutputOn 失敗", level: "ERR");
                return false;
            }
            return true;
        }

        public bool LoadVirtualData(string filePath)
        {
            foreach (var device in SourceMeterList)
            {
                if (device is IVirtualSourceMeter virtualDevice)
                {
                    bool result = virtualDevice.LoadVirtualData(filePath);
                    if (!result)
                    {
                        Tool.SaveLogToFile($"Failed to load virtual data from {filePath} for device {device.GetSourceMeterType()}", level: "ERR");
                        return false;
                    }
                }
            }
            return true;
        }

        public bool GetMeasureResult(ESMUName name, int data_len, out IList<SourceMeterMeasurement> measurements)
        {
            measurements = null;

            SourceMeterListDict.TryGetValue(name.ToString(), out SourceMeterData smu_data);

            if (smu_data == null || IsInitial == false)
                return false;

            var targetDevice = SourceMeterList.FirstOrDefault(device => device.GetSourceMeterType().ToString() == smu_data.Title_SMU_Type);

            int res = targetDevice.Fetch(smu_data.Title_CardID, count: data_len, timeoutSeconds: 5, out measurements);

            if (res != 0 || measurements == null || measurements.Count == 0)
            {
                Tool.SaveLogToFile("SMU Fetch 失敗", level: "ERR");
                return false;
            }

            return true;
        }

        public bool OutputOff(ESMUName name)
        {
            SourceMeterListDict.TryGetValue(name.ToString(), out SourceMeterData smu_data);
            if (smu_data == null || IsInitial == false)
                return false;
            var targetDevice = SourceMeterList.FirstOrDefault(device => device.GetSourceMeterType().ToString() == smu_data.Title_SMU_Type);
            int? res = targetDevice?.OutputOff(smu_data.Title_CardID);
            if (res != 0)
            {
                Tool.SaveLogToFile("SMU OutputOff 失敗", level: "ERR");
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
