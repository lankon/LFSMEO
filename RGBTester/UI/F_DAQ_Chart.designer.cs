
namespace RGBTester.UI
{
    partial class F_DAQ_Chart
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea7 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend7 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.Btn_CaptureData = new System.Windows.Forms.Button();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.TestColor = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TestMode = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.Select_HL_Mode = new System.Windows.Forms.ComboBox();
            this.label46 = new System.Windows.Forms.Label();
            this.DAC_Value = new System.Windows.Forms.TextBox();
            this.Btn_SaveData = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Labl_Result5 = new System.Windows.Forms.Label();
            this.Labl_Result4 = new System.Windows.Forms.Label();
            this.Labl_Result3 = new System.Windows.Forms.Label();
            this.Labl_Result2 = new System.Windows.Forms.Label();
            this.Labl_Result1 = new System.Windows.Forms.Label();
            this.Pnl_ShowSetting = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            chartArea7.AxisX.LabelStyle.ForeColor = System.Drawing.Color.LightGray;
            chartArea7.AxisX.LineColor = System.Drawing.Color.Gray;
            chartArea7.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            chartArea7.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea7.AxisX.MajorTickMark.Enabled = false;
            chartArea7.AxisY.LabelStyle.Enabled = false;
            chartArea7.AxisY.LineColor = System.Drawing.Color.Gray;
            chartArea7.AxisY.MajorGrid.Enabled = false;
            chartArea7.AxisY.MajorTickMark.Enabled = false;
            chartArea7.BackColor = System.Drawing.Color.Black;
            chartArea7.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea7.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea7.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea7);
            legend7.Alignment = System.Drawing.StringAlignment.Far;
            legend7.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend7.Name = "Legend1";
            legend7.TitleAlignment = System.Drawing.StringAlignment.Near;
            this.chart1.Legends.Add(legend7);
            this.chart1.Location = new System.Drawing.Point(12, 12);
            this.chart1.Name = "chart1";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Color = System.Drawing.Color.Red;
            series7.Legend = "Legend1";
            series7.MarkerBorderColor = System.Drawing.Color.White;
            series7.Name = "Series1";
            this.chart1.Series.Add(series7);
            this.chart1.Size = new System.Drawing.Size(1302, 599);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // Btn_CaptureData
            // 
            this.Btn_CaptureData.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_CaptureData.Location = new System.Drawing.Point(1004, 618);
            this.Btn_CaptureData.Name = "Btn_CaptureData";
            this.Btn_CaptureData.Size = new System.Drawing.Size(152, 36);
            this.Btn_CaptureData.TabIndex = 1;
            this.Btn_CaptureData.Text = "Capture";
            this.Btn_CaptureData.UseVisualStyleBackColor = true;
            this.Btn_CaptureData.Click += new System.EventHandler(this.Btn_CaptureData_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel5.ColumnCount = 8;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.tableLayoutPanel5.Controls.Add(this.label2, 6, 0);
            this.tableLayoutPanel5.Controls.Add(this.TestColor, 3, 0);
            this.tableLayoutPanel5.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.TestMode, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.label15, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.Select_HL_Mode, 5, 0);
            this.tableLayoutPanel5.Controls.Add(this.label46, 4, 0);
            this.tableLayoutPanel5.Controls.Add(this.DAC_Value, 7, 0);
            this.tableLayoutPanel5.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tableLayoutPanel5.Location = new System.Drawing.Point(12, 617);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(986, 40);
            this.tableLayoutPanel5.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(738, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 30);
            this.label2.TabIndex = 10;
            this.label2.Text = "DAC Value";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TestColor
            // 
            this.TestColor.FormattingEnabled = true;
            this.TestColor.Items.AddRange(new object[] {
            "Red",
            "Green",
            "Blue",
            "Blue2"});
            this.TestColor.Location = new System.Drawing.Point(412, 6);
            this.TestColor.Name = "TestColor";
            this.TestColor.Size = new System.Drawing.Size(73, 28);
            this.TestColor.TabIndex = 9;
            this.TestColor.Text = "Red";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(250, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 35);
            this.label1.TabIndex = 8;
            this.label1.Text = "Test Color";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TestMode
            // 
            this.TestMode.FormattingEnabled = true;
            this.TestMode.Items.AddRange(new object[] {
            "Left",
            "Right"});
            this.TestMode.Location = new System.Drawing.Point(168, 6);
            this.TestMode.Name = "TestMode";
            this.TestMode.Size = new System.Drawing.Size(73, 28);
            this.TestMode.TabIndex = 5;
            this.TestMode.Text = "Left";
            // 
            // label15
            // 
            this.label15.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label15.Location = new System.Drawing.Point(6, 3);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(132, 30);
            this.label15.TabIndex = 4;
            this.label15.Text = "Test Mode";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Select_HL_Mode
            // 
            this.Select_HL_Mode.FormattingEnabled = true;
            this.Select_HL_Mode.Items.AddRange(new object[] {
            "HCM",
            "LCM"});
            this.Select_HL_Mode.Location = new System.Drawing.Point(656, 6);
            this.Select_HL_Mode.Name = "Select_HL_Mode";
            this.Select_HL_Mode.Size = new System.Drawing.Size(73, 28);
            this.Select_HL_Mode.TabIndex = 7;
            this.Select_HL_Mode.Text = "HCM";
            // 
            // label46
            // 
            this.label46.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label46.Location = new System.Drawing.Point(494, 3);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(132, 30);
            this.label46.TabIndex = 6;
            this.label46.Text = "High/Low Mode";
            this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DAC_Value
            // 
            this.DAC_Value.Location = new System.Drawing.Point(900, 6);
            this.DAC_Value.Name = "DAC_Value";
            this.DAC_Value.Size = new System.Drawing.Size(80, 29);
            this.DAC_Value.TabIndex = 11;
            // 
            // Btn_SaveData
            // 
            this.Btn_SaveData.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_SaveData.Location = new System.Drawing.Point(1162, 618);
            this.Btn_SaveData.Name = "Btn_SaveData";
            this.Btn_SaveData.Size = new System.Drawing.Size(152, 36);
            this.Btn_SaveData.TabIndex = 6;
            this.Btn_SaveData.Text = "Save Picture";
            this.Btn_SaveData.UseVisualStyleBackColor = true;
            this.Btn_SaveData.Click += new System.EventHandler(this.Btn_SaveData_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.Labl_Result5, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.Labl_Result4, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.Labl_Result3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.Labl_Result2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.Labl_Result1, 0, 0);
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(937, 58);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(330, 18);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // Labl_Result5
            // 
            this.Labl_Result5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Labl_Result5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Labl_Result5.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Labl_Result5.ForeColor = System.Drawing.Color.White;
            this.Labl_Result5.Location = new System.Drawing.Point(267, 0);
            this.Labl_Result5.Name = "Labl_Result5";
            this.Labl_Result5.Size = new System.Drawing.Size(60, 18);
            this.Labl_Result5.TabIndex = 4;
            this.Labl_Result5.Text = "0.000";
            this.Labl_Result5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_Result4
            // 
            this.Labl_Result4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Labl_Result4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Labl_Result4.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Labl_Result4.ForeColor = System.Drawing.Color.White;
            this.Labl_Result4.Location = new System.Drawing.Point(201, 0);
            this.Labl_Result4.Name = "Labl_Result4";
            this.Labl_Result4.Size = new System.Drawing.Size(60, 18);
            this.Labl_Result4.TabIndex = 3;
            this.Labl_Result4.Text = "0.000";
            this.Labl_Result4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_Result3
            // 
            this.Labl_Result3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Labl_Result3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Labl_Result3.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Labl_Result3.ForeColor = System.Drawing.Color.White;
            this.Labl_Result3.Location = new System.Drawing.Point(135, 0);
            this.Labl_Result3.Name = "Labl_Result3";
            this.Labl_Result3.Size = new System.Drawing.Size(60, 18);
            this.Labl_Result3.TabIndex = 2;
            this.Labl_Result3.Text = "0.000";
            this.Labl_Result3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_Result2
            // 
            this.Labl_Result2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Labl_Result2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Labl_Result2.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Labl_Result2.ForeColor = System.Drawing.Color.White;
            this.Labl_Result2.Location = new System.Drawing.Point(69, 0);
            this.Labl_Result2.Name = "Labl_Result2";
            this.Labl_Result2.Size = new System.Drawing.Size(60, 18);
            this.Labl_Result2.TabIndex = 1;
            this.Labl_Result2.Text = "0.000";
            this.Labl_Result2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_Result1
            // 
            this.Labl_Result1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Labl_Result1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Labl_Result1.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Labl_Result1.ForeColor = System.Drawing.Color.White;
            this.Labl_Result1.Location = new System.Drawing.Point(3, 0);
            this.Labl_Result1.Name = "Labl_Result1";
            this.Labl_Result1.Size = new System.Drawing.Size(60, 18);
            this.Labl_Result1.TabIndex = 0;
            this.Labl_Result1.Text = "0.000";
            this.Labl_Result1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Pnl_ShowSetting
            // 
            this.Pnl_ShowSetting.BackColor = System.Drawing.Color.Transparent;
            this.Pnl_ShowSetting.Location = new System.Drawing.Point(921, 18);
            this.Pnl_ShowSetting.Name = "Pnl_ShowSetting";
            this.Pnl_ShowSetting.Size = new System.Drawing.Size(358, 51);
            this.Pnl_ShowSetting.TabIndex = 8;
            this.Pnl_ShowSetting.Click += new System.EventHandler(this.Pnl_ShowSetting_Click);
            // 
            // F_DAQ_Chart
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.Pnl_ShowSetting);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.Btn_SaveData);
            this.Controls.Add(this.tableLayoutPanel5);
            this.Controls.Add(this.Btn_CaptureData);
            this.Controls.Add(this.chart1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_DAQ_Chart";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button Btn_CaptureData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.ComboBox TestMode;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.ComboBox TestColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox Select_HL_Mode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DAC_Value;
        private System.Windows.Forms.Button Btn_SaveData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label Labl_Result5;
        private System.Windows.Forms.Label Labl_Result4;
        private System.Windows.Forms.Label Labl_Result3;
        private System.Windows.Forms.Label Labl_Result2;
        private System.Windows.Forms.Label Labl_Result1;
        private System.Windows.Forms.Panel Pnl_ShowSetting;
    }
}