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
using BurnInTester.Base.nUC_TCtrlBoxSetting;

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
        public string GetBoxNum()
        {
            if (this.InvokeRequired)
            {
                return (string)this.Invoke(new Func<string>(GetBoxNum));
            }

            return TxtBx_Box.Text.Trim();
        }
        public string GetChNum()
        {
            if (this.InvokeRequired)
            {
                return (string)this.Invoke(new Func<string>(GetChNum));
            }

            return TxtBx_Channel.Text.Trim();
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
        public void UpdateSetting(string boxNum, string chNum, string use)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, string, string>(UpdateSetting), boxNum, chNum, use);
                return;
            }
            TxtBx_Box.Text = boxNum.ToString();
            TxtBx_Channel.Text = chNum.ToString();
            Cmbx_Use.SelectedIndex = Tool.StringToInt(use);
        }
        public void SetTCtrlEnable(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(SetTCtrlEnable), enable);
                return;
            }
            
            Cmbx_Use.SelectedIndex = enable ? (int)eUseItem.USE : (int)eUseItem.PASS;
        }
        #endregion
    }
}
