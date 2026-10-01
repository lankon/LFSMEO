using System;
using System.Collections.Generic;
using System.IO;

namespace ArchitectureChecker.Core
{
    /// <summary>
    /// 掃描 solution root 底下符合規則範圍的原始碼，並回報不允許文字的命中位置。
    /// </summary>
    public class ArchitectureScanner
    {
        #region 掃描入口

        /// <summary>
        /// 對 solution root 底下符合規則 include 條件的 C# 檔案執行架構檢查。
        /// </summary>
        public ArchitectureScanResult Scan(string rootPath, RuleConfig config)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
            {
                throw new ArgumentException("Root path is required.", "rootPath");
            }

            string fullRoot = Path.GetFullPath(rootPath);
            //if (!Directory.Exists(Path.Combine(fullRoot, "LBT4500")))
            //{
            //    throw new DirectoryNotFoundException("Cannot find LBT4500 folder under: " + fullRoot);
            //}

            ArchitectureScanResult result = new ArchitectureScanResult();
            List<string> files = new List<string>();
            foreach (string file in Directory.GetFiles(fullRoot, "*.*", SearchOption.AllDirectories))
            {
                // 只掃描 .cs 和 .csproj
                if (!file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) &&
                    !file.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // 跳過 bin、obj、.vs 等輸出或 IDE 暫存資料夾，避免掃到舊檔案。
                if (IsUnderIgnoredFolder(file)) 
                    continue;
                
                files.Add(file);
            }

            foreach (string file in files)
            {
                string relativePath = NormalizeRelativePath(fullRoot, file);
                List<ArchitectureRule> includedRules = GetIncludedRules(relativePath, config);
                if (includedRules.Count == 0) continue;

                result.CheckedFiles++;
                string[] lines = File.ReadAllLines(file);

                foreach (ArchitectureRule rule in includedRules)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        foreach (string forbidden in rule.Forbidden)
                        {
                            if (string.IsNullOrEmpty(forbidden)) continue;
                            if (line.IndexOf(forbidden, StringComparison.Ordinal) < 0) continue;

                            result.Violations.Add(new ArchitectureViolation
                            {
                                Severity = rule.Severity,
                                RuleId = rule.Id,
                                Message = rule.Description,
                                FilePath = file,
                                RelativePath = relativePath,
                                LineNumber = i + 1,
                                MatchedText = forbidden,
                                CodeLine = line.Trim()
                            });
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region 路徑處理

        /// <summary>
        /// 將絕對檔案路徑轉成相對於 solution root 的路徑，並統一使用斜線。
        /// </summary>
        public static string NormalizeRelativePath(string rootPath, string filePath)
        {
            string fullRoot = Path.GetFullPath(rootPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string fullFile = Path.GetFullPath(filePath);
            string relative = fullFile.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase)
                ? fullFile.Substring(fullRoot.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                : fullFile;
            return relative.Replace('\\', '/');
        }

        /// <summary>
        /// 排除建置輸出與 IDE 資料夾，避免舊產物影響檢查結果。
        /// </summary>
        private static bool IsUnderIgnoredFolder(string file)
        {
            string normalized = file.Replace('\\', '/');
            return normalized.IndexOf("/bin/", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   normalized.IndexOf("/obj/", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   normalized.IndexOf("/.vs/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        #endregion

        #region 規則套用判斷

        /// <summary>
        /// 取得此檔案實際需要套用的規則；沒有符合 include 的檔案不會被列入 Checked files。
        /// </summary>
        private static List<ArchitectureRule> GetIncludedRules(string relativePath, RuleConfig config)
        {
            List<ArchitectureRule> rules = new List<ArchitectureRule>();
            foreach (ArchitectureRule rule in config.Rules)
            {
                if (IsIncluded(relativePath, rule))
                {
                    rules.Add(rule);
                }
            }

            return rules;
        }

        /// <summary>
        /// 判斷檔案是否符合規則的 include 條件，且沒有被 exclude 排除。
        /// </summary>
        private static bool IsIncluded(string relativePath, ArchitectureRule rule)
        {
            bool included = false;
            if (rule.Include == null || rule.Include.Count == 0)
            {
                included = true;
            }
            else
            {
                // 只要符合任一 include pattern，就代表此規則需要檢查這個檔案。
                foreach (string pattern in rule.Include)
                {
                    if (MatchesPattern(relativePath, pattern))
                    {
                        included = true;
                        break;
                    }
                }
            }

            if (!included) return false;

            if (rule.Exclude != null)
            {
                // exclude 優先權比 include 高；只要符合任一 exclude pattern，就不檢查。
                foreach (string pattern in rule.Exclude)
                {
                    if (MatchesPattern(relativePath, pattern))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 支援規則 JSON 使用的簡易 glob 格式：精確檔案、folder/*.cs、folder/**/*.cs。
        /// </summary>
        private static bool MatchesPattern(string relativePath, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return false;

            string normalizedPattern = pattern.Replace('\\', '/');
            string normalizedPath = relativePath.Replace('\\', '/');

            if (normalizedPattern.EndsWith("/**/*.cs", StringComparison.OrdinalIgnoreCase))
            {
                string prefix = normalizedPattern.Substring(0, normalizedPattern.Length - "/**/*.cs".Length) + "/";
                return normalizedPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                       normalizedPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);
            }

            if (normalizedPattern.EndsWith("/*.cs", StringComparison.OrdinalIgnoreCase))
            {
                string prefix = normalizedPattern.Substring(0, normalizedPattern.Length - "/*.cs".Length) + "/";
                if (!normalizedPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return false;
                string tail = normalizedPath.Substring(prefix.Length);
                return tail.IndexOf('/') < 0 && tail.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);
            }

            if (normalizedPattern.EndsWith("/**/*.csproj", StringComparison.OrdinalIgnoreCase))
            {
                string prefix = normalizedPattern.Substring(0, normalizedPattern.Length - "/**/*.csproj".Length) + "/";
                return normalizedPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                       normalizedPath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase);
            }

            if (normalizedPattern.EndsWith("/*.csproj", StringComparison.OrdinalIgnoreCase))
            {
                string prefix = normalizedPattern.Substring(0, normalizedPattern.Length - "/*.csproj".Length) + "/";
                if (!normalizedPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return false;
                string tail = normalizedPath.Substring(prefix.Length);
                return tail.IndexOf('/') < 0 && tail.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase);
            }

            return string.Equals(normalizedPath, normalizedPattern, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
