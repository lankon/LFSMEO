
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F_DAQ_Chart));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
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
            resources.ApplyResources(this.chart1, "chart1");
            this.chart1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            chartArea2.AxisX.LabelStyle.ForeColor = System.Drawing.Color.LightGray;
            chartArea2.AxisX.LineColor = System.Drawing.Color.Gray;
            chartArea2.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            chartArea2.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea2.AxisX.MajorTickMark.Enabled = false;
            chartArea2.AxisY.LabelStyle.Enabled = false;
            chartArea2.AxisY.LineColor = System.Drawing.Color.Gray;
            chartArea2.AxisY.MajorGrid.Enabled = false;
            chartArea2.AxisY.MajorTickMark.Enabled = false;
            chartArea2.BackColor = System.Drawing.Color.Black;
            chartArea2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea2.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Alignment = System.Drawing.StringAlignment.Far;
            legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend2.Name = "Legend1";
            legend2.TitleAlignment = System.Drawing.StringAlignment.Near;
            this.chart1.Legends.Add(legend2);
            this.chart1.Name = "chart1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.Red;
            series2.Legend = "Legend1";
            series2.MarkerBorderColor = System.Drawing.Color.White;
            series2.Name = "Series1";
            this.chart1.Series.Add(series2);
            // 
            // Btn_CaptureData
            // 
            resources.ApplyResources(this.Btn_CaptureData, "Btn_CaptureData");
            this.Btn_CaptureData.Name = "Btn_CaptureData";
            this.Btn_CaptureData.UseVisualStyleBackColor = true;
            this.Btn_CaptureData.Click += new System.EventHandler(this.Btn_CaptureData_Click);
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.label2, 6, 0);
            this.tableLayoutPanel5.Controls.Add(this.TestColor, 3, 0);
            this.tableLayoutPanel5.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.TestMode, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.label15, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.Select_HL_Mode, 5, 0);
            this.tableLayoutPanel5.Controls.Add(this.label46, 4, 0);
            this.tableLayoutPanel5.Controls.Add(this.DAC_Value, 7, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // TestColor
            // 
            resources.ApplyResources(this.TestColor, "TestColor");
            this.TestColor.FormattingEnabled = true;
            this.TestColor.Items.AddRange(new object[] {
            resources.GetString("TestColor.Items"),
            resources.GetString("TestColor.Items1"),
            resources.GetString("TestColor.Items2"),
            resources.GetString("TestColor.Items3")});
            this.TestColor.Name = "TestColor";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // TestMode
            // 
            resources.ApplyResources(this.TestMode, "TestMode");
            this.TestMode.FormattingEnabled = true;
            this.TestMode.Items.AddRange(new object[] {
            resources.GetString("TestMode.Items"),
            resources.GetString("TestMode.Items1")});
            this.TestMode.Name = "TestMode";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // Select_HL_Mode
            // 
            resources.ApplyResources(this.Select_HL_Mode, "Select_HL_Mode");
            this.Select_HL_Mode.FormattingEnabled = true;
            this.Select_HL_Mode.Items.AddRange(new object[] {
            resources.GetString("Select_HL_Mode.Items"),
            resources.GetString("Select_HL_Mode.Items1")});
            this.Select_HL_Mode.Name = "Select_HL_Mode";
            // 
            // label46
            // 
            resources.ApplyResources(this.label46, "label46");
            this.label46.Name = "label46";
            // 
            // DAC_Value
            // 
            resources.ApplyResources(this.DAC_Value, "DAC_Value");
            this.DAC_Value.Name = "DAC_Value";
            // 
            // Btn_SaveData
            // 
            resources.ApplyResources(this.Btn_SaveData, "Btn_SaveData");
            this.Btn_SaveData.Name = "Btn_SaveData";
            this.Btn_SaveData.UseVisualStyleBackColor = true;
            this.Btn_SaveData.Click += new System.EventHandler(this.Btn_SaveData_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tableLayoutPanel1.Controls.Add(this.Labl_Result5, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.Labl_Result4, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.Labl_Result3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.Labl_Result2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.Labl_Result1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // Labl_Result5
            // 
            resources.ApplyResources(this.Labl_Result5, "Labl_Result5");
            this.Labl_Result5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Labl_Result5.ForeColor = System.Drawing.Color.White;
            this.Labl_Result5.Name = "Labl_Result5";
            // 
            // Labl_Result4
            // 
            resources.ApplyResources(this.Labl_Result4, "Labl_Result4");
            this.Labl_Result4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Labl_Result4.ForeColor = System.Drawing.Color.White;
            this.Labl_Result4.Name = "Labl_Result4";
            // 
            // Labl_Result3
            // 
            resources.ApplyResources(this.Labl_Result3, "Labl_Result3");
            this.Labl_Result3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Labl_Result3.ForeColor = System.Drawing.Color.White;
            this.Labl_Result3.Name = "Labl_Result3";
            // 
            // Labl_Result2
            // 
            resources.ApplyResources(this.Labl_Result2, "Labl_Result2");
            this.Labl_Result2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Labl_Result2.ForeColor = System.Drawing.Color.White;
            this.Labl_Result2.Name = "Labl_Result2";
            // 
            // Labl_Result1
            // 
            resources.ApplyResources(this.Labl_Result1, "Labl_Result1");
            this.Labl_Result1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Labl_Result1.ForeColor = System.Drawing.Color.White;
            this.Labl_Result1.Name = "Labl_Result1";
            // 
            // Pnl_ShowSetting
            // 
            resources.ApplyResources(this.Pnl_ShowSetting, "Pnl_ShowSetting");
            this.Pnl_ShowSetting.BackColor = System.Drawing.Color.Transparent;
            this.Pnl_ShowSetting.Name = "Pnl_ShowSetting";
            this.Pnl_ShowSetting.Click += new System.EventHandler(this.Pnl_ShowSetting_Click);
            // 
            // F_DAQ_Chart
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.Controls.Add(this.Pnl_ShowSetting);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.Btn_SaveData);
            this.Controls.Add(this.tableLayoutPanel5);
            this.Controls.Add(this.Btn_CaptureData);
            this.Controls.Add(this.chart1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_DAQ_Chart";
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