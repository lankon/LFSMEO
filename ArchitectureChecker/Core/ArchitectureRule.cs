using System.Collections.Generic;

namespace ArchitectureChecker.Core
{
    /// <summary>
    /// 定義一筆架構檢查規則，可由規則 JSON 載入或由預設規則建立。
    /// </summary>
    public class ArchitectureRule
    {
        /// <summary>
        /// 顯示在 UI 上的唯一規則代號，例如 LBT4500-A001。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 規則嚴重程度，目前支援 error 與 warning。
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// 違反規則時顯示給使用者看的說明文字。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 此規則適用的檔案路徑樣式。
        /// </summary>
        public List<string> Include { get; set; }

        /// <summary>
        /// 在 include 命中後要排除的檔案路徑樣式。
        /// </summary>
        public List<string> Exclude { get; set; }

        /// <summary>
        /// 在命中檔案中不允許出現的文字樣式。
        /// </summary>
        public List<string> Forbidden { get; set; }

        public ArchitectureRule()
        {
            Include = new List<string>();
            Exclude = new List<string>();
            Forbidden = new List<string>();
        }

        /// <summary>
        /// 判斷此規則是否會讓檢查結果變成 NG。
        /// </summary>
        public bool IsError
        {
            get { return string.Equals(Severity, "error", System.StringComparison.OrdinalIgnoreCase); }
        }
    }
}
