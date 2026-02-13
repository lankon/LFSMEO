
namespace DeviceUI.Motion
{
    partial class F_MotionSetting
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Labl_RDY = new System.Windows.Forms.Label();
            this.Labl_INP = new System.Windows.Forms.Label();
            this.Labl_Servo = new System.Windows.Forms.Label();
            this.Labl_MEL = new System.Windows.Forms.Label();
            this.Labl_PEL = new System.Windows.Forms.Label();
            this.Labl_ORG = new System.Windows.Forms.Label();
            this.Labl_Alarm = new System.Windows.Forms.Label();
            this.Pnl_AxisButton = new System.Windows.Forms.Panel();
            this.Pnl_AxisSetting = new System.Windows.Forms.Panel();
            this.Timer_UpdateStatus = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Location = new System.Drawing.Point(1193, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(128, 559);
            this.panel1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.Labl_RDY, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.Labl_INP, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.Labl_Servo, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.Labl_MEL, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.Labl_PEL, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Labl_ORG, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.Labl_Alarm, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(126, 557);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // Labl_RDY
            // 
            this.Labl_RDY.BackColor = System.Drawing.Color.White;
            this.Labl_RDY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Labl_RDY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Labl_RDY.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_RDY.Location = new System.Drawing.Point(4, 478);
            this.Labl_RDY.Margin = new System.Windows.Forms.Padding(3);
            this.Labl_RDY.Name = "Labl_RDY";
            this.Labl_RDY.Size = new System.Drawing.Size(118, 75);
            this.Labl_RDY.TabIndex = 14;
            this.Labl_RDY.Text = "RDY";
            this.Labl_RDY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_INP
            // 
            this.Labl_INP.BackColor = System.Drawing.Color.White;
            this.Labl_INP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Labl_INP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Labl_INP.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_INP.Location = new System.Drawing.Point(4, 399);
            this.Labl_INP.Margin = new System.Windows.Forms.Padding(3);
            this.Labl_INP.Name = "Labl_INP";
            this.Labl_INP.Size = new System.Drawing.Size(118, 72);
            this.Labl_INP.TabIndex = 10;
            this.Labl_INP.Text = "INP";
            this.Labl_INP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_Servo
            // 
            this.Labl_Servo.BackColor = System.Drawing.Color.White;
            this.Labl_Servo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Labl_Servo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Labl_Servo.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_Servo.Location = new System.Drawing.Point(4, 320);
            this.Labl_Servo.Margin = new System.Windows.Forms.Padding(3);
            this.Labl_Servo.Name = "Labl_Servo";
            this.Labl_Servo.Size = new System.Drawing.Size(118, 72);
            this.Labl_Servo.TabIndex = 11;
            this.Labl_Servo.Text = "Servo";
            this.Labl_Servo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_MEL
            // 
            this.Labl_MEL.BackColor = System.Drawing.Color.White;
            this.Labl_MEL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Labl_MEL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Labl_MEL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Labl_MEL.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_MEL.Location = new System.Drawing.Point(4, 162);
            this.Labl_MEL.Margin = new System.Windows.Forms.Padding(3);
            this.Labl_MEL.Name = "Labl_MEL";
            this.Labl_MEL.Size = new System.Drawing.Size(118, 72);
            this.Labl_MEL.TabIndex = 12;
            this.Labl_MEL.Text = "MEL";
            this.Labl_MEL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_PEL
            // 
            this.Labl_PEL.BackColor = System.Drawing.Color.White;
            this.Labl_PEL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Labl_PEL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Labl_PEL.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_PEL.Location = new System.Drawing.Point(4, 83);
            this.Labl_PEL.Margin = new System.Windows.Forms.Padding(3);
            this.Labl_PEL.Name = "Labl_PEL";
            this.Labl_PEL.Size = new System.Drawing.Size(118, 72);
            this.Labl_PEL.TabIndex = 13;
            this.Labl_PEL.Text = "PEL";
            this.Labl_PEL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_ORG
            // 
            this.Labl_ORG.BackColor = System.Drawing.Color.White;
            this.Labl_ORG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Labl_ORG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Labl_ORG.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_ORG.Location = new System.Drawing.Point(4, 241);
            this.Labl_ORG.Margin = new System.Windows.Forms.Padding(3);
            this.Labl_ORG.Name = "Labl_ORG";
            this.Labl_ORG.Size = new System.Drawing.Size(118, 72);
            this.Labl_ORG.TabIndex = 9;
            this.Labl_ORG.Text = "ORG";
            this.Labl_ORG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_Alarm
            // 
            this.Labl_Alarm.BackColor = System.Drawing.Color.White;
            this.Labl_Alarm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Labl_Alarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Labl_Alarm.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_Alarm.Location = new System.Drawing.Point(4, 4);
            this.Labl_Alarm.Margin = new System.Windows.Forms.Padding(3);
            this.Labl_Alarm.Name = "Labl_Alarm";
            this.Labl_Alarm.Size = new System.Drawing.Size(118, 72);
            this.Labl_Alarm.TabIndex = 8;
            this.Labl_Alarm.Text = "Alarm";
            this.Labl_Alarm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Pnl_AxisButton
            // 
            this.Pnl_AxisButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Pnl_AxisButton.Location = new System.Drawing.Point(3, 568);
            this.Pnl_AxisButton.Name = "Pnl_AxisButton";
            this.Pnl_AxisButton.Size = new System.Drawing.Size(1318, 88);
            this.Pnl_AxisButton.TabIndex = 2;
            // 
            // Pnl_AxisSetting
            // 
            this.Pnl_AxisSetting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Pnl_AxisSetting.Location = new System.Drawing.Point(3, 3);
            this.Pnl_AxisSetting.Name = "Pnl_AxisSetting";
            this.Pnl_AxisSetting.Size = new System.Drawing.Size(1184, 559);
            this.Pnl_AxisSetting.TabIndex = 4;
            // 
            // Timer_UpdateStatus
            // 
            this.Timer_UpdateStatus.Tick += new System.EventHandler(this.Timer_UpdateStatus_Tick);
            // 
            // F_MotionSetting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.Pnl_AxisSetting);
            this.Controls.Add(this.Pnl_AxisButton);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_MotionSetting";
            this.Text = "F_MotionSetting";
            this.VisibleChanged += new System.EventHandler(this.F_Template_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel Pnl_AxisButton;
        private System.Windows.Forms.Label Labl_Alarm;
        private System.Windows.Forms.Label Labl_PEL;
        private System.Windows.Forms.Label Labl_MEL;
        private System.Windows.Forms.Label Labl_Servo;
        private System.Windows.Forms.Label Labl_INP;
        private System.Windows.Forms.Label Labl_ORG;
        private System.Windows.Forms.Label Labl_RDY;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel Pnl_AxisSetting;
        private System.Windows.Forms.Timer Timer_UpdateStatus;
    }
}