namespace BurnInTester.UI
{
    partial class F_VirtualTemperature
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.targetHeader = new System.Windows.Forms.Label();
            this.settingsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.targetLabel = new System.Windows.Forms.Label();
            this.Cmbx_BoxCh = new System.Windows.Forms.ComboBox();
            this.modeLabel = new System.Windows.Forms.Label();
            this.CkBx_Manual = new System.Windows.Forms.CheckBox();
            this.modeHint = new System.Windows.Forms.Label();
            this.temperatureHeader = new System.Windows.Forms.Label();
            this.temperatureLayout = new System.Windows.Forms.TableLayoutPanel();
            this.TrackBar_Temperature = new System.Windows.Forms.TrackBar();
            this.rangeLayout = new System.Windows.Forms.TableLayoutPanel();
            this.minimumLabel = new System.Windows.Forms.Label();
            this.maximumLabel = new System.Windows.Forms.Label();
            this.temperatureLabel = new System.Windows.Forms.Label();
            this.numericRow = new System.Windows.Forms.FlowLayoutPanel();
            this.NumUpDn_Temperature = new System.Windows.Forms.NumericUpDown();
            this.valueLabel = new System.Windows.Forms.Label();
            this.buttonRow = new System.Windows.Forms.FlowLayoutPanel();
            this.Btn_Reset = new System.Windows.Forms.Button();
            this.Btn_ApplyAll = new System.Windows.Forms.Button();
            this.layout.SuspendLayout();
            this.settingsLayout.SuspendLayout();
            this.temperatureLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar_Temperature)).BeginInit();
            this.rangeLayout.SuspendLayout();
            this.numericRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumUpDn_Temperature)).BeginInit();
            this.buttonRow.SuspendLayout();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.layout.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.targetHeader, 0, 0);
            this.layout.Controls.Add(this.settingsLayout, 0, 1);
            this.layout.Controls.Add(this.temperatureHeader, 0, 2);
            this.layout.Controls.Add(this.temperatureLayout, 0, 3);
            this.layout.Controls.Add(this.buttonRow, 0, 4);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Name = "layout";
            this.layout.Padding = new System.Windows.Forms.Padding(16);
            this.layout.RowCount = 5;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 116F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.layout.Size = new System.Drawing.Size(682, 425);
            this.layout.TabIndex = 0;
            // 
            // targetHeader
            // 
            this.targetHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.targetHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.targetHeader.ForeColor = System.Drawing.Color.Gold;
            this.targetHeader.Location = new System.Drawing.Point(17, 17);
            this.targetHeader.Margin = new System.Windows.Forms.Padding(0);
            this.targetHeader.Name = "targetHeader";
            this.targetHeader.Size = new System.Drawing.Size(648, 36);
            this.targetHeader.TabIndex = 1;
            this.targetHeader.Text = "Channel and Mode";
            this.targetHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // settingsLayout
            // 
            this.settingsLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.settingsLayout.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.settingsLayout.ColumnCount = 2;
            this.settingsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.settingsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.settingsLayout.Controls.Add(this.targetLabel, 0, 0);
            this.settingsLayout.Controls.Add(this.Cmbx_BoxCh, 1, 0);
            this.settingsLayout.Controls.Add(this.modeLabel, 0, 1);
            this.settingsLayout.Controls.Add(this.CkBx_Manual, 1, 1);
            this.settingsLayout.Controls.Add(this.modeHint, 0, 2);
            this.settingsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsLayout.Location = new System.Drawing.Point(17, 54);
            this.settingsLayout.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.settingsLayout.Name = "settingsLayout";
            this.settingsLayout.RowCount = 3;
            this.settingsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.settingsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.settingsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.settingsLayout.Size = new System.Drawing.Size(648, 104);
            this.settingsLayout.TabIndex = 2;
            // 
            // targetLabel
            // 
            this.targetLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.targetLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.targetLabel.ForeColor = System.Drawing.Color.Gold;
            this.targetLabel.Location = new System.Drawing.Point(4, 1);
            this.targetLabel.Name = "targetLabel";
            this.targetLabel.Size = new System.Drawing.Size(134, 38);
            this.targetLabel.TabIndex = 3;
            this.targetLabel.Text = "Box / Channel";
            this.targetLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Cmbx_BoxCh
            // 
            this.Cmbx_BoxCh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.Cmbx_BoxCh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Cmbx_BoxCh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Cmbx_BoxCh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cmbx_BoxCh.ForeColor = System.Drawing.Color.Gold;
            this.Cmbx_BoxCh.FormattingEnabled = true;
            this.Cmbx_BoxCh.Location = new System.Drawing.Point(150, 6);
            this.Cmbx_BoxCh.Margin = new System.Windows.Forms.Padding(8, 5, 8, 3);
            this.Cmbx_BoxCh.Name = "Cmbx_BoxCh";
            this.Cmbx_BoxCh.Size = new System.Drawing.Size(489, 28);
            this.Cmbx_BoxCh.TabIndex = 4;
            this.Cmbx_BoxCh.SelectedIndexChanged += new System.EventHandler(this.Cmbx_BoxCh_SelectedIndexChanged);
            // 
            // modeLabel
            // 
            this.modeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.modeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modeLabel.ForeColor = System.Drawing.Color.Gold;
            this.modeLabel.Location = new System.Drawing.Point(4, 40);
            this.modeLabel.Name = "modeLabel";
            this.modeLabel.Size = new System.Drawing.Size(134, 34);
            this.modeLabel.TabIndex = 5;
            this.modeLabel.Text = "Mode";
            this.modeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CkBx_Manual
            // 
            this.CkBx_Manual.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CkBx_Manual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CkBx_Manual.ForeColor = System.Drawing.Color.Gold;
            this.CkBx_Manual.Location = new System.Drawing.Point(150, 40);
            this.CkBx_Manual.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.CkBx_Manual.Name = "CkBx_Manual";
            this.CkBx_Manual.Size = new System.Drawing.Size(489, 34);
            this.CkBx_Manual.TabIndex = 6;
            this.CkBx_Manual.Text = "Manual Simulation";
            this.CkBx_Manual.UseVisualStyleBackColor = false;
            this.CkBx_Manual.CheckedChanged += new System.EventHandler(this.CkBx_Manual_CheckedChanged);
            // 
            // modeHint
            // 
            this.settingsLayout.SetColumnSpan(this.modeHint, 2);
            this.modeHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modeHint.Font = new System.Drawing.Font("微軟正黑體", 10F);
            this.modeHint.ForeColor = System.Drawing.Color.Gold;
            this.modeHint.Location = new System.Drawing.Point(4, 75);
            this.modeHint.Name = "modeHint";
            this.modeHint.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.modeHint.Size = new System.Drawing.Size(640, 28);
            this.modeHint.TabIndex = 7;
            this.modeHint.Text = "When enabled, Start / Stop will not override the manual value.";
            this.modeHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // temperatureHeader
            // 
            this.temperatureHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.temperatureHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.temperatureHeader.ForeColor = System.Drawing.Color.Gold;
            this.temperatureHeader.Location = new System.Drawing.Point(17, 171);
            this.temperatureHeader.Margin = new System.Windows.Forms.Padding(0);
            this.temperatureHeader.Name = "temperatureHeader";
            this.temperatureHeader.Size = new System.Drawing.Size(648, 36);
            this.temperatureHeader.TabIndex = 8;
            this.temperatureHeader.Text = "Manual Temperature";
            this.temperatureHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // temperatureLayout
            // 
            this.temperatureLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.temperatureLayout.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.temperatureLayout.ColumnCount = 2;
            this.temperatureLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.temperatureLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.temperatureLayout.Controls.Add(this.TrackBar_Temperature, 0, 0);
            this.temperatureLayout.Controls.Add(this.rangeLayout, 0, 1);
            this.temperatureLayout.Controls.Add(this.temperatureLabel, 0, 2);
            this.temperatureLayout.Controls.Add(this.numericRow, 1, 2);
            this.temperatureLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.temperatureLayout.Location = new System.Drawing.Point(17, 208);
            this.temperatureLayout.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.temperatureLayout.Name = "temperatureLayout";
            this.temperatureLayout.RowCount = 3;
            this.temperatureLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.temperatureLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.temperatureLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.temperatureLayout.Size = new System.Drawing.Size(648, 132);
            this.temperatureLayout.TabIndex = 9;
            // 
            // TrackBar_Temperature
            // 
            this.TrackBar_Temperature.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TrackBar_Temperature.AutoSize = false;
            this.TrackBar_Temperature.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.temperatureLayout.SetColumnSpan(this.TrackBar_Temperature, 2);
            this.TrackBar_Temperature.Enabled = false;
            this.TrackBar_Temperature.LargeChange = 100;
            this.TrackBar_Temperature.Location = new System.Drawing.Point(13, 9);
            this.TrackBar_Temperature.Margin = new System.Windows.Forms.Padding(12, 4, 12, 0);
            this.TrackBar_Temperature.Maximum = 1500;
            this.TrackBar_Temperature.Name = "TrackBar_Temperature";
            this.TrackBar_Temperature.Size = new System.Drawing.Size(622, 45);
            this.TrackBar_Temperature.TabIndex = 10;
            this.TrackBar_Temperature.TickFrequency = 100;
            this.TrackBar_Temperature.Value = 250;
            this.TrackBar_Temperature.ValueChanged += new System.EventHandler(this.TrackBar_Temperature_ValueChanged);
            // 
            // rangeLayout
            // 
            this.rangeLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.rangeLayout.ColumnCount = 2;
            this.temperatureLayout.SetColumnSpan(this.rangeLayout, 2);
            this.rangeLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rangeLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rangeLayout.Controls.Add(this.minimumLabel, 0, 0);
            this.rangeLayout.Controls.Add(this.maximumLabel, 1, 0);
            this.rangeLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rangeLayout.Location = new System.Drawing.Point(17, 60);
            this.rangeLayout.Margin = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.rangeLayout.Name = "rangeLayout";
            this.rangeLayout.RowCount = 1;
            this.rangeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rangeLayout.Size = new System.Drawing.Size(614, 24);
            this.rangeLayout.TabIndex = 11;
            // 
            // minimumLabel
            // 
            this.minimumLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.minimumLabel.Font = new System.Drawing.Font("微軟正黑體", 10F);
            this.minimumLabel.ForeColor = System.Drawing.Color.Gold;
            this.minimumLabel.Location = new System.Drawing.Point(3, 0);
            this.minimumLabel.Name = "minimumLabel";
            this.minimumLabel.Size = new System.Drawing.Size(301, 24);
            this.minimumLabel.TabIndex = 12;
            this.minimumLabel.Text = "0°C";
            this.minimumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // maximumLabel
            // 
            this.maximumLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maximumLabel.Font = new System.Drawing.Font("微軟正黑體", 10F);
            this.maximumLabel.ForeColor = System.Drawing.Color.Gold;
            this.maximumLabel.Location = new System.Drawing.Point(310, 0);
            this.maximumLabel.Name = "maximumLabel";
            this.maximumLabel.Size = new System.Drawing.Size(301, 24);
            this.maximumLabel.TabIndex = 13;
            this.maximumLabel.Text = "150°C";
            this.maximumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // temperatureLabel
            // 
            this.temperatureLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.temperatureLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.temperatureLabel.ForeColor = System.Drawing.Color.Gold;
            this.temperatureLabel.Location = new System.Drawing.Point(4, 85);
            this.temperatureLabel.Name = "temperatureLabel";
            this.temperatureLabel.Size = new System.Drawing.Size(134, 46);
            this.temperatureLabel.TabIndex = 14;
            this.temperatureLabel.Text = "Manual Value";
            this.temperatureLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericRow
            // 
            this.numericRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericRow.Controls.Add(this.NumUpDn_Temperature);
            this.numericRow.Controls.Add(this.valueLabel);
            this.numericRow.Location = new System.Drawing.Point(150, 85);
            this.numericRow.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.numericRow.Name = "numericRow";
            this.numericRow.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.numericRow.Size = new System.Drawing.Size(497, 46);
            this.numericRow.TabIndex = 15;
            this.numericRow.WrapContents = false;
            // 
            // NumUpDn_Temperature
            // 
            this.NumUpDn_Temperature.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.NumUpDn_Temperature.DecimalPlaces = 1;
            this.NumUpDn_Temperature.Enabled = false;
            this.NumUpDn_Temperature.Font = new System.Drawing.Font("微軟正黑體", 18F);
            this.NumUpDn_Temperature.ForeColor = System.Drawing.Color.Gold;
            this.NumUpDn_Temperature.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NumUpDn_Temperature.Location = new System.Drawing.Point(0, 4);
            this.NumUpDn_Temperature.Margin = new System.Windows.Forms.Padding(0);
            this.NumUpDn_Temperature.Maximum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.NumUpDn_Temperature.Name = "NumUpDn_Temperature";
            this.NumUpDn_Temperature.Size = new System.Drawing.Size(150, 39);
            this.NumUpDn_Temperature.TabIndex = 16;
            this.NumUpDn_Temperature.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NumUpDn_Temperature.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.NumUpDn_Temperature.ValueChanged += new System.EventHandler(this.NumUpDn_Temperature_ValueChanged);
            // 
            // valueLabel
            // 
            this.valueLabel.ForeColor = System.Drawing.Color.Gold;
            this.valueLabel.Location = new System.Drawing.Point(160, 4);
            this.valueLabel.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.valueLabel.Name = "valueLabel";
            this.valueLabel.Size = new System.Drawing.Size(50, 39);
            this.valueLabel.TabIndex = 17;
            this.valueLabel.Text = "°C";
            this.valueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonRow
            // 
            this.buttonRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.buttonRow.Controls.Add(this.Btn_Reset);
            this.buttonRow.Controls.Add(this.Btn_ApplyAll);
            this.buttonRow.Location = new System.Drawing.Point(17, 349);
            this.buttonRow.Margin = new System.Windows.Forms.Padding(0);
            this.buttonRow.Name = "buttonRow";
            this.buttonRow.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.buttonRow.Size = new System.Drawing.Size(648, 54);
            this.buttonRow.TabIndex = 18;
            this.buttonRow.WrapContents = false;
            // 
            // Btn_Reset
            // 
            this.Btn_Reset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Btn_Reset.FlatAppearance.BorderColor = System.Drawing.Color.Gold;
            this.Btn_Reset.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.Btn_Reset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.Btn_Reset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_Reset.ForeColor = System.Drawing.Color.Gold;
            this.Btn_Reset.Location = new System.Drawing.Point(3, 6);
            this.Btn_Reset.Margin = new System.Windows.Forms.Padding(3, 0, 12, 0);
            this.Btn_Reset.Name = "Btn_Reset";
            this.Btn_Reset.Size = new System.Drawing.Size(180, 42);
            this.Btn_Reset.TabIndex = 19;
            this.Btn_Reset.Text = "Reset to 25°C";
            this.Btn_Reset.UseVisualStyleBackColor = false;
            this.Btn_Reset.Click += new System.EventHandler(this.Btn_Reset_Click);
            // 
            // Btn_ApplyAll
            // 
            this.Btn_ApplyAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Btn_ApplyAll.FlatAppearance.BorderColor = System.Drawing.Color.Gold;
            this.Btn_ApplyAll.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.Btn_ApplyAll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.Btn_ApplyAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_ApplyAll.ForeColor = System.Drawing.Color.Gold;
            this.Btn_ApplyAll.Location = new System.Drawing.Point(195, 6);
            this.Btn_ApplyAll.Margin = new System.Windows.Forms.Padding(0);
            this.Btn_ApplyAll.Name = "Btn_ApplyAll";
            this.Btn_ApplyAll.Size = new System.Drawing.Size(240, 42);
            this.Btn_ApplyAll.TabIndex = 20;
            this.Btn_ApplyAll.Text = "Apply to All Active Channels";
            this.Btn_ApplyAll.UseVisualStyleBackColor = false;
            this.Btn_ApplyAll.Click += new System.EventHandler(this.Btn_ApplyAll_Click);
            // 
            // F_VirtualTemperature
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(682, 425);
            this.Controls.Add(this.layout);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "F_VirtualTemperature";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Virtual Temperature Test";
            this.VisibleChanged += new System.EventHandler(this.F_VirtualTemperature_VisibleChanged);
            this.layout.ResumeLayout(false);
            this.settingsLayout.ResumeLayout(false);
            this.temperatureLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar_Temperature)).EndInit();
            this.rangeLayout.ResumeLayout(false);
            this.numericRow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NumUpDn_Temperature)).EndInit();
            this.buttonRow.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Label targetHeader;
        private System.Windows.Forms.TableLayoutPanel settingsLayout;
        private System.Windows.Forms.Label targetLabel;
        private System.Windows.Forms.ComboBox Cmbx_BoxCh;
        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.CheckBox CkBx_Manual;
        private System.Windows.Forms.Label modeHint;
        private System.Windows.Forms.Label temperatureHeader;
        private System.Windows.Forms.TableLayoutPanel temperatureLayout;
        private System.Windows.Forms.TrackBar TrackBar_Temperature;
        private System.Windows.Forms.TableLayoutPanel rangeLayout;
        private System.Windows.Forms.Label minimumLabel;
        private System.Windows.Forms.Label maximumLabel;
        private System.Windows.Forms.Label temperatureLabel;
        private System.Windows.Forms.FlowLayoutPanel numericRow;
        private System.Windows.Forms.NumericUpDown NumUpDn_Temperature;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.FlowLayoutPanel buttonRow;
        private System.Windows.Forms.Button Btn_Reset;
        private System.Windows.Forms.Button Btn_ApplyAll;
    }
}
