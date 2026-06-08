namespace ArchitectureChecker.Core
{
    /// <summary>
    /// 表示原始碼檔案中命中的一筆架構規則違反項目。
    /// </summary>
    public class ArchitectureViolation
    {
        /// <summary>
        /// 從命中規則帶入的嚴重程度。
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// 從命中規則帶入的規則代號。
        /// </summary>
        public string RuleId { get; set; }

        /// <summary>
        /// 從命中規則說明帶入的訊息。
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 發生違規的檔案絕對路徑。
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 相對於使用者選擇的 solution root 的路徑，主要顯示在表格中。
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// 命中文字所在的行號，從 1 開始計算。
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// 觸發違規的不允許文字。
        /// </summary>
        public string MatchedText { get; set; }

        /// <summary>
        /// 顯示在結果表格中的原始碼行內容，已移除前後空白。
        /// </summary>
        public string CodeLine { get; set; }
    }
}
