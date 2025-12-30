using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using RGBTester.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolFunction;


namespace RGBTester.UI
{
    public partial class F_MainForm : Form, IF_MainForm
    {
        public F_MainForm(F_MainFormLogic f_MainFormLogic, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            MainFormLogic = f_MainFormLogic;
            ServiceProvider = serviceProvider;

            MainFormLogic.SetForm(this);

            InitialApplication();

            this.Shown += (s, e) =>
            {
                ShowStartForm();
            };
        }

        #region parameter define
        private F_MainFormLogic MainFormLogic;
        private IServiceProvider ServiceProvider;
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

            MainFormLogic.DeleteExpireFileInFolder();
            MainFormLogic.ReadAllSetting();
            MainFormLogic.Initial_IO_Function();
            Labl_Version.Text = MainFormLogic.GetVersion();

            ServiceProvider.GetRequiredService<IBaseMainTask>();
            ServiceProvider.GetRequiredService<IBaseMainTaskMulti>();
            ServiceProvider.GetRequiredService<IF_StatusBox>();
            ServiceProvider.GetRequiredService<IF_ProgressBar>();
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
            Tool.CreateFolder(Application.StartupPath + @"\Backup");
            Tool.CreateFolder(Application.StartupPath + @"\Setting\Package");

            Tool.CreateLog();
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
        private void UpdateAllSettingToForm()
        {

        }
        private void ShowStartForm()
        {
            Tool.HideElementOnPanel(Scope.MainPanel);

            var startForm = ServiceProvider.GetRequiredService<IF_StartForm>();

            if(startForm is Form form)
            {
                Tool.SetForm(Scope.MainPanel, form);
                form.Show();
            }

            var group = ServiceProvider.GetRequiredService<F_StartForm_ButtonGroup>();
            Tool.SetForm(Scope.UpButtonPanel, group);
            group.Show();
        }
        #endregion

        private void Btn_CloseApp_Click(object sender, EventArgs e)
        {
            // 顯示確認對話框
            DialogResult dialogResult = MessageBox.Show("Close Application ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // 根據用戶的選擇返回布爾值
            if (dialogResult == DialogResult.Yes)
            {
                //F_MainFormManage.SaveRecipeWhenCloseApp();

                Application.Exit();
                Tool.SaveLogToFile("關閉應用程式"); 
            }
            else
            {

            }
        }

        private void Btn_Home_Click(object sender, EventArgs e)
        {
            ShowStartForm();
        }
    }
}
