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
    public partial class F_StatusBox : Form, IF_StatusBox
    {
        public F_StatusBox(IServiceProvider serviceProvider)
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
            this.TopMost = false;           // 顯示在最上層（避免被遮）
            this.ShowInTaskbar = false;     //不顯示於工具列
            this.Owner = form;              // 指定主窗
            this.StartPosition = FormStartPosition.Manual;

            int x = 10;
            x = form.Left + (form.Width/2 - this.Width/2);

            int y = form.Top + (form.Height - this.Height)/2;
            this.Location = new Point(x, y);

            this.Show();
        }
        private void UpdateUI(string msg, Form form)
        {
            Labl_ShowMessage.Text = msg;
            Labl_Status.BackColor = Color.Red;
            Labl_Status.Text = "FAIL";
            InvokeShowForm(form);
            Tool.SaveLogToFile(msg, level:"ERR");
        }
        private void UpdateUI_OK(string msg, Form form)
        {
            Labl_ShowMessage.Text = msg;
            Labl_Status.BackColor = Color.Green;
            Labl_Status.Text = "PASS";
            InvokeShowForm(form);
            Tool.SaveLogToFile(msg, level: "ERR");
        }
        #endregion

        #region public function
        public void ShowMessage(string msg, string status = "ERR")
        {
            var main_form = ServiceProvider.GetRequiredService<F_MainForm>();

            if (!main_form.IsHandleCreated)
                _ = main_form.Handle;

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    if (status == "ERR")
                        UpdateUI(msg, main_form);
                    else
                        UpdateUI_OK(msg, main_form);

                }));
            }
            else
            {
                UpdateUI(msg, main_form);
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
