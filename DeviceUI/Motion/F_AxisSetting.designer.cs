
namespace DeviceUI.Motion
{
    partial class F_AxisSetting
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.Btn_AllSetting = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.Cmbx_AxisUse = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.Cmbx_AxisType = new System.Windows.Forms.ComboBox();
            this.TxtBx_AxisStation = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.Cmbx_AxisLimitLogic = new System.Windows.Forms.ComboBox();
            this.Cmbx_AxisLimitStopMode = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label13 = new System.Windows.Forms.Label();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtBx_AxisName = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1182, 557);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.tabPage1.Controls.Add(this.Btn_AllSetting);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1174, 524);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Parameter";
            // 
            // Btn_AllSetting
            // 
            this.Btn_AllSetting.Location = new System.Drawing.Point(1024, 479);
            this.Btn_AllSetting.Name = "Btn_AllSetting";
            this.Btn_AllSetting.Size = new System.Drawing.Size(144, 39);
            this.Btn_AllSetting.TabIndex = 2;
            this.Btn_AllSetting.Text = "All Setting";
            this.Btn_AllSetting.UseVisualStyleBackColor = true;
            this.Btn_AllSetting.Click += new System.EventHandler(this.Btn_AllSetting_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox1.Location = new System.Drawing.Point(9, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(374, 356);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Axis Configuration";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60.59783F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 39.40217F));
            this.tableLayoutPanel2.Controls.Add(this.label11, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.Cmbx_AxisUse, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label10, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.Cmbx_AxisType, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.TxtBx_AxisStation, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label12, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.Cmbx_AxisLimitLogic, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.Cmbx_AxisLimitStopMode, 1, 4);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 25);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 7;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(368, 328);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(6, 117);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(185, 35);
            this.label11.TabIndex = 6;
            this.label11.Text = "Limit Logic";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Cmbx_AxisUse
            // 
            this.Cmbx_AxisUse.FormattingEnabled = true;
            this.Cmbx_AxisUse.Items.AddRange(new object[] {
            "NO USE",
            "USE"});
            this.Cmbx_AxisUse.Location = new System.Drawing.Point(226, 82);
            this.Cmbx_AxisUse.Name = "Cmbx_AxisUse";
            this.Cmbx_AxisUse.Size = new System.Drawing.Size(136, 28);
            this.Cmbx_AxisUse.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(6, 79);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(185, 35);
            this.label10.TabIndex = 4;
            this.label10.Text = "Use";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(6, 41);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(185, 35);
            this.label9.TabIndex = 3;
            this.label9.Text = "Station";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Cmbx_AxisType
            // 
            this.Cmbx_AxisType.FormattingEnabled = true;
            this.Cmbx_AxisType.Items.AddRange(new object[] {
            "APS"});
            this.Cmbx_AxisType.Location = new System.Drawing.Point(226, 6);
            this.Cmbx_AxisType.Name = "Cmbx_AxisType";
            this.Cmbx_AxisType.Size = new System.Drawing.Size(136, 28);
            this.Cmbx_AxisType.TabIndex = 0;
            // 
            // TxtBx_AxisStation
            // 
            this.TxtBx_AxisStation.Location = new System.Drawing.Point(226, 44);
            this.TxtBx_AxisStation.Name = "TxtBx_AxisStation";
            this.TxtBx_AxisStation.Size = new System.Drawing.Size(136, 29);
            this.TxtBx_AxisStation.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(6, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(185, 35);
            this.label8.TabIndex = 2;
            this.label8.Text = "Type";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(6, 155);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(185, 35);
            this.label12.TabIndex = 7;
            this.label12.Text = "Limit Stop Mode";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Cmbx_AxisLimitLogic
            // 
            this.Cmbx_AxisLimitLogic.FormattingEnabled = true;
            this.Cmbx_AxisLimitLogic.Items.AddRange(new object[] {
            "Active Low",
            "Active Hight"});
            this.Cmbx_AxisLimitLogic.Location = new System.Drawing.Point(226, 120);
            this.Cmbx_AxisLimitLogic.Name = "Cmbx_AxisLimitLogic";
            this.Cmbx_AxisLimitLogic.Size = new System.Drawing.Size(136, 28);
            this.Cmbx_AxisLimitLogic.TabIndex = 8;
            // 
            // Cmbx_AxisLimitStopMode
            // 
            this.Cmbx_AxisLimitStopMode.FormattingEnabled = true;
            this.Cmbx_AxisLimitStopMode.Items.AddRange(new object[] {
            "Immediate",
            "Decelerated"});
            this.Cmbx_AxisLimitStopMode.Location = new System.Drawing.Point(226, 158);
            this.Cmbx_AxisLimitStopMode.Name = "Cmbx_AxisLimitStopMode";
            this.Cmbx_AxisLimitStopMode.Size = new System.Drawing.Size(136, 28);
            this.Cmbx_AxisLimitStopMode.TabIndex = 9;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.groupBox2.Controls.Add(this.tableLayoutPanel3);
            this.groupBox2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox2.Location = new System.Drawing.Point(426, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(397, 356);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Software Configuration";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.85994F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.14006F));
            this.tableLayoutPanel3.Controls.Add(this.label13, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.comboBox4, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.textBox2, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.label14, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.label15, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.textBox1, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.comboBox3, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label16, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.TxtBx_AxisName, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 25);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 7;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(391, 328);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(6, 155);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(180, 35);
            this.label13.TabIndex = 6;
            this.label13.Text = "Reverse Move";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBox4
            // 
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Items.AddRange(new object[] {
            "NO USE",
            "USE"});
            this.comboBox4.Location = new System.Drawing.Point(195, 158);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(190, 28);
            this.comboBox4.TabIndex = 8;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(195, 120);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(190, 29);
            this.textBox2.TabIndex = 9;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(6, 117);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(180, 35);
            this.label14.TabIndex = 4;
            this.label14.Text = "MEL Position(mm)";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(6, 79);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(180, 35);
            this.label15.TabIndex = 3;
            this.label15.Text = "PEL Position(mm)";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(195, 82);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(190, 29);
            this.textBox1.TabIndex = 1;
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "No USE",
            "USE"});
            this.comboBox3.Location = new System.Drawing.Point(195, 44);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(190, 28);
            this.comboBox3.TabIndex = 0;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(6, 41);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(180, 35);
            this.label16.TabIndex = 2;
            this.label16.Text = "USE Software Limit";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 35);
            this.label1.TabIndex = 10;
            this.label1.Text = "Axis Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TxtBx_AxisName
            // 
            this.TxtBx_AxisName.Location = new System.Drawing.Point(195, 6);
            this.TxtBx_AxisName.Name = "TxtBx_AxisName";
            this.TxtBx_AxisName.Size = new System.Drawing.Size(190, 29);
            this.TxtBx_AxisName.TabIndex = 11;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1174, 524);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Speed";
            // 
            // F_AxisSetting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1386, 649);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_AxisSetting";
            this.Text = "F_StartForm_ButtonGroup";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox Cmbx_AxisUse;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox Cmbx_AxisType;
        private System.Windows.Forms.TextBox TxtBx_AxisStation;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox Cmbx_AxisLimitLogic;
        private System.Windows.Forms.ComboBox Cmbx_AxisLimitStopMode;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtBx_AxisName;
        private System.Windows.Forms.Button Btn_AllSetting;
    }
}