
namespace RGBTester.UI
{
    partial class F_MFactorCalibration
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
            this.Plot_Spectrom = new ScottPlot.FormsPlot();
            this.Btn_ReadStdSpectrum = new System.Windows.Forms.Button();
            this.Labl_Intensity = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Btn_StopLive = new System.Windows.Forms.Button();
            this.Btn_Live = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtBx_IntgralTime = new System.Windows.Forms.TextBox();
            this.Btn_SaveData = new System.Windows.Forms.Button();
            this.PgBar_Intensity = new System.Windows.Forms.ProgressBar();
            this.Timer_GetSpectrum = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Plot_Spectrom
            // 
            this.Plot_Spectrom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Plot_Spectrom.Location = new System.Drawing.Point(12, 11);
            this.Plot_Spectrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Plot_Spectrom.Name = "Plot_Spectrom";
            this.Plot_Spectrom.Size = new System.Drawing.Size(1036, 639);
            this.Plot_Spectrom.TabIndex = 33;
            // 
            // Btn_ReadStdSpectrum
            // 
            this.Btn_ReadStdSpectrum.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_ReadStdSpectrum.Location = new System.Drawing.Point(1075, 11);
            this.Btn_ReadStdSpectrum.Name = "Btn_ReadStdSpectrum";
            this.Btn_ReadStdSpectrum.Size = new System.Drawing.Size(230, 73);
            this.Btn_ReadStdSpectrum.TabIndex = 34;
            this.Btn_ReadStdSpectrum.Text = "Read Std Spectrum";
            this.Btn_ReadStdSpectrum.UseVisualStyleBackColor = true;
            this.Btn_ReadStdSpectrum.Click += new System.EventHandler(this.Btn_ReadStdSpectrum_Click);
            // 
            // Labl_Intensity
            // 
            this.Labl_Intensity.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_Intensity.Location = new System.Drawing.Point(1163, 251);
            this.Labl_Intensity.Name = "Labl_Intensity";
            this.Labl_Intensity.Size = new System.Drawing.Size(68, 23);
            this.Labl_Intensity.TabIndex = 39;
            this.Labl_Intensity.Text = "%";
            this.Labl_Intensity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.68224F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.31776F));
            this.tableLayoutPanel1.Controls.Add(this.Btn_StopLive, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.Btn_Live, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TxtBx_IntgralTime, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.Btn_SaveData, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1075, 90);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(231, 145);
            this.tableLayoutPanel1.TabIndex = 37;
            // 
            // Btn_StopLive
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.Btn_StopLive, 2);
            this.Btn_StopLive.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_StopLive.Location = new System.Drawing.Point(1, 73);
            this.Btn_StopLive.Margin = new System.Windows.Forms.Padding(0);
            this.Btn_StopLive.Name = "Btn_StopLive";
            this.Btn_StopLive.Size = new System.Drawing.Size(229, 35);
            this.Btn_StopLive.TabIndex = 35;
            this.Btn_StopLive.Text = "Stop Live";
            this.Btn_StopLive.UseVisualStyleBackColor = true;
            this.Btn_StopLive.Click += new System.EventHandler(this.Btn_StopLive_Click);
            // 
            // Btn_Live
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.Btn_Live, 2);
            this.Btn_Live.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Live.Location = new System.Drawing.Point(1, 37);
            this.Btn_Live.Margin = new System.Windows.Forms.Padding(0);
            this.Btn_Live.Name = "Btn_Live";
            this.Btn_Live.Size = new System.Drawing.Size(229, 35);
            this.Btn_Live.TabIndex = 34;
            this.Btn_Live.Text = "Live";
            this.Btn_Live.UseVisualStyleBackColor = true;
            this.Btn_Live.Click += new System.EventHandler(this.Btn_Live_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(4, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 34);
            this.label1.TabIndex = 0;
            this.label1.Text = "IntgTime(ms)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxtBx_IntgralTime
            // 
            this.TxtBx_IntgralTime.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_IntgralTime.Location = new System.Drawing.Point(145, 4);
            this.TxtBx_IntgralTime.Name = "TxtBx_IntgralTime";
            this.TxtBx_IntgralTime.Size = new System.Drawing.Size(82, 29);
            this.TxtBx_IntgralTime.TabIndex = 1;
            // 
            // Btn_SaveData
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.Btn_SaveData, 2);
            this.Btn_SaveData.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_SaveData.Location = new System.Drawing.Point(1, 109);
            this.Btn_SaveData.Margin = new System.Windows.Forms.Padding(0);
            this.Btn_SaveData.Name = "Btn_SaveData";
            this.Btn_SaveData.Size = new System.Drawing.Size(229, 33);
            this.Btn_SaveData.TabIndex = 33;
            this.Btn_SaveData.Text = "Save MFactor";
            this.Btn_SaveData.UseVisualStyleBackColor = true;
            this.Btn_SaveData.Click += new System.EventHandler(this.Btn_SaveData_Click);
            // 
            // PgBar_Intensity
            // 
            this.PgBar_Intensity.Location = new System.Drawing.Point(1075, 241);
            this.PgBar_Intensity.Name = "PgBar_Intensity";
            this.PgBar_Intensity.Size = new System.Drawing.Size(231, 43);
            this.PgBar_Intensity.TabIndex = 38;
            // 
            // Timer_GetSpectrum
            // 
            this.Timer_GetSpectrum.Tick += new System.EventHandler(this.Timer_GetSpectrum_Tick);
            // 
            // F_MFactorCalibration
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.Labl_Intensity);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.PgBar_Intensity);
            this.Controls.Add(this.Btn_ReadStdSpectrum);
            this.Controls.Add(this.Plot_Spectrom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_MFactorCalibration";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ScottPlot.FormsPlot Plot_Spectrom;
        private System.Windows.Forms.Button Btn_ReadStdSpectrum;
        private System.Windows.Forms.Label Labl_Intensity;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button Btn_StopLive;
        private System.Windows.Forms.Button Btn_Live;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtBx_IntgralTime;
        private System.Windows.Forms.Button Btn_SaveData;
        private System.Windows.Forms.ProgressBar PgBar_Intensity;
        private System.Windows.Forms.Timer Timer_GetSpectrum;
    }
}