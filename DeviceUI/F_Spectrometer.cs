using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using ToolFunction;
using DeviceCore;

namespace DeviceUI.Spectrometer
{
    public partial class F_Spectrometer : Form, IF_Spectrometer
    {
        public F_Spectrometer(IFunction_Spectrometer function_Spectrometer)
        {
            InitializeComponent();

            Spectrometer = function_Spectrometer;

            InitialForm();
        }

        #region parameter define
        List<SpectrumData> SpectrumList = new List<SpectrumData>();
        IFunction_Spectrometer Spectrometer;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            SetSpectrumPlot();

            if (!Tool.DataGrid_DataLoad(DGV_Spectrum, "Spectrum.xml"))
                Tool.SaveLogToFile("Spectrum控制表讀取失敗");
        }
        private void ShowHint()
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
        }
        private void UpdatePage()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();
        }
        private void LeavePage()
        {
        }


        //private void SetSpectrumPlot()
        //{
        //    string professionalFont = "Segoe UI"; // 或 "Arial"

        //    // 統一 X 軸
        //    var xLabel = Plot_Spectrom.Plot.Axes.Bottom.Label;
        //    xLabel.Text = "Wavelength (nm)";
        //    xLabel.FontName = professionalFont;
        //    xLabel.FontSize = 16;
        //    xLabel.Bold = true;

        //    // 統一 Y 軸
        //    var yLabel = Plot_Spectrom.Plot.Axes.Left.Label;
        //    yLabel.Text = "Intensity (a.u.)";
        //    yLabel.FontName = professionalFont;
        //    yLabel.FontSize = 16;
        //    yLabel.Bold = true;

        //    // 統一標題
        //    var title = Plot_Spectrom.Plot.Axes.Title.Label;
        //    title.Text = "Spectrum";
        //    title.FontName = professionalFont;
        //    title.FontSize = 20;
        //    title.Bold = true;

        //    // 讓格線變淡 (Alpha 設低一點)
        //    Plot_Spectrom.Plot.Grid.MajorLineColor = ScottPlot.Colors.Black.WithAlpha(0.05);

        //    // 設定背景色為乾淨的白色
        //    Plot_Spectrom.Plot.FigureBackground.Color = ScottPlot.Colors.White;
        //    Plot_Spectrom.Plot.DataBackground.Color = ScottPlot.Colors.White;

        //    // 隱藏不必要的右邊與上方座標軸線
        //    Plot_Spectrom.Plot.Axes.Right.FrameLineStyle.Width = 0;
        //    Plot_Spectrom.Plot.Axes.Top.FrameLineStyle.Width = 0;

        //    Plot_Spectrom.Refresh();
        //}
        //private void DrawSpectrumData(double[] wavelength, double[] intensity)
        //{
        //    // 清除舊有的繪圖物件
        //    Plot_Spectrom.Plot.Clear();

        //    // 加入數據線條
        //    var myPlot = Plot_Spectrom.Plot.AddScatter(wavelength, intensity);

        //    // 設定線條樣式
        //    myPlot.LineWidth = 2;
        //    myPlot.Color = ScottPlot.Colors.Blue;
        //    myPlot.MarkerSize = 0;

        //    // 自動縮放並刷新
        //    Plot_Spectrom.Plot.Axes.AutoScale();
        //    Plot_Spectrom.Refresh();
        //}

        private void SetSpectrumPlot()
        {
            string professionalFont = "Segoe UI";

            //// --- X 軸設定 ---
            //Plot_Spectrom.Plot.XLabel("Wavelength (nm)");
            //// 4.1.74 必須直接存取 Font 屬性物件
            //Plot_Spectrom.Plot.XAxis.TitleFontName = professionalFont;
            //Plot_Spectrom.Plot.XAxis.TitleFontSize = 16;
            //Plot_Spectrom.Plot.XAxis.TitleFontBold = true;

            //// --- Y 軸設定 ---
            //Plot_Spectrom.Plot.YLabel("Intensity (a.u.)");
            //Plot_Spectrom.Plot.YAxis.TitleFontName = professionalFont;
            //Plot_Spectrom.Plot.YAxis.TitleFontSize = 16;
            //Plot_Spectrom.Plot.YAxis.TitleFontBold = true;

            //// --- 標題設定 ---
            //Plot_Spectrom.Plot.Title("Spectrum");
            //// 注意：標題在 4.1 中通常對應的是 MainAxis (也就是 Top Axis)
            //Plot_Spectrom.Plot.MainAxis.TitleFontName = professionalFont;
            //Plot_Spectrom.Plot.MainAxis.TitleFontSize = 20;
            //Plot_Spectrom.Plot.MainAxis.TitleFontBold = true;

            // --- 格線與背景 ---
            // 4.1.74 的 Grid 參數是 (bool enable, Color color)
            Plot_Spectrom.Plot.Grid(color: Color.FromArgb(13, Color.Black));

            // 設定背景
            Plot_Spectrom.Plot.Style(figureBackground: Color.White, dataBackground: Color.White);

            // 隱藏右邊與上方框線
            //Plot_Spectrom.Plot.Frame(right: false, top: false);

            Plot_Spectrom.Refresh();
        }

        private void DrawSpectrumData(double[] wavelength, double[] intensity)
        {
            // 清除舊有的繪圖物件 (4.1 的 Clear 會移除所有 Plottables)
            Plot_Spectrom.Plot.Clear();

            // 加入數據線條 (4.1 的 AddScatter 直接回傳 ScatterPlot 物件)
            var myPlot = Plot_Spectrom.Plot.AddScatter(wavelength, intensity);

            // 設定線條樣式 (4.1 使用屬性設定)
            myPlot.LineWidth = 2;
            myPlot.Color = Color.Blue; // 使用 System.Drawing.Color
            myPlot.MarkerSize = 0;

            // 自動縮放並刷新 (4.1 預設就是 AutoAxis)
            Plot_Spectrom.Plot.AxisAuto();
            Plot_Spectrom.Refresh();
        }
        private void Update_Spectrum_List(DataGridView DGV, List<SpectrumData> spectrum_list)
        {
            spectrum_list.Clear();

            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.IsNewRow) continue;

                var data = new SpectrumData()
                {
                    Title_SpectrumType = row.Cells["Title_SpectrumType"]?.Value?.ToString(),
                    Title_Name = row.Cells["Title_Name"]?.Value?.ToString(),
                    Title_ID = row.Cells["Title_ID"]?.Value?.ToString(),
                    Title_IntegralTime = row.Cells["Title_IntegralTime"]?.Value?.ToString(),
                };

                spectrum_list.Add(data);
            }

            Spectrometer.LoadConfiguration(spectrum_list);
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {
            if (show)
                Tool.ShowFormName(this);
        }
        public void Update_Spectrum_List()
        {
            Update_Spectrum_List(DGV_Spectrum, SpectrumList);
        }
        #endregion

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            string[] context = new string[] { "None", "None", "None", "0", "Get" };

            Tool.DataGrid_AddRow(DGV_Spectrum, context);
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_DataSave(DGV_Spectrum, "Spectrum.xml");
        }

        private void Btn_Load_Click(object sender, EventArgs e)
        {
            if (!Tool.DataGrid_DataLoad(DGV_Spectrum, "Spectrum.xml"))
                Tool.SaveLogToFile("Spectrum控制表讀取失敗");
        }

        private void Btn_Remove_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_DeleteRow(DGV_Spectrum);
        }

        private void Btn_FunctionTest_Click(object sender, EventArgs e)
        {
            double[] walength = new double[] { 400, 500, 600, 700, 800 };
            double[] intensity = new double[] { 10, 20, 15, 25, 5 };

            DrawSpectrumData(walength, intensity);
            //Spectrometer.Initial_All_Spectrometer();
            //LightControl.SetLightValue(ELightName.LIGHT_1, 50);
        }

        private void DGV_Spectrum_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (SpectrumList.Count == 0 || e.RowIndex < 0) return;

            if (DGV_Spectrum.Columns[e.ColumnIndex].Name == "Title_Name" ||
                DGV_Spectrum.Columns[e.ColumnIndex].Name == "Title_ID" ||
                DGV_Spectrum.Columns[e.ColumnIndex].Name == "Title_SpectrumType")
            {
                Update_Spectrum_List(DGV_Spectrum, SpectrumList);
            }
        }

        private void F_Spectrometer_VisibleChanged(object sender, EventArgs e)
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

        private void DGV_Spectrum_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (DGV_Spectrum.Columns[e.ColumnIndex].Name == "Title_GetSpectrum")
            {
                var row = DGV_Spectrum.Rows[e.RowIndex];

                string Title_SpectrumType = row.Cells["Title_SpectrumType"]?.Value?.ToString();
                string Title_Name = row.Cells["Title_Name"]?.Value?.ToString();
                string Title_ID = row.Cells["Title_ID"]?.Value?.ToString();
                Int32.TryParse(row.Cells["Title_IntegralTime"].Value.ToString(), out int IntegralTime);

                Enum.TryParse(Title_Name, out ESpectrumName spectrum_name);

                float[] f_spec_data = Spectrometer.GetSpectrumOneShot(spectrum_name, (uint)IntegralTime);
                float[] f_wave_data = Spectrometer.GetWavelengthSpan(spectrum_name);

                if (f_spec_data == null || f_wave_data == null)
                {
                    Plot_Spectrom.Plot.Clear();
                    Plot_Spectrom.Refresh();
                    return;
                }

                double[] spec_data = Array.ConvertAll(f_spec_data, x => (double)x);
                double[] wave_data = Array.ConvertAll(f_wave_data, x => (double)x);

                DrawSpectrumData(wave_data, spec_data);
            }
        }
    }
}
