namespace RGBTester.UI
{
    partial class F_HistoryLog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Lbl_Title = new System.Windows.Forms.Label();
            this.Cmbx_LogFile = new System.Windows.Forms.ComboBox();
            this.Btn_Refresh = new System.Windows.Forms.Button();
            this.Chk_AutoRefresh = new System.Windows.Forms.CheckBox();
            this.Rtxt_Log = new System.Windows.Forms.RichTextBox();
            this.RefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // Lbl_Title
            // 
            this.Lbl_Title.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Lbl_Title.Location = new System.Drawing.Point(20, 15);
            this.Lbl_Title.Name = "Lbl_Title";
            this.Lbl_Title.Size = new System.Drawing.Size(180, 36);
            this.Lbl_Title.TabIndex = 0;
            this.Lbl_Title.Text = "History Log";
            this.Lbl_Title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Cmbx_LogFile
            // 
            this.Cmbx_LogFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmbx_LogFile.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Cmbx_LogFile.FormattingEnabled = true;
            this.Cmbx_LogFile.Location = new System.Drawing.Point(205, 20);
            this.Cmbx_LogFile.Name = "Cmbx_LogFile";
            this.Cmbx_LogFile.Size = new System.Drawing.Size(520, 25);
            this.Cmbx_LogFile.TabIndex = 1;
            this.Cmbx_LogFile.SelectedIndexChanged += new System.EventHandler(this.Cmbx_LogFile_SelectedIndexChanged);
            // 
            // Btn_Refresh
            // 
            this.Btn_Refresh.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Refresh.Location = new System.Drawing.Point(740, 17);
            this.Btn_Refresh.Name = "Btn_Refresh";
            this.Btn_Refresh.Size = new System.Drawing.Size(90, 32);
            this.Btn_Refresh.TabIndex = 2;
            this.Btn_Refresh.Text = "Refresh";
            this.Btn_Refresh.UseVisualStyleBackColor = true;
            this.Btn_Refresh.Click += new System.EventHandler(this.Btn_Refresh_Click);
            // 
            // Chk_AutoRefresh
            // 
            this.Chk_AutoRefresh.AutoSize = true;
            this.Chk_AutoRefresh.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Chk_AutoRefresh.Location = new System.Drawing.Point(850, 22);
            this.Chk_AutoRefresh.Name = "Chk_AutoRefresh";
            this.Chk_AutoRefresh.Size = new System.Drawing.Size(60, 22);
            this.Chk_AutoRefresh.TabIndex = 3;
            this.Chk_AutoRefresh.Text = "Auto";
            this.Chk_AutoRefresh.UseVisualStyleBackColor = true;
            this.Chk_AutoRefresh.CheckedChanged += new System.EventHandler(this.Chk_AutoRefresh_CheckedChanged);
            // 
            // Rtxt_Log
            // 
            this.Rtxt_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Rtxt_Log.BackColor = System.Drawing.Color.White;
            this.Rtxt_Log.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Rtxt_Log.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Rtxt_Log.Location = new System.Drawing.Point(20, 62);
            this.Rtxt_Log.Name = "Rtxt_Log";
            this.Rtxt_Log.ReadOnly = true;
            this.Rtxt_Log.Size = new System.Drawing.Size(1286, 579);
            this.Rtxt_Log.TabIndex = 4;
            this.Rtxt_Log.Text = "";
            this.Rtxt_Log.WordWrap = false;
            // 
            // RefreshTimer
            // 
            this.RefreshTimer.Interval = 3000;
            this.RefreshTimer.Tick += new System.EventHandler(this.RefreshTimer_Tick);
            // 
            // F_HistoryLog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.Rtxt_Log);
            this.Controls.Add(this.Chk_AutoRefresh);
            this.Controls.Add(this.Btn_Refresh);
            this.Controls.Add(this.Cmbx_LogFile);
            this.Controls.Add(this.Lbl_Title);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_HistoryLog";
            this.Text = "F_HistoryLog";
            this.Load += new System.EventHandler(this.F_HistoryLog_Load);
            this.VisibleChanged += new System.EventHandler(this.F_HistoryLog_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Lbl_Title;
        private System.Windows.Forms.ComboBox Cmbx_LogFile;
        private System.Windows.Forms.Button Btn_Refresh;
        private System.Windows.Forms.CheckBox Chk_AutoRefresh;
        private System.Windows.Forms.RichTextBox Rtxt_Log;
        private System.Windows.Forms.Timer RefreshTimer;
    }
}
