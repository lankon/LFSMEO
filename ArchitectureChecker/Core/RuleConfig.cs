using System.Collections.Generic;

namespace ArchitectureChecker.Core
{
    /// <summary>
    /// 規則 JSON 的根節點物件。
    /// </summary>
    public class RuleConfig
    {
        /// <summary>
        /// 掃描器要執行的架構檢查規則集合。
        /// </summary>
        public List<ArchitectureRule> Rules { get; set; }

        public RuleConfig()
        {
            Rules = new List<ArchitectureRule>();
        }
    }
}
