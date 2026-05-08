
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.Plot_Spectrom = new ScottPlot.FormsPlot();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtBx_IntgralTime = new System.Windows.Forms.TextBox();
            this.Btn_Capture = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Spectrum)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DGV_Spectrum
            // 
            this.DGV_Spectrum.AllowUserToAddRows = false;
            this.DGV_Spectrum.AllowUserToDeleteRows = false;
            this.DGV_Spectrum.AllowUserToResizeColumns = false;
            this.DGV_Spectrum.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV_Spectrum.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.DGV_Spectrum.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Spectrum.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Title_SpectrumType,
            this.Title_Name,
            this.Title_ID,
            this.Title_IntegralTime,
            this.Title_GetSpectrum});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV_Spectrum.DefaultCellStyle = dataGridViewCellStyle4;
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
            this.Btn_FunctionTest.Location = new System.Drawing.Point(1159, 142);
            this.Btn_FunctionTest.Name = "Btn_FunctionTest";
            this.Btn_FunctionTest.Size = new System.Drawing.Size(123, 48);
            this.Btn_FunctionTest.TabIndex = 31;
            this.Btn_FunctionTest.Text = "FunctionTest";
            this.Btn_FunctionTest.UseVisualStyleBackColor = true;
            this.Btn_FunctionTest.Click += new System.EventHandler(this.Btn_FunctionTest_Click);
            // 
            // Plot_Spectrom
            // 
            this.Plot_Spectrom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Plot_Spectrom.Location = new System.Drawing.Point(5, 216);
            this.Plot_Spectrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Plot_Spectrom.Name = "Plot_Spectrom";
            this.Plot_Spectrom.Size = new System.Drawing.Size(967, 435);
            this.Plot_Spectrom.TabIndex = 32;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.68224F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.31776F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TxtBx_IntgralTime, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.Btn_Capture, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(978, 572);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 43.03798F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 56.96202F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(231, 79);
            this.tableLayoutPanel1.TabIndex = 33;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 34);
            this.label1.TabIndex = 0;
            this.label1.Text = "IntgTime(ms)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxtBx_IntgralTime
            // 
            this.TxtBx_IntgralTime.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_IntgralTime.Location = new System.Drawing.Point(145, 3);
            this.TxtBx_IntgralTime.Name = "TxtBx_IntgralTime";
            this.TxtBx_IntgralTime.Size = new System.Drawing.Size(83, 29);
            this.TxtBx_IntgralTime.TabIndex = 1;
            // 
            // Btn_Capture
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.Btn_Capture, 2);
            this.Btn_Capture.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Capture.Location = new System.Drawing.Point(3, 37);
            this.Btn_Capture.Name = "Btn_Capture";
            this.Btn_Capture.Size = new System.Drawing.Size(225, 39);
            this.Btn_Capture.TabIndex = 32;
            this.Btn_Capture.Text = "Capture";
            this.Btn_Capture.UseVisualStyleBackColor = true;
            this.Btn_Capture.Click += new System.EventHandler(this.Btn_Capture_Click);
            // 
            // F_Spectrometer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.Plot_Spectrom);
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
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView DGV_Spectrum;
        private System.Windows.Forms.Button Btn_Load;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Button Btn_Remove;
        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.Button Btn_FunctionTest;
        //private ScottPlot.WinForms.FormsPlot Plot_Spectrom;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_SpectrumType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_IntegralTime;
        private System.Windows.Forms.DataGridViewButtonColumn Title_GetSpectrum;
        private ScottPlot.FormsPlot Plot_Spectrom;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtBx_IntgralTime;
        private System.Windows.Forms.Button Btn_Capture;
    }
}