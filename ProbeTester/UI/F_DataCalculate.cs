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
        public F_DataCalculate()
        {
            InitializeComponent();

            InitialForm();
        }

        #region parameter define
        ScottPlot.Plottable.HSpan selectionSpan;
        List<double> PositionError = new List<double>();
        List<double> Current = new List<double>();
        DataFilter Filter = new DataFilter();
        int SelectStart = 0;
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

        //[計算靜摩擦力電流]
        private void FindStaticFrictionCur(int start, int end, List<double> current)
        {
            double max = 0; int position = 0; ;
            for (int i = end - 1500; i < end + 1500; i++)
            {
                if (current[i] > max)
                {
                    max = current[i];
                    position = i;
                }
            }

            var marker = Plot_DataShow.Plot.AddMarker(position + SelectStart, max);
            marker.YAxisIndex = 1;                  //依照副軸
            marker.Color = Color.Orange;            // 點的顏色
            marker.MarkerSize = 15;                 // 點的大小 (像素)

            TxtBx_MaxCurrent.Text = max.ToString();
        }
        
        //[計算爬升段電流]
        private void FindRising(List<double> data, List<double> current)
        {
            double startValue = 1;      // 稍微高於零點平台，代表開始上升
            double endValue = Filter.GetPreciseHighLevel(data, 0.9);

            int startIndex = data.FindIndex(x => x >= startValue);
            int endIndex = data.FindIndex(startIndex, x => x >= endValue);

            //找出靜摩擦力電流峰值
            FindStaticFrictionCur(startIndex, endIndex, current);

            if (startIndex != -1 && endIndex != -1)
            {
                //在ScottPlot上畫出這段範圍
                var span = Plot_DataShow.Plot.AddHorizontalSpan(startIndex + SelectStart, endIndex + SelectStart);
                span.Color = Color.FromArgb(100, Color.Yellow); // 使用淺紫色
                double time = ApplicationSetting.Get_Double_Recipe<eF_DataCalculate>((int)eF_DataCalculate.TxtBx_TimePerPoint);
                TxtBx_SystemStableTime.Text = ((endIndex - startIndex) * time / 1000).ToString();
                Plot_DataShow.Refresh();
            }

            int fallStartIndex = data.FindIndex(endIndex, x => x <= endValue*0.99);

            if (fallStartIndex != -1)
            {
                CalculateStableRMS(endIndex, fallStartIndex, current);
            }
        }

        //[計算平穩段RMS]
        private void CalculateStableRMS(int start, int end, List<double> data)
        {
            if (data == null || data.Count == 0) 
                return ;

            end = end - 1000;
            start = start + 1000;

            int count = (end) - (start) + 1;

            //平方和
            double sumOfSquares = data.Skip(start)
                                      .Take(count)
                                      .Select(x => x * x)
                                      .Sum();

            //rms
            double rms = Math.Sqrt(sumOfSquares / count);

            var span = Plot_DataShow.Plot.AddHorizontalSpan(start + SelectStart, end + SelectStart);
            span.Color = Color.FromArgb(100, Color.Green); // 使用淺紫色
            Plot_DataShow.Refresh();
            TxtBx_RMS.Text = rms.ToString();

            return;
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
                string[] lines = File.ReadAllLines(filePath);
                bool start_read = false;

                foreach (string line in lines)
                {
                    if(start_read == true)
                    {
                        string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                        if (double.TryParse(parts[1], out double value))
                        {
                            PositionError.Add(value);
                        }
                        if(double.TryParse(parts[2], out double value1))
                        {
                            Current.Add(value1);
                        }
                    }
                    
                    if (line.Contains("                                CH1                                CH2"))
                    {
                        start_read = true;
                    }
                }
            }

            Plot_DataShow.Plot.Clear();
            selectionSpan = Plot_DataShow.Plot.AddHorizontalSpan(7473, 54524);
            selectionSpan.Color = Color.FromArgb(50, Color.Blue);
            selectionSpan.DragEnabled = true;
            double[] dataY = PositionError.ToArray();
            double[] dataY1 = Current.ToArray();
            double[] dataX = ScottPlot.DataGen.Consecutive(dataY.Length);

            double time = ApplicationSetting.Get_Double_Recipe<eF_DataCalculate>((int)eF_DataCalculate.TxtBx_TimePerPoint);
            double[] scaledX = dataX.Select(x => x * time).ToArray();
            var scatter1 = Plot_DataShow.Plot.AddScatter(dataX, dataY);
            scatter1.Color = Color.Blue; // 設定顏色

            var scatter2 = Plot_DataShow.Plot.AddScatter(dataX, dataY1);
            scatter2.Color = Color.Red; // 設定顏色
            scatter2.YAxisIndex = 1; // 指定使用右側 Y 軸
            Plot_DataShow.Plot.YAxis2.Label("Amp");
            Plot_DataShow.Plot.YAxis2.Ticks(true); // 重要！預設右軸刻度是隱藏的，必須開啟
            Plot_DataShow.Plot.YAxis2.Color(scatter2.Color); // 讓右軸顏色跟第二條線一致

            Plot_DataShow.Refresh();
        }

        private void Btn_Calculate_Click(object sender, EventArgs e)
        {
            // 1. 取得目前框選的 X 軸範圍 (Index)
            double minX = selectionSpan.X1;
            double maxX = selectionSpan.X2;

            // 確保 min 小於 max (防止使用者反向拖拉)
            SelectStart = (int)Math.Max(0, Math.Min(minX, maxX));
            int endIndex = (int)Math.Min(PositionError.Count - 1, Math.Max(minX, maxX));

            // 2. 篩選出該範圍內的數據
            var rangeData = PositionError.Skip(SelectStart).Take(endIndex - SelectStart + 1).ToList();
            var cur_rangeDate = Current.Skip(SelectStart).Take(endIndex - SelectStart + 1).ToList();
            if (rangeData.Count > 0)
            {
                FindRising(rangeData, cur_rangeDate);
            }
        }
    }
}

