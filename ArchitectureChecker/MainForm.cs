using ArchitectureChecker.Core;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ArchitectureChecker
{
    public partial class MainForm : Form
    {
        #region 欄位與狀態

        private ArchitectureScanResult lastResult;
        private readonly Color openFileEnabledBackColor = Color.FromArgb(42, 50, 60);
        private readonly Color openFileDisabledBackColor = Color.FromArgb(48, 56, 66);
        private readonly Color openFileEnabledForeColor = Color.FromArgb(238, 244, 250);
        private readonly Color openFileDisabledForeColor = Color.FromArgb(210, 220, 230);

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化可視化檢查工具表單，並註冊執行期事件。
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            ConfigureGrid();
            LoadIcons();
            metricsPanel.Visible = true;

            txtRootPath.Text = FindDefaultRootPath();
            btnBrowse.Click += BtnBrowse_Click;
            btnRun.Click += BtnRun_Click;
            btnOpenFile.Click += BtnOpenFile_Click;
            SetOpenFileButtonState(false);
            gridViolations.SelectionChanged += GridViolations_SelectionChanged;
            gridViolations.CellDoubleClick += GridViolations_CellDoubleClick;
        }

        #endregion

        #region 結果表格設定

        /// <summary>
        /// 設定結果表格的執行期行為與欄位，讓欄位綁定邏輯集中維護。
        /// </summary>
        private void ConfigureGrid()
        {
            gridViolations.AllowUserToAddRows = false;
            gridViolations.AllowUserToDeleteRows = false;
            gridViolations.AllowUserToResizeRows = false;
            gridViolations.AutoGenerateColumns = false;
            gridViolations.BackgroundColor = Color.FromArgb(37, 43, 49);
            gridViolations.BorderStyle = BorderStyle.FixedSingle;
            gridViolations.MultiSelect = false;
            gridViolations.ReadOnly = true;
            gridViolations.RowHeadersVisible = false;
            gridViolations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridViolations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridViolations.EnableHeadersVisualStyles = false;
            gridViolations.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(55, 63, 72);
            gridViolations.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(232, 238, 244);
            gridViolations.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            gridViolations.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            gridViolations.GridColor = Color.FromArgb(63, 72, 82);
            gridViolations.DefaultCellStyle.BackColor = Color.FromArgb(37, 43, 49);
            gridViolations.DefaultCellStyle.ForeColor = Color.FromArgb(232, 238, 244);
            gridViolations.DefaultCellStyle.SelectionBackColor = Color.FromArgb(31, 96, 168);
            gridViolations.DefaultCellStyle.SelectionForeColor = Color.White;
            gridViolations.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(42, 48, 55);
            gridViolations.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(232, 238, 244);
            gridViolations.DefaultCellStyle.Font = new Font("Microsoft JhengHei UI", 11F);
            gridViolations.RowTemplate.Height = 38;

            gridViolations.Columns.Add(CreateColumn("Severity", "Severity", 86, 8F));
            gridViolations.Columns.Add(CreateColumn("RuleId", "Rule", 110, 10F));
            gridViolations.Columns.Add(CreateColumn("RelativePath", "File", 300, 28F));
            gridViolations.Columns.Add(CreateColumn("LineNumber", "Line", 64, 6F));
            gridViolations.Columns.Add(CreateColumn("Message", "Message", 260, 24F));
            gridViolations.Columns.Add(CreateColumn("CodeLine", "Code", 320, 24F));
        }

        /// <summary>
        /// 建立綁定到 ArchitectureViolation 屬性的文字欄位，並設定填滿表格時的最小寬度與分配比例。
        /// </summary>
        private static DataGridViewTextBoxColumn CreateColumn(string propertyName, string header, int minimumWidth, float fillWeight)
        {
            return new DataGridViewTextBoxColumn
            {
                DataPropertyName = propertyName,
                HeaderText = header,
                MinimumWidth = minimumWidth,
                FillWeight = fillWeight
            };
        }

        #endregion

        #region 圖示載入

        /// <summary>
        /// 從 Resources 載入 PNG 圖示，並套用到按鈕與統計摘要區。
        /// </summary>
        private void LoadIcons()
        {
            btnBrowse.Image = LoadIcon("icon_folder.png", 18, 18, Color.FromArgb(226, 234, 242));
            btnBrowse.ImageAlign = ContentAlignment.MiddleLeft;
            btnBrowse.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnBrowse.Padding = new Padding(4, 0, 6, 0);

            btnRun.Image = LoadIcon("icon_run_check.png", 18, 18, Color.White);
            btnRun.ImageAlign = ContentAlignment.MiddleLeft;
            btnRun.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnRun.Padding = new Padding(4, 0, 6, 0);

            btnOpenFile.ImageAlign = ContentAlignment.MiddleLeft;
            btnOpenFile.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnOpenFile.Padding = new Padding(4, 0, 6, 0);

            picErrors.Image = LoadIcon("icon_error.png", 22, 22);
            picWarnings.Image = LoadIcon("icon_warning.png", 22, 22);
            picChecked.Image = LoadIcon("icon_success.png", 22, 22);
            picFindings.Image = LoadIcon("icon_finding.png", 22, 22, Color.FromArgb(207, 133, 150));
        }

        /// <summary>
        /// 載入指定 PNG 並縮放為 UI 需要的尺寸；載入失敗時回傳 null，避免工具因圖示遺失而無法啟動。
        /// </summary>
        private static Image LoadIcon(string fileName, int width, int height)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", fileName);
            if (!File.Exists(path)) return null;

            using (Image source = Image.FromFile(path))
            {
                Bitmap resized = new Bitmap(width, height);
                using (Graphics graphics = Graphics.FromImage(resized))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(source, 0, 0, width, height);
                }

                return resized;
            }
        }

        /// <summary>
        /// 載入指定 PNG 並轉成指定前景色，讓深色按鈕上的線條圖示保持清楚。
        /// </summary>
        private static Image LoadIcon(string fileName, int width, int height, Color foregroundColor)
        {
            Image icon = LoadIcon(fileName, width, height);
            if (icon == null) return null;

            Bitmap source = icon as Bitmap;
            Bitmap tinted = new Bitmap(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = source.GetPixel(x, y);
                    if (pixel.A == 0)
                    {
                        tinted.SetPixel(x, y, Color.Transparent);
                        continue;
                    }

                    tinted.SetPixel(x, y, Color.FromArgb(pixel.A, foregroundColor));
                }
            }

            icon.Dispose();
            return tinted;
        }

        #endregion

        #region 使用者操作事件

        /// <summary>
        /// 讓使用者選擇包含 LBT4500 資料夾的 solution root。
        /// </summary>
        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select the solution root folder containing LBT4500.";
                dialog.SelectedPath = Directory.Exists(txtRootPath.Text) ? txtRootPath.Text : FindDefaultRootPath();
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    txtRootPath.Text = dialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// 使用者按下 Run Check 時執行架構檢查。
        /// </summary>
        private void BtnRun_Click(object sender, EventArgs e)
        {
            RunScan();
        }

        #endregion

        #region 掃描流程

        /// <summary>
        /// 載入規則、掃描原始碼、將違規項目綁定到表格，並更新摘要狀態。
        /// </summary>
        private void RunScan()
        {
            try
            {
                btnRun.Enabled = false;
                Cursor = Cursors.WaitCursor;
                txtDetails.Clear();
                SetOpenFileButtonState(false);

                string rootPath = txtRootPath.Text.Trim();
                string rulesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rules");
                RuleConfig config = RuleLoader.Load(rulesPath);
                ArchitectureScanner scanner = new ArchitectureScanner();
                lastResult = scanner.Scan(rootPath, config);

                gridViolations.DataSource = BuildSortedViolations(lastResult.Violations);

                ApplySummary(lastResult);
                ColorRows();
            }
            catch (Exception ex)
            {
                lastResult = null;
                gridViolations.DataSource = null;
                metricsPanel.Visible = true;
                lblStatus.Text = "NG";
                lblStatus.BackColor = Color.FromArgb(192, 57, 43);
                lblStatus.ForeColor = Color.White;
                lblErrorsMetric.Text = "Errors: 0";
                lblWarningsMetric.Text = "Warnings: 0";
                lblCheckedMetric.Text = "Checked files: 0";
                lblFindingsMetric.Text = "Findings: 0";
                txtDetails.Text = ex.ToString();
            }
            finally
            {
                Cursor = Cursors.Default;
                btnRun.Enabled = true;
            }
        }

        #endregion

        #region 摘要狀態顯示

        /// <summary>
        /// 更新 OK、OK with 或 NG 狀態牌，以及右側統計摘要。
        /// </summary>
        private void ApplySummary(ArchitectureScanResult result)
        {
            lblStatus.Text = GetDisplayStatus(result);
            if (result.ErrorCount > 0)
            {
                lblStatus.BackColor = Color.FromArgb(192, 57, 43);
                lblStatus.ForeColor = Color.White;
            }
            else if (result.WarningCount > 0)
            {
                lblStatus.BackColor = Color.FromArgb(255, 169, 77);
                lblStatus.ForeColor = Color.FromArgb(22, 25, 29);
            }
            else
            {
                lblStatus.BackColor = Color.FromArgb(39, 174, 96);
                lblStatus.ForeColor = Color.White;
            }

            metricsPanel.Visible = true;
            lblErrorsMetric.Text = string.Format("Errors: {0}", result.ErrorCount);
            lblWarningsMetric.Text = string.Format("Warnings: {0}", result.WarningCount);
            lblCheckedMetric.Text = string.Format("Checked files: {0}", result.CheckedFiles);
            lblFindingsMetric.Text = string.Format("Findings: {0}", result.Violations.Count);
        }

        /// <summary>
        /// 回傳適合顯示在狀態牌內的短狀態文字。
        /// </summary>
        private static string GetDisplayStatus(ArchitectureScanResult result)
        {
            if (result.ErrorCount > 0) return "NG";
            if (result.WarningCount > 0) return "OK with";
            return "OK";
        }

        #endregion

        #region 違規清單排序

        /// <summary>
        /// 建立要顯示在表格中的違規清單；排序順序為 error 優先，再依檔案路徑與行號排序。
        /// </summary>
        private static List<ArchitectureViolation> BuildSortedViolations(List<ArchitectureViolation> violations)
        {
            List<ArchitectureViolation> sortedViolations = new List<ArchitectureViolation>(violations);
            sortedViolations.Sort(CompareViolationForDisplay);
            return sortedViolations;
        }

        /// <summary>
        /// 比較兩筆違規資料的顯示順序；回傳小於 0 代表 left 排在前面，大於 0 代表 right 排在前面。
        /// </summary>
        private static int CompareViolationForDisplay(ArchitectureViolation left, ArchitectureViolation right)
        {
            int leftSeverityRank = GetSeverityRank(left);
            int rightSeverityRank = GetSeverityRank(right);
            int severityCompare = leftSeverityRank.CompareTo(rightSeverityRank);
            if (severityCompare != 0) return severityCompare;

            int pathCompare = string.Compare(left.RelativePath, right.RelativePath, StringComparison.OrdinalIgnoreCase);
            if (pathCompare != 0) return pathCompare;

            return left.LineNumber.CompareTo(right.LineNumber);
        }

        /// <summary>
        /// 將嚴重程度轉成排序權重；error 權重較小，所以會排在 warning 前面。
        /// </summary>
        private static int GetSeverityRank(ArchitectureViolation violation)
        {
            if (violation == null) return 99;
            if (string.Equals(violation.Severity, "error", StringComparison.OrdinalIgnoreCase)) return 0;
            return 1;
        }

        #endregion

        #region 結果列顯示與選取

        /// <summary>
        /// 在資料來源更新後，依嚴重程度套用結果列顏色。
        /// </summary>
        private void ColorRows()
        {
            foreach (DataGridViewRow row in gridViolations.Rows)
            {
                ArchitectureViolation violation = row.DataBoundItem as ArchitectureViolation;
                if (violation == null) continue;

                if (string.Equals(violation.Severity, "error", StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(72, 42, 44);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(255, 226, 226);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(69, 57, 38);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(255, 238, 204);
                }
            }
        }

        /// <summary>
        /// 顯示目前選取違規項目的詳細原始碼上下文。
        /// </summary>
        private void GridViolations_SelectionChanged(object sender, EventArgs e)
        {
            ArchitectureViolation violation = GetSelectedViolation();
            SetOpenFileButtonState(violation != null);
            txtDetails.Text = violation == null ? string.Empty : BuildDetailText(violation);
        }

        #endregion

        #region Open File 按鈕與檔案開啟

        /// <summary>
        /// 依目前是否有可開啟檔案，更新 Open File 按鈕的顏色與圖示對比；按鈕保持啟用外觀以避免系統灰階壓暗圖示。
        /// </summary>
        private void SetOpenFileButtonState(bool enabled)
        {
            btnOpenFile.Enabled = true;
            btnOpenFile.BackColor = enabled ? openFileEnabledBackColor : openFileDisabledBackColor;
            btnOpenFile.ForeColor = enabled ? openFileEnabledForeColor : openFileDisabledForeColor;
            btnOpenFile.Cursor = enabled ? Cursors.Hand : Cursors.Default;
            btnOpenFile.Image = LoadIcon(
                "icon_open_file.png",
                18,
                18,
                enabled ? Color.White : Color.FromArgb(220, 230, 240));
        }

        /// <summary>
        /// 使用者雙擊結果列時開啟該違規項目的檔案。
        /// </summary>
        private void GridViolations_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                OpenSelectedFile();
            }
        }

        /// <summary>
        /// 使用者按下 Open File 時開啟目前選取的違規檔案。
        /// </summary>
        private void BtnOpenFile_Click(object sender, EventArgs e)
        {
            OpenSelectedFile();
        }

        /// <summary>
        /// 使用 Windows 預設編輯器開啟目前選取的違規檔案。
        /// </summary>
        private void OpenSelectedFile()
        {
            ArchitectureViolation violation = GetSelectedViolation();
            if (violation == null || !File.Exists(violation.FilePath)) return;
            Process.Start(new ProcessStartInfo
            {
                FileName = violation.FilePath,
                UseShellExecute = true
            });
        }

        /// <summary>
        /// 取得目前在表格中選取的違規項目；未選取時回傳 null。
        /// </summary>
        private ArchitectureViolation GetSelectedViolation()
        {
            if (gridViolations.SelectedRows.Count == 0) return null;
            return gridViolations.SelectedRows[0].DataBoundItem as ArchitectureViolation;
        }

        #endregion

        #region Details 內容產生

        /// <summary>
        /// 建立表格下方的詳細文字，包含規則資訊與原始碼上下文。
        /// </summary>
        private static string BuildDetailText(ArchitectureViolation violation)
        {
            string context = ReadContext(violation.FilePath, violation.LineNumber);
            return string.Format(
                "{0} {1}\r\n{2}:{3}\r\nMatched: {4}\r\nMessage: {5}\r\n\r\n{6}",
                violation.Severity,
                violation.RuleId,
                violation.FilePath,
                violation.LineNumber,
                violation.MatchedText,
                violation.Message,
                context);
        }

        /// <summary>
        /// 讀取違規行附近的小段原始碼，方便使用者檢視問題。
        /// </summary>
        private static string ReadContext(string filePath, int lineNumber)
        {
            if (!File.Exists(filePath)) return string.Empty;

            string[] lines = File.ReadAllLines(filePath);
            int start = Math.Max(1, lineNumber - 3);
            int end = Math.Min(lines.Length, lineNumber + 3);
            string[] context = new string[end - start + 1];

            for (int i = start; i <= end; i++)
            {
                string marker = i == lineNumber ? ">" : " ";
                context[i - start] = string.Format("{0} {1,5}: {2}", marker, i, lines[i - 1]);
            }

            return string.Join(Environment.NewLine, context);
        }

        #endregion

        #region 路徑工具

        /// <summary>
        /// 優先從執行檔位置往上尋找 solution root；找不到時才使用目前工作目錄。
        /// </summary>
        private static string FindDefaultRootPath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            for (int i = 0; i < 8 && !string.IsNullOrEmpty(path); i++)
            {
                if (Directory.Exists(Path.Combine(path, "LBT4500")) &&
                    File.Exists(Path.Combine(path, "FTMachines.sln")))
                {
                    return path.TrimEnd(Path.DirectorySeparatorChar);
                }

                DirectoryInfo parent = Directory.GetParent(path);
                path = parent == null ? null : parent.FullName;
            }

            return Environment.CurrentDirectory;
        }

        #endregion
    }
}
