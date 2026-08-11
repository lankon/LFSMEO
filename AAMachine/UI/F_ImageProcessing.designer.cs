
namespace AAMachine.UI
{
    partial class F_ImageProcessing
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
            this.Btn_CaptureLineProfile = new System.Windows.Forms.Button();
            this.Pnl_ImageResult = new System.Windows.Forms.Panel();
            this.TxtBx_CenterX = new System.Windows.Forms.TextBox();
            this.TxtBx_CenterY = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Btn_CaptureLineProfile
            // 
            this.Btn_CaptureLineProfile.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_CaptureLineProfile.Location = new System.Drawing.Point(739, 361);
            this.Btn_CaptureLineProfile.Name = "Btn_CaptureLineProfile";
            this.Btn_CaptureLineProfile.Size = new System.Drawing.Size(146, 62);
            this.Btn_CaptureLineProfile.TabIndex = 0;
            this.Btn_CaptureLineProfile.Text = "Line Profile";
            this.Btn_CaptureLineProfile.UseVisualStyleBackColor = true;
            this.Btn_CaptureLineProfile.Click += new System.EventHandler(this.Btn_CaptureLineProfile_Click);
            // 
            // Pnl_ImageResult
            // 
            this.Pnl_ImageResult.Location = new System.Drawing.Point(12, 152);
            this.Pnl_ImageResult.Name = "Pnl_ImageResult";
            this.Pnl_ImageResult.Size = new System.Drawing.Size(700, 700);
            this.Pnl_ImageResult.TabIndex = 1;
            // 
            // TxtBx_CenterX
            // 
            this.TxtBx_CenterX.Location = new System.Drawing.Point(739, 445);
            this.TxtBx_CenterX.Name = "TxtBx_CenterX";
            this.TxtBx_CenterX.Size = new System.Drawing.Size(146, 22);
            this.TxtBx_CenterX.TabIndex = 2;
            // 
            // TxtBx_CenterY
            // 
            this.TxtBx_CenterY.Location = new System.Drawing.Point(739, 473);
            this.TxtBx_CenterY.Name = "TxtBx_CenterY";
            this.TxtBx_CenterY.Size = new System.Drawing.Size(146, 22);
            this.TxtBx_CenterY.TabIndex = 3;
            // 
            // F_ImageProcessing
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1896, 967);
            this.Controls.Add(this.TxtBx_CenterY);
            this.Controls.Add(this.TxtBx_CenterX);
            this.Controls.Add(this.Pnl_ImageResult);
            this.Controls.Add(this.Btn_CaptureLineProfile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_ImageProcessing";
            this.Text = "F_Equipment_Setting";
            this.Load += new System.EventHandler(this.F_SampleFull_Load);
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_CaptureLineProfile;
        private System.Windows.Forms.Panel Pnl_ImageResult;
        private System.Windows.Forms.TextBox TxtBx_CenterX;
        private System.Windows.Forms.TextBox TxtBx_CenterY;
    }
}