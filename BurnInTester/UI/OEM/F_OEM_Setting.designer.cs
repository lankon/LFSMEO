
namespace BurnInTester.UI
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
            this.Btn_TC_Setting = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Btn_TC_Setting
            // 
            this.Btn_TC_Setting.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Btn_TC_Setting.BackgroundImage")));
            this.Btn_TC_Setting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_TC_Setting.Location = new System.Drawing.Point(12, 12);
            this.Btn_TC_Setting.Name = "Btn_TC_Setting";
            this.Btn_TC_Setting.Size = new System.Drawing.Size(70, 70);
            this.Btn_TC_Setting.TabIndex = 40;
            this.Btn_TC_Setting.UseVisualStyleBackColor = true;
            this.Btn_TC_Setting.Click += new System.EventHandler(this.Btn_TC_Setting_Click);
            // 
            // F_OEM_Setting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1896, 967);
            this.Controls.Add(this.Btn_TC_Setting);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_OEM_Setting";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Btn_TC_Setting;
    }
}