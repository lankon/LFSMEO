using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using RGBTester.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using ToolFunction;
using static RGBTester.Logic.F_DAQ_ChartLogic;

namespace RGBTester.UI
{
    public partial class F_DAQ_Chart : Form
    {
        public F_DAQ_Chart(F_DAQ_ChartLogic f_DAQ_ChartLogic, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            DAQ_ChartLogic = f_DAQ_ChartLogic;
            ServiceProvider = serviceProvider;
            InitialForm();
        }

        #region parameter define
        private int _maxPoints = 50;        // 橫軸顯示的點數
        private int _dataCount = 0;         // 目前累計的點數
        private double visualScale = 0.9;   // 視覺放大倍率
        private double _offsetStep = 3;     // 每個通道錯開的距離
        private F_DAQ_ChartLogic DAQ_ChartLogic;
        private IServiceProvider ServiceProvider;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);
        }
        void ShowHint()
        {

        }
        private void ReadAllEnumSetting()
        {
            //ApplicationSetting.ReadAllRecipe<eOEMSetting>();
            //ApplicationSetting.ReadAllRecipe<eF_StartForm>();

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.ReadAllRecipe<eF_StartFormRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            //ApplicationSetting.UpdataRecipeToForm<eF_StartForm>(this);
            //ApplicationSetting.UpdataRecipeToForm<eF_StartFormRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            //ApplicationSetting.SaveRecipeFromForm<eF_StartForm>(this);

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.SaveRecipeFromForm<eF_StartFormRecipe>(this, recipe_name);
        }
        private void UpdatePage()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            SetupChart();
        }

        #region Chart Functions
        private void SetupChart()
        {
            // 1. 基本設定
            chart1.Series.Clear();
            chart1.BackColor = Color.FromArgb(30, 30, 30);          // 外部邊框深灰色
            //chart1.ChartAreas[0].AxisX.Title = "Point Count";
            //chart1.ChartAreas[0].AxisY.Title = "Voltage(V)";

            // 設定 Y 軸固定範圍，防止畫面一直縮放晃動
            chart1.ChartAreas[0].AxisY.Minimum = -1.5;
            chart1.ChartAreas[0].AxisY.Maximum = (4 * _offsetStep) + 1.5;

            chart1.ChartAreas[0].AxisY.LabelStyle.Enabled = true;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;

            chart1.ChartAreas[0].AxisY.CustomLabels.Clear();

            var area = chart1.ChartAreas[0];
            // 1. 整體背景設定
            area.BackColor = Color.Black;                          // 繪圖區黑色
            area.BorderColor = Color.FromArgb(64, 64, 64);         // 繪圖區外框線
            area.BorderDashStyle = ChartDashStyle.Solid;

            // 2. X 軸設定 (時間/點數軸)
            area.AxisX.LineColor = Color.Gray;
            area.AxisX.LabelStyle.ForeColor = Color.LightGray;
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(50, 50, 50); // 深灰色格線
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;     // 虛線格線
            area.AxisX.MajorTickMark.Enabled = false;                    // 隱藏凸出刻度

            // 3. Y 軸設定 (電壓軸)
            area.AxisY.LineColor = Color.Gray;
            area.AxisY.LabelStyle.ForeColor = Color.LightGray;
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(50, 50, 50);
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            area.AxisY.MajorTickMark.Enabled = false;

            // 取得第一個圖例物件（預設名稱為 "Default"）
            Legend legend = chart1.Legends[0];

            // 1. 背景設為透明
            legend.BackColor = Color.Transparent;

            // 2. 文字設為淺灰色或白色
            legend.ForeColor = Color.LightGray;

            // 3. 移除邊框
            legend.BorderColor = Color.Transparent;

            // 4. 設定字體與大小
            legend.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // 5. 調整對齊位置（例如放在圖表右上角內部）
            legend.Docking = Docking.Top; // 或者使用 Docking.InsideChartArea
            legend.Alignment = StringAlignment.Far; // 靠右
            legend.LegendStyle = LegendStyle.Row;    // 這行很重要，讓圖例橫向排列

            // 2. 建立 5 個通道 (Series)
            Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Purple };
            for (int i = 0; i < 5; i++)
            {
                string[] seriesNames = { "Vin", "Iin", "VLed", "Vf", "ILed" };
                Series series = new Series(seriesNames[i]);

                series.ChartType = SeriesChartType.Line;
                series.BorderWidth = 2;
                series.Color = colors[i];
                chart1.Series.Add(series);
            }
        }
        
        private void UpdateChart(DAQDataResult result)
        {
            SetupChart();

            ProcessChannel(0, result.Vin);
            ProcessChannel(1, result.Iin);
            ProcessChannel(2, result.Vled);
            ProcessChannel(3, result.Vf);
            ProcessChannel(4, result.Iled);

            UpdateAvgResult(result);
        }

        private void AddYAxisLabels(double offset, double maxAmp, double minAmp, double midAmp)
        {
            var axisY = chart1.ChartAreas[0].AxisY;

            axisY.CustomLabels.Add(offset - 0.2, offset + 0.2, $"{midAmp:F3}", 0, LabelMarkStyle.None);
            axisY.CustomLabels.Add(offset + visualScale - 0.2, offset + visualScale + 0.2, $"{maxAmp:F3}", 0, LabelMarkStyle.None);
            axisY.CustomLabels.Add(offset - visualScale - 0.2, offset - visualScale + 0.2, $"{minAmp:F3}", 0, LabelMarkStyle.None);
        }

        private void ProcessChannel(int channelIndex, double[] data)
        {
            if (data == null || data.Length == 0) return;

            double minVal = data.Min();
            double maxVal = data.Max();
            double range = maxVal - minVal;

            double offset = _offsetStep * channelIndex;

            // 設定一個最小顯示範圍，避免過度放大雜訊
            double displayRange = Math.Max(range, 0.01);
            double midVal = (maxVal + minVal) / 2.0;

            for (int pt = 0; pt < data.Length; pt++)
            {
                // 核心公式： ( (數值 - 中心值) / (範圍/2) ) * 視覺縮放 + 通道偏移
                // 這會把波形強行拉開，以 midVal 為中心上下震盪
                double normalizedVal = ((data[pt] - midVal) / (displayRange / 2.0)) * visualScale;

                // 加上通道偏移量
                chart1.Series[channelIndex].Points.AddXY((pt + 1)*0.04*5, normalizedVal + offset);
            }

            AddYAxisLabels(offset, maxVal, minVal, midVal);
        }

        private void UpdateAvgResult(DAQDataResult result)
        {
            var avgFunc = DAQ_ChartLogic.CalculateCaptureDataAvg(result);

            Labl_Result1.Text = avgFunc.Avg_Vin.ToString("F3");
            Labl_Result2.Text = avgFunc.Avg_Iin.ToString("F3");
            Labl_Result3.Text = avgFunc.Avg_Vled.ToString("F3");
            Labl_Result4.Text = avgFunc.Avg_Vf.ToString("F3");
            Labl_Result5.Text = avgFunc.Avg_Iled.ToString("F3");
        }
        private void AddZeroLines()
        {
            var axisY = chart1.ChartAreas[0].AxisY;
            axisY.StripLines.Clear(); // 清除舊的線條

            for (int i = 0; i < 5; i++)
            {
                double zeroPosition = i * _offsetStep; // 這是每個通道的 0V 基準點

                StripLine blackLine = new StripLine();
                blackLine.IntervalOffset = zeroPosition;  // 設定線條位置
                blackLine.StripWidth = 0;                 // 寬度設為 0 會變成一條單純的線

                // 設定線條樣式
                //blackLine.BorderColor = Color.Black;      // 黑色線
                blackLine.BorderColor = Color.FromArgb(100, Color.Black); // 稍微帶點透明度
                blackLine.BorderWidth = 1;                // 線條粗細
                blackLine.BorderDashStyle = ChartDashStyle.Dash; // 實線 (如果要虛線改用 Dash)

                // 確保線條畫在波形「下方」(背景層)
                // 如果想畫在波形「上方」，則需調整圖表層級
                axisY.StripLines.Add(blackLine);
            }
        }
        private void UpdateChart(double[] newVoltages)
        {
            _dataCount++;

            for (int i = 0; i < 5; i++)
            {
                double visualValue = newVoltages[i] + (i * _offsetStep);

                // 加入新點 (X 是點數, Y 是電壓)
                chart1.Series[i].Points.AddXY(_dataCount, visualValue);

                // 如果點數太多，移除最舊的點，達成「捲動」效果
                if (chart1.Series[i].Points.Count > _maxPoints)
                {
                    chart1.Series[i].Points.RemoveAt(0);
                }
            }

            // 自動調整 X 軸範圍，讓圖表動起來
            chart1.ChartAreas[0].AxisX.Minimum = chart1.Series[0].Points[0].XValue;
            chart1.ChartAreas[0].AxisX.Maximum = _dataCount;
        }
        private void AddYAxisLabels()
        {
            var axisY = chart1.ChartAreas[0].AxisY;
            axisY.CustomLabels.Clear(); // 清除舊標籤

            for (int i = 0; i < 5; i++)
            {
                double center = i * _offsetStep; // 這是該通道的 0V 位置

                // 1. 在中心點加入 "0V" 標籤
                // Add 參數：(起始值, 結束值, 文字, 列數, 標籤樣式)
                axisY.CustomLabels.Add(center - 1, center + 1, "0V", 0, LabelMarkStyle.None);

                // 2. (選配) 加入通道名稱，讓它顯示在 0V 旁邊
                //axisY.CustomLabels.Add(center - 4, center - 2, $"CH{i + 1}", 1, LabelMarkStyle.None);

                // 3. (選配) 加入邊界標籤
                axisY.CustomLabels.Add(center + 5 - 1, center + 5 + 1, "+5V", 0, LabelMarkStyle.None);
                axisY.CustomLabels.Add(center - 5 - 1, center - 5 + 1, "-5V", 0, LabelMarkStyle.None);
            }

            // 重新啟用標籤顯示
            axisY.LabelStyle.Enabled = true;
        }
        #endregion

        #endregion

        #region public function

        #endregion

        private void F_Equipment_Setting_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                SaveAllEnumSetting();
                ReadAllEnumSetting();

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

        private void timer1_Tick(object sender, EventArgs e)    //連續更新暫時用不到
        {
            //double[] mockValues = new double[5];
            //for (int i = 0; i < 5; i++)
            //{
            //    mockValues[i] = _random.NextDouble() * 1 - 0.5; // 產生 -5~5 隨機數
            //}
            //UpdateChart(mockValues);
        }

        private void Btn_CaptureData_Click(object sender, EventArgs e)
        {
            SetupChart();

            ILightEngineFunction lea = ServiceProvider.GetRequiredService<ILightEngineFunction>();
            string current_mode = Select_HL_Mode.Text;
            Int32.TryParse(DAC_Value.Text, out int vaule); 
            byte lea_side, lea_color;

            if (TestMode.SelectedIndex == 0)
                lea_side = lea.LED_LeftSide;
            else
                lea_side = lea.LED_RightSide;

            if (TestColor.SelectedIndex == 0)
                lea_color = lea.LED_R;
            else if (TestColor.SelectedIndex == 1)
                lea_color = lea.LED_G;
            else
                lea_color = lea.LED_B;

            DAQ_ChartLogic.SetLedCondition(lea_side, lea_color, vaule, current_mode);
            F_DAQ_ChartLogic.DAQDataResult data = DAQ_ChartLogic.Get_DAQ_Data(lea_side, lea_color, current_mode);
            UpdateChart(data);
        }

        private void Btn_SaveData_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "PNG Image|*.png";
                dlg.Title = "儲存 Chart 圖片";
                dlg.FileName = $"DAQChart_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    chart1.SaveImage(dlg.FileName, ChartImageFormat.Png);
                }
            }
        }
    }
}
