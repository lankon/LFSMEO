using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

namespace ToolFunction
{
    public static class ApplicationSetting
    {
        private static Dictionary<Type, string[]> _storage = new Dictionary<Type, string[]>();

        #region public function
        // Read & Save Function
        public static void SaveAllRecipe<T>(string recipe_name = "") where T : Enum
        {
            var enumType = typeof(T);
            string enumName = enumType.Name;

            if (recipe_name != "")
                enumName = $"Package\\{recipe_name}\\" + enumName;

            // 取得要寫入的 config 物件
            Configuration config = GetConfig(enumName);

            // 取得 enum 所有名稱
            string[] keys = Enum.GetNames(enumType);

            // 若 storage 中沒有對應資料，就不寫入
            if (!_storage.TryGetValue(enumType, out string[] values) || values == null)
                return;

            // 寫入設定
            WriteAppSettings(config, keys, values);

            // 儲存
            config.Save(ConfigurationSaveMode.Modified);

            // 只有主 app.config 才需要 RefreshSection
            if (enumName == "eDefaultSetting")
                ConfigurationManager.RefreshSection("appSettings");
        }
        //public static void ReadAllRecipe<T>(string function = "flash") where T : Enum
        //{
        //    string enumName = typeof(T).Name;
        //    var infoArray = GetInfoArray<T>();
        //    Configuration config; // 我們將在迴圈外，只載入一次

        //    // -----------------------------------------------------------------
        //    // 步驟 1: 根據 Enum 名稱，決定要載入「哪一個」設定檔
        //    // -----------------------------------------------------------------
        //    if (enumName == "eDefaultSetting")
        //    {
        //        // 載入主程式的 .config (例如 LFSMEO.exe.config)
        //        config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    }
        //    else
        //    {
        //        // --- 這是「客製化 .config」的全新路徑邏輯 ---

        //        // 1. 【關鍵】使用這個來取代 Application.StartupPath
        //        //    (它不需要參考 System.Windows.Forms)
        //        string baseDir = AppDomain.CurrentDomain.BaseDirectory;

        //        // 2. 使用您在 SaveAllRecipe 中「相同」的資料夾名稱
        //        //    (在您的範例中是 "Setting")
        //        string configFolder = System.IO.Path.Combine(baseDir, "Setting");

        //        // 3. 組合出「完整的絕對路徑」
        //        string fileName = enumName + ".exe.Config";
        //        string customConfigPath = System.IO.Path.Combine(configFolder, fileName);

        //        // 4. 【重要】檢查檔案是否存在
        //        if (!System.IO.File.Exists(customConfigPath))
        //        {
        //            // 檔案不存在，我們無法讀取
        //            // 快速將所有值設為 "Not Found" 並返回
        //            foreach (var value in Enum.GetValues(typeof(T)))
        //            {
        //                infoArray[(int)value] = "Not Found";
        //            }
        //            return; // 結束方法
        //        }

        //        // 5. 載入客製化的 .config 檔案 (只在迴圈外做這一次)
        //        ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap
        //        {
        //            ExeConfigFilename = customConfigPath // 使用絕對路徑
        //        };
        //        config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        //    }

        //    // -----------------------------------------------------------------
        //    // 步驟 2: 迴圈遍歷 Enum，從「已經載入」的 config 中讀取資料
        //    // -----------------------------------------------------------------

        //    // 取得設定檔中的 appSettings 區段
        //    KeyValueConfigurationCollection appSettings = config.AppSettings.Settings;

        //    foreach (var value in Enum.GetValues(typeof(T)))
        //    {
        //        string eName = Enum.GetName(typeof(T), value);

