
namespace ProbeTester.UI
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
            this.label6 = new System.Windows.Forms.Label();
            this.Btn_CCD = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Btn_Light = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.Location = new System.Drawing.Point(508, 173);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 23);
            this.label6.TabIndex = 47;
            this.label6.Text = "CCD";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Btn_CCD
            // 
            this.Btn_CCD.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_CCD.Location = new System.Drawing.Point(512, 100);
            this.Btn_CCD.Name = "Btn_CCD";
            this.Btn_CCD.Size = new System.Drawing.Size(70, 70);
            this.Btn_CCD.TabIndex = 46;
            this.Btn_CCD.UseVisualStyleBackColor = true;
            this.Btn_CCD.Click += new System.EventHandler(this.Btn_CCD_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(592, 320);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 23);
            this.label1.TabIndex = 49;
            this.label1.Text = "Light";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Btn_Light
            // 
            this.Btn_Light.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_Light.Location = new System.Drawing.Point(596, 247);
            this.Btn_Light.Name = "Btn_Light";
            this.Btn_Light.Size = new System.Drawing.Size(70, 70);
            this.Btn_Light.TabIndex = 48;
            this.Btn_Light.UseVisualStyleBackColor = true;
            this.Btn_Light.Click += new System.EventHandler(this.Btn_Light_Click);
            // 
            // F_OEM_Setting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1896, 967);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Btn_Light);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Btn_CCD);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_OEM_Setting";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button Btn_CCD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Btn_Light;
    }
}