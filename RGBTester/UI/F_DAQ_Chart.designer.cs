
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.Btn_CaptureData = new System.Windows.Forms.Button();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.TestMode = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.Select_HL_Mode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TestColor = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DAC_Value = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            chartArea3.AxisX.LabelStyle.ForeColor = System.Drawing.Color.LightGray;
            chartArea3.AxisX.LineColor = System.Drawing.Color.Gray;
            chartArea3.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            chartArea3.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea3.AxisX.MajorTickMark.Enabled = false;
            chartArea3.AxisY.LabelStyle.Enabled = false;
            chartArea3.AxisY.LineColor = System.Drawing.Color.Gray;
            chartArea3.AxisY.MajorGrid.Enabled = false;
            chartArea3.AxisY.MajorTickMark.Enabled = false;
            chartArea3.BackColor = System.Drawing.Color.Black;
            chartArea3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea3.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chart1.Legends.Add(legend3);
            this.chart1.Location = new System.Drawing.Point(12, 12);
            this.chart1.Name = "chart1";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Color = System.Drawing.Color.Red;
            series3.Legend = "Legend1";
            series3.MarkerBorderColor = System.Drawing.Color.White;
            series3.Name = "Series1";
            this.chart1.Series.Add(series3);
            this.chart1.Size = new System.Drawing.Size(1302, 571);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // Btn_CaptureData
            // 
            this.Btn_CaptureData.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_CaptureData.Location = new System.Drawing.Point(1162, 600);
            this.Btn_CaptureData.Name = "Btn_CaptureData";
            this.Btn_CaptureData.Size = new System.Drawing.Size(152, 38);
            this.Btn_CaptureData.TabIndex = 1;
            this.Btn_CaptureData.Text = "Capture";
            this.Btn_CaptureData.UseVisualStyleBackColor = true;
            this.Btn_CaptureData.Click += new System.EventHandler(this.Btn_CaptureData_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel5.ColumnCount = 8;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel5.Controls.Add(this.label2, 6, 0);
            this.tableLayoutPanel5.Controls.Add(this.TestColor, 3, 0);
            this.tableLayoutPanel5.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.TestMode, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.label15, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.Select_HL_Mode, 5, 0);
            this.tableLayoutPanel5.Controls.Add(this.label46, 4, 0);
            this.tableLayoutPanel5.Controls.Add(this.DAC_Value, 7, 0);
            this.tableLayoutPanel5.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tableLayoutPanel5.Location = new System.Drawing.Point(12, 600);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1132, 40);
            this.tableLayoutPanel5.TabIndex = 5;
            // 
            // TestMode
            // 
            this.TestMode.FormattingEnabled = true;
            this.TestMode.Items.AddRange(new object[] {
            "Left",
            "Right"});
            this.TestMode.Location = new System.Drawing.Point(147, 6);
            this.TestMode.Name = "TestMode";
            this.TestMode.Size = new System.Drawing.Size(129, 28);
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
            // label46
            // 
            this.label46.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label46.Location = new System.Drawing.Point(570, 3);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(132, 30);
            this.label46.TabIndex = 6;
            this.label46.Text = "High/Low Mode";
            this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Select_HL_Mode
            // 
            this.Select_HL_Mode.FormattingEnabled = true;
            this.Select_HL_Mode.Items.AddRange(new object[] {
            "HCM",
            "LCM"});
            this.Select_HL_Mode.Location = new System.Drawing.Point(711, 6);
            this.Select_HL_Mode.Name = "Select_HL_Mode";
            this.Select_HL_Mode.Size = new System.Drawing.Size(132, 28);
            this.Select_HL_Mode.TabIndex = 7;
            this.Select_HL_Mode.Text = "HCM";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(288, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 35);
            this.label1.TabIndex = 8;
            this.label1.Text = "Test Color";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TestColor
            // 
            this.TestColor.FormattingEnabled = true;
            this.TestColor.Items.AddRange(new object[] {
            "Red",
            "Green",
            "Blue"});
            this.TestColor.Location = new System.Drawing.Point(429, 6);
            this.TestColor.Name = "TestColor";
            this.TestColor.Size = new System.Drawing.Size(132, 28);
            this.TestColor.TabIndex = 9;
            this.TestColor.Text = "Red";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(852, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 30);
            this.label2.TabIndex = 10;
            this.label2.Text = "DAC Value";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DAC_Value
            // 
            this.DAC_Value.Location = new System.Drawing.Point(993, 6);
            this.DAC_Value.Name = "DAC_Value";
            this.DAC_Value.Size = new System.Drawing.Size(133, 29);
            this.DAC_Value.TabIndex = 11;
            // 
            // F_DAQ_Chart
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
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
    }
}