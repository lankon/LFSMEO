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
    public partial class UC_ShowTemperatureValue : UserControl
    {
        public UC_ShowTemperatureValue()
        {
            InitializeComponent();
        }

        #region parameter define
        string ItemIndex = "";
        #endregion

        #region private function
        #endregion

        #region public function
        public void SetItemIndex(string index)
        {
            ItemIndex = index;

            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(SetItemIndex), index);
                return;
            }

            int indexNum = int.Parse(index.Replace("ShowTemperatureValue", ""));
            indexNum--;
            Labl_BoxNum.Text = $"Box {indexNum / 4 + 1}-{indexNum % 4 + 1}";
        }
        public void Set_PV(double[] value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<double[]>(Set_PV), value);
                return;
            }

            Label[] labels = new Label[] { Labl_PV5, Labl_PV1, Labl_PV3, Labl_PV7, Labl_PV9 };

            for(int i = 0; i < value.Length; i++)
            {
                labels[i].Text = value[i].ToString("F2");
            }
        }
        public void Set_SV(double value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<double>(Set_SV), value);
                return;
            }
            Labl_SV.Text = value.ToString("F2");
        }
        #endregion
    }
}
