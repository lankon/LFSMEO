
namespace UserPrivilege.UI
{
    partial class F_UserPrivilege
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtBx_Password = new System.Windows.Forms.TextBox();
            this.Btn_Login = new System.Windows.Forms.Button();
            this.Btn_Logout = new System.Windows.Forms.Button();
            this.TxtBx_Account = new System.Windows.Forms.TextBox();
            this.Labl_LevelResult = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DGV_UserLevel = new System.Windows.Forms.DataGridView();
            this.Title_Account = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_Password = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title_Level = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Pnl_CreateAccount = new System.Windows.Forms.Panel();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Btn_Remove = new System.Windows.Forms.Button();
            this.Btn_Add = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_UserLevel)).BeginInit();
            this.Pnl_CreateAccount.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TxtBx_Password, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.Btn_Login, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.Btn_Logout, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.TxtBx_Account, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Labl_LevelResult, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(272, 241);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "Account";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // TxtBx_Password
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.TxtBx_Password, 2);
            this.TxtBx_Password.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_Password.Location = new System.Drawing.Point(3, 108);
            this.TxtBx_Password.Name = "TxtBx_Password";
            this.TxtBx_Password.PasswordChar = '*';
            this.TxtBx_Password.Size = new System.Drawing.Size(266, 29);
            this.TxtBx_Password.TabIndex = 3;
            // 
            // Btn_Login
            // 
            this.Btn_Login.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Login.Location = new System.Drawing.Point(3, 143);
            this.Btn_Login.Name = "Btn_Login";
            this.Btn_Login.Size = new System.Drawing.Size(130, 39);
            this.Btn_Login.TabIndex = 4;
            this.Btn_Login.Text = "Login";
            this.Btn_Login.UseVisualStyleBackColor = true;
            this.Btn_Login.Click += new System.EventHandler(this.Btn_Login_Click);
            // 
            // Btn_Logout
            // 
            this.Btn_Logout.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Logout.Location = new System.Drawing.Point(139, 143);
            this.Btn_Logout.Name = "Btn_Logout";
            this.Btn_Logout.Size = new System.Drawing.Size(130, 39);
            this.Btn_Logout.TabIndex = 5;
            this.Btn_Logout.Text = "Logout";
            this.Btn_Logout.UseVisualStyleBackColor = true;
            this.Btn_Logout.Click += new System.EventHandler(this.Btn_Logout_Click);
            // 
            // TxtBx_Account
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.TxtBx_Account, 2);
            this.TxtBx_Account.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_Account.Location = new System.Drawing.Point(3, 38);
            this.TxtBx_Account.Name = "TxtBx_Account";
            this.TxtBx_Account.Size = new System.Drawing.Size(266, 29);
            this.TxtBx_Account.TabIndex = 1;
            this.TxtBx_Account.Text = "Maintenance";
            // 
            // Labl_LevelResult
            // 
            this.Labl_LevelResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tableLayoutPanel1.SetColumnSpan(this.Labl_LevelResult, 2);
            this.Labl_LevelResult.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Labl_LevelResult.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.Labl_LevelResult.Location = new System.Drawing.Point(3, 185);
            this.Labl_LevelResult.Name = "Labl_LevelResult";
            this.Labl_LevelResult.Size = new System.Drawing.Size(266, 50);
            this.Labl_LevelResult.TabIndex = 6;
            this.Labl_LevelResult.Text = "OP OK";
            this.Labl_LevelResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(3, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 35);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DGV_UserLevel
            // 
            this.DGV_UserLevel.AllowUserToAddRows = false;
            this.DGV_UserLevel.AllowUserToDeleteRows = false;
            this.DGV_UserLevel.AllowUserToResizeColumns = false;
            this.DGV_UserLevel.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV_UserLevel.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DGV_UserLevel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_UserLevel.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Title_Account,
            this.Title_Password,
            this.Title_Level});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DGV_UserLevel.DefaultCellStyle = dataGridViewCellStyle3;
            this.DGV_UserLevel.Location = new System.Drawing.Point(3, 3);
            this.DGV_UserLevel.Name = "DGV_UserLevel";
            this.DGV_UserLevel.RowHeadersVisible = false;
            this.DGV_UserLevel.RowTemplate.Height = 24;
            this.DGV_UserLevel.Size = new System.Drawing.Size(366, 289);
            this.DGV_UserLevel.TabIndex = 14;
            this.DGV_UserLevel.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DGV_UserLevel_CellFormatting);
            // 
            // Title_Account
            // 
            this.Title_Account.HeaderText = "Account";
            this.Title_Account.Name = "Title_Account";
            this.Title_Account.Width = 122;
            // 
            // Title_Password
            // 
            this.Title_Password.HeaderText = "Password";
            this.Title_Password.Name = "Title_Password";
            this.Title_Password.Width = 122;
            // 
            // Title_Level
            // 
            this.Title_Level.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Title_Level.DefaultCellStyle = dataGridViewCellStyle2;
            this.Title_Level.HeaderText = "Level";
            this.Title_Level.Items.AddRange(new object[] {
            "ENG",
            "OP"});
            this.Title_Level.Name = "Title_Level";
            this.Title_Level.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Title_Level.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Title_Level.Width = 119;
            // 
            // Pnl_CreateAccount
            // 
            this.Pnl_CreateAccount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Pnl_CreateAccount.Controls.Add(this.Btn_Save);
            this.Pnl_CreateAccount.Controls.Add(this.Btn_Remove);
            this.Pnl_CreateAccount.Controls.Add(this.Btn_Add);
            this.Pnl_CreateAccount.Controls.Add(this.DGV_UserLevel);
            this.Pnl_CreateAccount.Location = new System.Drawing.Point(290, 12);
            this.Pnl_CreateAccount.Name = "Pnl_CreateAccount";
            this.Pnl_CreateAccount.Size = new System.Drawing.Size(498, 298);
            this.Pnl_CreateAccount.TabIndex = 15;
            // 
            // Btn_Save
            // 
            this.Btn_Save.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Save.Location = new System.Drawing.Point(372, 101);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(123, 47);
            this.Btn_Save.TabIndex = 23;
            this.Btn_Save.Text = "Save";
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Btn_Remove
            // 
            this.Btn_Remove.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Remove.Location = new System.Drawing.Point(372, 52);
            this.Btn_Remove.Name = "Btn_Remove";
            this.Btn_Remove.Size = new System.Drawing.Size(123, 47);
            this.Btn_Remove.TabIndex = 20;
            this.Btn_Remove.Text = "Remove";
            this.Btn_Remove.UseVisualStyleBackColor = true;
            this.Btn_Remove.Click += new System.EventHandler(this.Btn_Remove_Click);
            // 
            // Btn_Add
            // 
            this.Btn_Add.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Add.Location = new System.Drawing.Point(372, 3);
            this.Btn_Add.Name = "Btn_Add";
            this.Btn_Add.Size = new System.Drawing.Size(123, 47);
            this.Btn_Add.TabIndex = 19;
            this.Btn_Add.Text = "Add";
            this.Btn_Add.UseVisualStyleBackColor = true;
            this.Btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // F_UserPrivilege
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.Pnl_CreateAccount);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_UserPrivilege";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_UserLevel)).EndInit();
            this.Pnl_CreateAccount.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtBx_Password;
        private System.Windows.Forms.Button Btn_Login;
        private System.Windows.Forms.Button Btn_Logout;
        private System.Windows.Forms.TextBox TxtBx_Account;
        private System.Windows.Forms.Label Labl_LevelResult;
        private System.Windows.Forms.DataGridView DGV_UserLevel;
        private System.Windows.Forms.Panel Pnl_CreateAccount;
        private System.Windows.Forms.Button Btn_Add;
        private System.Windows.Forms.Button Btn_Remove;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Account;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title_Password;
        private System.Windows.Forms.DataGridViewComboBoxColumn Title_Level;
        private System.Windows.Forms.Button Btn_Save;
    }
}