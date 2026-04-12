
namespace DeviceUI.Spectrometer
{
    partial class F_Spectrometer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DGV_Spectrum = new System.Windows.Forms.DataGridView();
            this.Title_SpectrumType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Title_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_IntegralTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_GetSpectrum = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Btn_Load = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Btn_Remove = new System.Windows.Forms.Button();
            this.Btn_Add = new System.Windows.Forms.Button();
            this.Btn_FunctionTest = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Spectrum)).BeginInit();
            this.SuspendLayout();
            // 
            // DGV_Spectrum
            // 
            this.DGV_Spectrum.AllowUserToAddRows = false;
            this.DGV_Spectrum.AllowUserToDeleteRows = false;
            this.DGV_Spectrum.AllowUserToResizeColumns = false;
            this.DGV_Spectrum.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV_Spectrum.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DGV_Spectrum.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Spectrum.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Title_SpectrumType,
            this.Title_Name,
            this.Title_ID,
            this.Title_IntegralTime,
            this.Title_GetSpectrum});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV_Spectrum.DefaultCellStyle = dataGridViewCellStyle2;
            this.DGV_Spectrum.Location = new System.Drawing.Point(5, 5);
            this.DGV_Spectrum.Name = "DGV_Spectrum";
            this.DGV_Spectrum.RowHeadersVisible = false;
            this.DGV_Spectrum.RowTemplate.Height = 24;
            this.DGV_Spectrum.Size = new System.Drawing.Size(967, 206);
            this.DGV_Spectrum.TabIndex = 14;
            this.DGV_Spectrum.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_Spectrum_CellContentClick);
            this.DGV_Spectrum.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_Spectrum_CellEnter);
            // 
            // Title_SpectrumType
            // 
            this.Title_SpectrumType.HeaderText = "Spectrum Type";
            this.Title_SpectrumType.Items.AddRange(new object[] {
            "None",
            "VIRTUAL",
            "OTO"});
            this.Title_SpectrumType.Name = "Title_SpectrumType";
            this.Title_SpectrumType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_SpectrumType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Title_SpectrumType.Width = 200;
            // 
            // Title_Name
            // 
            this.Title_Name.HeaderText = "Name";
            this.Title_Name.Name = "Title_Name";
            this.Title_Name.Width = 200;
            // 
            // Title_ID
            // 
            this.Title_ID.HeaderText = "ID";
            this.Title_ID.Name = "Title_ID";
            this.Title_ID.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_ID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Title_ID.Width = 200;
            // 
            // Title_IntegralTime
            // 
            this.Title_IntegralTime.HeaderText = "IntegralTime(ms)";
            this.Title_IntegralTime.Name = "Title_IntegralTime";
            this.Title_IntegralTime.Width = 150;
            // 
            // Title_GetSpectrum
            // 
            this.Title_GetSpectrum.HeaderText = "Get Spectrum";
            this.Title_GetSpectrum.Name = "Title_GetSpectrum";
            this.Title_GetSpectrum.Width = 200;
            // 
            // Btn_Load
            // 
            this.Btn_Load.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Load.Location = new System.Drawing.Point(978, 58);
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
            this.Btn_Save.Location = new System.Drawing.Point(978, 5);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(123, 47);
            this.Btn_Save.TabIndex = 29;
            this.Btn_Save.Text = "Save";
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Btn_Remove
            // 
            this.Btn_Remove.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Remove.Location = new System.Drawing.Point(978, 164);
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
            this.Btn_Add.Location = new System.Drawing.Point(978, 111);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(123, 47);
            this.Btn_Add.TabIndex = 25;
            this.Btn_Add.Text = "Add";
            this.Btn_Add.UseVisualStyleBackColor = true;
            this.Btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // Btn_FunctionTest
            // 
            this.Btn_FunctionTest.Location = new System.Drawing.Point(1026, 375);
            this.Btn_FunctionTest.Name = "Btn_FunctionTest";
            this.Btn_FunctionTest.Size = new System.Drawing.Size(145, 48);
            this.Btn_FunctionTest.TabIndex = 31;
            this.Btn_FunctionTest.Text = "FunctionTest";
            this.Btn_FunctionTest.UseVisualStyleBackColor = true;
            this.Btn_FunctionTest.Visible = false;
            this.Btn_FunctionTest.Click += new System.EventHandler(this.Btn_FunctionTest_Click);
            // 
            // F_Spectrometer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.Controls.Add(this.Btn_FunctionTest);
            this.Controls.Add(this.Btn_Load);
            this.Controls.Add(this.Btn_Save);
            this.Controls.Add(this.Btn_Remove);
            this.Controls.Add(this.Btn_Add);
            this.Controls.Add(this.DGV_Spectrum);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_Spectrometer";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Spectrometer_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Spectrum)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView DGV_Spectrum;
        private System.Windows.Forms.Button Btn_Load;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Button Btn_Remove;
        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.Button Btn_FunctionTest;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_SpectrumType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_IntegralTime;
        private System.Windows.Forms.DataGridViewButtonColumn Title_GetSpectrum;
    }
}