using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using HistoryReader.Base;

namespace HistoryReader.UI
{
    public partial class F_HistoryReader : Form, IF_HistoryReader
    {
        public F_HistoryReader(IF_HistoryReaderLogic logic)
        {
            // 初始化視窗元件並設定歷史紀錄邏輯。
            InitializeComponent();

            _logic = logic;

            InitialItem();
            Disposed += F_HistoryReader_Disposed;
        }

        public void ShowOrActivate()
        {
            // 由 DI 管理視窗實例；這裡只負責顯示或啟用目前的視窗。
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(F_HistoryReader));

            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;
            if (!Visible)
                Show();

            BringToFront();
            Activate();
        }


        #region parameter
        private string SelectedFileName
        {
            get
            {
                SourceOption source = Cmbx_Source.SelectedItem as SourceOption;
                return source == null ? null : source.FileName;
            }
        }
        private bool IsLiveSource => _currentFileName == null;
        private bool CanUpdate => _shown && !_closing && !IsDisposed && Visible && WindowState != FormWindowState.Minimized;
        private LogLevelFilter SelectedFilter
        {
            get
            {
                return Cmbx_Filter.SelectedItem is LogLevelFilter
                    ? (LogLevelFilter)Cmbx_Filter.SelectedItem
                    : LogLevelFilter.All;
            }
        }
        private readonly IF_HistoryReaderLogic _logic = null;
        private sealed class SourceOption
        {
            // 歷史紀錄檔名
            public SourceOption(string fileName)
            {
                // 設定來源選項所代表的檔名。
                FileName = fileName;
            }

            public string FileName { get; }

            public override string ToString()
            {
                // 提供來源下拉選單要顯示的文字。
                return FileName == null ? "Live" : FileName;
            }
        }
        #endregion

        #region private function
        private void InitialItem()
        {
            // 初始化來源與 log 等級篩選選項。
            _updatingSources = true;

            Cmbx_Source.Items.Add(new SourceOption(null));
            Cmbx_Source.SelectedIndex = 0;
            Cmbx_Filter.DataSource = Enum.GetValues(typeof(LogLevelFilter));
            Cmbx_Filter.SelectedItem = LogLevelFilter.All;
            _updatingSources = false;
        }

        private void ShowEntries(IReadOnlyList<LogEntry> source, bool forceRebuild)
        {
            // 依目前篩選條件更新畫面上的 log 與狀態文字。
            // 顯示狀態列
            if (!CanUpdate)
            {
                _displayPending = true;
                return;
            }

            _displayPending = false;
            IReadOnlyList<LogEntry> filtered = _logic.Filter(source, SelectedFilter);
            RenderEntries(filtered, forceRebuild);
            string sourceName = IsLiveSource ? "Live" : _currentFileName;
            Lbl_Status.Text = String.Format($"{sourceName}: showing {filtered.Count} of latest {source.Count} logs");
        }

        private void AddRow(LogEntry entry)
        {
            // 新增一列並填入 log 項目的內容。
            int index = DGV_log.Rows.Add();
            UpdateRow(DGV_log.Rows[index], entry);
        }

        private void UpdateRow(DataGridViewRow row, LogEntry entry)
        {
            // 將 log 項目的時間、等級與訊息寫入表格列。
            if (ReferenceEquals(row.Tag, entry))
                return;

            row.Tag = entry;
            row.Cells[0].Value = entry.Timestamp.HasValue
                ? entry.Timestamp.Value.ToString("yyyy-MM-dd HH:mm:ss.fff")
                : String.Empty;
            row.Cells[1].Value = entry.Level.ToString();
            row.Cells[2].Value = entry.Message;
            row.DefaultCellStyle.ForeColor = GetLevelColor(entry.Level);
        }

        private Color GetLevelColor(LogLevel level)
        {
            // 依 log 等級回傳表格文字顏色。
            switch (level)
            {
                case LogLevel.Error: return Color.FromArgb(220, 53, 69);
                case LogLevel.Warning: return Color.FromArgb(230, 126, 0);
                case LogLevel.Info: return Color.FromArgb(25, 118, 210);
                case LogLevel.Debug: return Color.FromArgb(90, 90, 90);
                default: return Color.Black;
            }
        }

