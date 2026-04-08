using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BurnInTester.UI
{
    public partial class UC_CtrlBoxStatus : UserControl
    {
        public UC_CtrlBoxStatus()
        {
            InitializeComponent();

            Labl_Information.Click += Control_Click;
        }

        #region parameter define
        string ItemIndex = "";
        #endregion

        #region private function
        private void Control_Click(object sender, EventArgs e)
        {
            // 觸發 UserControl 本身的 Click 事件去呼叫外面的事件
            this.OnClick(e);
        }
        #endregion

        #region public function
        public void SetInformation(string information)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action<string>(SetInformation), information);
                return;
            }

            Labl_Information.Text = information;
        }
        public void SetItemIndex(string index)
        {
            ItemIndex = index;

            if(this.InvokeRequired)
            {
                this.Invoke(new Action<string>(SetItemIndex), index);
                return;
            }

            int indexNum = int.Parse(index.Replace("CtrlBoxStatus", ""));
            indexNum--;
            Labl_BoxNum.Text = $"Box {indexNum / 4 + 1}-{indexNum % 4 + 1}";
        }
        #endregion
    }
}
