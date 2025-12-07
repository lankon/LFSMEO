
namespace RGBTester.UI
{
    partial class F_Recipe
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F_Recipe));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ListBx_RecipeList = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TxtBx_RecipeDescribe = new System.Windows.Forms.TextBox();
            this.TxtBx_RecipeName = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.Btn_LoadRecipe = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.Btn_Delete = new System.Windows.Forms.Button();
            this.Btn_SaveAs = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Pnl_SaveAs = new System.Windows.Forms.Panel();
            this.Btn_SaveAsCancel = new System.Windows.Forms.Button();
            this.Btn_SaveAsConfirm = new System.Windows.Forms.Button();
            this.TxtBx_SaveAsRecipeName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.Pnl_SaveAs.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ListBx_RecipeList);
            this.groupBox1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox1.Location = new System.Drawing.Point(84, 224);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(317, 417);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Recipe List";
            // 
            // ListBx_RecipeList
            // 
            this.ListBx_RecipeList.FormattingEnabled = true;
            this.ListBx_RecipeList.ItemHeight = 20;
            this.ListBx_RecipeList.Items.AddRange(new object[] {
            "Leo",
            "BBBB"});
            this.ListBx_RecipeList.Location = new System.Drawing.Point(6, 28);
            this.ListBx_RecipeList.Name = "ListBx_RecipeList";
            this.ListBx_RecipeList.Size = new System.Drawing.Size(305, 384);
            this.ListBx_RecipeList.TabIndex = 0;
            this.ListBx_RecipeList.SelectedIndexChanged += new System.EventHandler(this.ListBx_RecipeList_SelectedIndexChanged);
            this.ListBx_RecipeList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListBx_RecipeList_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel1);
            this.groupBox2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox2.Location = new System.Drawing.Point(84, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(317, 206);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Current Recipe";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.TxtBx_RecipeDescribe, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.TxtBx_RecipeName, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(305, 175);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // TxtBx_RecipeDescribe
            // 
            this.TxtBx_RecipeDescribe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(210)))));
            this.TxtBx_RecipeDescribe.Location = new System.Drawing.Point(6, 44);
            this.TxtBx_RecipeDescribe.Multiline = true;
            this.TxtBx_RecipeDescribe.Name = "TxtBx_RecipeDescribe";
            this.TxtBx_RecipeDescribe.Size = new System.Drawing.Size(293, 125);
            this.TxtBx_RecipeDescribe.TabIndex = 1;
            this.TxtBx_RecipeDescribe.Text = "Describe:\r\n";
            // 
            // TxtBx_RecipeName
            // 
            this.TxtBx_RecipeName.Location = new System.Drawing.Point(6, 6);
            this.TxtBx_RecipeName.Name = "TxtBx_RecipeName";
            this.TxtBx_RecipeName.ReadOnly = true;
            this.TxtBx_RecipeName.Size = new System.Drawing.Size(293, 29);
            this.TxtBx_RecipeName.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.Btn_LoadRecipe, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.Btn_Save, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.Btn_Delete, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.Btn_SaveAs, 0, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 152F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(66, 265);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // Btn_LoadRecipe
            // 
            this.Btn_LoadRecipe.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Btn_LoadRecipe.BackgroundImage")));
            this.Btn_LoadRecipe.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_LoadRecipe.Location = new System.Drawing.Point(3, 135);
            this.Btn_LoadRecipe.Name = "Btn_LoadRecipe";
            this.Btn_LoadRecipe.Size = new System.Drawing.Size(60, 60);
            this.Btn_LoadRecipe.TabIndex = 30;
            this.Btn_LoadRecipe.UseVisualStyleBackColor = true;
            this.Btn_LoadRecipe.Click += new System.EventHandler(this.Btn_LoadRecipe_Click);
            // 
            // Btn_Save
            // 
            this.Btn_Save.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Btn_Save.BackgroundImage")));
            this.Btn_Save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_Save.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Save.Location = new System.Drawing.Point(3, 3);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(60, 60);
            this.Btn_Save.TabIndex = 31;
            this.Btn_Save.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Btn_Delete
            // 
            this.Btn_Delete.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Btn_Delete.BackgroundImage")));
            this.Btn_Delete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_Delete.Location = new System.Drawing.Point(3, 201);
            this.Btn_Delete.Name = "Btn_Delete";
            this.Btn_Delete.Size = new System.Drawing.Size(60, 60);
            this.Btn_Delete.TabIndex = 32;
            this.Btn_Delete.UseVisualStyleBackColor = true;
            this.Btn_Delete.Click += new System.EventHandler(this.Btn_Delete_Click);
            // 
            // Btn_SaveAs
            // 
            this.Btn_SaveAs.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Btn_SaveAs.BackgroundImage")));
            this.Btn_SaveAs.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_SaveAs.Font = new System.Drawing.Font("微軟正黑體", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_SaveAs.Location = new System.Drawing.Point(3, 69);
            this.Btn_SaveAs.Name = "Btn_SaveAs";
            this.Btn_SaveAs.Size = new System.Drawing.Size(60, 60);
            this.Btn_SaveAs.TabIndex = 33;
            this.Btn_SaveAs.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.Btn_SaveAs.UseVisualStyleBackColor = true;
            this.Btn_SaveAs.Click += new System.EventHandler(this.Btn_SaveAs_Click);
            // 
            // Pnl_SaveAs
            // 
            this.Pnl_SaveAs.BackColor = System.Drawing.Color.White;
            this.Pnl_SaveAs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Pnl_SaveAs.Controls.Add(this.Btn_SaveAsCancel);
            this.Pnl_SaveAs.Controls.Add(this.Btn_SaveAsConfirm);
            this.Pnl_SaveAs.Controls.Add(this.TxtBx_SaveAsRecipeName);
            this.Pnl_SaveAs.Controls.Add(this.label1);
            this.Pnl_SaveAs.Location = new System.Drawing.Point(616, 277);
            this.Pnl_SaveAs.Name = "Pnl_SaveAs";
            this.Pnl_SaveAs.Size = new System.Drawing.Size(256, 97);
            this.Pnl_SaveAs.TabIndex = 5;
            this.Pnl_SaveAs.Visible = false;
            // 
            // Btn_SaveAsCancel
            // 
            this.Btn_SaveAsCancel.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_SaveAsCancel.Location = new System.Drawing.Point(140, 58);
            this.Btn_SaveAsCancel.Name = "Btn_SaveAsCancel";
            this.Btn_SaveAsCancel.Size = new System.Drawing.Size(88, 34);
            this.Btn_SaveAsCancel.TabIndex = 3;
            this.Btn_SaveAsCancel.Text = "Cancel";
            this.Btn_SaveAsCancel.UseVisualStyleBackColor = true;
            this.Btn_SaveAsCancel.Click += new System.EventHandler(this.Btn_SaveAsCancel_Click);
            // 
            // Btn_SaveAsConfirm
            // 
            this.Btn_SaveAsConfirm.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_SaveAsConfirm.Location = new System.Drawing.Point(20, 58);
            this.Btn_SaveAsConfirm.Name = "Btn_SaveAsConfirm";
            this.Btn_SaveAsConfirm.Size = new System.Drawing.Size(88, 34);
            this.Btn_SaveAsConfirm.TabIndex = 2;
            this.Btn_SaveAsConfirm.Text = "Confirm";
            this.Btn_SaveAsConfirm.UseVisualStyleBackColor = true;
            this.Btn_SaveAsConfirm.Click += new System.EventHandler(this.Btn_SaveAsConfirm_Click);
            // 
            // TxtBx_SaveAsRecipeName
            // 
            this.TxtBx_SaveAsRecipeName.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TxtBx_SaveAsRecipeName.Location = new System.Drawing.Point(92, 12);
            this.TxtBx_SaveAsRecipeName.Name = "TxtBx_SaveAsRecipeName";
            this.TxtBx_SaveAsRecipeName.Size = new System.Drawing.Size(136, 29);
            this.TxtBx_SaveAsRecipeName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(16, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Save As";
            // 
            // F_Recipe
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.Pnl_SaveAs);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_Recipe";
            this.Text = "F_Equipment_Setting";
            this.VisibleChanged += new System.EventHandler(this.F_Equipment_Setting_VisibleChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.Pnl_SaveAs.ResumeLayout(false);
            this.Pnl_SaveAs.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox ListBx_RecipeList;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox TxtBx_RecipeDescribe;
        private System.Windows.Forms.TextBox TxtBx_RecipeName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.Button Btn_LoadRecipe;
        private System.Windows.Forms.Button Btn_Delete;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel Pnl_SaveAs;
        private System.Windows.Forms.Button Btn_SaveAsCancel;
        private System.Windows.Forms.Button Btn_SaveAsConfirm;
        private System.Windows.Forms.TextBox TxtBx_SaveAsRecipeName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Btn_SaveAs;
    }
}