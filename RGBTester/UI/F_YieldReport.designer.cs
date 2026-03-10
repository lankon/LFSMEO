
namespace RGBTester.UI
{
    partial class F_YieldReport
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Btn_Search = new System.Windows.Forms.Button();
            this.Cmbx_YieldRecord = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.TxtBx_StartTime = new System.Windows.Forms.TextBox();
            this.TxtBx_SN = new System.Windows.Forms.TextBox();
            this.TxtBx_ProductType = new System.Windows.Forms.TextBox();
            this.TxtBx_EndTime = new System.Windows.Forms.TextBox();
            this.TxtBx_Description = new System.Windows.Forms.TextBox();
            this.Cmbx_Pass = new System.Windows.Forms.ComboBox();
            this.Cmbx_Exclude = new System.Windows.Forms.ComboBox();
            this.DGV_ProductRawData = new System.Windows.Forms.DataGridView();
            this.Title_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_ProductType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_SN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_TestTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_IsPass = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Title_Exclude = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Title_Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.TxtBx_Fail = new System.Windows.Forms.TextBox();
            this.TxtBx_Pass = new System.Windows.Forms.TextBox();
            this.TxtBx_Total = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.TxtBx_Yield = new System.Windows.Forms.TextBox();
            this.Btn_OutputResult = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_ProductRawData)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Btn_Search);
            this.panel1.Controls.Add(this.Cmbx_YieldRecord);
            this.panel1.Controls.Add(this.tableLayoutPanel6);
            this.panel1.Controls.Add(this.DGV_ProductRawData);
            this.panel1.Controls.Add(this.tableLayoutPanel5);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(913, 656);
            this.panel1.TabIndex = 34;
            // 
            // Btn_Search
            // 
            this.Btn_Search.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Search.Location = new System.Drawing.Point(809, 5);
            this.Btn_Search.Name = "Btn_Search";
            this.Btn_Search.Size = new System.Drawing.Size(99, 28);
            this.Btn_Search.TabIndex = 30;
            this.Btn_Search.Text = "Search";
            this.Btn_Search.UseVisualStyleBackColor = true;
            this.Btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
            // 
            // Cmbx_YieldRecord
            // 
            this.Cmbx_YieldRecord.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Cmbx_YieldRecord.FormattingEnabled = true;
            this.Cmbx_YieldRecord.Items.AddRange(new object[] {
            "NoRecord",
            "Record"});
            this.Cmbx_YieldRecord.Location = new System.Drawing.Point(704, 5);
            this.Cmbx_YieldRecord.Name = "Cmbx_YieldRecord";
            this.Cmbx_YieldRecord.Size = new System.Drawing.Size(99, 28);
            this.Cmbx_YieldRecord.TabIndex = 32;
            this.Cmbx_YieldRecord.Text = "Record";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel6.ColumnCount = 7;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 43.4375F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 56.5625F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 89F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 98F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 203F));
            this.tableLayoutPanel6.Controls.Add(this.TxtBx_StartTime, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.TxtBx_SN, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.TxtBx_ProductType, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.TxtBx_EndTime, 3, 0);
            this.tableLayoutPanel6.Controls.Add(this.TxtBx_Description, 6, 0);
            this.tableLayoutPanel6.Controls.Add(this.Cmbx_Pass, 4, 0);
            this.tableLayoutPanel6.Controls.Add(this.Cmbx_Exclude, 5, 0);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 36);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(905, 38);
            this.tableLayoutPanel6.TabIndex = 31;
            // 
            // TxtBx_StartTime
            // 
            this.TxtBx_StartTime.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_StartTime.Location = new System.Drawing.Point(324, 4);
            this.TxtBx_StartTime.Name = "TxtBx_StartTime";
            this.TxtBx_StartTime.Size = new System.Drawing.Size(82, 29);
            this.TxtBx_StartTime.TabIndex = 14;
            this.TxtBx_StartTime.DoubleClick += new System.EventHandler(this.TxtBx_StartTime_DoubleClick);
            // 
            // TxtBx_SN
            // 
            this.TxtBx_SN.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_SN.Location = new System.Drawing.Point(143, 4);
            this.TxtBx_SN.Name = "TxtBx_SN";
            this.TxtBx_SN.Size = new System.Drawing.Size(174, 29);
            this.TxtBx_SN.TabIndex = 10;
            // 
            // TxtBx_ProductType
            // 
            this.TxtBx_ProductType.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_ProductType.Location = new System.Drawing.Point(4, 4);
            this.TxtBx_ProductType.Name = "TxtBx_ProductType";
            this.TxtBx_ProductType.Size = new System.Drawing.Size(132, 29);
            this.TxtBx_ProductType.TabIndex = 9;
            // 
            // TxtBx_EndTime
            // 
            this.TxtBx_EndTime.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_EndTime.Location = new System.Drawing.Point(413, 4);
            this.TxtBx_EndTime.Name = "TxtBx_EndTime";
            this.TxtBx_EndTime.Size = new System.Drawing.Size(83, 29);
            this.TxtBx_EndTime.TabIndex = 8;
            this.TxtBx_EndTime.DoubleClick += new System.EventHandler(this.TxtBx_StartTime_DoubleClick);
            // 
            // TxtBx_Description
            // 
            this.TxtBx_Description.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_Description.Location = new System.Drawing.Point(703, 4);
            this.TxtBx_Description.Name = "TxtBx_Description";
            this.TxtBx_Description.Size = new System.Drawing.Size(198, 29);
            this.TxtBx_Description.TabIndex = 13;
            // 
            // Cmbx_Pass
            // 
            this.Cmbx_Pass.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Cmbx_Pass.FormattingEnabled = true;
            this.Cmbx_Pass.Items.AddRange(new object[] {
            "Fail",
            "Pass",
            "All"});
            this.Cmbx_Pass.Location = new System.Drawing.Point(503, 4);
            this.Cmbx_Pass.Name = "Cmbx_Pass";
            this.Cmbx_Pass.Size = new System.Drawing.Size(93, 28);
            this.Cmbx_Pass.TabIndex = 15;
            // 
            // Cmbx_Exclude
            // 
            this.Cmbx_Exclude.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Cmbx_Exclude.FormattingEnabled = true;
            this.Cmbx_Exclude.Items.AddRange(new object[] {
            "NonExclude",
            "Exclude",
            "All"});
            this.Cmbx_Exclude.Location = new System.Drawing.Point(604, 4);
            this.Cmbx_Exclude.Name = "Cmbx_Exclude";
            this.Cmbx_Exclude.Size = new System.Drawing.Size(92, 28);
            this.Cmbx_Exclude.TabIndex = 16;
            // 
            // DGV_ProductRawData
            // 
            this.DGV_ProductRawData.AllowUserToAddRows = false;
            this.DGV_ProductRawData.AllowUserToDeleteRows = false;
            this.DGV_ProductRawData.AllowUserToResizeColumns = false;
            this.DGV_ProductRawData.AllowUserToResizeRows = false;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV_ProductRawData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.DGV_ProductRawData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_ProductRawData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Title_ID,
            this.Title_ProductType,
            this.Title_SN,
            this.Title_TestTime,
            this.Title_IsPass,
            this.Title_Exclude,
            this.Title_Description});
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV_ProductRawData.DefaultCellStyle = dataGridViewCellStyle12;
            this.DGV_ProductRawData.Location = new System.Drawing.Point(3, 80);
            this.DGV_ProductRawData.Name = "DGV_ProductRawData";
            this.DGV_ProductRawData.RowHeadersVisible = false;
            this.DGV_ProductRawData.RowTemplate.Height = 24;
            this.DGV_ProductRawData.Size = new System.Drawing.Size(905, 530);
            this.DGV_ProductRawData.TabIndex = 27;
            this.DGV_ProductRawData.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_ProductRawData_CellValueChanged);
            // 
            // Title_ID
            // 
            this.Title_ID.DataPropertyName = "ID";
            this.Title_ID.HeaderText = "ID";
            this.Title_ID.Name = "Title_ID";
            this.Title_ID.ReadOnly = true;
            this.Title_ID.Visible = false;
            // 
            // Title_ProductType
            // 
            this.Title_ProductType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Title_ProductType.DataPropertyName = "ProductType";
            this.Title_ProductType.HeaderText = "Product Type";
            this.Title_ProductType.Name = "Title_ProductType";
            this.Title_ProductType.ReadOnly = true;
            this.Title_ProductType.Width = 140;
            // 
            // Title_SN
            // 
            this.Title_SN.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Title_SN.DataPropertyName = "SN";
            this.Title_SN.HeaderText = "SN";
            this.Title_SN.Name = "Title_SN";
            this.Title_SN.ReadOnly = true;
            this.Title_SN.Width = 180;
            // 
            // Title_TestTime
            // 
            this.Title_TestTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Title_TestTime.DataPropertyName = "TestTime";
            dataGridViewCellStyle11.Format = "yyyy-MM-dd HH:mm:ss";
            this.Title_TestTime.DefaultCellStyle = dataGridViewCellStyle11;
            this.Title_TestTime.HeaderText = "Test Time";
            this.Title_TestTime.Name = "Title_TestTime";
            this.Title_TestTime.ReadOnly = true;
            this.Title_TestTime.Width = 180;
            // 
            // Title_IsPass
            // 
            this.Title_IsPass.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Title_IsPass.DataPropertyName = "IsPass";
            this.Title_IsPass.HeaderText = "Pass";
            this.Title_IsPass.Name = "Title_IsPass";
            this.Title_IsPass.ReadOnly = true;
            this.Title_IsPass.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_IsPass.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Title_Exclude
            // 
            this.Title_Exclude.DataPropertyName = "Exclude";
            this.Title_Exclude.HeaderText = "Exclude";
            this.Title_Exclude.Name = "Title_Exclude";
            this.Title_Exclude.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_Exclude.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Title_Description
            // 
            this.Title_Description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Title_Description.DataPropertyName = "Description";
            this.Title_Description.HeaderText = "Description";
            this.Title_Description.Name = "Title_Description";
            this.Title_Description.Width = 180;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel5.ColumnCount = 10;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel5.Controls.Add(this.TxtBx_Fail, 5, 0);
            this.tableLayoutPanel5.Controls.Add(this.TxtBx_Pass, 3, 0);
            this.tableLayoutPanel5.Controls.Add(this.TxtBx_Total, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.label12, 2, 0);
            this.tableLayoutPanel5.Controls.Add(this.label13, 4, 0);
            this.tableLayoutPanel5.Controls.Add(this.label15, 6, 0);
            this.tableLayoutPanel5.Controls.Add(this.TxtBx_Yield, 7, 0);
            this.tableLayoutPanel5.Controls.Add(this.Btn_OutputResult, 8, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 616);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(905, 37);
            this.tableLayoutPanel5.TabIndex = 30;
            // 
            // TxtBx_Fail
            // 
            this.TxtBx_Fail.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_Fail.Location = new System.Drawing.Point(469, 4);
            this.TxtBx_Fail.Name = "TxtBx_Fail";
            this.TxtBx_Fail.ReadOnly = true;
            this.TxtBx_Fail.Size = new System.Drawing.Size(74, 29);
            this.TxtBx_Fail.TabIndex = 33;
            // 
            // TxtBx_Pass
            // 
            this.TxtBx_Pass.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_Pass.Location = new System.Drawing.Point(297, 4);
            this.TxtBx_Pass.Name = "TxtBx_Pass";
            this.TxtBx_Pass.ReadOnly = true;
            this.TxtBx_Pass.Size = new System.Drawing.Size(74, 29);
            this.TxtBx_Pass.TabIndex = 32;
            // 
            // TxtBx_Total
            // 
            this.TxtBx_Total.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_Total.Location = new System.Drawing.Point(125, 4);
            this.TxtBx_Total.Name = "TxtBx_Total";
            this.TxtBx_Total.ReadOnly = true;
            this.TxtBx_Total.Size = new System.Drawing.Size(74, 29);
            this.TxtBx_Total.TabIndex = 31;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.Location = new System.Drawing.Point(4, 1);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 35);
            this.label6.TabIndex = 8;
            this.label6.Text = "Total Units";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label12.Location = new System.Drawing.Point(206, 1);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(84, 35);
            this.label12.TabIndex = 9;
            this.label12.Text = "Pass";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label13.Location = new System.Drawing.Point(378, 1);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(84, 35);
            this.label13.TabIndex = 10;
            this.label13.Text = "Fail";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label15
            // 
            this.label15.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label15.Location = new System.Drawing.Point(550, 1);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(84, 35);
            this.label15.TabIndex = 12;
            this.label15.Text = "Yield(%)";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TxtBx_Yield
            // 
            this.TxtBx_Yield.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_Yield.Location = new System.Drawing.Point(641, 4);
            this.TxtBx_Yield.Name = "TxtBx_Yield";
            this.TxtBx_Yield.ReadOnly = true;
            this.TxtBx_Yield.Size = new System.Drawing.Size(74, 29);
            this.TxtBx_Yield.TabIndex = 35;
            // 
            // Btn_OutputResult
            // 
            this.tableLayoutPanel5.SetColumnSpan(this.Btn_OutputResult, 2);
            this.Btn_OutputResult.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_OutputResult.Location = new System.Drawing.Point(719, 1);
            this.Btn_OutputResult.Margin = new System.Windows.Forms.Padding(0);
            this.Btn_OutputResult.Name = "Btn_OutputResult";
            this.Btn_OutputResult.Size = new System.Drawing.Size(185, 35);
            this.Btn_OutputResult.TabIndex = 36;
            this.Btn_OutputResult.Text = "Output Result";
            this.Btn_OutputResult.UseVisualStyleBackColor = true;
            this.Btn_OutputResult.Click += new System.EventHandler(this.Btn_OutputResult_Click);
            // 
            // F_YieldReport
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_YieldReport";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_ProductRawData)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Btn_Search;
        private System.Windows.Forms.ComboBox Cmbx_YieldRecord;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.TextBox TxtBx_StartTime;
        private System.Windows.Forms.TextBox TxtBx_SN;
        private System.Windows.Forms.TextBox TxtBx_ProductType;
        private System.Windows.Forms.TextBox TxtBx_EndTime;
        private System.Windows.Forms.TextBox TxtBx_Description;
        private System.Windows.Forms.ComboBox Cmbx_Pass;
        private System.Windows.Forms.ComboBox Cmbx_Exclude;
        private System.Windows.Forms.DataGridView DGV_ProductRawData;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_ProductType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_SN;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_TestTime;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Title_IsPass;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Title_Exclude;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Description;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TextBox TxtBx_Fail;
        private System.Windows.Forms.TextBox TxtBx_Pass;
        private System.Windows.Forms.TextBox TxtBx_Total;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox TxtBx_Yield;
        private System.Windows.Forms.Button Btn_OutputResult;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}