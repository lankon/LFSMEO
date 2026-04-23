using System.Windows.Forms;

namespace BurnInTester.UI
{
    partial class UC_ShowTemperatureValue
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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.Labl_BoxNum = new System.Windows.Forms.Label();
            this.Labl_SV = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.Labl_PV9 = new System.Windows.Forms.Label();
            this.Labl_PV8 = new System.Windows.Forms.Label();
            this.Labl_PV7 = new System.Windows.Forms.Label();
            this.Labl_PV6 = new System.Windows.Forms.Label();
            this.Labl_PV4 = new System.Windows.Forms.Label();
            this.Labl_PV3 = new System.Windows.Forms.Label();
            this.Labl_PV2 = new System.Windows.Forms.Label();
            this.Labl_PV1 = new System.Windows.Forms.Label();
            this.Labl_PV5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.Labl_BoxNum, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.Labl_SV, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 3);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 99F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(226, 196);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // Labl_BoxNum
            // 
            this.Labl_BoxNum.BackColor = System.Drawing.Color.Silver;
            this.tableLayoutPanel2.SetColumnSpan(this.Labl_BoxNum, 2);
            this.Labl_BoxNum.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_BoxNum.Location = new System.Drawing.Point(1, 1);
            this.Labl_BoxNum.Margin = new System.Windows.Forms.Padding(0);
            this.Labl_BoxNum.Name = "Labl_BoxNum";
            this.Labl_BoxNum.Size = new System.Drawing.Size(224, 27);
            this.Labl_BoxNum.TabIndex = 0;
            this.Labl_BoxNum.Text = "Box 1-1";
            this.Labl_BoxNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_SV
            // 
            this.Labl_SV.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_SV.Location = new System.Drawing.Point(116, 29);
            this.Labl_SV.Name = "Labl_SV";
            this.Labl_SV.Size = new System.Drawing.Size(106, 27);
            this.Labl_SV.TabIndex = 3;
            this.Labl_SV.Text = "000.00";
            this.Labl_SV.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(4, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 27);
            this.label5.TabIndex = 2;
            this.label5.Text = "SV";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.label6, 2);
            this.label6.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.Location = new System.Drawing.Point(4, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(218, 27);
            this.label6.TabIndex = 1;
            this.label6.Text = "PV";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel3, 2);
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Controls.Add(this.Labl_PV9, 2, 2);
            this.tableLayoutPanel3.Controls.Add(this.Labl_PV8, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.Labl_PV7, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.Labl_PV6, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.Labl_PV4, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.Labl_PV3, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.Labl_PV2, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.Labl_PV1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.Labl_PV5, 1, 1);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(1, 85);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(224, 108);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // Labl_PV9
            // 
            this.Labl_PV9.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_PV9.Location = new System.Drawing.Point(152, 71);
            this.Labl_PV9.Name = "Labl_PV9";
            this.Labl_PV9.Size = new System.Drawing.Size(67, 35);
            this.Labl_PV9.TabIndex = 12;
            this.Labl_PV9.Text = "-.-";
            this.Labl_PV9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_PV8
            // 
            this.Labl_PV8.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_PV8.Location = new System.Drawing.Point(78, 71);
            this.Labl_PV8.Name = "Labl_PV8";
            this.Labl_PV8.Size = new System.Drawing.Size(67, 35);
            this.Labl_PV8.TabIndex = 11;
            this.Labl_PV8.Text = "-.-";
            this.Labl_PV8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_PV7
            // 
            this.Labl_PV7.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_PV7.Location = new System.Drawing.Point(4, 71);
            this.Labl_PV7.Name = "Labl_PV7";
            this.Labl_PV7.Size = new System.Drawing.Size(67, 35);
            this.Labl_PV7.TabIndex = 10;
            this.Labl_PV7.Text = "-.-";
            this.Labl_PV7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_PV6
            // 
            this.Labl_PV6.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_PV6.Location = new System.Drawing.Point(152, 36);
            this.Labl_PV6.Name = "Labl_PV6";
            this.Labl_PV6.Size = new System.Drawing.Size(67, 34);
            this.Labl_PV6.TabIndex = 9;
            this.Labl_PV6.Text = "-.-";
            this.Labl_PV6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_PV4
            // 
            this.Labl_PV4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_PV4.Location = new System.Drawing.Point(4, 36);
            this.Labl_PV4.Name = "Labl_PV4";
            this.Labl_PV4.Size = new System.Drawing.Size(67, 34);
            this.Labl_PV4.TabIndex = 8;
            this.Labl_PV4.Text = "-.-";
            this.Labl_PV4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_PV3
            // 
            this.Labl_PV3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_PV3.Location = new System.Drawing.Point(152, 1);
            this.Labl_PV3.Name = "Labl_PV3";
            this.Labl_PV3.Size = new System.Drawing.Size(67, 34);
            this.Labl_PV3.TabIndex = 7;
            this.Labl_PV3.Text = "-.-";
            this.Labl_PV3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_PV2
            // 
            this.Labl_PV2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_PV2.Location = new System.Drawing.Point(78, 1);
            this.Labl_PV2.Name = "Labl_PV2";
            this.Labl_PV2.Size = new System.Drawing.Size(67, 34);
            this.Labl_PV2.TabIndex = 6;
            this.Labl_PV2.Text = "-.-";
            this.Labl_PV2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_PV1
            // 
            this.Labl_PV1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_PV1.Location = new System.Drawing.Point(4, 1);
            this.Labl_PV1.Name = "Labl_PV1";
            this.Labl_PV1.Size = new System.Drawing.Size(67, 34);
            this.Labl_PV1.TabIndex = 5;
            this.Labl_PV1.Text = "-.-";
            this.Labl_PV1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_PV5
            // 
            this.Labl_PV5.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_PV5.Location = new System.Drawing.Point(78, 36);
            this.Labl_PV5.Name = "Labl_PV5";
            this.Labl_PV5.Size = new System.Drawing.Size(67, 34);
            this.Labl_PV5.TabIndex = 4;
            this.Labl_PV5.Text = "-.-";
            this.Labl_PV5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UC_ShowTemperatureValue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "UC_ShowTemperatureValue";
            this.Size = new System.Drawing.Size(228, 201);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel2;
        private Label Labl_BoxNum;
        private Label Labl_SV;
        private Label label5;
        private Label label6;
        private TableLayoutPanel tableLayoutPanel3;
        private Label Labl_PV9;
        private Label Labl_PV8;
        private Label Labl_PV7;
        private Label Labl_PV6;
        private Label Labl_PV4;
        private Label Labl_PV3;
        private Label Labl_PV2;
        private Label Labl_PV1;
        private Label Labl_PV5;
    }
}
