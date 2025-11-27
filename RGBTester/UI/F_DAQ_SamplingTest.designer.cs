
namespace RGBTester.UI
{
    partial class F_DAQ_SamplingTest
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label15 = new System.Windows.Forms.Label();
            this.DGV_DAQ_Result = new System.Windows.Forms.DataGridView();
            this.Btn_Start = new System.Windows.Forms.Button();
            this.Btn_SaveData = new System.Windows.Forms.Button();
            this.TxtBx_AveraingCount = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.Title_CH0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CH15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_DAQ_Result)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.94152F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.05848F));
            this.tableLayoutPanel5.Controls.Add(this.label15, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.TxtBx_AveraingCount, 1, 0);
            this.tableLayoutPanel5.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tableLayoutPanel5.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(345, 40);
            this.tableLayoutPanel5.TabIndex = 5;
            // 
            // label15
            // 
            this.label15.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label15.Location = new System.Drawing.Point(6, 3);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(195, 30);
            this.label15.TabIndex = 4;
            this.label15.Text = "Averaging Count";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DGV_DAQ_Result
            // 
            this.DGV_DAQ_Result.AllowUserToAddRows = false;
            this.DGV_DAQ_Result.AllowUserToDeleteRows = false;
            this.DGV_DAQ_Result.AllowUserToResizeColumns = false;
            this.DGV_DAQ_Result.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV_DAQ_Result.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.DGV_DAQ_Result.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_DAQ_Result.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Title_CH0,
            this.Title_CH1,
            this.Title_CH2,
            this.Title_CH3,
            this.Title_CH4,
            this.Title_CH5,
            this.Title_CH6,
            this.Title_CH7,
            this.Title_CH8,
            this.Title_CH9,
            this.Title_CH10,
            this.Title_CH11,
            this.Title_CH12,
            this.Title_CH13,
            this.Title_CH14,
            this.Title_CH15});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV_DAQ_Result.DefaultCellStyle = dataGridViewCellStyle6;
            this.DGV_DAQ_Result.Location = new System.Drawing.Point(12, 58);
            this.DGV_DAQ_Result.Name = "DGV_DAQ_Result";
            this.DGV_DAQ_Result.RowHeadersVisible = false;
            this.DGV_DAQ_Result.RowTemplate.Height = 24;
            this.DGV_DAQ_Result.Size = new System.Drawing.Size(1302, 534);
            this.DGV_DAQ_Result.TabIndex = 14;
            // 
            // Btn_Start
            // 
            this.Btn_Start.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Start.Location = new System.Drawing.Point(1075, 598);
            this.Btn_Start.Name = "Btn_Start";
            this.Btn_Start.Size = new System.Drawing.Size(114, 51);
            this.Btn_Start.TabIndex = 15;
            this.Btn_Start.Text = "Start";
            this.Btn_Start.UseVisualStyleBackColor = true;
            this.Btn_Start.Click += new System.EventHandler(this.Btn_Start_Click);
            // 
            // Btn_SaveData
            // 
            this.Btn_SaveData.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_SaveData.Location = new System.Drawing.Point(1200, 598);
            this.Btn_SaveData.Name = "Btn_SaveData";
            this.Btn_SaveData.Size = new System.Drawing.Size(114, 51);
            this.Btn_SaveData.TabIndex = 16;
            this.Btn_SaveData.Text = "Save Data";
            this.Btn_SaveData.UseVisualStyleBackColor = true;
            this.Btn_SaveData.Click += new System.EventHandler(this.Btn_SaveData_Click);
            // 
            // TxtBx_AveraingCount
            // 
            this.TxtBx_AveraingCount.Location = new System.Drawing.Point(210, 6);
            this.TxtBx_AveraingCount.Name = "TxtBx_AveraingCount";
            this.TxtBx_AveraingCount.Size = new System.Drawing.Size(129, 29);
            this.TxtBx_AveraingCount.TabIndex = 5;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.40502F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.59498F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBox1, 1, 0);
            this.tableLayoutPanel1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(363, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(282, 40);
            this.tableLayoutPanel1.TabIndex = 6;
            this.tableLayoutPanel1.Visible = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 30);
            this.label1.TabIndex = 4;
            this.label1.Text = "Channel Count";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16"});
            this.comboBox1.Location = new System.Drawing.Point(154, 6);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(122, 28);
            this.comboBox1.TabIndex = 5;
            // 
            // Title_CH0
            // 
            this.Title_CH0.HeaderText = "CH0";
            this.Title_CH0.Name = "Title_CH0";
            this.Title_CH0.Width = 80;
            // 
            // Title_CH1
            // 
            this.Title_CH1.HeaderText = "CH1";
            this.Title_CH1.Name = "Title_CH1";
            this.Title_CH1.Width = 80;
            // 
            // Title_CH2
            // 
            this.Title_CH2.HeaderText = "CH2";
            this.Title_CH2.Name = "Title_CH2";
            this.Title_CH2.Width = 80;
            // 
            // Title_CH3
            // 
            this.Title_CH3.HeaderText = "CH3";
            this.Title_CH3.Name = "Title_CH3";
            this.Title_CH3.Width = 80;
            // 
            // Title_CH4
            // 
            this.Title_CH4.HeaderText = "CH4";
            this.Title_CH4.Name = "Title_CH4";
            this.Title_CH4.Width = 80;
            // 
            // Title_CH5
            // 
            this.Title_CH5.HeaderText = "CH5";
            this.Title_CH5.Name = "Title_CH5";
            this.Title_CH5.Width = 80;
            // 
            // Title_CH6
            // 
            this.Title_CH6.HeaderText = "CH6";
            this.Title_CH6.Name = "Title_CH6";
            this.Title_CH6.Width = 80;
            // 
            // Title_CH7
            // 
            this.Title_CH7.HeaderText = "CH7";
            this.Title_CH7.Name = "Title_CH7";
            this.Title_CH7.Width = 80;
            // 
            // Title_CH8
            // 
            this.Title_CH8.HeaderText = "CH8";
            this.Title_CH8.Name = "Title_CH8";
            this.Title_CH8.Width = 80;
            // 
            // Title_CH9
            // 
            this.Title_CH9.HeaderText = "CH9";
            this.Title_CH9.Name = "Title_CH9";
            this.Title_CH9.Width = 80;
            // 
            // Title_CH10
            // 
            this.Title_CH10.HeaderText = "CH10";
            this.Title_CH10.Name = "Title_CH10";
            this.Title_CH10.Width = 80;
            // 
            // Title_CH11
            // 
            this.Title_CH11.HeaderText = "CH11";
            this.Title_CH11.Name = "Title_CH11";
            this.Title_CH11.Width = 80;
            // 
            // Title_CH12
            // 
            this.Title_CH12.HeaderText = "CH12";
            this.Title_CH12.Name = "Title_CH12";
            this.Title_CH12.Width = 80;
            // 
            // Title_CH13
            // 
            this.Title_CH13.HeaderText = "CH13";
            this.Title_CH13.Name = "Title_CH13";
            this.Title_CH13.Width = 80;
            // 
            // Title_CH14
            // 
            this.Title_CH14.HeaderText = "CH14";
            this.Title_CH14.Name = "Title_CH14";
            this.Title_CH14.Width = 80;
            // 
            // Title_CH15
            // 
            this.Title_CH15.HeaderText = "CH15";
            this.Title_CH15.Name = "Title_CH15";
            this.Title_CH15.Width = 80;
            // 
            // F_DAQ_SamplingTest
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.Btn_SaveData);
            this.Controls.Add(this.Btn_Start);
            this.Controls.Add(this.DGV_DAQ_Result);
            this.Controls.Add(this.tableLayoutPanel5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_DAQ_SamplingTest";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_DAQ_Result)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox TxtBx_AveraingCount;
        private System.Windows.Forms.DataGridView DGV_DAQ_Result;
        private System.Windows.Forms.Button Btn_Start;
        private System.Windows.Forms.Button Btn_SaveData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH0;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CH15;
    }
}