using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using RGBTester.Base;

namespace RGBTester.UI
{
    public partial class F_StateControl : Form, IF_StateControl
    {
        public F_StateControl(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            this.Hide();
            ServiceProvider = serviceProvider;
            SetHint();
        }

        #region parameter define
        private IBaseMainTask MainTask;
        private IServiceProvider ServiceProvider;
        #endregion

        #region private function
        private void BtnEnableVisible(Button btn, bool flag)
        {
            btn.Visible = flag;
            btn.Enabled = flag;
        }
        private void SetHint()
        {
            toolTip1.SetToolTip(BtnPause, "Pause");
            toolTip1.SetToolTip(BtnAbort, "Abort");
            toolTip1.SetToolTip(BtnContinue, "Continue");

        }
        private void UpdateUI(TASK_STATUS status)
        {
            if (status == TASK_STATUS.ABORT_CONTINUE)
            {
                BtnEnableVisible(BtnPause, false);
                BtnEnableVisible(BtnAbort, true);
                BtnEnableVisible(BtnContinue, true);
            }
            else if (status == TASK_STATUS.ABORT)
            {
                BtnEnableVisible(BtnPause, false);
                BtnEnableVisible(BtnAbort, true);
                BtnEnableVisible(BtnContinue, false);
            }
            else if (status == TASK_STATUS.PAUSE)
            {
                BtnEnableVisible(BtnPause, true);
                BtnEnableVisible(BtnAbort, false);
                BtnEnableVisible(BtnContinue, false);
            }
            else
            {
                BtnEnableVisible(BtnPause, false);
                BtnEnableVisible(BtnAbort, false);
                BtnEnableVisible(BtnContinue, false);
            }
        }
        private void InvokeShowForm(Form form, int pos)
        {
            this.TopMost = false;           // 顯示在最上層（避免被遮）
            this.ShowInTaskbar = false;     //不顯示於工具列
            this.Owner = form;              // 指定主窗
            this.StartPosition = FormStartPosition.Manual;

            int x = 10;
            if (pos == 0)
                x = form.Left + 10;
            else if(pos == 1)
                x = form.Left + 10 + 420;

            int y = form.Top + form.Height - this.Height - 10;
            this.Location = new Point(x, y);

            this.Show();
        }
        #endregion

        #region public function
        public void SetMainTask(IBaseMainTask baseMainTask)
        {
            MainTask = baseMainTask;
        }
        public void UpdateTask(string msg)
        {
            if (LablTaskState.InvokeRequired)
            {
                LablTaskState.Invoke(new Action(() => LablTaskState.Text = msg));
            }
            else
            {
                LablTaskState.Text = msg;
            }
        }
        public void SetPauseAbortContinue(TASK_STATUS status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateUI(status)));
            }
            else
            {
                UpdateUI(status);
            }
        }
        public void ShowForm(int pos)
        {
            var main_form = ServiceProvider.GetRequiredService<F_MainForm>();

            if (InvokeRequired)
            {
                Invoke(new Action(() => InvokeShowForm(main_form, pos)));
            }
            else
            {
                InvokeShowForm(main_form, pos);
            }
        }
        public void HideForm()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => this.Hide()));
            }
            else
            {
                this.Hide();
            }
        }
        public void CloseForm()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => this.Close()));
            }
            else
            {
                this.Close();
            }
        }
        public void GotoPause()
        {

        }
        #endregion

        private void BtnPause_Click(object sender, EventArgs e)
        {
            MainTask.GoToPause();
        }

        private void BtnAbort_Click(object sender, EventArgs e)
        {
            MainTask.GoToAbort();
        }

        private void BtnContinue_Click(object sender, EventArgs e)
        {
            MainTask.GoToContinue();
        }

        private void F_StateControl_VisibleChanged(object sender, EventArgs e)
        {
           
        }
    }
}
