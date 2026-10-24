
namespace RGBTester.UI
{
    partial class F_Equipment_Setting
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Cmbx_MachineType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Cmbx_ShowFormName = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.Cmbx_OpticalModule = new System.Windows.Forms.ComboBox();
            this.Cmbx_ElectricalModule = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 68.64989F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.35011F));
            this.tableLayoutPanel1.Controls.Add(this.Cmbx_MachineType, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(440, 40);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // Cmbx_MachineType
            // 
            this.Cmbx_MachineType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Cmbx_MachineType.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Cmbx_MachineType.FormattingEnabled = true;
            this.Cmbx_MachineType.Items.AddRange(new object[] {
            "NONE",
            "ProbeTester",
            "RGBTester",
            "BurnInTester",
            "DETester"});
            this.Cmbx_MachineType.Location = new System.Drawing.Point(304, 6);
            this.Cmbx_MachineType.Name = "Cmbx_MachineType";
            this.Cmbx_MachineType.Size = new System.Drawing.Size(130, 28);
            this.Cmbx_MachineType.TabIndex = 6;
            this.Cmbx_MachineType.Tag = "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(6, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(289, 34);
            this.label3.TabIndex = 6;
            this.label3.Text = "Machine";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(6, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(289, 35);
            this.label1.TabIndex = 7;
            this.label1.Text = "Debug Show Form Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Cmbx_ShowFormName
            // 
            this.Cmbx_ShowFormName.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Cmbx_ShowFormName.FormattingEnabled = true;
            this.Cmbx_ShowFormName.Items.AddRange(new object[] {
            "NoUse",
            "Use"});
            this.Cmbx_ShowFormName.Location = new System.Drawing.Point(305, 146);
            this.Cmbx_ShowFormName.Name = "Cmbx_ShowFormName";
            this.Cmbx_ShowFormName.Size = new System.Drawing.Size(129, 28);
            this.Cmbx_ShowFormName.TabIndex = 8;
            this.Cmbx_ShowFormName.Tag = "0";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 296F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel2.Controls.Add(this.Cmbx_OpticalModule, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.Cmbx_ElectricalModule, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.Cmbx_ShowFormName, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 74);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(440, 252);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // Cmbx_OpticalModule
            // 
            this.Cmbx_OpticalModule.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Cmbx_OpticalModule.FormattingEnabled = true;
            this.Cmbx_OpticalModule.Items.AddRange(new object[] {
            "NoUse",
            "Use"});
            this.Cmbx_OpticalModule.Location = new System.Drawing.Point(305, 70);
            this.Cmbx_OpticalModule.Name = "Cmbx_OpticalModule";
            this.Cmbx_OpticalModule.Size = new System.Drawing.Size(129, 28);
            this.Cmbx_OpticalModule.TabIndex = 13;
            this.Cmbx_OpticalModule.Tag = "0";
            // 
            // Cmbx_ElectricalModule
            // 
            this.Cmbx_ElectricalModule.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Cmbx_ElectricalModule.FormattingEnabled = true;
            this.Cmbx_ElectricalModule.Items.AddRange(new object[] {
            "NoUse",
            "Use"});
            this.Cmbx_ElectricalModule.Location = new System.Drawing.Point(305, 32);
            this.Cmbx_ElectricalModule.Name = "Cmbx_ElectricalModule";
            this.Cmbx_ElectricalModule.Size = new System.Drawing.Size(129, 28);
            this.Cmbx_ElectricalModule.TabIndex = 12;
            this.Cmbx_ElectricalModule.Tag = "0";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(6, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(289, 35);
            this.label4.TabIndex = 10;
            this.label4.Text = "Electrical";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.tableLayoutPanel2.SetColumnSpan(this.label2, 2);
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(428, 23);
            this.label2.TabIndex = 9;
            this.label2.Text = "Equipment Module";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(6, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(289, 35);
            this.label5.TabIndex = 11;
            this.label5.Text = "Optical";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // F_Equipment_Setting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_Equipment_Setting";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox Cmbx_MachineType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox Cmbx_ShowFormName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox Cmbx_OpticalModule;
        private System.Windows.Forms.ComboBox Cmbx_ElectricalModule;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}