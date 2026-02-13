
namespace DeviceUI.Motion
{
    partial class F_AxisButton
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
            this.Btn_PreviousPnlPart1 = new System.Windows.Forms.Button();
            this.Btn_NextPnlPart1 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Pnl_Part1 = new System.Windows.Forms.Panel();
            this.Labl_PostionAxis0 = new System.Windows.Forms.Label();
            this.Btn_Axis0 = new System.Windows.Forms.Button();
            this.Pnl_Part2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.Btn_NextPnlPart2 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.Btn_PreviousPnlPart2 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.Timer_UpdatePosition = new System.Windows.Forms.Timer(this.components);
            this.Pnl_Part1.SuspendLayout();
            this.Pnl_Part2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Btn_PreviousPnlPart1
            // 
            this.Btn_PreviousPnlPart1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_PreviousPnlPart1.Location = new System.Drawing.Point(0, 3);
            this.Btn_PreviousPnlPart1.Name = "Btn_PreviousPnlPart1";
            this.Btn_PreviousPnlPart1.Size = new System.Drawing.Size(19, 82);
            this.Btn_PreviousPnlPart1.TabIndex = 34;
            this.Btn_PreviousPnlPart1.Text = "<";
            this.Btn_PreviousPnlPart1.UseVisualStyleBackColor = true;
            this.Btn_PreviousPnlPart1.Click += new System.EventHandler(this.Btn_PreviousPnlPart1_Click);
            // 
            // Btn_NextPnlPart1
            // 
            this.Btn_NextPnlPart1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_NextPnlPart1.Location = new System.Drawing.Point(1297, 3);
            this.Btn_NextPnlPart1.Name = "Btn_NextPnlPart1";
            this.Btn_NextPnlPart1.Size = new System.Drawing.Size(19, 82);
            this.Btn_NextPnlPart1.TabIndex = 32;
            this.Btn_NextPnlPart1.Text = ">";
            this.Btn_NextPnlPart1.UseVisualStyleBackColor = true;
            this.Btn_NextPnlPart1.Click += new System.EventHandler(this.Btn_NextPnlPart1_Click);
            // 
            // Pnl_Part1
            // 
            this.Pnl_Part1.Controls.Add(this.Labl_PostionAxis0);
            this.Pnl_Part1.Controls.Add(this.Btn_NextPnlPart1);
            this.Pnl_Part1.Controls.Add(this.Btn_PreviousPnlPart1);
            this.Pnl_Part1.Controls.Add(this.Btn_Axis0);
            this.Pnl_Part1.Location = new System.Drawing.Point(12, 85);
            this.Pnl_Part1.Name = "Pnl_Part1";
            this.Pnl_Part1.Size = new System.Drawing.Size(1318, 88);
            this.Pnl_Part1.TabIndex = 35;
            // 
            // Labl_PostionAxis0
            // 
            this.Labl_PostionAxis0.AutoSize = true;
            this.Labl_PostionAxis0.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.Labl_PostionAxis0.Location = new System.Drawing.Point(47, 64);
            this.Labl_PostionAxis0.Name = "Labl_PostionAxis0";
            this.Labl_PostionAxis0.Size = new System.Drawing.Size(44, 12);
            this.Labl_PostionAxis0.TabIndex = 4;
            this.Labl_PostionAxis0.Text = "000.000";
            this.Labl_PostionAxis0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Btn_Axis0
            // 
            this.Btn_Axis0.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Axis0.Location = new System.Drawing.Point(32, 3);
            this.Btn_Axis0.Name = "Btn_Axis0";
            this.Btn_Axis0.Size = new System.Drawing.Size(78, 82);
            this.Btn_Axis0.TabIndex = 3;
            this.Btn_Axis0.Tag = "0";
            this.Btn_Axis0.Text = "X";
            this.Btn_Axis0.UseVisualStyleBackColor = true;
            this.Btn_Axis0.Click += new System.EventHandler(this.Btn_Axis0_Click);
            // 
            // Pnl_Part2
            // 
            this.Pnl_Part2.Controls.Add(this.label1);
            this.Pnl_Part2.Controls.Add(this.button1);
            this.Pnl_Part2.Controls.Add(this.Btn_NextPnlPart2);
            this.Pnl_Part2.Controls.Add(this.button7);
            this.Pnl_Part2.Controls.Add(this.Btn_PreviousPnlPart2);
            this.Pnl_Part2.Controls.Add(this.button6);
            this.Pnl_Part2.Location = new System.Drawing.Point(146, 203);
            this.Pnl_Part2.Name = "Pnl_Part2";
            this.Pnl_Part2.Size = new System.Drawing.Size(1318, 88);
            this.Pnl_Part2.TabIndex = 36;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label1.Location = new System.Drawing.Point(40, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 12);
            this.label1.TabIndex = 35;
            this.label1.Text = "000.000";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(277, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 82);
            this.button1.TabIndex = 4;
            this.button1.Tag = "3";
            this.button1.Text = "A";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Btn_NextPnlPart2
            // 
            this.Btn_NextPnlPart2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_NextPnlPart2.Location = new System.Drawing.Point(1297, 3);
            this.Btn_NextPnlPart2.Name = "Btn_NextPnlPart2";
            this.Btn_NextPnlPart2.Size = new System.Drawing.Size(19, 82);
            this.Btn_NextPnlPart2.TabIndex = 32;
            this.Btn_NextPnlPart2.Text = ">";
            this.Btn_NextPnlPart2.UseVisualStyleBackColor = true;
            this.Btn_NextPnlPart2.Click += new System.EventHandler(this.Btn_NextPnlPart1_Click);
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(109, 3);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(78, 82);
            this.button7.TabIndex = 1;
            this.button7.Tag = "1";
            this.button7.Text = "Y";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // Btn_PreviousPnlPart2
            // 
            this.Btn_PreviousPnlPart2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_PreviousPnlPart2.Location = new System.Drawing.Point(0, 3);
            this.Btn_PreviousPnlPart2.Name = "Btn_PreviousPnlPart2";
            this.Btn_PreviousPnlPart2.Size = new System.Drawing.Size(19, 82);
            this.Btn_PreviousPnlPart2.TabIndex = 34;
            this.Btn_PreviousPnlPart2.Text = "<";
            this.Btn_PreviousPnlPart2.UseVisualStyleBackColor = true;
            this.Btn_PreviousPnlPart2.Click += new System.EventHandler(this.Btn_PreviousPnlPart1_Click);
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.Location = new System.Drawing.Point(193, 3);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(78, 82);
            this.button6.TabIndex = 2;
            this.button6.Tag = "2";
            this.button6.Text = "Z";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // Timer_UpdatePosition
            // 
            this.Timer_UpdatePosition.Interval = 200;
            this.Timer_UpdatePosition.Tick += new System.EventHandler(this.Timer_UpdatePosition_Tick);
            // 
            // F_AxisButton
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1386, 465);
            this.Controls.Add(this.Pnl_Part2);
            this.Controls.Add(this.Pnl_Part1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_AxisButton";
            this.Text = "F_StartForm_ButtonGroup";
            this.Pnl_Part1.ResumeLayout(false);
            this.Pnl_Part1.PerformLayout();
            this.Pnl_Part2.ResumeLayout(false);
            this.Pnl_Part2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Btn_PreviousPnlPart1;
        private System.Windows.Forms.Button Btn_NextPnlPart1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel Pnl_Part1;
        private System.Windows.Forms.Label Labl_PostionAxis0;
        private System.Windows.Forms.Button Btn_Axis0;
        private System.Windows.Forms.Panel Pnl_Part2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button Btn_NextPnlPart2;
        private System.Windows.Forms.Button Btn_PreviousPnlPart2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Timer Timer_UpdatePosition;
        private System.Windows.Forms.Label label1;
    }
}