        //        if (enumName == "eDefaultSetting" && function == "flash")
        //        {
        //            // 「快閃」模式是特例，它會讀取「目前記憶體中」的快取設定
        //            // (這是您原本的邏輯，我們保留它)
        //            infoArray[(int)value] = ConfigurationManager.AppSettings[eName] ?? "Not Found";
        //        }
        //        else
        //        {
        //            // 對於「ReRead」或「任何客製化 config」，
        //            // 我們都從「步驟 1」載入的 'config' 物件中讀取
        //            string setting = appSettings[eName]?.Value;
        //            infoArray[(int)value] = setting ?? "Not Found";
        //        }
        //    }
        //}
        public static int ReadAllRecipe<T>(string recipe_name = "", string function = "Reread") where T : Enum
        {
            string enumName = typeof(T).Name;

            if (recipe_name != "")
                enumName = $"Package\\{recipe_name}\\" + enumName;

            string[] infoArray = GetInfoArray<T>();

            // Flash 模式 → 一律從快取讀取，完全不需要讀任何檔案
            if (function == "flash")
            {
                LoadFlashCacheToArray<T>(infoArray);
                return 0;
            }

            // 不是 flash → 依據 enumName 載入對應 config
            Configuration config = LoadConfiguration(enumName);

            if (config == null)
            {
                foreach (var value in Enum.GetValues(typeof(T)))
                {
                    infoArray[(int)value] = "Not Found";
                }
                return -1;
            }

            LoadSettingsToArray<T>(config, infoArray);

            return 0;
        }
        public static void SaveRecipeFromForm<T>(Form form, string recipe_name = "") where T:Enum
        {
            string enumName = typeof(T).Name;

            if (recipe_name != "")
                enumName = $"Package\\{recipe_name}\\" + enumName;

            Configuration config = null;

            config = GetConfig(enumName);

            TraverseControlsSave(form, config);

            // 儲存更改
            config.Save(ConfigurationSaveMode.Modified);

            if (enumName == "eDefaultSetting")
                ConfigurationManager.RefreshSection("appSettings");
        }
        
        // Set Function
        public static void UpdataRecipeToForm<T>(Form form) where T : Enum
        {
            string enumName = typeof(T).Name;

            Configuration config = null;

            if (enumName == "eDefaultSetting")
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            else
            {
                //Application.StartupPath

                string customConfigPath = Application.StartupPath + "\\Setting\\" + enumName + ".exe.Config";

                // 設定自訂的配置檔路徑
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = customConfigPath
                };

                config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            }

            TraverseControlsUpdate<T>(form, config);
        }
        public static void SetRecipe<T>(int element, string data) where T : Enum
        {
            var infoArray = GetInfoArray<T>();
            infoArray[element] = data;
        }

        // Get Value Function
        public static bool Get_Bool_Recipe<T>(int element) where T:Enum
        {
            bool Flag = false;

            if (GetInfoArray<T>()[element] == "0")
                return false;
            else if (GetInfoArray<T>()[element] == "1")
                return true;

            if (bool.TryParse(GetInfoArray<T>()[element], out Flag))
                return Flag;
            else
                return Flag;

        }
        public static string Get_String_Recipe<T>(int element) where T:Enum
        {
            return GetInfoArray<T>()[element];
        }
        public static int Get_Int_Recipe<T>(int element) where T:Enum
        {
            int Flag = -1;

            if (int.TryParse(GetInfoArray<T>()[element], out Flag))
            {
                return Flag;
            }
            else
            {
                return -1;
            }
        }
        public static double Get_Double_Recipe<T>(int element) where T : Enum
        {
            double Flag = -1.0;

            if (double.TryParse(GetInfoArray<T>()[element], out Flag))
            {
                return Flag;
            }
            else
            {
                return -1.0;
            }
        }
        #endregion

