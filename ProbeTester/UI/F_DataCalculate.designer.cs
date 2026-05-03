
namespace ProbeTester.UI
{
    partial class F_DataCalculate
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
            this.TxtBx_PathName = new System.Windows.Forms.TextBox();
            this.Btn_LoadFilePath = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Plot_DataShow = new ScottPlot.FormsPlot();
            this.Btn_Calculate = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.TxtBx_MaxCurrent = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TxtBx_RMS = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtBx_SystemStableTime = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.TxtBx_TimePerPoint = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // TxtBx_PathName
            // 
            this.TxtBx_PathName.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_PathName.Location = new System.Drawing.Point(6, 6);
            this.TxtBx_PathName.Name = "TxtBx_PathName";
            this.TxtBx_PathName.Size = new System.Drawing.Size(1341, 29);
            this.TxtBx_PathName.TabIndex = 0;
            // 
            // Btn_LoadFilePath
            // 
            this.Btn_LoadFilePath.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_LoadFilePath.Location = new System.Drawing.Point(1356, 6);
            this.Btn_LoadFilePath.Name = "Btn_LoadFilePath";
            this.Btn_LoadFilePath.Size = new System.Drawing.Size(148, 29);
            this.Btn_LoadFilePath.TabIndex = 1;
            this.Btn_LoadFilePath.Text = "Load File Path";
            this.Btn_LoadFilePath.UseVisualStyleBackColor = true;
            this.Btn_LoadFilePath.Click += new System.EventHandler(this.Btn_LoadFilePath_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1347F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.Controls.Add(this.TxtBx_PathName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Btn_LoadFilePath, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1510, 41);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // Plot_DataShow
            // 
            this.Plot_DataShow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Plot_DataShow.Location = new System.Drawing.Point(12, 58);
            this.Plot_DataShow.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Plot_DataShow.Name = "Plot_DataShow";
            this.Plot_DataShow.Size = new System.Drawing.Size(1510, 857);
            this.Plot_DataShow.TabIndex = 3;
            // 
            // Btn_Calculate
            // 
            this.Btn_Calculate.Enabled = false;
            this.Btn_Calculate.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Calculate.Location = new System.Drawing.Point(1378, 920);
            this.Btn_Calculate.Name = "Btn_Calculate";
            this.Btn_Calculate.Size = new System.Drawing.Size(144, 45);
            this.Btn_Calculate.TabIndex = 4;
            this.Btn_Calculate.Text = "Calculate";
            this.Btn_Calculate.UseVisualStyleBackColor = true;
            this.Btn_Calculate.Click += new System.EventHandler(this.Btn_Calculate_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.58923F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.41077F));
            this.tableLayoutPanel2.Controls.Add(this.TxtBx_MaxCurrent, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.TxtBx_RMS, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.TxtBx_SystemStableTime, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(1528, 107);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(356, 118);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // TxtBx_MaxCurrent
            // 
            this.TxtBx_MaxCurrent.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_MaxCurrent.Location = new System.Drawing.Point(233, 82);
            this.TxtBx_MaxCurrent.Name = "TxtBx_MaxCurrent";
            this.TxtBx_MaxCurrent.Size = new System.Drawing.Size(117, 29);
            this.TxtBx_MaxCurrent.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(6, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(218, 35);
            this.label4.TabIndex = 5;
            this.label4.Text = "克服靜摩擦力電流(Amp)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TxtBx_RMS
            // 
            this.TxtBx_RMS.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_RMS.Location = new System.Drawing.Point(233, 44);
            this.TxtBx_RMS.Name = "TxtBx_RMS";
            this.TxtBx_RMS.Size = new System.Drawing.Size(117, 29);
            this.TxtBx_RMS.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(6, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(167, 35);
            this.label3.TabIndex = 3;
            this.label3.Text = "等速段RMS";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TxtBx_SystemStableTime
            // 
            this.TxtBx_SystemStableTime.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_SystemStableTime.Location = new System.Drawing.Point(233, 6);
            this.TxtBx_SystemStableTime.Name = "TxtBx_SystemStableTime";
            this.TxtBx_SystemStableTime.Size = new System.Drawing.Size(117, 29);
            this.TxtBx_SystemStableTime.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "系統整定時間(ms)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 221F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tableLayoutPanel3.Controls.Add(this.TxtBx_TimePerPoint, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(1528, 58);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(356, 43);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // TxtBx_TimePerPoint
            // 
            this.TxtBx_TimePerPoint.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_TimePerPoint.Location = new System.Drawing.Point(230, 6);
            this.TxtBx_TimePerPoint.Name = "TxtBx_TimePerPoint";
            this.TxtBx_TimePerPoint.Size = new System.Drawing.Size(120, 29);
            this.TxtBx_TimePerPoint.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(215, 35);
            this.label2.TabIndex = 0;
            this.label2.Text = "時間(us)/資料點";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // F_DataCalculate
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1896, 967);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.Btn_Calculate);
            this.Controls.Add(this.Plot_DataShow);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_DataCalculate";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox TxtBx_PathName;
        private System.Windows.Forms.Button Btn_LoadFilePath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ScottPlot.FormsPlot Plot_DataShow;
        private System.Windows.Forms.Button Btn_Calculate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TxtBx_RMS;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtBx_SystemStableTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TextBox TxtBx_TimePerPoint;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtBx_MaxCurrent;
    }
}