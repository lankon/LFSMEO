
namespace RGBTester.UI
{
    partial class F_StatusBox
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Btn_Confirm = new System.Windows.Forms.Button();
            this.Labl_Status = new System.Windows.Forms.Label();
            this.Labl_ShowMessage = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.Btn_Confirm, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.Labl_Status, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Labl_ShowMessage, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.8972F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 77.10281F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(635, 254);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // Btn_Confirm
            // 
            this.Btn_Confirm.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Confirm.Location = new System.Drawing.Point(3, 219);
            this.Btn_Confirm.Name = "Btn_Confirm";
            this.Btn_Confirm.Size = new System.Drawing.Size(629, 32);
            this.Btn_Confirm.TabIndex = 1;
            this.Btn_Confirm.Text = "Confirm";
            this.Btn_Confirm.UseVisualStyleBackColor = true;
            this.Btn_Confirm.Click += new System.EventHandler(this.Btn_Confirm_Click);
            // 
            // Labl_Status
            // 
            this.Labl_Status.BackColor = System.Drawing.Color.Red;
            this.Labl_Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Labl_Status.Font = new System.Drawing.Font("Arial", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Labl_Status.Location = new System.Drawing.Point(0, 0);
            this.Labl_Status.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.Labl_Status.Name = "Labl_Status";
            this.Labl_Status.Size = new System.Drawing.Size(635, 46);
            this.Labl_Status.TabIndex = 0;
            this.Labl_Status.Text = "Error";
            this.Labl_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Labl_ShowMessage
            // 
            this.Labl_ShowMessage.BackColor = System.Drawing.Color.White;
            this.Labl_ShowMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Labl_ShowMessage.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Labl_ShowMessage.Location = new System.Drawing.Point(0, 50);
            this.Labl_ShowMessage.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.Labl_ShowMessage.Name = "Labl_ShowMessage";
            this.Labl_ShowMessage.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.Labl_ShowMessage.Size = new System.Drawing.Size(635, 166);
            this.Labl_ShowMessage.TabIndex = 2;
            this.Labl_ShowMessage.Text = "Show Message";
            this.Labl_ShowMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // F_StatusBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(635, 254);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_StatusBox";
            this.Text = "F_StateControl";
            this.TopMost = true;
            this.VisibleChanged += new System.EventHandler(this.F_StatusBox_VisibleChanged);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button Btn_Confirm;
        private System.Windows.Forms.Label Labl_Status;
        private System.Windows.Forms.Label Labl_ShowMessage;
    }
}