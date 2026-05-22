
namespace RGBTester.UI
{
    partial class F_EngineerSetting
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
            this.button1 = new System.Windows.Forms.Button();
            this.Btn_ElectricalSetting = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.Btn_OpticalSetting = new System.Windows.Forms.Button();
            this.Btn_CalMFactor = new System.Windows.Forms.Button();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(244, 30);
            this.button1.TabIndex = 0;
            this.button1.Text = "Quanta Log Setting";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Btn_ElectricalSetting
            // 
            this.Btn_ElectricalSetting.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_ElectricalSetting.Location = new System.Drawing.Point(253, 3);
            this.Btn_ElectricalSetting.Name = "Btn_ElectricalSetting";
            this.Btn_ElectricalSetting.Size = new System.Drawing.Size(244, 30);
            this.Btn_ElectricalSetting.TabIndex = 1;
            this.Btn_ElectricalSetting.Text = "Electrical Setting";
            this.Btn_ElectricalSetting.UseVisualStyleBackColor = true;
            this.Btn_ElectricalSetting.Click += new System.EventHandler(this.Btn_ElectricalSetting_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel2.Controls.Add(this.Btn_OpticalSetting, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.Btn_ElectricalSetting, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.button1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.Btn_CalMFactor, 3, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 36F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 64F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1002, 100);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // Btn_OpticalSetting
            // 
            this.Btn_OpticalSetting.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_OpticalSetting.Location = new System.Drawing.Point(503, 3);
            this.Btn_OpticalSetting.Name = "Btn_OpticalSetting";
            this.Btn_OpticalSetting.Size = new System.Drawing.Size(244, 30);
            this.Btn_OpticalSetting.TabIndex = 3;
            this.Btn_OpticalSetting.Text = "Optical Setting";
            this.Btn_OpticalSetting.UseVisualStyleBackColor = true;
            this.Btn_OpticalSetting.Click += new System.EventHandler(this.Btn_OpticalSetting_Click);
            // 
            // Btn_CalMFactor
            // 
            this.Btn_CalMFactor.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_CalMFactor.Location = new System.Drawing.Point(753, 3);
            this.Btn_CalMFactor.Name = "Btn_CalMFactor";
            this.Btn_CalMFactor.Size = new System.Drawing.Size(244, 30);
            this.Btn_CalMFactor.TabIndex = 2;
            this.Btn_CalMFactor.Text = "CalMFactor Setting";
            this.Btn_CalMFactor.UseVisualStyleBackColor = true;
            this.Btn_CalMFactor.Click += new System.EventHandler(this.Btn_CalMFactor_Click);
            // 
            // F_EngineerSetting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.tableLayoutPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_EngineerSetting";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button Btn_ElectricalSetting;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button Btn_CalMFactor;
        private System.Windows.Forms.Button Btn_OpticalSetting;
    }
}