        private void ShowSelectedDetail()
        {
            // 將目前選取項目的完整內容顯示在明細區。
            if (DGV_log.SelectedRows.Count == 0)
            {
                if (TxtBx_Detail.TextLength > 0)
                    TxtBx_Detail.Clear();
                return;
            }

            LogEntry entry = DGV_log.SelectedRows[0].Tag as LogEntry;
            string detail = entry == null ? String.Empty : entry.FullText;

            if (!String.Equals(TxtBx_Detail.Text, detail, StringComparison.Ordinal))
                TxtBx_Detail.Text = detail;
        }
        #endregion



        private readonly List<LogEntry> _displayedEntries = new List<LogEntry>();
        private IReadOnlyList<LogEntry> _liveEntries = new LogEntry[0];
        private IReadOnlyList<LogEntry> _fileEntries = new LogEntry[0];
        private CancellationTokenSource _fileListCancellation;
        private CancellationTokenSource _fileLoadCancellation;
        private string _currentFileName;
        private long _lastLiveVersion = -1;
        private bool _shown;
        private bool _closing;
        private bool _updatingSources;
        private bool _rendering;
        private bool _fileLoadInProgress;
        private bool _displayPending;
        private int _visibilityVersion;

        

        protected override async void OnShown(EventArgs e)
        {
            // 視窗首次顯示時載入 Live 資料並更新檔案清單。
            base.OnShown(e);
            _shown = true;
            RefreshLive(true);
            UpdateTimers();
            await RefreshFileListAsync(false);
        }

        protected override async void OnVisibleChanged(EventArgs e)
        {
            // 隱藏時停止刷新與讀取；再次顯示時更新目前來源。
            base.OnVisibleChanged(e);
            if (!_shown)
                return;

            int version = ++_visibilityVersion;
            if (!Visible)
            {
                UpdateTimers();
                CancelFileList();
                CancelFileLoad();
                return;
            }

            await RefreshFileListAsync(true, true);
            if (version == _visibilityVersion)
                UpdateTimers();
        }

        protected override void OnResize(EventArgs e)
        {
            // 視窗尺寸改變時重新檢查計時器狀態。
            base.OnResize(e);
            if (_shown)
                UpdateTimers();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.Cancel)
                return;

