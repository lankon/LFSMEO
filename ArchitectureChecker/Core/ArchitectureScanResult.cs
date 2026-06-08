using System.Collections.Generic;

namespace ArchitectureChecker.Core
{
    /// <summary>
    /// 彙整一次掃描結果，並提供 UI 顯示所需的統計數量。
    /// </summary>
    public class ArchitectureScanResult
    {
        #region 資料欄位

        /// <summary>
        /// 排除忽略資料夾後實際掃描的 C# 檔案數量。
        /// </summary>
        public int CheckedFiles { get; set; }

        /// <summary>
        /// 此次掃描找到的所有規則違反項目。
        /// </summary>
        public List<ArchitectureViolation> Violations { get; private set; }

        #endregion

        #region 建構式

        public ArchitectureScanResult()
        {
            Violations = new List<ArchitectureViolation>();
        }

        #endregion

        #region 統計資訊

        /// <summary>
        /// 嚴重程度為 error 的違規數量。
        /// </summary>
        public int ErrorCount
        {
            get { return CountBySeverity("error"); }
        }

        /// <summary>
        /// 嚴重程度為 warning 的違規數量。
        /// </summary>
        public int WarningCount
        {
            get { return CountBySeverity("warning"); }
        }

        /// <summary>
        /// 依 error 與 warning 數量推算出的完整狀態文字。
        /// </summary>
        public string StatusText
        {
            get
            {
                if (ErrorCount > 0) return "NG";
                if (WarningCount > 0) return "OK with Warnings";
                return "OK";
            }
        }

        /// <summary>
        /// 逐筆計算指定嚴重程度的違規數量，避免使用 LINQ 讓統計邏輯更直覺。
        /// </summary>
        private int CountBySeverity(string severity)
        {
            int count = 0;
            foreach (ArchitectureViolation violation in Violations)
            {
                if (string.Equals(violation.Severity, severity, System.StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }

            return count;
        }

        #endregion
    }
}
