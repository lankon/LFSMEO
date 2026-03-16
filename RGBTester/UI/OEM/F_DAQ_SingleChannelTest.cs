using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using ToolFunction;
using DeviceCore;

namespace RGBTester.UI
{
    public partial class F_DAQ_SingleChannelTest : Form
    {
        public F_DAQ_SingleChannelTest(IFunction_IO_Card io)
        {
            InitializeComponent();

            DIOL = io;

            InitialForm();
        }

        #region parameter define
        IFunction_IO_Card DIOL;
        #endregion

        #region private function
        void InitialForm()
        {
            //ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();
            //ApplicationSetting.ReadAllRecipe<eMachineSetting>();
            //ApplicationSetting.UpdataRecipeToForm<eF_Equipment_Setting>(this);
            //ApplicationSetting.UpdataRecipeToForm<eMachineSetting>(this);

            ShowHint();

            //if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
            //    Tool.ShowFormName(this);
        }
        void ShowHint()
        {

        }
        private void UpdatePage()
        {
        }
        #endregion

        #region public function
        
        #endregion
        private void F_Equipment_Setting_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                ////儲存參數
                //ApplicationSetting.SaveRecipeFromForm<eF_Equipment_Setting>(this);
                //ApplicationSetting.SaveRecipeFromForm<eMachineSetting>(this);
                ////重新讀取變數值
                //ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();
                //ApplicationSetting.ReadAllRecipe<eMachineSetting>();

                ////釋放記憶體資源
                //Tool.ReleaseButtonImages(this);
                //this.Close();
                //this.Dispose();
            }
            else
            {
                UpdatePage();
            }
        }

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            if (TxtBx_AveraingCount.Text == "" || TxtBx_IOChannel.Text == "")
                return;
            long CycleTime = 0;
            const int channel_count = 10;
            double total_res = 0;
            string[] context = new string[channel_count];
            double[] result = new double[channel_count];
            List<string[]> buffer = new List<string[]>();
            int AvgCount = Int32.Parse(TxtBx_AveraingCount.Text);
            int IO_Channel = Int32.Parse(TxtBx_IOChannel.Text);

            _ = DIOL.GetAInputStatus(EIOCardType.PCI_9111HR, 0, 0, 0, (byte)IO_Channel, "+-10V",0);

            DGV_DAQ_Result.Rows.Clear();

            for (int i=0; i<result.Length; i++)
                result[i] = 0;

            Tool.ResetTimeCount(out CycleTime);

            for(int i=0; i< channel_count; i++)
            {
                for (int j = 0; j < AvgCount; j++)
                {
                    double res = DIOL.GetAInputStatus(EIOCardType.PCI_9111HR, 0, 0, 0, (byte)IO_Channel, "+-10V", 0);
                    total_res += res;
                }

                result[i] = total_res / AvgCount;
                context[i] = result[i].ToString();

                total_res = 0;
            }
            buffer.Add(context);

            TxtBx_TotalTestTime.Text =  Tool.GetTime(CycleTime, "us").ToString();

            foreach (var row in buffer)
                Tool.DataGrid_AddInEndRow(DGV_DAQ_Result, row);
        }

        private void Btn_SaveData_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Now;
            string path = Application.StartupPath + $@"\Result\DAQ_SamplingTest_{date.ToString("yyyyMMddHHmmss")}.csv";

            Tool.DataGridSaveToCsv(DGV_DAQ_Result, path);
        }
    }
}
