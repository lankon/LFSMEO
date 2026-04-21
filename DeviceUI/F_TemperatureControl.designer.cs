
namespace DeviceUI.TemperatureControl
{
    partial class F_TemperatureControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DGV_Temperature = new System.Windows.Forms.DataGridView();
            this.Btn_Load = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Btn_RowDown = new System.Windows.Forms.Button();
            this.Btn_RowUp = new System.Windows.Forms.Button();
            this.Btn_Remove = new System.Windows.Forms.Button();
            this.Btn_Add = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Title_TC_Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Title_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_Comport = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Title_BaudRate = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Title_DataBits = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Title_StopBits = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Title_Parity = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Temperature)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DGV_Temperature
            // 
            this.DGV_Temperature.AllowUserToAddRows = false;
            this.DGV_Temperature.AllowUserToDeleteRows = false;
            this.DGV_Temperature.AllowUserToResizeColumns = false;
            this.DGV_Temperature.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV_Temperature.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DGV_Temperature.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Temperature.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Title_TC_Type,
            this.Title_Name,
            this.Title_Description,
            this.Title_Comport,
            this.Title_BaudRate,
            this.Title_DataBits,
            this.Title_StopBits,
            this.Title_Parity});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV_Temperature.DefaultCellStyle = dataGridViewCellStyle3;
            this.DGV_Temperature.Location = new System.Drawing.Point(12, 12);
            this.DGV_Temperature.Name = "DGV_Temperature";
            this.DGV_Temperature.RowHeadersVisible = false;
            this.DGV_Temperature.RowTemplate.Height = 24;
            this.DGV_Temperature.Size = new System.Drawing.Size(1084, 644);
            this.DGV_Temperature.TabIndex = 15;
            this.DGV_Temperature.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_Temperature_CellValueChanged);
            // 
            // Btn_Load
            // 
            this.Btn_Load.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Load.Location = new System.Drawing.Point(3, 66);
            this.Btn_Load.Name = "Btn_Load";
            this.Btn_Load.Size = new System.Drawing.Size(131, 57);
            this.Btn_Load.TabIndex = 36;
            this.Btn_Load.Text = "Load";
            this.Btn_Load.UseVisualStyleBackColor = true;
            this.Btn_Load.Click += new System.EventHandler(this.Btn_Load_Click);
            // 
            // Btn_Save
            // 
            this.Btn_Save.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Save.Location = new System.Drawing.Point(3, 3);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(131, 57);
            this.Btn_Save.TabIndex = 35;
            this.Btn_Save.Text = "Save";
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Btn_RowDown
            // 
            this.Btn_RowDown.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_RowDown.Location = new System.Drawing.Point(3, 318);
            this.Btn_RowDown.Name = "Btn_RowDown";
            this.Btn_RowDown.Size = new System.Drawing.Size(131, 57);
            this.Btn_RowDown.TabIndex = 34;
            this.Btn_RowDown.Text = "Row Down";
            this.Btn_RowDown.UseVisualStyleBackColor = true;
            this.Btn_RowDown.Click += new System.EventHandler(this.Btn_RowDown_Click);
            // 
            // Btn_RowUp
            // 
            this.Btn_RowUp.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_RowUp.Location = new System.Drawing.Point(3, 255);
            this.Btn_RowUp.Name = "Btn_RowUp";
            this.Btn_RowUp.Size = new System.Drawing.Size(131, 57);
            this.Btn_RowUp.TabIndex = 33;
            this.Btn_RowUp.Text = "Row Up";
            this.Btn_RowUp.UseVisualStyleBackColor = true;
            this.Btn_RowUp.Click += new System.EventHandler(this.Btn_RowUp_Click);
            // 
            // Btn_Remove
            // 
            this.Btn_Remove.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Remove.Location = new System.Drawing.Point(3, 192);
            this.Btn_Remove.Name = "Btn_Remove";
            this.Btn_Remove.Size = new System.Drawing.Size(131, 57);
            this.Btn_Remove.TabIndex = 32;
            this.Btn_Remove.Text = "Remove";
            this.Btn_Remove.UseVisualStyleBackColor = true;
            this.Btn_Remove.Click += new System.EventHandler(this.Btn_Remove_Click);
            // 
            // Btn_Add
            // 
            this.Btn_Add.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Add.Location = new System.Drawing.Point(3, 129);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(131, 57);
            this.Btn_Add.TabIndex = 31;
            this.Btn_Add.Text = "Add";
            this.Btn_Add.UseVisualStyleBackColor = true;
            this.Btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.Btn_Save, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Btn_RowDown, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.Btn_Load, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Btn_RowUp, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.Btn_Add, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.Btn_Remove, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1102, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(137, 378);
            this.tableLayoutPanel1.TabIndex = 37;
            // 
            // Title_TC_Type
            // 
            this.Title_TC_Type.HeaderText = "TC Type";
            this.Title_TC_Type.Items.AddRange(new object[] {
            "None",
            "VIRTUAL",
            "Guishan_AMIDA_WIN",
            "FT"});
            this.Title_TC_Type.Name = "Title_TC_Type";
            this.Title_TC_Type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_TC_Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Title_TC_Type.Width = 120;
            // 
            // Title_Name
            // 
            this.Title_Name.HeaderText = "Name";
            this.Title_Name.Name = "Title_Name";
            this.Title_Name.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_Name.Width = 150;
            // 
            // Title_Description
            // 
            this.Title_Description.HeaderText = "Description";
            this.Title_Description.Name = "Title_Description";
            this.Title_Description.Width = 150;
            // 
            // Title_Comport
            // 
            this.Title_Comport.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Title_Comport.DefaultCellStyle = dataGridViewCellStyle2;
            this.Title_Comport.HeaderText = "Comport";
            this.Title_Comport.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10",
            "COM11",
            "COM12",
            "COM13",
            "COM14"});
            this.Title_Comport.Name = "Title_Comport";
            this.Title_Comport.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_Comport.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Title_Comport.Width = 130;
            // 
            // Title_BaudRate
            // 
            this.Title_BaudRate.HeaderText = "BaudRate";
            this.Title_BaudRate.Items.AddRange(new object[] {
            "2400",
            "9600",
            "19200",
            "38400",
            "115200"});
            this.Title_BaudRate.Name = "Title_BaudRate";
            this.Title_BaudRate.Width = 130;
            // 
            // Title_DataBits
            // 
            this.Title_DataBits.HeaderText = "DataBits";
            this.Title_DataBits.Items.AddRange(new object[] {
            "7",
            "8"});
            this.Title_DataBits.Name = "Title_DataBits";
            this.Title_DataBits.Width = 130;
            // 
            // Title_StopBits
            // 
            this.Title_StopBits.HeaderText = "StopBits";
            this.Title_StopBits.Items.AddRange(new object[] {
            "None",
            "One",
            "Two",
            "OnePointFive"});
            this.Title_StopBits.Name = "Title_StopBits";
            this.Title_StopBits.Width = 130;
            // 
            // Title_Parity
            // 
            this.Title_Parity.HeaderText = "Parity";
            this.Title_Parity.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even",
            "Mark",
            "Space"});
            this.Title_Parity.Name = "Title_Parity";
            this.Title_Parity.Width = 130;
            // 
            // F_TemperatureControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1896, 967);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.DGV_Temperature);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_TemperatureControl";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Temperature)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DGV_Temperature;
        private System.Windows.Forms.Button Btn_Load;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Button Btn_RowDown;
        private System.Windows.Forms.Button Btn_RowUp;
        private System.Windows.Forms.Button Btn_Remove;
        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_TC_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Description;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_Comport;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_BaudRate;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_DataBits;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_StopBits;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_Parity;
    }
}