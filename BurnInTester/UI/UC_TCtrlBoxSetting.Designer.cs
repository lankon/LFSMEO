using System.Windows.Forms;

namespace BurnInTester.UI
{
    partial class UC_TCtrlBoxSetting
    {
        private void ForceClearGDI(Control parent)
        {
            if (parent == null) return;

            // 必須從最後一個控制項開始倒著刪除，避免 Index 跑掉
            for (int i = parent.Controls.Count - 1; i >= 0; i--)
            {
                Control ctrl = parent.Controls[i];

                // 遞迴進入下一層 Panel
                if (ctrl.HasChildren)
                {
                    ForceClearGDI(ctrl);
                }

                // 針對大魔王 TextBox 強力處決
                if (ctrl is TextBox)
                {
                    ctrl.Parent = null; // 1. 先切斷父子關係 (最重要的動作)
                    ctrl.Dispose();     // 2. 釋放 GDI 資源
                }
                else
                {
                    parent.Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
            }
        }


        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //ForceClearGDI(LyOutPnl_Main);

                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.LyOutPnl_Main = new System.Windows.Forms.TableLayoutPanel();
            this.Labl_BoxNum = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.TxtBx_Channel = new System.Windows.Forms.TextBox();
            this.TxtBx_Box = new System.Windows.Forms.TextBox();
            this.Cmbx_Use = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LyOutPnl_Main.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // LyOutPnl_Main
            // 
            this.LyOutPnl_Main.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.LyOutPnl_Main.ColumnCount = 1;
            this.LyOutPnl_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.LyOutPnl_Main.Controls.Add(this.Labl_BoxNum, 0, 0);
            this.LyOutPnl_Main.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.LyOutPnl_Main.Location = new System.Drawing.Point(0, 0);
            this.LyOutPnl_Main.Name = "LyOutPnl_Main";
            this.LyOutPnl_Main.RowCount = 2;
            this.LyOutPnl_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.11111F));
            this.LyOutPnl_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 83.88889F));
            this.LyOutPnl_Main.Size = new System.Drawing.Size(198, 174);
            this.LyOutPnl_Main.TabIndex = 4;
            // 
            // Labl_BoxNum
            // 
            this.Labl_BoxNum.BackColor = System.Drawing.Color.Silver;
            this.Labl_BoxNum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Labl_BoxNum.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_BoxNum.Location = new System.Drawing.Point(1, 1);
            this.Labl_BoxNum.Margin = new System.Windows.Forms.Padding(0);
            this.Labl_BoxNum.Name = "Labl_BoxNum";
            this.Labl_BoxNum.Size = new System.Drawing.Size(196, 27);
            this.Labl_BoxNum.TabIndex = 2;
            this.Labl_BoxNum.Text = "Box 1-1";
            this.Labl_BoxNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.TxtBx_Channel, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.TxtBx_Box, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.Cmbx_Use, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 32);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(190, 138);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // TxtBx_Channel
            // 
            this.TxtBx_Channel.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_Channel.Location = new System.Drawing.Point(98, 76);
            this.TxtBx_Channel.Name = "TxtBx_Channel";
            this.TxtBx_Channel.Size = new System.Drawing.Size(88, 29);
            this.TxtBx_Channel.TabIndex = 8;
            // 
            // TxtBx_Box
            // 
            this.TxtBx_Box.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_Box.Location = new System.Drawing.Point(98, 40);
            this.TxtBx_Box.Name = "TxtBx_Box";
            this.TxtBx_Box.Size = new System.Drawing.Size(88, 29);
            this.TxtBx_Box.TabIndex = 4;
            // 
            // Cmbx_Use
            // 
            this.Cmbx_Use.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Cmbx_Use.FormattingEnabled = true;
            this.Cmbx_Use.Items.AddRange(new object[] {
            "PASS",
            "USE"});
            this.Cmbx_Use.Location = new System.Drawing.Point(98, 4);
            this.Cmbx_Use.Name = "Cmbx_Use";
            this.Cmbx_Use.Size = new System.Drawing.Size(88, 28);
            this.Cmbx_Use.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(4, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "Use";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(4, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 35);
            this.label3.TabIndex = 5;
            this.label3.Text = "Ch";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(4, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 35);
            this.label2.TabIndex = 4;
            this.label2.Text = "Box";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UC_TCtrlBoxSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LyOutPnl_Main);
            this.Name = "UC_TCtrlBoxSetting";
            this.Size = new System.Drawing.Size(200, 177);
            this.LyOutPnl_Main.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel LyOutPnl_Main;
        private Label Labl_BoxNum;
        private TableLayoutPanel tableLayoutPanel2;
        private TextBox TxtBx_Channel;
        private TextBox TxtBx_Box;
        private ComboBox Cmbx_Use;
        private Label label1;
        private Label label3;
        private Label label2;
    }
}
