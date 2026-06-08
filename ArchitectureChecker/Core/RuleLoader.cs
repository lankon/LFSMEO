using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace ArchitectureChecker.Core
{
    /// <summary>
    /// 從 JSON 載入架構檢查規則；找不到設定檔時提供內建預設規則。
    /// </summary>
    public static class RuleLoader
    {
        /// <summary>
        /// 從 JSON 檔或規則資料夾載入規則設定；路徑空白、檔案不存在或反序列化為 null 時改用預設規則。
        /// </summary>
        public static RuleConfig Load(string rulesPath)
        {
            if (!string.IsNullOrWhiteSpace(rulesPath) && Directory.Exists(rulesPath))
            {
                RuleConfig mergedConfig = new RuleConfig();
                foreach (string rulesFilePath in Directory.GetFiles(rulesPath, "*.json", SearchOption.TopDirectoryOnly))
                {
                    AddRulesFromFile(mergedConfig, rulesFilePath);
                }

                return mergedConfig.Rules.Count > 0 ? mergedConfig : CreateDefault();
            }

            if (!string.IsNullOrWhiteSpace(rulesPath) && File.Exists(rulesPath))
            {
                RuleConfig config = new RuleConfig();
                AddRulesFromFile(config, rulesPath);
                return config.Rules.Count > 0 ? config : CreateDefault();
            }

            return CreateDefault();
        }

        /// <summary>
        /// 從單一 JSON 檔讀取規則，並加入到合併後的設定中。
        /// </summary>
        private static void AddRulesFromFile(RuleConfig targetConfig, string rulesFilePath)
        {
            if (targetConfig == null || string.IsNullOrWhiteSpace(rulesFilePath) || !File.Exists(rulesFilePath))
            {
                return;
            }

            try
            {
                string json = File.ReadAllText(rulesFilePath);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                RuleConfig config = serializer.Deserialize<RuleConfig>(json);
                if (config == null || config.Rules == null) return;

                foreach (ArchitectureRule rule in config.Rules)
                {
                    targetConfig.Rules.Add(rule);
                }
            }
            catch
            {
                // 單一規則檔格式錯誤時略過該檔，避免整個工具無法啟動。
            }
        }

        /// <summary>
        /// 建立預設架構規則，供 UI 在缺少規則 JSON 時使用。
        /// </summary>
        public static RuleConfig CreateDefault()
        {
            RuleConfig config = new RuleConfig();
            config.Rules.Add(CreateRule(
                "LBT4500-A001",
                "error",
                "Modules must not depend on Scope.",
                new[] { "LBT4500/Modules/**/*.cs" },
                new string[0],
                new[] { "using LBT4500.Scopes", "Scope.", "LBT4500.Scopes.Scope" }));

            config.Rules.Add(CreateRule(
                "LBT4500-A002",
                "error",
                "SubTasks must not depend on Scope.",
                new[] { "LBT4500/SubTasks/**/*.cs" },
                new string[0],
                new[] { "using LBT4500.Scopes", "Scope.", "LBT4500.Scopes.Scope" }));

            config.Rules.Add(CreateRule(
                "LBT4500-A003",
                "warning",
                "Forms should not depend on Scope.",
                new[] { "LBT4500/Forms/**/*.cs" },
                new string[0],
                new[] { "using LBT4500.Scopes", "Scope.", "LBT4500.Scopes.Scope" }));

            config.Rules.Add(CreateRule(
                "LBT4500-A004",
                "warning",
                "Modules should not depend on WinForms.",
                new[] { "LBT4500/Modules/**/*.cs" },
                new string[0],
                new[] { "using System.Windows.Forms" }));

            return config;
        }

        /// <summary>
        /// 建立規則物件，並確保清單型屬性都有可用的集合實例。
        /// </summary>
        private static ArchitectureRule CreateRule(
            string id,
            string severity,
            string description,
            IEnumerable<string> include,
            IEnumerable<string> exclude,
            IEnumerable<string> forbidden)
        {
            return new ArchitectureRule
            {
                Id = id,
                Severity = severity,
                Description = description,
                Include = new List<string>(include),
                Exclude = new List<string>(exclude),
                Forbidden = new List<string>(forbidden)
            };
        }
    }
}
