
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DGV_TestCondition = new System.Windows.Forms.DataGridView();
            this.Title_Temperature = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_BurnInTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_RestTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TitleCurrentStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TitleCurrentEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TitleCurrentStep = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_VoltageCond1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Btn_Load = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Btn_RowDown = new System.Windows.Forms.Button();
            this.Btn_RowUp = new System.Windows.Forms.Button();
            this.Btn_Remove = new System.Windows.Forms.Button();
            this.Btn_Add = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_TestCondition)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DGV_TestCondition
            // 
            this.DGV_TestCondition.AllowUserToAddRows = false;
            this.DGV_TestCondition.AllowUserToDeleteRows = false;
            this.DGV_TestCondition.AllowUserToResizeColumns = false;
            this.DGV_TestCondition.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV_TestCondition.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DGV_TestCondition.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_TestCondition.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Title_Temperature,
            this.Title_BurnInTime,
            this.Title_RestTime,
            this.TitleCurrentStart,
            this.TitleCurrentEnd,
            this.TitleCurrentStep,
            this.Title_VoltageCond1});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV_TestCondition.DefaultCellStyle = dataGridViewCellStyle2;
            this.DGV_TestCondition.Location = new System.Drawing.Point(3, 3);
            this.DGV_TestCondition.Name = "DGV_TestCondition";
            this.DGV_TestCondition.RowHeadersVisible = false;
            this.DGV_TestCondition.RowTemplate.Height = 24;
            this.DGV_TestCondition.Size = new System.Drawing.Size(1738, 961);
            this.DGV_TestCondition.TabIndex = 15;
            // 
            // Title_Temperature
            // 
            this.Title_Temperature.HeaderText = "Temperature (°C)";
            this.Title_Temperature.Name = "Title_Temperature";
            this.Title_Temperature.Width = 170;
            // 
            // Title_BurnInTime
            // 
            this.Title_BurnInTime.HeaderText = "BurnIn Time (hr.)";
            this.Title_BurnInTime.Name = "Title_BurnInTime";
            this.Title_BurnInTime.Width = 170;
            // 
            // Title_RestTime
            // 
            this.Title_RestTime.HeaderText = "Rest Time (hr.)";
            this.Title_RestTime.Name = "Title_RestTime";
            this.Title_RestTime.Width = 170;
            // 
            // TitleCurrentStart
            // 
            this.TitleCurrentStart.HeaderText = "I Start (mA)";
            this.TitleCurrentStart.Name = "TitleCurrentStart";
            this.TitleCurrentStart.Width = 130;
            // 
            // TitleCurrentEnd
            // 
            this.TitleCurrentEnd.HeaderText = "I End (mA)";
            this.TitleCurrentEnd.Name = "TitleCurrentEnd";
            this.TitleCurrentEnd.Width = 130;
            // 
            // TitleCurrentStep
            // 
            this.TitleCurrentStep.HeaderText = "I Step (mA)";
            this.TitleCurrentStep.Name = "TitleCurrentStep";
            this.TitleCurrentStep.Width = 130;
            // 
            // Title_VoltageCond1
            // 
            this.Title_VoltageCond1.HeaderText = "V1 (mA)";
            this.Title_VoltageCond1.Name = "Title_VoltageCond1";
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1747, 3);
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
            // F_TestSetting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1896, 967);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.DGV_TestCondition);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_TestSetting";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_TestCondition)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Temperature;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_BurnInTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_RestTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn TitleCurrentStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn TitleCurrentEnd;
        private System.Windows.Forms.DataGridViewTextBoxColumn TitleCurrentStep;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_VoltageCond1;
    }
}