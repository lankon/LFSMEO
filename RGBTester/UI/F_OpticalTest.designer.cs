
namespace RGBTester.UI
{
    partial class F_OpticalTest
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
            this.Btn_StartTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Btn_StartTest
            // 
            this.Btn_StartTest.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_StartTest.Location = new System.Drawing.Point(393, 291);
            this.Btn_StartTest.Name = "Btn_StartTest";
            this.Btn_StartTest.Size = new System.Drawing.Size(555, 125);
            this.Btn_StartTest.TabIndex = 10;
            this.Btn_StartTest.Text = "Start Test";
            this.Btn_StartTest.UseVisualStyleBackColor = true;
            this.Btn_StartTest.Click += new System.EventHandler(this.Btn_StartTest_Click);
            // 
            // F_OpticalTest
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.Btn_StartTest);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_OpticalTest";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Btn_StartTest;
    }
}