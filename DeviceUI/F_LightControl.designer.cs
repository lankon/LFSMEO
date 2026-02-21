
namespace DeviceUI.LightControl
{
    partial class F_LightControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DGV_Light = new System.Windows.Forms.DataGridView();
            this.Title_LightType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Title_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_Comport = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Title_Station = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_OutPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_Open = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Btn_Load = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Btn_RowDown = new System.Windows.Forms.Button();
            this.Btn_RowUp = new System.Windows.Forms.Button();
            this.Btn_Remove = new System.Windows.Forms.Button();
            this.Btn_Add = new System.Windows.Forms.Button();
            this.Btn_FunctionTest = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Light)).BeginInit();
            this.SuspendLayout();
            // 
            // DGV_Light
            // 
            this.DGV_Light.AllowUserToAddRows = false;
            this.DGV_Light.AllowUserToDeleteRows = false;
            this.DGV_Light.AllowUserToResizeColumns = false;
            this.DGV_Light.AllowUserToResizeRows = false;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV_Light.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle13;
            this.DGV_Light.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Light.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Title_LightType,
            this.Title_Name,
            this.Title_Description,
            this.Title_Comport,
            this.Title_Station,
            this.Title_OutPort,
            this.Title_Value,
            this.Title_Open});
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV_Light.DefaultCellStyle = dataGridViewCellStyle15;
            this.DGV_Light.Location = new System.Drawing.Point(5, 5);
            this.DGV_Light.Name = "DGV_Light";
            this.DGV_Light.RowHeadersVisible = false;
            this.DGV_Light.RowTemplate.Height = 24;
            this.DGV_Light.Size = new System.Drawing.Size(933, 644);
            this.DGV_Light.TabIndex = 14;
            this.DGV_Light.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_Light_CellContentClick);
            this.DGV_Light.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_Light_CellValueChanged);
            // 
            // Title_LightType
            // 
            this.Title_LightType.HeaderText = "Light Type";
            this.Title_LightType.Items.AddRange(new object[] {
            "None",
            "Virtual",
            "FT"});
            this.Title_LightType.Name = "Title_LightType";
            this.Title_LightType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_LightType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Title_LightType.Width = 120;
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
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Title_Comport.DefaultCellStyle = dataGridViewCellStyle14;
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
            "COM12"});
            this.Title_Comport.Name = "Title_Comport";
            this.Title_Comport.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_Comport.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Title_Station
            // 
            this.Title_Station.HeaderText = "Station";
            this.Title_Station.Name = "Title_Station";
            // 
            // Title_OutPort
            // 
            this.Title_OutPort.HeaderText = "Out Port";
            this.Title_OutPort.Name = "Title_OutPort";
            // 
            // Title_Value
            // 
            this.Title_Value.HeaderText = "Value";
            this.Title_Value.Name = "Title_Value";
            this.Title_Value.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Title_Open
            // 
            this.Title_Open.HeaderText = "Open";
            this.Title_Open.Name = "Title_Open";
            this.Title_Open.Text = "Open";
            // 
            // Btn_Load
            // 
            this.Btn_Load.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Load.Location = new System.Drawing.Point(944, 55);
            this.Btn_Load.Name = "Btn_Load";
            this.Btn_Load.Size = new System.Drawing.Size(123, 47);
            this.Btn_Load.TabIndex = 30;
            this.Btn_Load.Text = "Load";
            this.Btn_Load.UseVisualStyleBackColor = true;
            this.Btn_Load.Click += new System.EventHandler(this.Btn_Load_Click);
            // 
            // Btn_Save
            // 
            this.Btn_Save.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Save.Location = new System.Drawing.Point(944, 5);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(123, 47);
            this.Btn_Save.TabIndex = 29;
            this.Btn_Save.Text = "Save";
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Btn_RowDown
            // 
            this.Btn_RowDown.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_RowDown.Location = new System.Drawing.Point(944, 255);
            this.Btn_RowDown.Name = "Btn_RowDown";
            this.Btn_RowDown.Size = new System.Drawing.Size(123, 47);
            this.Btn_RowDown.TabIndex = 28;
            this.Btn_RowDown.Text = "Row Down";
            this.Btn_RowDown.UseVisualStyleBackColor = true;
            this.Btn_RowDown.Click += new System.EventHandler(this.Btn_RowDown_Click);
            // 
            // Btn_RowUp
            // 
            this.Btn_RowUp.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_RowUp.Location = new System.Drawing.Point(944, 205);
            this.Btn_RowUp.Name = "Btn_RowUp";
            this.Btn_RowUp.Size = new System.Drawing.Size(123, 47);
            this.Btn_RowUp.TabIndex = 27;
            this.Btn_RowUp.Text = "Row Up";
            this.Btn_RowUp.UseVisualStyleBackColor = true;
            this.Btn_RowUp.Click += new System.EventHandler(this.Btn_RowUp_Click);
            // 
            // Btn_Remove
            // 
            this.Btn_Remove.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Remove.Location = new System.Drawing.Point(944, 155);
            this.Btn_Remove.Name = "Btn_Remove";
            this.Btn_Remove.Size = new System.Drawing.Size(123, 47);
            this.Btn_Remove.TabIndex = 26;
            this.Btn_Remove.Text = "Remove";
            this.Btn_Remove.UseVisualStyleBackColor = true;
            this.Btn_Remove.Click += new System.EventHandler(this.Btn_Remove_Click);
            // 
            // Btn_Add
            // 
            this.Btn_Add.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Add.Location = new System.Drawing.Point(944, 105);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(123, 47);
            this.Btn_Add.TabIndex = 25;
            this.Btn_Add.Text = "Add";
            this.Btn_Add.UseVisualStyleBackColor = true;
            this.Btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // Btn_FunctionTest
            // 
            this.Btn_FunctionTest.Location = new System.Drawing.Point(1074, 433);
            this.Btn_FunctionTest.Name = "Btn_FunctionTest";
            this.Btn_FunctionTest.Size = new System.Drawing.Size(145, 48);
            this.Btn_FunctionTest.TabIndex = 31;
            this.Btn_FunctionTest.Text = "FunctionTest";
            this.Btn_FunctionTest.UseVisualStyleBackColor = true;
            this.Btn_FunctionTest.Click += new System.EventHandler(this.Btn_FunctionTest_Click);
            // 
            // F_LightControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.Controls.Add(this.Btn_FunctionTest);
            this.Controls.Add(this.Btn_Load);
            this.Controls.Add(this.Btn_Save);
            this.Controls.Add(this.Btn_RowDown);
            this.Controls.Add(this.Btn_RowUp);
            this.Controls.Add(this.Btn_Remove);
            this.Controls.Add(this.Btn_Add);
            this.Controls.Add(this.DGV_Light);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_LightControl";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Light)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView DGV_Light;
        private System.Windows.Forms.Button Btn_Load;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Button Btn_RowDown;
        private System.Windows.Forms.Button Btn_RowUp;
        private System.Windows.Forms.Button Btn_Remove;
        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_LightType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Description;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_Comport;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Station;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_OutPort;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Value;
        private System.Windows.Forms.DataGridViewButtonColumn Title_Open;
        private System.Windows.Forms.Button Btn_FunctionTest;
    }
}