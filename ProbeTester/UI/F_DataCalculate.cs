using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ToolFunction;
using ProbeTester.Logic;
using ProbeTester.Base;

namespace ProbeTester.UI
{
    public partial class F_DataCalculate : Form
    {
        public F_DataCalculate(F_DataCalculateLogic f_DataCalculateLogic)
        {
            InitializeComponent();

            DataCalculateLogic = f_DataCalculateLogic;

            InitialForm();
        }

        #region parameter define
        private F_DataCalculateLogic DataCalculateLogic;
        private ScottPlot.Plottable.HSpan SelectionSpan;    //滑鼠選擇範圍
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            //if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
            //    Tool.ShowFormName(this);
        }
        void ShowHint()
        {

        }
        private void ReadAllEnumSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_DataCalculate>();
            //ApplicationSetting.ReadAllRecipe<eF_StartForm>();

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.ReadAllRecipe<eF_StartFormRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            ApplicationSetting.UpdataRecipeToForm<eF_DataCalculate>(this);
            //ApplicationSetting.UpdataRecipeToForm<eF_StartFormRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_DataCalculate>(this);

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.SaveRecipeFromForm<eF_StartFormRecipe>(this, recipe_name);
        }
        private void UpdatePage()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();
        }
        private void LeavePage()
        {
        }
        private void InitialPlot()
        {
            Plot_DataShow.Plot.Clear();
            SelectionSpan = Plot_DataShow.Plot.AddHorizontalSpan(7473, 54524);
            SelectionSpan.Color = Color.FromArgb(50, Color.Blue);
            SelectionSpan.DragEnabled = true;
        }
        private void DrawPlot()
        {
            double[] dataY = DataCalculateLogic.GetPositionErrorData().ToArray();
            double[] dataY1 = DataCalculateLogic.GetCurrentData().ToArray();
            double[] dataX = ScottPlot.DataGen.Consecutive(dataY.Length);

            //資料1
            //var scatter1 = Plot_DataShow.Plot.AddScatter(dataX, dataY);
            var scatter1 = Plot_DataShow.Plot.AddSignal(dataY);
            scatter1.Color = Color.Blue;

            //資料2
            //var scatter2 = Plot_DataShow.Plot.AddSignal(dataX, dataY1);
            var scatter2 = Plot_DataShow.Plot.AddSignal(dataY1);
            scatter2.Color = Color.Red;                         // 設定顏色
            scatter2.YAxisIndex = 1;                            //指定使用右側Y軸
            Plot_DataShow.Plot.YAxis2.Label("Amp");
            Plot_DataShow.Plot.YAxis2.Ticks(true);              //開啟右側刻度顯示
            Plot_DataShow.Plot.YAxis2.Color(scatter2.Color);    // 讓右軸顏色跟第二條線一致

            Plot_DataShow.Refresh();
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {

        }
        #endregion
        private void F_Equipment_Setting_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                SaveAllEnumSetting();
                ReadAllEnumSetting();

                LeavePage();
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

        private void Btn_LoadFilePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV 檔案 (*.csv)|*.csv|所有檔案 (*.*)|*.*";
            openFileDialog.Title = "請選擇 CSV 檔案";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                TxtBx_PathName.Text = filePath;
                DataCalculateLogic.ReadFile(filePath);
            }
            else
                return;

            InitialPlot();
            DrawPlot();

            Btn_Calculate.Enabled = true;
        }

        private void Btn_Calculate_Click(object sender, EventArgs e)
        {
            SaveAllEnumSetting();
            ReadAllEnumSetting();

            double SelectStart = SelectionSpan.X1;
            DataCalculateLogic.SelectRange(SelectionSpan.X1, SelectionSpan.X2);
            DataCalculateLogic.Calculate();

            //畫出爬升計算區間
            int start = DataCalculateLogic.CalculateDataResult.RisingStart;
            int end = DataCalculateLogic.CalculateDataResult.RisingEnd;
            if (start != -1 && end != -1)
            {
                var span = Plot_DataShow.Plot.AddHorizontalSpan(start + SelectStart, end + SelectStart);
                span.Color = Color.FromArgb(100, Color.Yellow);
                double time = ApplicationSetting.Get_Double_Recipe<eF_DataCalculate>((int)eF_DataCalculate.TxtBx_TimePerPoint);
                TxtBx_SystemStableTime.Text = ((end - start) * time / 1000).ToString();
            }

            //畫出穩定運動區間
            int stable_start = DataCalculateLogic.CalculateDataResult.StableStart;
            int stable_end = DataCalculateLogic.CalculateDataResult.StableEnd;
            var span_stable = Plot_DataShow.Plot.AddHorizontalSpan(stable_start + SelectStart, stable_end + SelectStart);
            span_stable.Color = Color.FromArgb(100, Color.Green);
            TxtBx_RMS.Text = DataCalculateLogic.CalculateDataResult.RMS.ToString();

            //畫出靜摩擦力位置
            var marker = Plot_DataShow.Plot.AddMarker(DataCalculateLogic.CalculateDataResult.Fs_pos + SelectStart, 
                                                        DataCalculateLogic.CalculateDataResult.Fs);
            marker.YAxisIndex = 1;                  //依照副軸
            marker.Color = Color.Orange;            // 點的顏色
            marker.MarkerSize = 15;                 // 點的大小 (像素)
            TxtBx_MaxCurrent.Text = DataCalculateLogic.CalculateDataResult.Fs.ToString();

            //更新畫面
            Plot_DataShow.Refresh();

            Btn_Calculate.Enabled = false;
        }
    }
}

