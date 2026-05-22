
namespace RGBTester.UI
{
    partial class F_OpticalSetting
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TxtBx_OpticalKValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtBx_BackgroundGain = new System.Windows.Forms.TextBox();
            this.TxtBx_BackgroundFOffset = new System.Windows.Forms.TextBox();
            this.Btn_Calibration = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.0781F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.9219F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label59, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TxtBx_OpticalKValue, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.TxtBx_BackgroundGain, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.TxtBx_BackgroundFOffset, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.Btn_Calibration, 1, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(656, 200);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // TxtBx_OpticalKValue
            // 
            this.TxtBx_OpticalKValue.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_OpticalKValue.Location = new System.Drawing.Point(223, 6);
            this.TxtBx_OpticalKValue.Name = "TxtBx_OpticalKValue";
            this.TxtBx_OpticalKValue.Size = new System.Drawing.Size(427, 29);
            this.TxtBx_OpticalKValue.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 35);
            this.label1.TabIndex = 3;
            this.label1.Text = "K Value";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label59
            // 
            this.label59.BackColor = System.Drawing.Color.Silver;
            this.tableLayoutPanel1.SetColumnSpan(this.label59, 2);
            this.label59.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label59.Location = new System.Drawing.Point(6, 41);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(644, 25);
            this.label59.TabIndex = 12;
            this.label59.Text = "Background Calibration";
            this.label59.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(6, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 35);
            this.label2.TabIndex = 13;
            this.label2.Text = "Gain";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(6, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(180, 35);
            this.label3.TabIndex = 14;
            this.label3.Text = "Offset";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TxtBx_BackgroundGain
            // 
            this.TxtBx_BackgroundGain.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_BackgroundGain.Location = new System.Drawing.Point(223, 75);
            this.TxtBx_BackgroundGain.Name = "TxtBx_BackgroundGain";
            this.TxtBx_BackgroundGain.Size = new System.Drawing.Size(427, 29);
            this.TxtBx_BackgroundGain.TabIndex = 15;
            // 
            // TxtBx_BackgroundFOffset
            // 
            this.TxtBx_BackgroundFOffset.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_BackgroundFOffset.Location = new System.Drawing.Point(223, 113);
            this.TxtBx_BackgroundFOffset.Name = "TxtBx_BackgroundFOffset";
            this.TxtBx_BackgroundFOffset.Size = new System.Drawing.Size(427, 29);
            this.TxtBx_BackgroundFOffset.TabIndex = 16;
            // 
            // Btn_Calibration
            // 
            this.Btn_Calibration.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Calibration.Location = new System.Drawing.Point(223, 151);
            this.Btn_Calibration.Name = "Btn_Calibration";
            this.Btn_Calibration.Size = new System.Drawing.Size(427, 42);
            this.Btn_Calibration.TabIndex = 17;
            this.Btn_Calibration.Text = "Calibration";
            this.Btn_Calibration.UseVisualStyleBackColor = true;
            this.Btn_Calibration.Click += new System.EventHandler(this.Btn_Calibration_Click);
            // 
            // F_OpticalSetting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_OpticalSetting";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtBx_OpticalKValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label59;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtBx_BackgroundGain;
        private System.Windows.Forms.TextBox TxtBx_BackgroundFOffset;
        private System.Windows.Forms.Button Btn_Calibration;
    }
}