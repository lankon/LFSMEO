
namespace DeviceUI.IO
{
    partial class F_IO_Card
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DGV_IO = new System.Windows.Forms.DataGridView();
            this.Title_IO = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Title_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_CardType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Title_IO_Num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_Range = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Title_Inverse = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Title_CardNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_LineNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_DevNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Btn_RowDown = new System.Windows.Forms.Button();
            this.Btn_RowUp = new System.Windows.Forms.Button();
            this.Btn_Remove = new System.Windows.Forms.Button();
            this.Btn_Add = new System.Windows.Forms.Button();
            this.Btn_Load = new System.Windows.Forms.Button();
            this.Timer_IO = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_IO)).BeginInit();
            this.SuspendLayout();
            // 
            // DGV_IO
            // 
            this.DGV_IO.AllowUserToAddRows = false;
            this.DGV_IO.AllowUserToDeleteRows = false;
            this.DGV_IO.AllowUserToResizeColumns = false;
            this.DGV_IO.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV_IO.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DGV_IO.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_IO.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Title_IO,
            this.Title_Name,
            this.Title_Description,
            this.Title_CardType,
            this.Title_IO_Num,
            this.Title_Status,
            this.Title_Range,
            this.Title_Inverse,
            this.Title_CardNum,
            this.Title_LineNum,
            this.Title_DevNum});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV_IO.DefaultCellStyle = dataGridViewCellStyle3;
            this.DGV_IO.Location = new System.Drawing.Point(5, 5);
            this.DGV_IO.Name = "DGV_IO";
            this.DGV_IO.RowHeadersVisible = false;
            this.DGV_IO.RowTemplate.Height = 24;
            this.DGV_IO.Size = new System.Drawing.Size(1189, 653);
            this.DGV_IO.TabIndex = 13;
            this.DGV_IO.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_IO_CellClick);
            this.DGV_IO.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_IO_CellValueChanged);
            // 
            // Title_IO
            // 
            this.Title_IO.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Title_IO.DefaultCellStyle = dataGridViewCellStyle2;
            this.Title_IO.HeaderText = "I/O";
            this.Title_IO.Items.AddRange(new object[] {
            "None",
            "Input",
            "Output"});
            this.Title_IO.Name = "Title_IO";
            this.Title_IO.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_IO.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Title_Name
            // 
            this.Title_Name.HeaderText = "Name";
            this.Title_Name.Name = "Title_Name";
            this.Title_Name.Width = 162;
            // 
            // Title_Description
            // 
            this.Title_Description.HeaderText = "Description";
            this.Title_Description.Name = "Title_Description";
            this.Title_Description.Width = 162;
            // 
            // Title_CardType
            // 
            this.Title_CardType.HeaderText = "Card Type";
            this.Title_CardType.Items.AddRange(new object[] {
            "None",
            "Virtual",
            "AMP_204C",
            "MN200",
            "PCI_9111DG",
            "P32C32",
            "PCI_9111HR"});
            this.Title_CardType.Name = "Title_CardType";
            this.Title_CardType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_CardType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Title_CardType.Width = 120;
            // 
            // Title_IO_Num
            // 
            this.Title_IO_Num.HeaderText = "I/O No.";
            this.Title_IO_Num.Name = "Title_IO_Num";
            this.Title_IO_Num.Width = 120;
            // 
            // Title_Status
            // 
            this.Title_Status.HeaderText = "Status";
            this.Title_Status.Name = "Title_Status";
            this.Title_Status.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Title_Range
            // 
            this.Title_Range.HeaderText = "Range";
            this.Title_Range.Items.AddRange(new object[] {
            "Digital",
            "+-5V"});
            this.Title_Range.Name = "Title_Range";
            // 
            // Title_Inverse
            // 
            this.Title_Inverse.HeaderText = "Inverse";
            this.Title_Inverse.Name = "Title_Inverse";
            this.Title_Inverse.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_Inverse.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Title_CardNum
            // 
            this.Title_CardNum.HeaderText = "Card No.";
            this.Title_CardNum.Name = "Title_CardNum";
            this.Title_CardNum.Width = 120;
            // 
            // Title_LineNum
            // 
            this.Title_LineNum.HeaderText = "Line No.";
            this.Title_LineNum.Name = "Title_LineNum";
            // 
            // Title_DevNum
            // 
            this.Title_DevNum.HeaderText = "Dev No.";
            this.Title_DevNum.Name = "Title_DevNum";
            // 
            // Btn_Save
            // 
            this.Btn_Save.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Save.Location = new System.Drawing.Point(1200, 5);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(123, 47);
            this.Btn_Save.TabIndex = 22;
            this.Btn_Save.Text = "Save";
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Btn_RowDown
            // 
            this.Btn_RowDown.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_RowDown.Location = new System.Drawing.Point(1200, 255);
            this.Btn_RowDown.Name = "Btn_RowDown";
            this.Btn_RowDown.Size = new System.Drawing.Size(123, 47);
            this.Btn_RowDown.TabIndex = 21;
            this.Btn_RowDown.Text = "Row Down";
            this.Btn_RowDown.UseVisualStyleBackColor = true;
            this.Btn_RowDown.Click += new System.EventHandler(this.Btn_RowDown_Click);
            // 
            // Btn_RowUp
            // 
            this.Btn_RowUp.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_RowUp.Location = new System.Drawing.Point(1200, 205);
            this.Btn_RowUp.Name = "Btn_RowUp";
            this.Btn_RowUp.Size = new System.Drawing.Size(123, 47);
            this.Btn_RowUp.TabIndex = 20;
            this.Btn_RowUp.Text = "Row Up";
            this.Btn_RowUp.UseVisualStyleBackColor = true;
            this.Btn_RowUp.Click += new System.EventHandler(this.Btn_RowUp_Click);
            // 
            // Btn_Remove
            // 
            this.Btn_Remove.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Remove.Location = new System.Drawing.Point(1200, 155);
            this.Btn_Remove.Name = "Btn_Remove";
            this.Btn_Remove.Size = new System.Drawing.Size(123, 47);
            this.Btn_Remove.TabIndex = 19;
            this.Btn_Remove.Text = "Remove";
            this.Btn_Remove.UseVisualStyleBackColor = true;
            this.Btn_Remove.Click += new System.EventHandler(this.Btn_Remove_Click);
            // 
            // Btn_Add
            // 
            this.Btn_Add.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Add.Location = new System.Drawing.Point(1200, 105);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(123, 47);
            this.Btn_Add.TabIndex = 18;
            this.Btn_Add.Text = "Add";
            this.Btn_Add.UseVisualStyleBackColor = true;
            this.Btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // Btn_Load
            // 
            this.Btn_Load.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Load.Location = new System.Drawing.Point(1200, 55);
            this.Btn_Load.Name = "Btn_Load";
            this.Btn_Load.Size = new System.Drawing.Size(123, 47);
            this.Btn_Load.TabIndex = 24;
            this.Btn_Load.Text = "Load";
            this.Btn_Load.UseVisualStyleBackColor = true;
            this.Btn_Load.Click += new System.EventHandler(this.Btn_Load_Click);
            // 
            // Timer_IO
            // 
            this.Timer_IO.Interval = 150;
            this.Timer_IO.Tick += new System.EventHandler(this.Timer_IO_Tick);
            // 
            // F_IO_Card
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.Btn_Load);
            this.Controls.Add(this.Btn_Save);
            this.Controls.Add(this.Btn_RowDown);
            this.Controls.Add(this.Btn_RowUp);
            this.Controls.Add(this.Btn_Remove);
            this.Controls.Add(this.Btn_Add);
            this.Controls.Add(this.DGV_IO);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_IO_Card";
            this.Text = "F_IO_Card";
            this.VisibleChanged += new System.EventHandler(this.F_IO_Card_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_IO)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DGV_IO;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Button Btn_RowDown;
        private System.Windows.Forms.Button Btn_RowUp;
        private System.Windows.Forms.Button Btn_Remove;
        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.Button Btn_Load;
        private System.Windows.Forms.Timer Timer_IO;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_IO;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Description;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_CardType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_IO_Num;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Status;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_Range;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Title_Inverse;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_CardNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_LineNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_DevNum;
    }
}