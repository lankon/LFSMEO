using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ToolFunction;
using LFSMEO.Base_LFSMEO;
using System.Threading;

namespace LFSMEO
{
    public partial class F_MainForm : Form
    {
        #region parameter define
        F_MainFormManage F_MainFormManage = new F_MainFormManage();
        #endregion

        #region private function
        private void Pnl_Function_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = (Panel)sender;

            e.Graphics.Clear(pnl.BackColor);
            e.Graphics.DrawString(pnl.Text, pnl.Font, Brushes.Black, 10, 1);
            var vSize = e.Graphics.MeasureString(pnl.Text, pnl.Font);
            //e.Graphics.DrawLine(Pens.Black, 1, vSize.Height / 2, 8, vSize.Height / 2);
            //e.Graphics.DrawLine(Pens.Black, vSize.Width + 8, vSize.Height / 2, pnl.Width - 2, vSize.Height / 2);
            //e.Graphics.DrawLine(Pens.Black, 1, vSize.Height / 2, 1, pnl.Height - 2);
            e.Graphics.DrawLine(Pens.Black, 1, pnl.Height - 2, pnl.Width - 2, pnl.Height - 2);
            //e.Graphics.DrawLine(Pens.Black, pnl.Width - 2, vSize.Height / 2, pnl.Width - 2, pnl.Height - 2);
        }
        private void InitialApplication()
        {
            SetHint();

            CreateDynamicElement();

            CreateFolder();

            ApplicationSetting.ReadAllRecipe<eOEMSetting>();
            ApplicationSetting.UpdataRecipeToForm<eOEMSetting>(this);

            F_MainFormManage.InitialMachine();
            F_MainFormManage.CreateStartForm();
        }
        private void CreateDynamicElement()
        {
            // Panel 主要顯示頁面
            //
            Scope.MainPanel = new Panel();
            Scope.MainPanel.Location = new System.Drawing.Point(0, 0);
            Scope.MainPanel.Size = new System.Drawing.Size(1326, 661);
            Scope.MainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            Scope.MainPanel.Visible = true;
            this.Pnl_Group.Controls.Add(Scope.MainPanel);

            //
            // Panel 顯示上方選項頁面
            //
            Scope.UpButtonPanel = new Panel();
            Scope.UpButtonPanel.Location = new System.Drawing.Point(69, 0);
            Scope.UpButtonPanel.Size = new System.Drawing.Size(1180, 65);
            Scope.UpButtonPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.Pnl_Function.Controls.Add(Scope.UpButtonPanel);

        }
        private void CreateFolder()
        {
            Tool.CreateFolder(Application.StartupPath + @"\Temp");
            Tool.CreateFolder(Application.StartupPath + @"\History");
            Tool.CreateFolder(Application.StartupPath + @"\Picture");
            Tool.CreateFolder(Application.StartupPath + @"\Result");
            Tool.CreateFolder(Application.StartupPath + @"\Setting");
        }
        private void SetHint()
        {
            toolTip1.SetToolTip(Btn_CloseApp, "Close");
            toolTip1.SetToolTip(Btn_Home, "Home");
        }
        protected override CreateParams CreateParams    //防止UI元件更新時畫面閃爍
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        #endregion

        public F_MainForm()
        {
            InitializeComponent();

            InitialApplication();

            this.Shown += (s, e) =>
            {
                F_MainFormManage.ShowStartForm();
            };
        }

        private void Btn_CloseApp_Click(object sender, EventArgs e)
        {
            // 顯示確認對話框
            DialogResult dialogResult = MessageBox.Show("Close Application ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // 根據用戶的選擇返回布爾值
            if (dialogResult == DialogResult.Yes)
            {
                F_MainFormManage.SaveRecipeWhenCloseApp();

                Application.Exit();
                Tool.SaveLogToFile("關閉應用程式"); 
            }
            else
            {

            }
        }

        private void Btn_Home_Click(object sender, EventArgs e)
        {
            F_MainFormManage.ShowStartForm();
        }
    }
}
