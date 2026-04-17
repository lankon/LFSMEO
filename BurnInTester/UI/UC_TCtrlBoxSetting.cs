using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolFunction;

namespace BurnInTester.UI
{
    public partial class UC_TCtrlBoxSetting : UserControl
    {
        public UC_TCtrlBoxSetting()
        {
            InitializeComponent();

            //Labl_Information.Click += Control_Click;
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
        //public void SetInformation(string information)
        //{
        //    if(this.InvokeRequired)
        //    {
        //        this.Invoke(new Action<string>(SetInformation), information);
        //        return;
        //    }

        //    Labl_Information.Text = information;
        //}
        

        public void SetItemIndex(string index)
        {
            ItemIndex = index;

            if(this.InvokeRequired)
            {
                this.Invoke(new Action<string>(SetItemIndex), index);
                return;
            }

            int indexNum = int.Parse(index.Replace("TCtrlBoxSetting", ""));
            indexNum--;
            Labl_BoxNum.Text = $"Box {indexNum / 4 + 1}-{indexNum % 4 + 1}";
        }
        public int GetBoxNum()
        {
            if (this.InvokeRequired)
            {
                return (int)this.Invoke(new Func<int>(GetBoxNum));
            }

            int res = Tool.StringToInt(TxtBx_Box.Text.Trim());
            return res;
        }
        public int GetChNum()
        {
            if (this.InvokeRequired)
            {
                return (int)this.Invoke(new Func<int>(GetChNum));
            }
            int res = Tool.StringToInt(TxtBx_Channel.Text.Trim());
            return res;
        }
        public int GetBoxIndex()
        {
            return int.Parse(ItemIndex.Replace("TCtrlBoxSetting", ""));
        }
        public int GetUse()
        {
            if (this.InvokeRequired)
            {
                return (int)this.Invoke(new Func<int>(GetUse));
            }
            int res = Cmbx_Use.SelectedIndex;
            return res;
        }
        #endregion
    }
}
