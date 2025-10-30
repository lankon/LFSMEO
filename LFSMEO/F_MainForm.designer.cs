
namespace LFSMEO
{
    partial class F_MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F_MainForm));
            this.Pnl_Group = new System.Windows.Forms.Panel();
            this.Pnl_Function = new System.Windows.Forms.Panel();
            this.Pnl_Group1 = new System.Windows.Forms.Panel();
            this.Btn_Home = new System.Windows.Forms.Button();
            this.Btn_CloseApp = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Pnl_Function.SuspendLayout();
            this.SuspendLayout();
            // 
            // Pnl_Group
            // 
            this.Pnl_Group.Location = new System.Drawing.Point(12, 77);
            this.Pnl_Group.Name = "Pnl_Group";
            this.Pnl_Group.Size = new System.Drawing.Size(1326, 661);
            this.Pnl_Group.TabIndex = 29;
            // 
            // Pnl_Function
            // 
            this.Pnl_Function.Controls.Add(this.Pnl_Group1);
            this.Pnl_Function.Controls.Add(this.Btn_Home);
            this.Pnl_Function.Controls.Add(this.Btn_CloseApp);
            this.Pnl_Function.Location = new System.Drawing.Point(12, 3);
            this.Pnl_Function.Name = "Pnl_Function";
            this.Pnl_Function.Size = new System.Drawing.Size(1326, 68);
            this.Pnl_Function.TabIndex = 30;
            this.Pnl_Function.Paint += new System.Windows.Forms.PaintEventHandler(this.Pnl_Function_Paint);
            // 
            // Pnl_Group1
            // 
            this.Pnl_Group1.Location = new System.Drawing.Point(69, 3);
            this.Pnl_Group1.Name = "Pnl_Group1";
            this.Pnl_Group1.Size = new System.Drawing.Size(1180, 65);
            this.Pnl_Group1.TabIndex = 35;
            this.Pnl_Group1.Visible = false;
            // 
            // Btn_Home
            // 
            this.Btn_Home.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Btn_Home.BackgroundImage")));
            this.Btn_Home.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_Home.Location = new System.Drawing.Point(3, 5);
            this.Btn_Home.Name = "Btn_Home";
            this.Btn_Home.Size = new System.Drawing.Size(60, 60);
            this.Btn_Home.TabIndex = 29;
            this.Btn_Home.UseVisualStyleBackColor = true;
            this.Btn_Home.Click += new System.EventHandler(this.Btn_Home_Click);
            // 
            // Btn_CloseApp
            // 
            this.Btn_CloseApp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Btn_CloseApp.BackgroundImage")));
            this.Btn_CloseApp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Btn_CloseApp.Location = new System.Drawing.Point(1260, 5);
            this.Btn_CloseApp.Name = "Btn_CloseApp";
            this.Btn_CloseApp.Size = new System.Drawing.Size(60, 60);
            this.Btn_CloseApp.TabIndex = 27;
            this.Btn_CloseApp.UseVisualStyleBackColor = true;
            this.Btn_CloseApp.Click += new System.EventHandler(this.Btn_CloseApp_Click);
            // 
            // F_MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.ClientSize = new System.Drawing.Size(1350, 750);
            this.Controls.Add(this.Pnl_Function);
            this.Controls.Add(this.Pnl_Group);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Pnl_Function.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Pnl_Group;
        private System.Windows.Forms.Panel Pnl_Function;
        private System.Windows.Forms.Panel Pnl_Group1;
        private System.Windows.Forms.Button Btn_Home;
        private System.Windows.Forms.Button Btn_CloseApp;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

