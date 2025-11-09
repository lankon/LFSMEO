
namespace RGBTester.UI
{
    partial class F_OEM_Setting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F_OEM_Setting));
            this.Btn_IO_Form = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Btn_EquipmentSetting = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.Btn_MotionSetting = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Btn_IO_Form
            // 
            this.Btn_IO_Form.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_IO_Form.Location = new System.Drawing.Point(44, 12);
            this.Btn_IO_Form.Name = "Btn_IO_Form";
            this.Btn_IO_Form.Size = new System.Drawing.Size(60, 60);
            this.Btn_IO_Form.TabIndex = 32;
            this.Btn_IO_Form.UseVisualStyleBackColor = true;
            this.Btn_IO_Form.Click += new System.EventHandler(this.Btn_IO_Form_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(44, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 23);
            this.label1.TabIndex = 33;
            this.label1.Text = "IO";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Btn_EquipmentSetting
            // 
            this.Btn_EquipmentSetting.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Btn_EquipmentSetting.BackgroundImage")));
            this.Btn_EquipmentSetting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_EquipmentSetting.Location = new System.Drawing.Point(155, 12);
            this.Btn_EquipmentSetting.Name = "Btn_EquipmentSetting";
            this.Btn_EquipmentSetting.Size = new System.Drawing.Size(60, 60);
            this.Btn_EquipmentSetting.TabIndex = 35;
            this.Btn_EquipmentSetting.UseVisualStyleBackColor = true;
            this.Btn_EquipmentSetting.Click += new System.EventHandler(this.Btn_EquipmentSetting_Click);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(133, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 23);
            this.label2.TabIndex = 36;
            this.label2.Text = "Equipment";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Btn_MotionSetting
            // 
            this.Btn_MotionSetting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_MotionSetting.Location = new System.Drawing.Point(277, 12);
            this.Btn_MotionSetting.Name = "Btn_MotionSetting";
            this.Btn_MotionSetting.Size = new System.Drawing.Size(60, 60);
            this.Btn_MotionSetting.TabIndex = 37;
            this.Btn_MotionSetting.UseVisualStyleBackColor = true;
            this.Btn_MotionSetting.Click += new System.EventHandler(this.Btn_MotionSetting_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(266, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 23);
            this.label3.TabIndex = 38;
            this.label3.Text = "Motion";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // F_OEM_Setting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Btn_MotionSetting);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Btn_EquipmentSetting);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Btn_IO_Form);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_OEM_Setting";
            this.Text = "F_OEM_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_OEM_Setting_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Btn_IO_Form;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Btn_EquipmentSetting;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Btn_MotionSetting;
        private System.Windows.Forms.Label label3;
    }
}