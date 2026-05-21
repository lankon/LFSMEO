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
using RGBTester.Logic;
using RGBTester.Base;
using DeviceCore;

namespace RGBTester.UI
{
    public partial class F_MFactorCalibration : Form
    {
        public F_MFactorCalibration(IFunction_Spectrometer function_Spectrometer)
        {
            InitializeComponent();

            Spectrometer = function_Spectrometer;

            InitialForm();
        }

        #region parameter define
        IFunction_Spectrometer Spectrometer;
        F_MFactorCalibrationLogic MFactorCalibrationLogic = new F_MFactorCalibrationLogic();
        private double[] MatchWavelength;
        private double[] MatchIntensity;
        private ProcessStep currentStep = ProcessStep.Init;
        public enum ProcessStep
        {
            Init,
            LIVE,
            MFactorCalibration,
        }
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            SetSpectrumPlot();

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

            UpdateUIState(ProcessStep.Init);
        }
        private void LeavePage()
        {
        }


        private void SetSpectrumPlot()
        {
            Plot_Spectrom.Plot.Grid(color: Color.FromArgb(13, Color.Black));
            Plot_Spectrom.Plot.Style(figureBackground: Color.White, dataBackground: Color.White);

            Plot_Spectrom.Plot.YAxis2.Color(Color.Green);
            Plot_Spectrom.Plot.YAxis2.TickLabelStyle(color: Color.Green);

            Plot_Spectrom.Plot.YAxis.Color(Color.Blue);
            Plot_Spectrom.Plot.YAxis.TickLabelStyle(color: Color.Blue);

            Plot_Spectrom.Refresh();
        }
        private void DrawSpectrumData(double[] wavelength, double[] intensity)
        {
            Plot_Spectrom.Plot.Clear();
            var myPlot = Plot_Spectrom.Plot.AddScatter(wavelength, intensity);

            myPlot.LineWidth = 2;
            myPlot.Color = Color.Blue; // 使用 System.Drawing.Color
            myPlot.MarkerSize = 0;

            Plot_Spectrom.Plot.AxisAuto();
            Plot_Spectrom.Plot.SetAxisLimitsY(0, Plot_Spectrom.Plot.GetAxisLimits().YMax);
            Plot_Spectrom.Refresh();
        }
        private void Draw2ndSpectrumData(double[] wavelength, double[] intensity)
        {
            var myPlot = Plot_Spectrom.Plot.AddScatter(wavelength, intensity);

            myPlot.Color = System.Drawing.Color.Green;
            myPlot.YAxisIndex = 1;

            Plot_Spectrom.Plot.YAxis2.Ticks(true);

            double mainYMax = Plot_Spectrom.Plot.GetAxisLimits(0, 0).YMax;
            Plot_Spectrom.Plot.SetAxisLimitsY(0, mainYMax, yAxisIndex: 0);

            // 對齊「右邊副軸 (yAxisIndex: 1)」
            double subYMax = Plot_Spectrom.Plot.GetAxisLimits(0, 1).YMax;
            Plot_Spectrom.Plot.SetAxisLimitsY(0, subYMax, yAxisIndex: 1);

            Plot_Spectrom.Refresh();
        }
        private void UpdateUIState(ProcessStep step)
        {
            currentStep = step;

            switch (step)
            {
                case ProcessStep.Init:
                    Btn_ReadStdSpectrum.Enabled = true;
                    Btn_SaveData.Enabled = false;
                    Btn_Live.Enabled = false;
                    break;

                case ProcessStep.LIVE:
                    Btn_Live.Enabled = true;
                    break;

                case ProcessStep.MFactorCalibration:
                    Btn_SaveData.Enabled = true;
                    break;
            }
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

        private void Btn_ReadStdSpectrum_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select File";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fullFilePath = openFileDialog.FileName;
                    string fileName = openFileDialog.SafeFileName;

                    var spec = MFactorCalibrationLogic.ReadStdSpectrumFile(fullFilePath);
                    
                    double[] wavelength = spec.Wavelength.Select(x => (double)x).ToArray();
                    double[] intensity = spec.Intensity.Select(x => x).ToArray();

                    //繪圖
                    DrawSpectrumData(wavelength, intensity);

                    UpdateUIState(ProcessStep.LIVE);
                }
            }
        }

        private void Btn_SaveData_Click(object sender, EventArgs e)
        {
            MFactorCalibrationLogic.CalMFactor(MatchWavelength, MatchIntensity);
            MessageBox.Show("Save Success");
        }

        private void Timer_GetSpectrum_Tick(object sender, EventArgs e)
        {
            if (UInt16.TryParse(TxtBx_IntgralTime.Text, out ushort intgTime) == true)
            {
                float[] intensity = Spectrometer.GetSpectrumRelativelyOneShot(ESpectrumName.SPECTRUM_1, intgTime);
                float[] wl = Spectrometer.GetWavelengthSpan(ESpectrumName.SPECTRUM_1);
                double[] intensityDouble = Array.ConvertAll(intensity, x => (double)x);
                double[] wlDouble = Array.ConvertAll(wl, x => (double)x);

                //MFactorCalibrationLogic.SetMatchSpectrum(wlDouble, intensityDouble);

                //繪圖
                PgBar_Intensity.Value = (int)intensity.Max();
                Labl_Intensity.Text = intensity.Max().ToString() + "%";
                Draw2ndSpectrumData(wlDouble, intensityDouble);

                MatchWavelength = wlDouble;
                MatchIntensity = intensityDouble;
            }
        }

        private void Btn_Live_Click(object sender, EventArgs e)
        {
            if (UInt16.TryParse(TxtBx_IntgralTime.Text, out ushort intgTime) == true)
            {
                Timer_GetSpectrum.Interval = intgTime + 200;
                Timer_GetSpectrum.Enabled = true;
                TxtBx_IntgralTime.Enabled = false;
                UpdateUIState(ProcessStep.MFactorCalibration);
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
    }
}