            // 使用者關閉視窗時保留 Form 與畫面狀態。
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                return;
            }

            // 應用程式真正結束時才停止並取消工作。
            _closing = true;
            LiveTimer.Stop();
            FileTimer.Stop();
            CancelFileList();
            CancelFileLoad();
        }

        private void F_HistoryReader_Disposed(object sender, EventArgs e)
        {
            // 容器直接 Dispose 視窗時也取消尚未完成的讀取。
            _closing = true;
            CancelFileList();
            CancelFileLoad();
        }

        private void UpdateTimers()
        {
            // 依視窗、來源及自動刷新設定啟停計時器。
            bool wasActive = LiveTimer.Enabled || FileTimer.Enabled;
            LiveTimer.Stop();
            FileTimer.Stop();

            if (!CanUpdate)
                return;

            if (_displayPending)
                ShowEntries(IsLiveSource ? _liveEntries : _fileEntries, true);

            if (!Chk_Auto.Checked)
                return;

            if (IsLiveSource)
            {
                LiveTimer.Start();
                if (!wasActive)
                    RefreshLive(false);
            }
            else
            {
                FileTimer.Start();
            }
        }

        private void RefreshLive(bool forceRebuild)
        {
            // 取得最新 Live 快照，並在資料變更時更新畫面。
            if (_closing || !IsLiveSource)
                return;

            long version = _logic.LiveVersion;
            if (!forceRebuild && version == _lastLiveVersion)
                return;

            _liveEntries = _logic.GetLiveSnapshot();
            _lastLiveVersion = version;
            ShowEntries(_liveEntries, forceRebuild);
        }

        private async Task RefreshFileListAsync(bool refreshSelectedSource,
            bool incrementalSelectedSource = false)
        {
            // 非同步更新歷史檔案清單並維持目前選取來源。
            CancelFileList();
            CancellationTokenSource cancellation = new CancellationTokenSource();
            _fileListCancellation = cancellation;

            try
            {
                IReadOnlyList<string> files = await _logic.GetFilesAsync(cancellation.Token);
                if (_closing || IsDisposed || !Visible || cancellation.IsCancellationRequested ||
                    !ReferenceEquals(_fileListCancellation, cancellation))
                    return;

                string selectedFile = SelectedFileName;
                int selectedIndex = 0;
                _updatingSources = true;
                try
                {
                    Cmbx_Source.BeginUpdate();
                    Cmbx_Source.Items.Clear();
                    Cmbx_Source.Items.Add(new SourceOption(null));
                    for (int index = 0; index < files.Count; index++)
                    {
                        Cmbx_Source.Items.Add(new SourceOption(files[index]));
                        if (String.Equals(files[index], selectedFile, StringComparison.OrdinalIgnoreCase))
                            selectedIndex = index + 1;
                    }

                    if (selectedFile != null && selectedIndex == 0 && files.Count > 0)
                        selectedIndex = 1;

                    Cmbx_Source.SelectedIndex = selectedIndex;
                }
                finally
                {
                    Cmbx_Source.EndUpdate();
                    _updatingSources = false;
                }

                if (!String.Equals(_currentFileName, SelectedFileName, StringComparison.OrdinalIgnoreCase))
                    await HandleSourceChangedAsync();
                else if (refreshSelectedSource)
                {
                    if (IsLiveSource)
                        RefreshLive(!incrementalSelectedSource);
                    else
                        await LoadSelectedFileAsync(incrementalSelectedSource);
                }
                else if (files.Count == 0 && _liveEntries.Count == 0)
                {
                    Lbl_Status.Text = "No log file.";
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                if (!_closing && Visible && !cancellation.IsCancellationRequested &&
                    ReferenceEquals(_fileListCancellation, cancellation))
                    Lbl_Status.Text = "List log files failed: " + ex.Message;
            }
            finally
            {
                if (ReferenceEquals(_fileListCancellation, cancellation))
                    _fileListCancellation = null;
                cancellation.Dispose();
            }
        }

        private async Task HandleSourceChangedAsync()
        {
            // 切換來源時清除舊資料並載入新來源內容。
            string fileName = SelectedFileName;
            if (String.Equals(_currentFileName, fileName, StringComparison.OrdinalIgnoreCase))
                return;

            CancelFileLoad();
            _currentFileName = fileName;
            _fileEntries = new LogEntry[0];
            if (CanUpdate)
            {
                _displayedEntries.Clear();
                DGV_log.Rows.Clear();
                TxtBx_Detail.Clear();
            }
            else
            {
                _displayPending = true;
            }
            UpdateTimers();

            if (IsLiveSource)
                RefreshLive(true);
            else
                await LoadSelectedFileAsync(false);
        }

        private async Task LoadSelectedFileAsync(bool incremental)
        {
            // 載入所選歷史檔案，或增量讀取檔案的新內容。
            if (_closing || IsLiveSource)
                return;
            CancelFileLoad();
            CancellationTokenSource cancellation = new CancellationTokenSource();
            _fileLoadCancellation = cancellation;
            _fileLoadInProgress = true;
            string fileName = _currentFileName;
            if (!incremental)
                Lbl_Status.Text = "Loading " + fileName + "...";

            try
            {
                IReadOnlyList<LogEntry> entries = incremental
                    ? await _logic.RefreshFileAsync(fileName, cancellation.Token)
                    : await _logic.LoadFileAsync(fileName, cancellation.Token);

                if (_closing || IsDisposed || !Visible || cancellation.IsCancellationRequested ||
                    !ReferenceEquals(_fileLoadCancellation, cancellation) ||
                    !String.Equals(_currentFileName, fileName, StringComparison.OrdinalIgnoreCase))
                    return;

                _fileEntries = entries;
                ShowEntries(_fileEntries, !incremental);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                if (!_closing && Visible && !cancellation.IsCancellationRequested &&
                    ReferenceEquals(_fileLoadCancellation, cancellation))
                {
                    string message = "Read log failed: " + ex.Message;
                    Lbl_Status.Text = message;
                    TxtBx_Detail.Text = message;
                }
            }
            finally
            {
                if (ReferenceEquals(_fileLoadCancellation, cancellation))
                {
                    _fileLoadCancellation = null;
                    _fileLoadInProgress = false;
                }
                cancellation.Dispose();
            }
        }

        

        private void RenderEntries(IReadOnlyList<LogEntry> entries, bool forceRebuild)
        {
            // 更新表格列，並盡可能保留使用者的選取與捲動位置。
            if (!forceRebuild && HasSameDisplayedEntries(entries))
                return;

            bool followBottom = Chk_Follow.Checked &&
                                (_displayedEntries.Count == 0 || IsScrolledToBottom());
            long? selectedId = SelectedEntryId();
            long? firstVisibleId = FirstVisibleEntryId();

            int removeCount = 0;
            int commonCount = 0;
            bool canReuse = !forceRebuild && TryFindOverlap(entries, out removeCount, out commonCount);

            _rendering = true;
            DGV_log.SuspendLayout();
            try
            {
                if (!canReuse)
                {
                    DGV_log.Rows.Clear();
                    for (int index = 0; index < entries.Count; index++)
                        AddRow(entries[index]);
                }
                else
                {
                    for (int index = 0; index < removeCount; index++)
                        DGV_log.Rows.RemoveAt(0);
                    while (DGV_log.Rows.Count > commonCount)
                        DGV_log.Rows.RemoveAt(DGV_log.Rows.Count - 1);
                    for (int index = 0; index < commonCount; index++)
                        UpdateRow(DGV_log.Rows[index], entries[index]);
                    for (int index = commonCount; index < entries.Count; index++)
                        AddRow(entries[index]);
                }

                _displayedEntries.Clear();
                for (int index = 0; index < entries.Count; index++)
                    _displayedEntries.Add(entries[index]);

                RestoreSelection(selectedId);
                RestoreScroll(firstVisibleId, followBottom);
            }
            finally
            {
                DGV_log.ResumeLayout();
                _rendering = false;
            }

            ShowSelectedDetail();
        }

        private bool HasSameDisplayedEntries(IReadOnlyList<LogEntry> entries)
        {
            // 檢查新清單是否與目前顯示的項目完全相同。
            if (_displayedEntries.Count != entries.Count)
                return false;

            for (int index = 0; index < entries.Count; index++)
            {
                if (!ReferenceEquals(_displayedEntries[index], entries[index]))
                    return false;
            }

            return true;
        }

        private bool TryFindOverlap(IReadOnlyList<LogEntry> incoming,
            out int removeCount, out int commonCount)
        {
            // 找出新舊清單的重疊區段，以便重用表格列。
            removeCount = 0;
            commonCount = 0;

            if (incoming.Count == 0 || _displayedEntries.Count == 0)
                return false;

            long firstNewId = incoming[0].Id;
            int oldIndex = _displayedEntries.FindIndex(entry => entry.Id == firstNewId);
            if (oldIndex < 0)
                return false;

            int overlap = Math.Min(_displayedEntries.Count - oldIndex, incoming.Count);
            for (int index = 0; index < overlap; index++)
            {
                if (_displayedEntries[oldIndex + index].Id != incoming[index].Id)
                    return false;
            }

            removeCount = oldIndex;
            commonCount = overlap;
            return true;
        }

        

        

        

        private bool IsScrolledToBottom()
        {
            // 判斷表格目前是否已捲動至最底部。
            if (DGV_log.Rows.Count == 0)
                return true;

            int first = DGV_log.FirstDisplayedScrollingRowIndex;
            if (first < 0)
                return true;

            return first + DGV_log.DisplayedRowCount(false) >= DGV_log.Rows.Count;
        }

        private long? SelectedEntryId()
        {
            // 取得目前選取 log 項目的識別碼。
            if (DGV_log.SelectedRows.Count == 0)
                return null;
            LogEntry entry = DGV_log.SelectedRows[0].Tag as LogEntry;
            return entry == null ? (long?)null : entry.Id;
        }

        private long? FirstVisibleEntryId()
        {
            // 取得表格第一筆可見 log 項目的識別碼。
            int index = DGV_log.FirstDisplayedScrollingRowIndex;
            return index >= 0 && index < _displayedEntries.Count
                ? _displayedEntries[index].Id
                : (long?)null;
        }

        private void RestoreSelection(long? selectedId)
        {
            // 重新選取更新前的 log 項目。
            if (SelectedEntryId() == selectedId)
                return;

            DGV_log.ClearSelection();
            if (!selectedId.HasValue)
                return;

            for (int index = 0; index < DGV_log.Rows.Count; index++)
            {
                LogEntry entry = DGV_log.Rows[index].Tag as LogEntry;
                if (entry != null && entry.Id == selectedId.Value)
                {
                    DGV_log.Rows[index].Selected = true;
                    DGV_log.CurrentCell = DGV_log.Rows[index].Cells[2];
                    break;
                }
            }
        }

        private void RestoreScroll(long? firstVisibleId, bool followBottom)
        {
            // 更新表格後恢復原捲動位置或跟隨最新資料。
            if (DGV_log.Rows.Count == 0)
                return;

            int target = -1;
            if (followBottom)
            {
                target = DGV_log.Rows.Count - 1;
            }
            else if (firstVisibleId.HasValue)
            {
                for (int index = 0; index < DGV_log.Rows.Count; index++)
                {
                    LogEntry entry = DGV_log.Rows[index].Tag as LogEntry;
                    if (entry != null && entry.Id == firstVisibleId.Value)
                    {
                        target = index;
                        break;
                    }
                }

                // The viewed row can leave the 300-entry window while the user reads.
                // In that case keep the grid at the oldest remaining row.
                if (target < 0)
                    target = 0;
            }

            if (target >= 0)
                DGV_log.FirstDisplayedScrollingRowIndex = target;
        }

        

        private void CancelFileList()
        {
            // 取消尚未完成的歷史檔案清單載入。
            if (_fileListCancellation != null)
                _fileListCancellation.Cancel();
        }

        private void CancelFileLoad()
        {
            // 取消尚未完成的歷史檔案讀取。
            if (_fileLoadCancellation != null)
                _fileLoadCancellation.Cancel();
        }

        private async void Cmbx_Source_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 來源選項改變時載入對應資料。
            if (!_updatingSources && !_closing)
                await HandleSourceChangedAsync();
        }

        private async void Btn_Refresh_Click(object sender, EventArgs e)
        {
            // 手動重新整理歷史檔案清單及目前來源。
            await RefreshFileListAsync(true);
        }

        private void Chk_Auto_CheckedChanged(object sender, EventArgs e)
        {
            // 自動刷新選項改變時更新計時器。
            if (_shown)
                UpdateTimers();
        }

        private void Cmbx_Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 篩選條件改變時重新顯示符合條件的 log。
            if (!_updatingSources && _shown && !_closing)
                ShowEntries(IsLiveSource ? _liveEntries : _fileEntries, true);
        }

        private void Grid_Log_SelectionChanged(object sender, EventArgs e)
        {
            // 表格選取項目改變時更新明細內容。
            if (!_rendering)
                ShowSelectedDetail();
        }

        private void LiveTimer_Tick(object sender, EventArgs e)
        {
            // Live 計時器觸發時檢查並更新即時資料。
            if (CanUpdate && Chk_Auto.Checked && IsLiveSource)
                RefreshLive(false);
        }

        private async void FileTimer_Tick(object sender, EventArgs e)
        {
            // 檔案計時器觸發時增量讀取目前選取的 log 檔。
            if (CanUpdate && Chk_Auto.Checked && !IsLiveSource && !_fileLoadInProgress)
                await LoadSelectedFileAsync(true);
        }

        
    }
}
