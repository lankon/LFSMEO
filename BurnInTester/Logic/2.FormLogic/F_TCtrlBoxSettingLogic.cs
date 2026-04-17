using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using ToolFunction;
using BurnInTester.Device;
using BurnInTester.UI;
using BurnInTester.Logic.nTCtrlBoxSetting;

namespace BurnInTester.Logic.nTCtrlBoxSetting
{
    public struct TCtrlBoxSetting
    {
        public int TCtrlBoxID { get; set; } //溫控箱ID
        public int Use { get; set; }        //是否使用
        public int BoxNum { get; set; }     //溫控箱編號
        public int ChNum { get; set; }      //溫控箱通道編號
    }
}

namespace BurnInTester.Logic
{
    public class F_TCtrlBoxSettingLogic
    {
        public F_TCtrlBoxSettingLogic(HW_ParamSetting hw_ParamSetting)
        {
            _HW_ParamSetting = hw_ParamSetting;
        }

        #region paramter define
        private string _DataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Setting\\TCtrlBoxSettings.json");
        public string DataPath { get => _DataPath; set => _DataPath = value; }
        private HW_ParamSetting _HW_ParamSetting;

        #endregion
        public bool SaveTCtrlBoxSetting(Dictionary<string, TCtrlBoxSetting> DicSetting)
        {
            string filePath = _DataPath;
            string jsonString = JsonSerializer.Serialize(DicSetting, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);

            return true;
        }
        public bool LoadTCtrlBoxSetting()
        {
            string filePath = _DataPath;

            if (!File.Exists(filePath))
            {
                Tool.SaveLogToFile("TCtrlBoxSettings.json not found.");
                return false;
            }

            try
            {
                string jsonString = File.ReadAllText(filePath);

                var dicSetting = JsonSerializer.Deserialize<Dictionary<string, TCtrlBoxSetting>>(jsonString);

                if (dicSetting != null)
                {
                    foreach (var kvp in dicSetting)
                    {
                        int boxID = kvp.Value.TCtrlBoxID;
                        if (boxID - 1 >= 0 && boxID - 1 < _HW_ParamSetting.TC_Box.Use.Length)
                        {
                            _HW_ParamSetting.TC_Box.Use[boxID - 1] = kvp.Value.Use == 1 ? true : false;
                            _HW_ParamSetting.TC_Box.BoxNum[boxID - 1] = kvp.Value.BoxNum.ToString();
                            _HW_ParamSetting.TC_Box.ChNum[boxID - 1] = kvp.Value.ChNum.ToString();
                        }
                        else
                        {
                            Tool.SaveLogToFile($"Invalid TCtrlBoxID {boxID} in TCtrlBoxSettings.json.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tool.SaveLogToFile($"Error loading TCtrlBoxSettings.json: {ex.Message}");
                return false;
            }

            return true;
        }
    }
}
