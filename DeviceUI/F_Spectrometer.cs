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

            //Labl_Intensity.Parent = PgBar_Intensity;
            
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
            Timer_GetSpectrum.Enabled = false;
        }
        private void SetSpectrumPlot()
        {
            Plot_Spectrom.Plot.Grid(color: Color.FromArgb(13, Color.Black));
            Plot_Spectrom.Plot.Style(figureBackground: Color.White, dataBackground: Color.White);

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

        private void Btn_Capture_Click(object sender, EventArgs e)
        {
            if(UInt16.TryParse(TxtBx_IntgralTime.Text, out ushort intgTime) == true)
            {
                float[] intensity = Spectrometer.GetSpectrumOneShot(ESpectrumName.SPECTRUM_1, intgTime);
                float[] wl = Spectrometer.GetWavelengthSpan(ESpectrumName.SPECTRUM_1);
                double[] intensityDouble = Array.ConvertAll(intensity, x => (double)x);
                double[] wlDouble = Array.ConvertAll(wl, x => (double)x);

                DrawSpectrumData(wlDouble, intensityDouble);
            }
            else
            {
                MessageBox.Show("IntetgralTime Setting Fail");
            }
        }

        private void Btn_StopLive_Click(object sender, EventArgs e)
        {
            Timer_GetSpectrum.Enabled = false;
            TxtBx_IntgralTime.Enabled = true;
        }

        private void Btn_Live_Click(object sender, EventArgs e)
        {
            if (UInt16.TryParse(TxtBx_IntgralTime.Text, out ushort intgTime) == true)
            {
                Timer_GetSpectrum.Interval = intgTime + 200;
                Timer_GetSpectrum.Enabled = true;
                TxtBx_IntgralTime.Enabled = false;
            }
            else
            {
                MessageBox.Show("IntetgralTime Setting Fail");
            }
        }

        private void Timer_GetSpectrum_Tick(object sender, EventArgs e)
        {
            if (UInt16.TryParse(TxtBx_IntgralTime.Text, out ushort intgTime) == true)
            {
                float[] intensity = Spectrometer.GetSpectrumRelativelyOneShot(ESpectrumName.SPECTRUM_1, intgTime);
                float[] wl = Spectrometer.GetWavelengthSpan(ESpectrumName.SPECTRUM_1);
                double[] intensityDouble = Array.ConvertAll(intensity, x => (double)x);
                double[] wlDouble = Array.ConvertAll(wl, x => (double)x);

                PgBar_Intensity.Value = (int)intensity.Max();
                Labl_Intensity.Text = intensity.Max().ToString() + "%";
                DrawSpectrumData(wlDouble, intensityDouble);
            }
        }
    }
}
