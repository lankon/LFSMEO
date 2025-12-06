
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
            this.Btn_Delete = new System.Windows.Forms.Button();
            this.Btn_LoadRecipe = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.TxtBx_CurRecipeName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
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
            this.groupBox2.Text = "Recipe";
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
            this.TxtBx_RecipeDescribe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(230)))));
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
            this.TxtBx_RecipeName.Size = new System.Drawing.Size(293, 29);
            this.TxtBx_RecipeName.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.Btn_Delete, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.Btn_LoadRecipe, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.Btn_Save, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 152F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(66, 200);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // Btn_Delete
            // 
            this.Btn_Delete.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Btn_Delete.BackgroundImage")));
            this.Btn_Delete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_Delete.Location = new System.Drawing.Point(3, 135);
            this.Btn_Delete.Name = "Btn_Delete";
            this.Btn_Delete.Size = new System.Drawing.Size(60, 60);
            this.Btn_Delete.TabIndex = 32;
            this.Btn_Delete.UseVisualStyleBackColor = true;
            this.Btn_Delete.Click += new System.EventHandler(this.Btn_Delete_Click);
            // 
            // Btn_LoadRecipe
            // 
            this.Btn_LoadRecipe.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Btn_LoadRecipe.BackgroundImage")));
            this.Btn_LoadRecipe.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_LoadRecipe.Location = new System.Drawing.Point(3, 69);
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
            // TxtBx_CurRecipeName
            // 
            this.TxtBx_CurRecipeName.Location = new System.Drawing.Point(12, 218);
            this.TxtBx_CurRecipeName.Name = "TxtBx_CurRecipeName";
            this.TxtBx_CurRecipeName.Size = new System.Drawing.Size(63, 22);
            this.TxtBx_CurRecipeName.TabIndex = 4;
            this.TxtBx_CurRecipeName.Visible = false;
            // 
            // F_Recipe
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1326, 661);
            this.Controls.Add(this.TxtBx_CurRecipeName);
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
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.TextBox TxtBx_CurRecipeName;
    }
}