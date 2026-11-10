namespace HistoryReader.UI
{
    partial class F_HistoryReader
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.Toolbar = new System.Windows.Forms.FlowLayoutPanel();
            this.Lbl_Source = new System.Windows.Forms.Label();
            this.Cmbx_Source = new System.Windows.Forms.ComboBox();
            this.Btn_Refresh = new System.Windows.Forms.Button();
            this.Chk_Auto = new System.Windows.Forms.CheckBox();
            this.Lbl_Filter = new System.Windows.Forms.Label();
            this.Cmbx_Filter = new System.Windows.Forms.ComboBox();
            this.Chk_Follow = new System.Windows.Forms.CheckBox();
            this.Split_Log = new System.Windows.Forms.SplitContainer();
            this.DGV_log = new System.Windows.Forms.DataGridView();
            this.Col_Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pnl_Detail = new System.Windows.Forms.TableLayoutPanel();
            this.Lbl_Detail = new System.Windows.Forms.Label();
            this.TxtBx_Detail = new System.Windows.Forms.TextBox();
            this.Lbl_Status = new System.Windows.Forms.Label();
            this.LiveTimer = new System.Windows.Forms.Timer(this.components);
            this.FileTimer = new System.Windows.Forms.Timer(this.components);
            this.MainLayout.SuspendLayout();
            this.Toolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Split_Log)).BeginInit();
            this.Split_Log.Panel1.SuspendLayout();
            this.Split_Log.Panel2.SuspendLayout();
            this.Split_Log.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_log)).BeginInit();
            this.Pnl_Detail.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainLayout
            // 
            this.MainLayout.ColumnCount = 1;
            this.MainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayout.Controls.Add(this.Toolbar, 0, 0);
            this.MainLayout.Controls.Add(this.Split_Log, 0, 1);
            this.MainLayout.Controls.Add(this.Lbl_Status, 0, 2);
            this.MainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainLayout.Location = new System.Drawing.Point(0, 0);
            this.MainLayout.Name = "MainLayout";
            this.MainLayout.RowCount = 3;
            this.MainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.MainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.MainLayout.Size = new System.Drawing.Size(1100, 700);
            this.MainLayout.TabIndex = 0;
            // 
            // Toolbar
            // 
            this.Toolbar.AutoScroll = true;
            this.Toolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(244)))), ((int)(((byte)(247)))));
            this.Toolbar.Controls.Add(this.Lbl_Source);
            this.Toolbar.Controls.Add(this.Cmbx_Source);
            this.Toolbar.Controls.Add(this.Btn_Refresh);
            this.Toolbar.Controls.Add(this.Chk_Auto);
            this.Toolbar.Controls.Add(this.Lbl_Filter);
            this.Toolbar.Controls.Add(this.Cmbx_Filter);
            this.Toolbar.Controls.Add(this.Chk_Follow);
            this.Toolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Toolbar.Location = new System.Drawing.Point(3, 3);
            this.Toolbar.Name = "Toolbar";
            this.Toolbar.Padding = new System.Windows.Forms.Padding(10, 8, 0, 0);
            this.Toolbar.Size = new System.Drawing.Size(1094, 44);
            this.Toolbar.TabIndex = 0;
            this.Toolbar.WrapContents = false;
            // 
            // Lbl_Source
            // 
            this.Lbl_Source.AutoSize = true;
            this.Lbl_Source.Location = new System.Drawing.Point(10, 14);
            this.Lbl_Source.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.Lbl_Source.Name = "Lbl_Source";
            this.Lbl_Source.Size = new System.Drawing.Size(54, 18);
            this.Lbl_Source.TabIndex = 0;
            this.Lbl_Source.Text = "Source";
            // 
            // Cmbx_Source
            // 
            this.Cmbx_Source.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmbx_Source.FormattingEnabled = true;
            this.Cmbx_Source.Location = new System.Drawing.Point(70, 10);
            this.Cmbx_Source.Margin = new System.Windows.Forms.Padding(0, 2, 9, 0);
            this.Cmbx_Source.Name = "Cmbx_Source";
            this.Cmbx_Source.Size = new System.Drawing.Size(360, 25);
            this.Cmbx_Source.TabIndex = 1;
            this.Cmbx_Source.SelectedIndexChanged += new System.EventHandler(this.Cmbx_Source_SelectedIndexChanged);
            // 
            // Btn_Refresh
            // 
            this.Btn_Refresh.Location = new System.Drawing.Point(439, 8);
            this.Btn_Refresh.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.Btn_Refresh.Name = "Btn_Refresh";
            this.Btn_Refresh.Size = new System.Drawing.Size(80, 29);
            this.Btn_Refresh.TabIndex = 2;
            this.Btn_Refresh.Text = "Refresh";
            this.Btn_Refresh.UseVisualStyleBackColor = true;
            this.Btn_Refresh.Click += new System.EventHandler(this.Btn_Refresh_Click);
            // 
            // Chk_Auto
            // 
            this.Chk_Auto.AutoSize = true;
            this.Chk_Auto.Checked = true;
            this.Chk_Auto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Chk_Auto.Location = new System.Drawing.Point(531, 14);
            this.Chk_Auto.Margin = new System.Windows.Forms.Padding(0, 6, 14, 0);
            this.Chk_Auto.Name = "Chk_Auto";
            this.Chk_Auto.Size = new System.Drawing.Size(60, 22);
            this.Chk_Auto.TabIndex = 3;
            this.Chk_Auto.Text = "Auto";
            this.Chk_Auto.CheckedChanged += new System.EventHandler(this.Chk_Auto_CheckedChanged);
            // 
            // Lbl_Filter
            // 
            this.Lbl_Filter.AutoSize = true;
            this.Lbl_Filter.Location = new System.Drawing.Point(605, 14);
            this.Lbl_Filter.Margin = new System.Windows.Forms.Padding(0, 6, 6, 0);
            this.Lbl_Filter.Name = "Lbl_Filter";
            this.Lbl_Filter.Size = new System.Drawing.Size(42, 18);
            this.Lbl_Filter.TabIndex = 4;
            this.Lbl_Filter.Text = "Level";
            // 
            // Cmbx_Filter
            // 
            this.Cmbx_Filter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmbx_Filter.FormattingEnabled = true;
            this.Cmbx_Filter.Location = new System.Drawing.Point(653, 10);
            this.Cmbx_Filter.Margin = new System.Windows.Forms.Padding(0, 2, 14, 0);
            this.Cmbx_Filter.Name = "Cmbx_Filter";
            this.Cmbx_Filter.Size = new System.Drawing.Size(120, 25);
            this.Cmbx_Filter.TabIndex = 5;
            this.Cmbx_Filter.SelectedIndexChanged += new System.EventHandler(this.Cmbx_Filter_SelectedIndexChanged);
            // 
            // Chk_Follow
            // 
            this.Chk_Follow.AutoSize = true;
            this.Chk_Follow.Checked = true;
            this.Chk_Follow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Chk_Follow.Location = new System.Drawing.Point(787, 14);
            this.Chk_Follow.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.Chk_Follow.Name = "Chk_Follow";
            this.Chk_Follow.Size = new System.Drawing.Size(125, 22);
            this.Chk_Follow.TabIndex = 6;
            this.Chk_Follow.Text = "Follow bottom";
            // 
            // Split_Log
            // 
            this.Split_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Split_Log.Location = new System.Drawing.Point(8, 55);
            this.Split_Log.Margin = new System.Windows.Forms.Padding(8, 5, 8, 2);
            this.Split_Log.Name = "Split_Log";
            this.Split_Log.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // Split_Log.Panel1
            // 
            this.Split_Log.Panel1.Controls.Add(this.DGV_log);
            // 
            // Split_Log.Panel2
            // 
            this.Split_Log.Panel2.Controls.Add(this.Pnl_Detail);
            this.Split_Log.Size = new System.Drawing.Size(1084, 615);
            this.Split_Log.SplitterDistance = 429;
            this.Split_Log.SplitterWidth = 6;
            this.Split_Log.TabIndex = 1;
            // 
            // DGV_log
            // 
            this.DGV_log.AllowUserToAddRows = false;
            this.DGV_log.AllowUserToDeleteRows = false;
            this.DGV_log.AllowUserToResizeRows = false;
            this.DGV_log.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DGV_log.BackgroundColor = System.Drawing.Color.White;
            this.DGV_log.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.DGV_log.ColumnHeadersHeight = 31;
            this.DGV_log.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.DGV_log.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col_Time,
            this.Col_Level,
            this.Col_Message});
            this.DGV_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGV_log.Location = new System.Drawing.Point(0, 0);
            this.DGV_log.MultiSelect = false;
            this.DGV_log.Name = "DGV_log";
            this.DGV_log.ReadOnly = true;
            this.DGV_log.RowHeadersVisible = false;
            this.DGV_log.RowTemplate.Height = 27;
            this.DGV_log.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV_log.Size = new System.Drawing.Size(1084, 429);
            this.DGV_log.TabIndex = 0;
            this.DGV_log.SelectionChanged += new System.EventHandler(this.Grid_Log_SelectionChanged);
            // 
            // Col_Time
            // 
            this.Col_Time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Col_Time.HeaderText = "Time";
            this.Col_Time.Name = "Col_Time";
            this.Col_Time.ReadOnly = true;
            this.Col_Time.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Col_Time.Width = 180;
            // 
            // Col_Level
            // 
            this.Col_Level.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Col_Level.HeaderText = "Level";
            this.Col_Level.Name = "Col_Level";
            this.Col_Level.ReadOnly = true;
            this.Col_Level.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Col_Level.Width = 95;
            // 
            // Col_Message
            // 
            this.Col_Message.HeaderText = "Message";
            this.Col_Message.MinimumWidth = 180;
            this.Col_Message.Name = "Col_Message";
            this.Col_Message.ReadOnly = true;
            this.Col_Message.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Pnl_Detail
            // 
            this.Pnl_Detail.ColumnCount = 1;
            this.Pnl_Detail.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Pnl_Detail.Controls.Add(this.Lbl_Detail, 0, 0);
            this.Pnl_Detail.Controls.Add(this.TxtBx_Detail, 0, 1);
            this.Pnl_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Pnl_Detail.Location = new System.Drawing.Point(0, 0);
            this.Pnl_Detail.Name = "Pnl_Detail";
            this.Pnl_Detail.RowCount = 2;
            this.Pnl_Detail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.Pnl_Detail.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Pnl_Detail.Size = new System.Drawing.Size(1084, 180);
            this.Pnl_Detail.TabIndex = 0;
            // 
            // Lbl_Detail
            // 
            this.Lbl_Detail.AutoSize = true;
            this.Lbl_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Lbl_Detail.Location = new System.Drawing.Point(3, 0);
            this.Lbl_Detail.Name = "Lbl_Detail";
            this.Lbl_Detail.Padding = new System.Windows.Forms.Padding(4, 4, 0, 0);
            this.Lbl_Detail.Size = new System.Drawing.Size(1078, 26);
            this.Lbl_Detail.TabIndex = 0;
            this.Lbl_Detail.Text = "Selected log detail";
            // 
            // TxtBx_Detail
            // 
            this.TxtBx_Detail.BackColor = System.Drawing.Color.White;
            this.TxtBx_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtBx_Detail.Font = new System.Drawing.Font("Consolas", 10F);
            this.TxtBx_Detail.Location = new System.Drawing.Point(3, 29);
            this.TxtBx_Detail.Multiline = true;
            this.TxtBx_Detail.Name = "TxtBx_Detail";
            this.TxtBx_Detail.ReadOnly = true;
            this.TxtBx_Detail.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtBx_Detail.Size = new System.Drawing.Size(1078, 148);
            this.TxtBx_Detail.TabIndex = 1;
            this.TxtBx_Detail.WordWrap = false;
            // 
            // Lbl_Status
            // 
            this.Lbl_Status.AutoEllipsis = true;
            this.Lbl_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Lbl_Status.Location = new System.Drawing.Point(3, 672);
            this.Lbl_Status.Name = "Lbl_Status";
            this.Lbl_Status.Padding = new System.Windows.Forms.Padding(10, 3, 0, 0);
            this.Lbl_Status.Size = new System.Drawing.Size(1094, 28);
            this.Lbl_Status.TabIndex = 2;
            this.Lbl_Status.Text = "Ready";
            // 
            // LiveTimer
            // 
            this.LiveTimer.Tick += new System.EventHandler(this.LiveTimer_Tick);
            // 
            // FileTimer
            // 
            this.FileTimer.Interval = 3000;
            this.FileTimer.Tick += new System.EventHandler(this.FileTimer_Tick);
            // 
            // F_HistoryReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.MainLayout);
            this.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F);
            this.MinimumSize = new System.Drawing.Size(850, 480);
            this.Name = "F_HistoryReader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "History Log";
            this.MainLayout.ResumeLayout(false);
            this.Toolbar.ResumeLayout(false);
            this.Toolbar.PerformLayout();
            this.Split_Log.Panel1.ResumeLayout(false);
            this.Split_Log.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Split_Log)).EndInit();
            this.Split_Log.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_log)).EndInit();
            this.Pnl_Detail.ResumeLayout(false);
            this.Pnl_Detail.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TableLayoutPanel MainLayout;
        private System.Windows.Forms.FlowLayoutPanel Toolbar;
        private System.Windows.Forms.Label Lbl_Source;
        private System.Windows.Forms.ComboBox Cmbx_Source;
        private System.Windows.Forms.Button Btn_Refresh;
        private System.Windows.Forms.CheckBox Chk_Auto;
        private System.Windows.Forms.Label Lbl_Filter;
        private System.Windows.Forms.ComboBox Cmbx_Filter;
        private System.Windows.Forms.CheckBox Chk_Follow;
        private System.Windows.Forms.SplitContainer Split_Log;
        private System.Windows.Forms.DataGridView DGV_log;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Level;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Message;
        private System.Windows.Forms.TableLayoutPanel Pnl_Detail;
        private System.Windows.Forms.Label Lbl_Detail;
        private System.Windows.Forms.TextBox TxtBx_Detail;
        private System.Windows.Forms.Label Lbl_Status;
        private System.Windows.Forms.Timer LiveTimer;
        private System.Windows.Forms.Timer FileTimer;
    }
}
