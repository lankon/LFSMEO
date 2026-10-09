using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using BurnInTester.Base;
using BurnInTester.Logic;
using DeviceCore;

namespace BurnInTester.UI
{
    public partial class F_StartForm : Form
    {
        public F_StartForm(IServiceProvider serviceProvider, F_StartFormLogic startFormLogic)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            StartFormLogic = startFormLogic;
            InitialForm();
        }

        #region parameter define
        private UC_CtrlBoxStatus[] CtrlBoxStatusArray = new UC_CtrlBoxStatus[39];
        private IServiceProvider ServiceProvider;
        private DieMap _DieMap = new DieMap();
        private F_StartFormLogic StartFormLogic;
        UC_CtrlBoxStatus selectedCtrlBox;

        private readonly List<InformationTrendPoint> InformationTrendPoints = new List<InformationTrendPoint>();
        private readonly Random InformationTestRandom = new Random();
        private readonly TimeSpan InformationKeepTime = TimeSpan.FromHours(5);
        private System.Windows.Forms.Timer InformationTestTimer;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            CreateDynamicElement();

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);

            TC_Test.UpdateTemperature += UpdateTemperature;
            TC_Test.UpdateErrorCount += UpdateErrorCount;
            //TC_Test.Open();
        }
        void ShowHint()
        {

        }
        private void ReadAllEnumSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_StartForm>();

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.ReadAllRecipe<eF_StartFormRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            ApplicationSetting.UpdataRecipeToForm<eF_StartForm>(this);
            //ApplicationSetting.UpdataRecipeToForm<eF_StartFormRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_StartForm>(this);

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.SaveRecipeFromForm<eF_StartFormRecipe>(this, recipe_name);
        }
        private void UpdatePage()
        {
            ReadAllEnumSetting();
            StartFormLogic.UpdateAgingParam();
            UpdateEnumSettingToForm();
        }
        private void LeavePage()
        {
        }
        private void CreateDynamicElement()
        {
            CtrlBoxStatus1.SetInformation("Finish");

            //ControlBox
            for (int i = 0; i < CtrlBoxStatusArray.Length; i++)
            {
                CtrlBoxStatusArray[i] = new UC_CtrlBoxStatus();

                if (i / 4 == 0 && i % 4 != 3)
                    LyPnl_CtrlBoxStatus.Controls.Add(CtrlBoxStatusArray[i], i + 1, 0);
                else if (i % 4 == 3)
                    LyPnl_CtrlBoxStatus.Controls.Add(CtrlBoxStatusArray[i], 0, i / 4 + 1);
                else
                    LyPnl_CtrlBoxStatus.Controls.Add(CtrlBoxStatusArray[i], i % 4 + 1, i / 4);

                CtrlBoxStatusArray[i].Dock = System.Windows.Forms.DockStyle.Fill;
                CtrlBoxStatusArray[i].Location = new System.Drawing.Point(4, 4);
                CtrlBoxStatusArray[i].Name = $"CtrlBoxStatus{i + 2}";
                CtrlBoxStatusArray[i].Size = new System.Drawing.Size(167, 87);
                CtrlBoxStatusArray[i].TabIndex = 1;
                CtrlBoxStatusArray[i].SetItemIndex($"CtrlBoxStatus{i + 2}");
                CtrlBoxStatusArray[i].Click += new System.EventHandler(this.CtrlBoxStatus1_Click);
            }

            //DieMap
            _DieMap.Dock = System.Windows.Forms.DockStyle.Fill;
            _DieMap.Location = new System.Drawing.Point(0, 0);
            _DieMap.Name = "DieMap";
            Pnl_Info.Controls.Add(_DieMap);

            CreateInformationTrendPanel();
            CreateInformationTestData();
            RefreshInformationPlot();
            StartInformationTestTimer();
        }

        private void CreateInformationTrendPanel()
        {
            Chk_InfoTemperature.CheckedChanged += InformationCheckBox_CheckedChanged;
            Chk_InfoVoltage.CheckedChanged += InformationCheckBox_CheckedChanged;
            Chk_InfoCurrent.CheckedChanged += InformationCheckBox_CheckedChanged;
            Chk_InfoPower.CheckedChanged += InformationCheckBox_CheckedChanged;
            Chk_InfoAutoFollow.CheckedChanged += InformationAutoFollow_CheckedChanged;
        }

        private void InformationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            RefreshInformationPlot();
        }

        private void InformationAutoFollow_CheckedChanged(object sender, EventArgs e)
        {
            RefreshInformationPlot();
        }

        private void CreateInformationTestData()
        {
            InformationTrendPoints.Clear();

            DateTime startTime = DateTime.Now.Subtract(InformationKeepTime);
            for (int i = 0; i <= 300; i++)
            {
                DateTime time = startTime.AddMinutes(i);
                double wave = Math.Sin(i / 18.0);
                double temperature = 55 + wave * 14 + InformationTestRandom.NextDouble() * 3;
                double voltage = 12.1 + Math.Sin(i / 30.0) * 0.25 + InformationTestRandom.NextDouble() * 0.06;
                double current = 2.2 + Math.Cos(i / 24.0) * 0.7 + InformationTestRandom.NextDouble() * 0.12;

                AddInformationPoint(time, temperature, voltage, current);
            }
        }

        private void StartInformationTestTimer()
        {
            InformationTestTimer = new System.Windows.Forms.Timer();
            InformationTestTimer.Interval = 100;
            InformationTestTimer.Tick += InformationTestTimer_Tick;
            InformationTestTimer.Start();
        }

        private void InformationTestTimer_Tick(object sender, EventArgs e)
        {
            InformationTrendPoint lastPoint = InformationTrendPoints.LastOrDefault();
            double baseTemperature = lastPoint == null ? 55 : lastPoint.Temperature;
            double baseVoltage = lastPoint == null ? 12.1 : lastPoint.Voltage;
            double baseCurrent = lastPoint == null ? 2.2 : lastPoint.Current;

            double temperature = Clamp(baseTemperature + InformationTestRandom.NextDouble() * 2.0 - 0.9, 25, 95);
            double voltage = Clamp(baseVoltage + InformationTestRandom.NextDouble() * 0.08 - 0.04, 10.5, 13.2);
            double current = Clamp(baseCurrent + InformationTestRandom.NextDouble() * 0.20 - 0.10, 0.1, 5.0);

            AddInformationPoint(DateTime.Now, temperature, voltage, current);
            RefreshInformationPlot();
        }

        private void AddInformationPoint(DateTime time, double temperature, double voltage, double current)
        {
            InformationTrendPoints.Add(new InformationTrendPoint()
            {
                Time = time,
                Temperature = temperature,
                Voltage = voltage,
                Current = current,
                Power = voltage * current
            });

            TrimInformationPoints(time);
        }

        private void TrimInformationPoints(DateTime now)
        {
            DateTime keepAfter = now.Subtract(InformationKeepTime);
            InformationTrendPoints.RemoveAll(point => point.Time < keepAfter);
        }

        private void RefreshInformationPlot()
        {
            if (Plot_Information == null)
                return;

            bool autoFollow = Chk_InfoAutoFollow == null || Chk_InfoAutoFollow.Checked;
            ScottPlot.AxisLimits axisLimitsBeforeRefresh = Plot_Information.Plot.GetAxisLimits();

            TrimInformationPoints(DateTime.Now);

            Plot_Information.Plot.Clear();
            Plot_Information.Plot.Style(
                figureBackground: Color.FromArgb(217, 217, 217),
                dataBackground: Color.White);

            double[] timeValues = InformationTrendPoints.Select(point => point.Time.ToOADate()).ToArray();

            if (Chk_InfoTemperature == null || Chk_InfoTemperature.Checked)
                AddInformationScatter(timeValues, InformationTrendPoints.Select(point => point.Temperature).ToArray(), "溫度 (°C)", Color.FromArgb(210, 85, 35));

            if (Chk_InfoVoltage == null || Chk_InfoVoltage.Checked)
                AddInformationScatter(timeValues, InformationTrendPoints.Select(point => point.Voltage).ToArray(), "電壓 (V)", Color.FromArgb(0, 92, 175));

            if (Chk_InfoCurrent != null && Chk_InfoCurrent.Checked)
                AddInformationScatter(timeValues, InformationTrendPoints.Select(point => point.Current).ToArray(), "電流 (A)", Color.FromArgb(0, 135, 75));

            if (Chk_InfoPower != null && Chk_InfoPower.Checked)
                AddInformationScatter(timeValues, InformationTrendPoints.Select(point => point.Power).ToArray(), "功率 (W)", Color.FromArgb(125, 75, 155));

            Plot_Information.Plot.Title("Information Trend - Last 5 Hours");
            Plot_Information.Plot.XLabel("Time");
            Plot_Information.Plot.YLabel("Value");
            Plot_Information.Plot.XAxis.DateTimeFormat(true);
            if (autoFollow)
                SetInformationPlotTimeRange();
            else
                KeepInformationPlotZoom(axisLimitsBeforeRefresh);
            Plot_Information.Plot.Legend();
            Plot_Information.Refresh();

            UpdateInformationLatestLabel();
        }

        private void SetInformationPlotTimeRange()
        {
            DateTime latestTime = InformationTrendPoints.Count == 0
                ? DateTime.Now
                : InformationTrendPoints[InformationTrendPoints.Count - 1].Time;

            DateTime startTime = latestTime.Subtract(InformationKeepTime);
            Plot_Information.Plot.SetAxisLimitsX(startTime.ToOADate(), latestTime.ToOADate());
            Plot_Information.Plot.AxisAutoY();
        }

        private void KeepInformationPlotZoom(ScottPlot.AxisLimits axisLimitsBeforeRefresh)
        {
            Plot_Information.Plot.AxisAutoY();

            if (double.IsNaN(axisLimitsBeforeRefresh.XMin) || double.IsNaN(axisLimitsBeforeRefresh.XMax))
                return;

            Plot_Information.Plot.SetAxisLimitsX(axisLimitsBeforeRefresh.XMin, axisLimitsBeforeRefresh.XMax);
        }

        private void AddInformationScatter(double[] timeValues, double[] values, string label, Color color)
        {
            if (timeValues.Length == 0 || values.Length == 0)
                return;

            Plot_Information.Plot.AddScatter(
                timeValues,
                values,
                color: color,
                lineWidth: 2,
                markerSize: 0,
                label: label);
        }

        private void UpdateInformationLatestLabel()
        {
            if (Labl_InformationLatest == null)
                return;

            InformationTrendPoint latest = InformationTrendPoints.LastOrDefault();
            if (latest == null)
            {
                Labl_InformationLatest.Text = "Latest\r\nNo data";
                return;
            }

            Labl_InformationLatest.Text =
                $"Latest  {latest.Time:HH:mm:ss}\r\n" +
                $"Temp.   {latest.Temperature,6:0.0} °C\r\n" +
                $"Volt.   {latest.Voltage,6:0.00} V\r\n" +
                $"Curr.   {latest.Current,6:0.00} A\r\n" +
                $"Power   {latest.Power,6:0.00} W";
        }

        private double Clamp(double value, double min, double max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        private class InformationTrendPoint
        {
            public DateTime Time { get; set; }
            public double Temperature { get; set; }
            public double Voltage { get; set; }
            public double Current { get; set; }
            public double Power { get; set; }
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {

        }

        public void AddInformationTrendData(double temperature, double voltage, double current)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<double, double, double>(AddInformationTrendData), temperature, voltage, current);
                return;
            }

            AddInformationPoint(DateTime.Now, temperature, voltage, current);
            RefreshInformationPlot();
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

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            StartFormLogic.StartTest();
        }

        private void Btn_TestSetting_Click(object sender, EventArgs e)
        {
            var para_set = ServiceProvider.GetRequiredService<F_TestSetting>();

            if (para_set is Form form)
            {
                Tool.HideElementOnPanel(Scope.MainPanel);
                Tool.SetForm(Scope.MainPanel, form);
                form.Show();
            }
        }

        private void CtrlBoxStatus1_Click(object sender, EventArgs e)
        {
            SaveAllEnumSetting();
            ReadAllEnumSetting();
            
            if (sender is UC_CtrlBoxStatus clickedCtrlBox)
            {
                string name = clickedCtrlBox.Name;
                int boxNum = int.Parse(name.Replace("CtrlBoxStatus", ""));

                StartFormLogic.SaveAgingParam();
                StartFormLogic.SetCurBoxNum(boxNum);
                StartFormLogic.UpdateAgingParam();
                UpdateEnumSettingToForm();

                selectedCtrlBox?.SetSelected(false);
                selectedCtrlBox = clickedCtrlBox;
                selectedCtrlBox.SetSelected(true);
            }
        }

        #region 龜山溫控測試
        Guishan_TC_Test TC_Test = new Guishan_TC_Test();

        private void UpdateErrorCount(string count)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateErrorCount), count);
            }
            else
            {
                TxtBx_ErrorCount.Text = count;
            }
        }
        private void UpdateTemperature(string temp)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateTemperature), temp);
            }
            else
            {
                Labl_PresentValue.Text = temp;
            }
        }
        private void Btn_Start_TC_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    SaveAllEnumSetting();
            //    double sv = double.Parse(TxtBx_SetValue.Text);
            //    int resp_delay = int.Parse(TxtBx_RespDelay.Text);
            //    int send_delay = int.Parse(TxtBx_SendDelay.Text);

            //    TC_Test.Start(sv, resp_delay, send_delay);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("請輸入正確的數值格式");
            //    return;
            //}
        }
        private void Btn_Stop_TC_Click(object sender, EventArgs e)
        {
            TC_Test.Stop();
        }

        #endregion

        private void Btn_Test_TC_Click(object sender, EventArgs e)
        {
            var TC = ServiceProvider.GetRequiredService<TC_Task>();
        }
    }


}

