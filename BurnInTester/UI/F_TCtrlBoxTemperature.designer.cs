
namespace BurnInTester.UI
{
    partial class F_TCtrlBoxTemperature
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
            this.LyPnl_CtrlBoxSetting = new System.Windows.Forms.TableLayoutPanel();
            this.LyPnl_CtrlBoxSetting1 = new System.Windows.Forms.TableLayoutPanel();
            this.Tm_UpdatePV = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // LyPnl_CtrlBoxSetting
            // 
            this.LyPnl_CtrlBoxSetting.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.LyPnl_CtrlBoxSetting.ColumnCount = 4;
            this.LyPnl_CtrlBoxSetting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LyPnl_CtrlBoxSetting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LyPnl_CtrlBoxSetting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LyPnl_CtrlBoxSetting.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LyPnl_CtrlBoxSetting.Location = new System.Drawing.Point(5, 4);
            this.LyPnl_CtrlBoxSetting.Name = "LyPnl_CtrlBoxSetting";
            this.LyPnl_CtrlBoxSetting.RowCount = 5;
            this.LyPnl_CtrlBoxSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxSetting.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxSetting.Size = new System.Drawing.Size(933, 960);
            this.LyPnl_CtrlBoxSetting.TabIndex = 1;
            // 
            // LyPnl_CtrlBoxSetting1
            // 
            this.LyPnl_CtrlBoxSetting1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.LyPnl_CtrlBoxSetting1.ColumnCount = 4;
            this.LyPnl_CtrlBoxSetting1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LyPnl_CtrlBoxSetting1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LyPnl_CtrlBoxSetting1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LyPnl_CtrlBoxSetting1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.LyPnl_CtrlBoxSetting1.Location = new System.Drawing.Point(948, 4);
            this.LyPnl_CtrlBoxSetting1.Name = "LyPnl_CtrlBoxSetting1";
            this.LyPnl_CtrlBoxSetting1.RowCount = 5;
            this.LyPnl_CtrlBoxSetting1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxSetting1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxSetting1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxSetting1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxSetting1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.LyPnl_CtrlBoxSetting1.Size = new System.Drawing.Size(933, 960);
            this.LyPnl_CtrlBoxSetting1.TabIndex = 2;
            // 
            // Tm_UpdatePV
            // 
            this.Tm_UpdatePV.Enabled = true;
            this.Tm_UpdatePV.Tick += new System.EventHandler(this.Tm_UpdatePV_Tick);
            // 
            // F_TCtrlBoxTemperature
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1896, 967);
            this.Controls.Add(this.LyPnl_CtrlBoxSetting1);
            this.Controls.Add(this.LyPnl_CtrlBoxSetting);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_TCtrlBoxTemperature";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel LyPnl_CtrlBoxSetting;
        private System.Windows.Forms.TableLayoutPanel LyPnl_CtrlBoxSetting1;
        private System.Windows.Forms.Timer Tm_UpdatePV;
    }
}