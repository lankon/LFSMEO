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
    public partial class F_ProgressBar : Form, IF_ProgressBar
    {
        public F_ProgressBar(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            this.Show();
            this.Hide();
            
            ServiceProvider = serviceProvider;
            SetHint();
        }

        #region parameter define
        private IServiceProvider ServiceProvider;
        #endregion

        #region private function
        private void SetHint()
        {
            //toolTip1.SetToolTip(BtnPause, "Pause");
            //toolTip1.SetToolTip(BtnAbort, "Abort");
            //toolTip1.SetToolTip(BtnContinue, "Continue");

        }
        private void InvokeShowForm(Form form)
        {
            this.TopMost = true;            // 顯示在最上層（避免被遮）
            this.ShowInTaskbar = false;     //不顯示於工具列
            this.Owner = form;              // 指定主窗
            this.StartPosition = FormStartPosition.Manual;

            int x = 10;
            x = form.Left + (form.Width/2 - this.Width/2);

            int y = form.Top + (form.Height - this.Height)/2;
            this.Location = new Point(x, y);

            this.Show();
        }
        private void UpdateUI(int value, Form form)
        {
            InvokeShowForm(form);

            PgBar_Testing.Minimum = 0;
            PgBar_Testing.Maximum = 100;

            PgBar_Testing.Value = value;
        }
        private void Hideform()
        {
            var main_form = ServiceProvider.GetRequiredService<F_MainForm>();

            // 顯示到最上層
            main_form.Activate();
            main_form.BringToFront();

            this.Hide();
        }
        #endregion

        #region public function
        public void UpateProgress(int value)
        {
            var main_form = ServiceProvider.GetRequiredService<F_MainForm>();

            if (!main_form.IsHandleCreated)
                _ = main_form.Handle;

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    UpdateUI(value, main_form);
                }));
            }
            else
            {
                UpdateUI(value, main_form);
            }
        }
        public void HideForm()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Hideform()));
            }
            else
            {
                Hideform();
            }
        }
        #endregion

        private void F_StatusBox_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void Btn_Confirm_Click(object sender, EventArgs e)
        {
            var main_form = ServiceProvider.GetRequiredService<F_MainForm>();
            
            // 顯示到最上層
            main_form.Activate();
            main_form.BringToFront();

            this.Hide();
        }
    }
}
