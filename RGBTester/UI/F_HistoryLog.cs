using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RGBTester.UI
{
    public partial class F_HistoryLog : Form
    {
        private readonly Dictionary<string, Color> LevelColors = new Dictionary<string, Color>
        {
            { "ERR", Color.FromArgb(220, 53, 69) },
            { "ERROR", Color.FromArgb(220, 53, 69) },
            { "WRN", Color.FromArgb(255, 152, 0) },
            { "WARN", Color.FromArgb(255, 152, 0) },
            { "INF", Color.FromArgb(25, 118, 210) },
            //{ "INFO", Color.FromArgb(25, 118, 210) },
            { "DBG", Color.FromArgb(90, 90, 90) },
            { "DEBUG", Color.FromArgb(90, 90, 90) }
        };

        public F_HistoryLog()
        {
            InitializeComponent();
        }

        private void F_HistoryLog_Load(object sender, EventArgs e)
        {
            LoadLogFiles();
        }

        private void F_HistoryLog_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                LoadLogFiles();
        }

        private void Cmbx_LogFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSelectedLog();
        }

        private void Btn_Refresh_Click(object sender, EventArgs e)
        {
            RefreshLog();
        }

        private void Chk_AutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            RefreshTimer.Enabled = Chk_AutoRefresh.Checked;
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadSelectedLog(false);
        }

        private void LoadLogFiles()
        {
            string folder = GetHistoryFolder();
            Directory.CreateDirectory(folder);

            string selectedFile = Cmbx_LogFile.SelectedItem as string;
            string[] logFiles = Directory.GetFiles(folder, "*.log", SearchOption.TopDirectoryOnly)
                .OrderByDescending(File.GetLastWriteTime)
                .Select(Path.GetFileName)
                .ToArray();

            Cmbx_LogFile.Items.Clear();
            Cmbx_LogFile.Items.AddRange(logFiles);

            if (logFiles.Length == 0)
            {
                Rtxt_Log.Clear();
                Rtxt_Log.Text = "No log file.";
                return;
            }

            int selectedIndex = Array.IndexOf(logFiles, selectedFile);
            Cmbx_LogFile.SelectedIndex = selectedIndex >= 0 ? selectedIndex : 0;
        }

        private void RefreshLog()
        {
            string selectedFile = Cmbx_LogFile.SelectedItem as string;
            LoadLogFiles();

            if (!string.IsNullOrEmpty(selectedFile) && Cmbx_LogFile.Items.Contains(selectedFile))
                Cmbx_LogFile.SelectedItem = selectedFile;

            LoadSelectedLog();
        }

        private void LoadSelectedLog(bool scrollToEnd = true)
        {
            if (Cmbx_LogFile.SelectedItem == null)
                return;

            string filePath = Path.Combine(GetHistoryFolder(), Cmbx_LogFile.SelectedItem.ToString());
            if (!File.Exists(filePath))
                return;

            string[] lines;
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, true))
                {
                    lines = reader.ReadToEnd()
                        .Replace("\r\n", "\n")
                        .Replace("\r", "\n")
                        .Split('\n');
                }
            }
            catch (IOException ex)
            {
                Rtxt_Log.Text = "Read log fail: " + ex.Message;
                return;
            }

            PaintLog(lines);

            if (scrollToEnd)
            {
                Rtxt_Log.SelectionStart = Rtxt_Log.TextLength;
                Rtxt_Log.ScrollToCaret();
            }
        }

        private void PaintLog(IEnumerable<string> lines)
        {
            Rtxt_Log.SuspendLayout();
            Rtxt_Log.Clear();
            Rtxt_Log.Font = new Font(Rtxt_Log.Font.FontFamily, 14f, Rtxt_Log.Font.Style);

            foreach (string line in lines)
            {
                Color color = GetLineColor(line);
                Rtxt_Log.SelectionColor = color;
                Rtxt_Log.AppendText(line + Environment.NewLine);
            }

            Rtxt_Log.SelectionColor = Rtxt_Log.ForeColor;
            Rtxt_Log.ResumeLayout();
        }

        private Color GetLineColor(string line)
        {
            foreach (KeyValuePair<string, Color> pair in LevelColors)
            {
                if (line.IndexOf("[" + pair.Key + "]", StringComparison.OrdinalIgnoreCase) >= 0)
                    return pair.Value;
            }

            return Color.Black;
        }

        private string GetHistoryFolder()
        {
            return Path.Combine(Application.StartupPath, "History");
        }
    }
}
