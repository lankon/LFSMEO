
namespace BurnInTester.UI
{
    partial class F_TestSetting
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
            this.DGV_TestCondition = new System.Windows.Forms.DataGridView();
            this.Title_No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_Skip = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Title_TestProcess = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Title_Content = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Btn_Load = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Btn_RowDown = new System.Windows.Forms.Button();
            this.Btn_RowUp = new System.Windows.Forms.Button();
            this.Btn_Remove = new System.Windows.Forms.Button();
            this.Btn_Add = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TabCtrl_TestCondition = new System.Windows.Forms.TabControl();
            this.TabPg_Temperature = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtBx_TargetTemperature = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Btn_SetTemperature = new System.Windows.Forms.Button();
            this.TabPg_LIV = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.TxtBx_BurnInDuration = new System.Windows.Forms.TextBox();
            this.TxtBx_StabiliztionWaitTime = new System.Windows.Forms.TextBox();
            this.TxtBx_TemperatureTolerance = new System.Windows.Forms.TextBox();
            this.TxtBx_StabilizationTimeout = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.TxtBx_StartI = new System.Windows.Forms.TextBox();
            this.TxtBx_StepI = new System.Windows.Forms.TextBox();
            this.TxtBx_EndI = new System.Windows.Forms.TextBox();
            this.TxtBx_LIVDelayTime = new System.Windows.Forms.TextBox();
            this.Btn_SetLIV = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_TestCondition)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.TabCtrl_TestCondition.SuspendLayout();
            this.TabPg_Temperature.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.TabPg_LIV.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // DGV_TestCondition
            // 
            this.DGV_TestCondition.AllowUserToAddRows = false;
            this.DGV_TestCondition.AllowUserToDeleteRows = false;
            this.DGV_TestCondition.AllowUserToResizeColumns = false;
            this.DGV_TestCondition.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV_TestCondition.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.DGV_TestCondition.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_TestCondition.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Title_No,
            this.Title_Skip,
            this.Title_TestProcess,
            this.Title_Content,
            this.Title_ID});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV_TestCondition.DefaultCellStyle = dataGridViewCellStyle4;
            this.DGV_TestCondition.Location = new System.Drawing.Point(3, 3);
            this.DGV_TestCondition.Name = "DGV_TestCondition";
            this.DGV_TestCondition.RowHeadersVisible = false;
            this.DGV_TestCondition.RowTemplate.Height = 24;
            this.DGV_TestCondition.Size = new System.Drawing.Size(931, 961);
            this.DGV_TestCondition.TabIndex = 15;
            this.DGV_TestCondition.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DGV_TestCondition_CellClick);
            this.DGV_TestCondition.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.DGV_TestCondition_EditingControlShowing);
            // 
            // Title_No
            // 
            this.Title_No.HeaderText = "No";
            this.Title_No.Name = "Title_No";
            this.Title_No.ReadOnly = true;
            this.Title_No.Width = 70;
            // 
            // Title_Skip
            // 
            this.Title_Skip.HeaderText = "Skip";
            this.Title_Skip.Name = "Title_Skip";
            this.Title_Skip.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_Skip.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Title_Skip.Width = 70;
            // 
            // Title_TestProcess
            // 
            this.Title_TestProcess.HeaderText = "Test Process";
            this.Title_TestProcess.Items.AddRange(new object[] {
            "None",
            "Temperature",
            "LIV",
            "VF",
            "IR"});
            this.Title_TestProcess.Name = "Title_TestProcess";
            this.Title_TestProcess.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_TestProcess.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Title_TestProcess.Width = 170;
            // 
            // Title_Content
            // 
            this.Title_Content.HeaderText = "Content";
            this.Title_Content.Name = "Title_Content";
            this.Title_Content.ReadOnly = true;
            this.Title_Content.Width = 600;
            // 
            // Title_ID
            // 
            this.Title_ID.HeaderText = "ID";
            this.Title_ID.Name = "Title_ID";
            this.Title_ID.ReadOnly = true;
            this.Title_ID.Visible = false;
            // 
            // Btn_Load
            // 
            this.Btn_Load.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Load.Location = new System.Drawing.Point(4, 90);
            this.Btn_Load.Name = "Btn_Load";
            this.Btn_Load.Size = new System.Drawing.Size(137, 79);
            this.Btn_Load.TabIndex = 30;
            this.Btn_Load.Text = "Load";
            this.Btn_Load.UseVisualStyleBackColor = true;
            this.Btn_Load.Click += new System.EventHandler(this.Btn_Load_Click);
            // 
            // Btn_Save
            // 
            this.Btn_Save.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Save.Location = new System.Drawing.Point(4, 4);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(137, 79);
            this.Btn_Save.TabIndex = 29;
            this.Btn_Save.Text = "Save";
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Btn_RowDown
            // 
            this.Btn_RowDown.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_RowDown.Location = new System.Drawing.Point(4, 434);
            this.Btn_RowDown.Name = "Btn_RowDown";
            this.Btn_RowDown.Size = new System.Drawing.Size(137, 79);
            this.Btn_RowDown.TabIndex = 28;
            this.Btn_RowDown.Text = "Row Down";
            this.Btn_RowDown.UseVisualStyleBackColor = true;
            // 
            // Btn_RowUp
            // 
            this.Btn_RowUp.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_RowUp.Location = new System.Drawing.Point(4, 348);
            this.Btn_RowUp.Name = "Btn_RowUp";
            this.Btn_RowUp.Size = new System.Drawing.Size(137, 79);
            this.Btn_RowUp.TabIndex = 27;
            this.Btn_RowUp.Text = "Row Up";
            this.Btn_RowUp.UseVisualStyleBackColor = true;
            // 
            // Btn_Remove
            // 
            this.Btn_Remove.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Remove.Location = new System.Drawing.Point(4, 262);
            this.Btn_Remove.Name = "Btn_Remove";
            this.Btn_Remove.Size = new System.Drawing.Size(137, 79);
            this.Btn_Remove.TabIndex = 26;
            this.Btn_Remove.Text = "Remove";
            this.Btn_Remove.UseVisualStyleBackColor = true;
            this.Btn_Remove.Click += new System.EventHandler(this.Btn_Remove_Click);
            // 
            // Btn_Add
            // 
            this.Btn_Add.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Add.Location = new System.Drawing.Point(4, 176);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(137, 79);
            this.Btn_Add.TabIndex = 25;
            this.Btn_Add.Text = "Add";
            this.Btn_Add.UseVisualStyleBackColor = true;
            this.Btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.Btn_Save, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Btn_RowDown, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.Btn_Load, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Btn_RowUp, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.Btn_Add, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.Btn_Remove, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1413, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(145, 517);
            this.tableLayoutPanel1.TabIndex = 31;
            // 
            // TabCtrl_TestCondition
            // 
            this.TabCtrl_TestCondition.Controls.Add(this.TabPg_Temperature);
            this.TabCtrl_TestCondition.Controls.Add(this.TabPg_LIV);
            this.TabCtrl_TestCondition.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TabCtrl_TestCondition.Location = new System.Drawing.Point(940, 3);
            this.TabCtrl_TestCondition.Name = "TabCtrl_TestCondition";
            this.TabCtrl_TestCondition.SelectedIndex = 0;
            this.TabCtrl_TestCondition.Size = new System.Drawing.Size(467, 961);
            this.TabCtrl_TestCondition.TabIndex = 32;
            // 
            // TabPg_Temperature
            // 
            this.TabPg_Temperature.Controls.Add(this.tableLayoutPanel3);
            this.TabPg_Temperature.Controls.Add(this.Btn_SetTemperature);
            this.TabPg_Temperature.Location = new System.Drawing.Point(4, 29);
            this.TabPg_Temperature.Name = "TabPg_Temperature";
            this.TabPg_Temperature.Padding = new System.Windows.Forms.Padding(3);
            this.TabPg_Temperature.Size = new System.Drawing.Size(459, 928);
            this.TabPg_Temperature.TabIndex = 0;
            this.TabPg_Temperature.Text = "Temperature";
            this.TabPg_Temperature.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.66666F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.33334F));
            this.tableLayoutPanel3.Controls.Add(this.TxtBx_StabilizationTimeout, 1, 4);
            this.tableLayoutPanel3.Controls.Add(this.TxtBx_TemperatureTolerance, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.TxtBx_StabiliztionWaitTime, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.TxtBx_BurnInDuration, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.TxtBx_TargetTemperature, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 5;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(453, 193);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(281, 35);
            this.label2.TabIndex = 2;
            this.label2.Text = "Post-Stabilization Wait Time (min)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TxtBx_TargetTemperature
            // 
            this.TxtBx_TargetTemperature.Location = new System.Drawing.Point(296, 6);
            this.TxtBx_TargetTemperature.Name = "TxtBx_TargetTemperature";
            this.TxtBx_TargetTemperature.Size = new System.Drawing.Size(151, 29);
            this.TxtBx_TargetTemperature.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(281, 35);
            this.label1.TabIndex = 1;
            this.label1.Text = "Temperature (°C)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(281, 35);
            this.label3.TabIndex = 3;
            this.label3.Text = "Temperature Tolerance (±°C)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(281, 35);
            this.label4.TabIndex = 4;
            this.label4.Text = "Stabilization Timeout (min)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(281, 35);
            this.label5.TabIndex = 5;
            this.label5.Text = "Burn-in Duration (min)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Btn_SetTemperature
            // 
            this.Btn_SetTemperature.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.Btn_SetTemperature.Location = new System.Drawing.Point(3, 202);
            this.Btn_SetTemperature.Name = "Btn_SetTemperature";
            this.Btn_SetTemperature.Size = new System.Drawing.Size(453, 42);
            this.Btn_SetTemperature.TabIndex = 6;
            this.Btn_SetTemperature.Text = "Set";
            this.Btn_SetTemperature.UseVisualStyleBackColor = false;
            this.Btn_SetTemperature.Click += new System.EventHandler(this.Btn_SetTemperature_Click);
            // 
            // TabPg_LIV
            // 
            this.TabPg_LIV.Controls.Add(this.tableLayoutPanel2);
            this.TabPg_LIV.Controls.Add(this.Btn_SetLIV);
            this.TabPg_LIV.Location = new System.Drawing.Point(4, 29);
            this.TabPg_LIV.Name = "TabPg_LIV";
            this.TabPg_LIV.Padding = new System.Windows.Forms.Padding(3);
            this.TabPg_LIV.Size = new System.Drawing.Size(459, 928);
            this.TabPg_LIV.TabIndex = 1;
            this.TabPg_LIV.Text = "LIV";
            this.TabPg_LIV.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.33334F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.66667F));
            this.tableLayoutPanel2.Controls.Add(this.TxtBx_LIVDelayTime, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.TxtBx_EndI, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.TxtBx_StepI, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.TxtBx_StartI, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 3);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(453, 153);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // TxtBx_BurnInDuration
            // 
            this.TxtBx_BurnInDuration.Location = new System.Drawing.Point(296, 44);
            this.TxtBx_BurnInDuration.Name = "TxtBx_BurnInDuration";
            this.TxtBx_BurnInDuration.Size = new System.Drawing.Size(151, 29);
            this.TxtBx_BurnInDuration.TabIndex = 7;
            // 
            // TxtBx_StabiliztionWaitTime
            // 
            this.TxtBx_StabiliztionWaitTime.Location = new System.Drawing.Point(296, 82);
            this.TxtBx_StabiliztionWaitTime.Name = "TxtBx_StabiliztionWaitTime";
            this.TxtBx_StabiliztionWaitTime.Size = new System.Drawing.Size(151, 29);
            this.TxtBx_StabiliztionWaitTime.TabIndex = 8;
            // 
            // TxtBx_TemperatureTolerance
            // 
            this.TxtBx_TemperatureTolerance.Location = new System.Drawing.Point(296, 120);
            this.TxtBx_TemperatureTolerance.Name = "TxtBx_TemperatureTolerance";
            this.TxtBx_TemperatureTolerance.Size = new System.Drawing.Size(151, 29);
            this.TxtBx_TemperatureTolerance.TabIndex = 9;
            // 
            // TxtBx_StabilizationTimeout
            // 
            this.TxtBx_StabilizationTimeout.Location = new System.Drawing.Point(296, 158);
            this.TxtBx_StabilizationTimeout.Name = "TxtBx_StabilizationTimeout";
            this.TxtBx_StabilizationTimeout.Size = new System.Drawing.Size(151, 29);
            this.TxtBx_StabilizationTimeout.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(281, 35);
            this.label6.TabIndex = 2;
            this.label6.Text = "Start I (mA)";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(6, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(281, 35);
            this.label7.TabIndex = 3;
            this.label7.Text = "Step I (mA)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(6, 79);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(281, 35);
            this.label8.TabIndex = 4;
            this.label8.Text = "End I (mA)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(6, 117);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(281, 35);
            this.label9.TabIndex = 5;
            this.label9.Text = "Delay Time (ms)";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TxtBx_StartI
            // 
            this.TxtBx_StartI.Location = new System.Drawing.Point(299, 6);
            this.TxtBx_StartI.Name = "TxtBx_StartI";
            this.TxtBx_StartI.Size = new System.Drawing.Size(148, 29);
            this.TxtBx_StartI.TabIndex = 6;
            // 
            // TxtBx_StepI
            // 
            this.TxtBx_StepI.Location = new System.Drawing.Point(299, 44);
            this.TxtBx_StepI.Name = "TxtBx_StepI";
            this.TxtBx_StepI.Size = new System.Drawing.Size(148, 29);
            this.TxtBx_StepI.TabIndex = 7;
            // 
            // TxtBx_EndI
            // 
            this.TxtBx_EndI.Location = new System.Drawing.Point(299, 82);
            this.TxtBx_EndI.Name = "TxtBx_EndI";
            this.TxtBx_EndI.Size = new System.Drawing.Size(148, 29);
            this.TxtBx_EndI.TabIndex = 8;
            // 
            // TxtBx_LIVDelayTime
            // 
            this.TxtBx_LIVDelayTime.Location = new System.Drawing.Point(299, 120);
            this.TxtBx_LIVDelayTime.Name = "TxtBx_LIVDelayTime";
            this.TxtBx_LIVDelayTime.Size = new System.Drawing.Size(148, 29);
            this.TxtBx_LIVDelayTime.TabIndex = 9;
            // 
            // Btn_SetLIV
            // 
            this.Btn_SetLIV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.Btn_SetLIV.Location = new System.Drawing.Point(3, 158);
            this.Btn_SetLIV.Name = "Btn_SetLIV";
            this.Btn_SetLIV.Size = new System.Drawing.Size(453, 42);
            this.Btn_SetLIV.TabIndex = 10;
            this.Btn_SetLIV.Text = "Set";
            this.Btn_SetLIV.UseVisualStyleBackColor = false;
            this.Btn_SetLIV.Click += new System.EventHandler(this.Btn_SetLIV_Click);
            // 
            // F_TestSetting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1896, 967);
            this.Controls.Add(this.TabCtrl_TestCondition);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.DGV_TestCondition);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_TestSetting";
            this.Text = "F_TestSetting";
            this.VisibleChanged += new System.EventHandler(this.F_TestSetting_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_TestCondition)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.TabCtrl_TestCondition.ResumeLayout(false);
            this.TabPg_Temperature.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.TabPg_LIV.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DGV_TestCondition;
        private System.Windows.Forms.Button Btn_Load;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Button Btn_RowDown;
        private System.Windows.Forms.Button Btn_RowUp;
        private System.Windows.Forms.Button Btn_Remove;
        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabControl TabCtrl_TestCondition;
        private System.Windows.Forms.TabPage TabPg_Temperature;
        private System.Windows.Forms.TabPage TabPg_LIV;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TextBox TxtBx_TargetTemperature;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button Btn_SetTemperature;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_No;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Title_Skip;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_TestProcess;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Content;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_ID;
        private System.Windows.Forms.TextBox TxtBx_StabilizationTimeout;
        private System.Windows.Forms.TextBox TxtBx_TemperatureTolerance;
        private System.Windows.Forms.TextBox TxtBx_StabiliztionWaitTime;
        private System.Windows.Forms.TextBox TxtBx_BurnInDuration;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox TxtBx_LIVDelayTime;
        private System.Windows.Forms.TextBox TxtBx_EndI;
        private System.Windows.Forms.TextBox TxtBx_StepI;
        private System.Windows.Forms.TextBox TxtBx_StartI;
        private System.Windows.Forms.Button Btn_SetLIV;
    }
}