        #region private function
        private static void LoadSettingsToArray<T>(Configuration config, string[] infoArray) where T : Enum
        {
            var settings = config.AppSettings.Settings;

            foreach (var value in Enum.GetValues(typeof(T)))
            {
                string key = Enum.GetName(typeof(T), value);
                infoArray[(int)value] = settings[key]?.Value ?? "Not Found";
            }
        }
        private static void LoadFlashCacheToArray<T>(string[] infoArray) where T : Enum
        {
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                string key = Enum.GetName(typeof(T), value);
                infoArray[(int)value] = ConfigurationManager.AppSettings[key] ?? "Not Found";
            }
        }
        private static Configuration LoadConfiguration(string enumName)
        {
            if (enumName == "eDefaultSetting")
            {
                return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string configFolder = Path.Combine(baseDir, "Setting");
            string configFile = Path.Combine(configFolder, $"{enumName}.exe.Config");

            if (!File.Exists(configFile))
                return null;

            var map = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configFile
            };

            return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }
        private static Configuration GetConfig(string enumName)
        {
            if (enumName == "eDefaultSetting")
            {
                return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            else
            {
                string customConfigPath = Path.Combine(Application.StartupPath, "Setting", $"{enumName}.exe.Config");

                var configMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = customConfigPath
                };

                return ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            }
        }
        private static void WriteAppSettings(Configuration config, string[] keys, string[] values)
        {
            var settings = config.AppSettings.Settings;

            int count = Math.Min(keys.Length, values.Length);
            for (int i = 0; i < count; i++)
            {
                string key = keys[i];
                string value = values[i];

                if (settings[key] != null)
                    settings.Remove(key);

                settings.Add(key, value);
            }
        }
        private static void TraverseControlsSave(Control parent, Configuration config1)
        {
            foreach (Control control in parent.Controls)
            {
                // 假設我們只儲存所有 TextBox 的 Text 屬性
                if (control is TextBox)
                {
                    // 移除舊的設定值（如果存在）
                    config1.AppSettings.Settings.Remove(control.Name);

                    // 添加新的設定值
                    config1.AppSettings.Settings.Add(control.Name, control.Text);
                }
                else if (control is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)control;

                    // 移除舊的設定值（如果存在）
                    config1.AppSettings.Settings.Remove(comboBox.Name);

                    // 添加新的設定值                   
                    string sIndex = comboBox.SelectedIndex.ToString();
                    config1.AppSettings.Settings.Add(control.Name, sIndex);
                }

                // 如果這個控件包含其他控件，遞歸遍歷它們
                if (control.HasChildren)
                {
                    TraverseControlsSave(control, config1);
                }
            }
        }
        private static void TraverseControlsUpdate<T>(Control parent, Configuration config1) where T : Enum
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox)
                {
                    foreach (var value in Enum.GetValues(typeof(T)))
                    {
                        string eName = Enum.GetName(typeof(T), value);
                        if (eName == control.Name)
                        {
                            control.Text = GetInfoArray<T>()[(int)value];
                        }
                    }
                }
                else if (control is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)control;

                    foreach (var value in Enum.GetValues(typeof(T)))
                    {
                        string eName = Enum.GetName(typeof(T), value);
                        if (eName == comboBox.Name)
                        {
                            int Index;

                            if (int.TryParse(GetInfoArray<T>()[(int)value], out Index))
                            {
                                try
                                {
                                    comboBox.SelectedIndex = Index;
                                }
                                catch
                                {
                                    comboBox.SelectedIndex = 0;
                                }
                            }
                            else
                            {
                                try
                                {
                                    int tag_value = Convert.ToInt32(comboBox.Tag);

                                    var infoArray = GetInfoArray<T>();
                                    infoArray[(int)value] = (string)comboBox.Tag;

                                    comboBox.SelectedIndex = tag_value;
                                }
                                catch
                                {
                                    comboBox.SelectedIndex = 0;
                                }
                            }
                        }
                    }
                }

                // 如果這個控件包含其他控件，遞歸遍歷它們
                if (control.HasChildren)
                {
                    TraverseControlsUpdate<T>(control, config1);
                }
            }
        }
        private static string[] GetInfoArray<T>() where T : Enum
        {
            Type type = typeof(T);
            if (!_storage.ContainsKey(type))
            {
                _storage[type] = new string[Enum.GetValues(type).Length];
            }
            return _storage[type];
        }
        #endregion
    }
}
