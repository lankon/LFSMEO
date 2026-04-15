
namespace BurnInTester.UI
{
    partial class F_TCtrlBox_Setting
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
            this.LyPnl_CtrlBoxStatus = new System.Windows.Forms.TableLayoutPanel();
            this.CtrlBoxStatus1 = new BurnInTester.UI.UC_CtrlBoxStatus();
            this.uC_TCtrlBoxSetting1 = new BurnInTester.UI.UC_TCtrlBoxSetting();
            this.LyPnl_CtrlBoxStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // LyPnl_CtrlBoxStatus
            // 
            this.LyPnl_CtrlBoxStatus.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.LyPnl_CtrlBoxStatus.ColumnCount = 4;
            this.LyPnl_CtrlBoxStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LyPnl_CtrlBoxStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LyPnl_CtrlBoxStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LyPnl_CtrlBoxStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LyPnl_CtrlBoxStatus.Controls.Add(this.CtrlBoxStatus1, 0, 0);
            this.LyPnl_CtrlBoxStatus.Controls.Add(this.uC_TCtrlBoxSetting1, 1, 3);
            this.LyPnl_CtrlBoxStatus.Location = new System.Drawing.Point(12, 12);
            this.LyPnl_CtrlBoxStatus.Name = "LyPnl_CtrlBoxStatus";
            this.LyPnl_CtrlBoxStatus.RowCount = 10;
            this.LyPnl_CtrlBoxStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxStatus.Size = new System.Drawing.Size(653, 943);
            this.LyPnl_CtrlBoxStatus.TabIndex = 1;
            // 
            // CtrlBoxStatus1
            // 
            this.CtrlBoxStatus1.Location = new System.Drawing.Point(4, 4);
            this.CtrlBoxStatus1.Name = "CtrlBoxStatus1";
            this.CtrlBoxStatus1.Size = new System.Drawing.Size(156, 87);
            this.CtrlBoxStatus1.TabIndex = 0;
            // 
            // uC_TCtrlBoxSetting1
            // 
            this.uC_TCtrlBoxSetting1.Location = new System.Drawing.Point(167, 286);
            this.uC_TCtrlBoxSetting1.Name = "uC_TCtrlBoxSetting1";
            this.uC_TCtrlBoxSetting1.Size = new System.Drawing.Size(156, 87);
            this.uC_TCtrlBoxSetting1.TabIndex = 1;
            // 
            // F_TCtrlBox_Setting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1896, 967);
            this.Controls.Add(this.LyPnl_CtrlBoxStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_TCtrlBox_Setting";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.LyPnl_CtrlBoxStatus.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel LyPnl_CtrlBoxStatus;
        private BurnInTester.UI.UC_CtrlBoxStatus CtrlBoxStatus1;
        private UC_TCtrlBoxSetting uC_TCtrlBoxSetting1;
    }
}