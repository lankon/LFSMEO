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
        public F_StartForm(
            IServiceProvider serviceProvider, 
            F_StartFormLogic startFormLogic,
            AgingInformation agingInformation,
            TC_Task tC_Task)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            StartFormLogic = startFormLogic;
            _TC_Task = tC_Task;
            _AgingInformation = agingInformation;

            InitialForm();
        }

        #region parameter define
        private UC_CtrlBoxStatus[] CtrlBoxStatusArray = new UC_CtrlBoxStatus[39];
        private IServiceProvider ServiceProvider;
        private DieMap _DieMap = new DieMap();
        private TC_Task _TC_Task;
        private AgingInformation _AgingInformation;
        private F_StartFormLogic StartFormLogic;
        private UC_CtrlBoxStatus selectedCtrlBox;
        private InformationTrendPlot InformationPlot;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            StartFormLogic.ReadAllSetting();

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
            CtrlBoxStatus1.SetSelected(true);
            selectedCtrlBox = CtrlBoxStatus1;

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

            InformationPlot = new InformationTrendPlot(Plot_Information);
            UpdateInformationTrendDisplay();
        }
        private InformationTrendOptions GetInformationTrendOptions()
        {
            return new InformationTrendOptions(
                Chk_InfoTemperature.Checked,
                Chk_InfoVoltage.Checked,
                Chk_InfoCurrent.Checked,
                Chk_InfoPower.Checked,
                Chk_InfoAutoFollow.Checked);
        }
        private void UpdateInformationTrendDisplay()
        {
            InformationPlot?.UpdateDisplay(_AgingInformation.CurSelectBoxNo, GetInformationTrendOptions());
        }
        private void InformationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateInformationTrendDisplay();
        }
        private void OnUpdateTemperature(int box_num)
        {
            if (box_num < 0 || box_num >= _AgingInformation.TemperatureInfos.Length)
            {
                Tool.SaveLogToFile($"收到無效的溫控箱索引: {box_num}", level: "ERR");
                return;
            }

            DateTime current_time = _AgingInformation.TemperatureInfos[box_num].GetRecordTime();
            double main_temperature = _AgingInformation.TemperatureInfos[box_num].GetPV()[0];
            InformationPlot.AddData(box_num, current_time, main_temperature, 5, 1);

            //if (box_num == 0)
            //{
            //    Tool.SaveLogToFile($"Box1 time: {current_time:HH:mm:ss.fff}", level:"INF");
            //}
        }
        private void SubscribeEvent()
        {
            _TC_Task.TemperatureUpdated += OnUpdateTemperature;
            Chk_InfoTemperature.CheckedChanged += InformationCheckBox_CheckedChanged;
            Chk_InfoVoltage.CheckedChanged += InformationCheckBox_CheckedChanged;
            Chk_InfoCurrent.CheckedChanged += InformationCheckBox_CheckedChanged;
            Chk_InfoPower.CheckedChanged += InformationCheckBox_CheckedChanged;
            Chk_InfoAutoFollow.CheckedChanged += InformationCheckBox_CheckedChanged;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _TC_Task.TemperatureUpdated -= OnUpdateTemperature;
                Chk_InfoTemperature.CheckedChanged -= InformationCheckBox_CheckedChanged;
                Chk_InfoVoltage.CheckedChanged -= InformationCheckBox_CheckedChanged;
                Chk_InfoCurrent.CheckedChanged -= InformationCheckBox_CheckedChanged;
                Chk_InfoPower.CheckedChanged -= InformationCheckBox_CheckedChanged;
                Chk_InfoAutoFollow.CheckedChanged -= InformationCheckBox_CheckedChanged;
                InformationPlot?.Dispose();
                components?.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

        #region public function
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
                int boxIndex = boxNum - 1;

                if (boxIndex < 0 || boxIndex >= _AgingInformation.TemperatureInfos.Length)
                {
                    Tool.SaveLogToFile($"選取了無效的溫控箱編號: {boxNum}", level: "ERR");
                    return;
                }

                StartFormLogic.SaveAgingParam();
                StartFormLogic.SetCurBoxNum(boxIndex);
                StartFormLogic.UpdateAgingParam();
                UpdateEnumSettingToForm();
                UpdateInformationTrendDisplay();

                // 更新選取狀態
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

        private void F_StartForm_Load(object sender, EventArgs e)
        {
            SubscribeEvent();
        }
    }

    public sealed class InformationTrendOptions
    {
        public InformationTrendOptions(bool showTemperature, bool showVoltage,
            bool showCurrent, bool showPower, bool autoFollow)
        {
            ShowTemperature = showTemperature;
            ShowVoltage = showVoltage;
            ShowCurrent = showCurrent;
            ShowPower = showPower;
            AutoFollow = autoFollow;
        }

        public bool ShowTemperature { get; }
        public bool ShowVoltage { get; }
        public bool ShowCurrent { get; }
        public bool ShowPower { get; }
        public bool AutoFollow { get; }
    }

    public class InformationTrendPlot : IDisposable
    {
        // 處理即時顯示圖表用
        public InformationTrendPlot(ScottPlot.FormsPlot plot)
        {
            _Plot_Information = plot;
            UpdateInterval = GetCurrentTickCount();
        }

        #region Parameter define
        private readonly ScottPlot.FormsPlot _Plot_Information;     // 顯示資訊趨勢的ScottPlot控制項
        private InformationTrendOptions _Options;                   // 圖表項目顯示與自動跟隨設定
        private int _SelectedBoxIndex;                              // 目前圖表顯示的溫控箱索引
        private int UpdateInterval;                                 // 上次排入圖表更新時的系統毫秒計時值
        private volatile bool _disposed;                            // 物件是否已被釋放
        private readonly object _InformationDataSync = new object();   // 保護各溫控箱的趨勢資料
        private readonly Dictionary<int, List<InformationTrendPoint>> InformationTrendPointsByBox = new Dictionary<int, List<InformationTrendPoint>>();    // 依溫控箱紀錄資料 int: boxIndex, List<InformationTrendPoint>: 資料點
        private readonly TimeSpan InformationKeepTime = TimeSpan.FromHours(5);  // 圖表顯示時間,最多N小時

        private class InformationTrendPoint
        {
            // 圖表紀錄資訊
            public DateTime Time { get; set; }
            public double Temperature { get; set; }
            public double Voltage { get; set; }
            public double Current { get; set; }
            public double Power { get; set; }
        }

        private class InformationTrendPlotData
        {
            // 交給UI繪圖的資料快照
            public int BoxIndex { get; set; }
            public double[] TimeValues { get; set; }
            public double[] Temperatures { get; set; }
            public double[] Voltages { get; set; }
            public double[] Currents { get; set; }
            public double[] Powers { get; set; }
        }
        #endregion

        #region private function
        private List<InformationTrendPoint> GetInformationTrendPoints(int boxIndex)
        {
            if (!InformationTrendPointsByBox.TryGetValue(boxIndex, out List<InformationTrendPoint> points))
            {
                points = new List<InformationTrendPoint>();
                InformationTrendPointsByBox.Add(boxIndex, points);
            }

            return points;
        }

        private void AddInformationPoint(int boxIndex, DateTime time, double temperature, double voltage, double current)
        {
            List<InformationTrendPoint> points = GetInformationTrendPoints(boxIndex);
            points.Add(new InformationTrendPoint()
            {
                Time = time,
                Temperature = temperature,
                Voltage = voltage,
                Current = current,
                Power = voltage * current
            });

            TrimInformationPoints(points, time);
        }

        private void TrimInformationPoints(List<InformationTrendPoint> points, DateTime now)
        {
            DateTime keepAfter = now.Subtract(InformationKeepTime);
            points.RemoveAll(point => point.Time < keepAfter);
        }

        private InformationTrendPlotData CreateInformationTrendPlotData(int boxIndex)
        {
            List<InformationTrendPoint> points = GetInformationTrendPoints(boxIndex);
            TrimInformationPoints(points, DateTime.Now);

            return new InformationTrendPlotData()
            {
                BoxIndex = boxIndex,
                TimeValues = points.Select(point => point.Time.ToOADate()).ToArray(),
                Temperatures = points.Select(point => point.Temperature).ToArray(),
                Voltages = points.Select(point => point.Voltage).ToArray(),
                Currents = points.Select(point => point.Current).ToArray(),
                Powers = points.Select(point => point.Power).ToArray()
            };
        }

        private void KeepInformationPlotZoom(ScottPlot.AxisLimits axisLimitsBeforeRefresh)
        {
            _Plot_Information.Plot.AxisAutoY();

            if (double.IsNaN(axisLimitsBeforeRefresh.XMin) || double.IsNaN(axisLimitsBeforeRefresh.XMax))
                return;

            _Plot_Information.Plot.SetAxisLimitsX(axisLimitsBeforeRefresh.XMin, axisLimitsBeforeRefresh.XMax);
        }

        private void AddInformationScatter(double[] timeValues, double[] values, string label, Color color)
        {
            if (timeValues.Length == 0 || values.Length == 0)
                return;

            _Plot_Information.Plot.AddScatter(
                timeValues,
                values,
                color: color,
                lineWidth: 2,
                markerSize: 5,
                label: label);
        }

        private void SetInformationPlotTimeRange()
        {
            DateTime now = DateTime.Now;
            DateTime startTime = now.AddSeconds(-60);
            DateTime endTime = now.AddSeconds(1);
            _Plot_Information.Plot.SetAxisLimitsX(startTime.ToOADate(), endTime.ToOADate());
            _Plot_Information.Plot.AxisAutoY();
        }

        private void RefreshInformationPlot(InformationTrendPlotData plotData)
        {
            if (_Options == null || plotData == null || plotData.BoxIndex != _SelectedBoxIndex)
                return;

            ScottPlot.AxisLimits axisLimitsBeforeRefresh = _Plot_Information.Plot.GetAxisLimits();

            _Plot_Information.Plot.Clear();
            _Plot_Information.Plot.Style(figureBackground: Color.FromArgb(217, 217, 217), dataBackground: Color.White);

            // 這邊要想一下之後怎麼因應測試項目新增或刪減
            if (_Options.ShowTemperature)
                AddInformationScatter(plotData.TimeValues, plotData.Temperatures, "Temperature (°C)", Color.FromArgb(210, 85, 35));

            if (_Options.ShowVoltage)
                AddInformationScatter(plotData.TimeValues, plotData.Voltages, "Voltage (V)", Color.FromArgb(0, 92, 175));

            if (_Options.ShowCurrent)
                AddInformationScatter(plotData.TimeValues, plotData.Currents, "Current (A)", Color.FromArgb(0, 135, 75));

            if (_Options.ShowPower)
                AddInformationScatter(plotData.TimeValues, plotData.Powers, "Power (W)", Color.FromArgb(125, 75, 155));

            _Plot_Information.Plot.Title($"Information Trend - Box{(plotData.BoxIndex / 4) + 1}-{(plotData.BoxIndex % 4) + 1}");
            _Plot_Information.Plot.XLabel("Time");
            _Plot_Information.Plot.YLabel("Value");
            _Plot_Information.Plot.XAxis.DateTimeFormat(true);

            if (_Options.AutoFollow)
                SetInformationPlotTimeRange();
            else
                KeepInformationPlotZoom(axisLimitsBeforeRefresh);

            _Plot_Information.Plot.Legend();
            _Plot_Information.Refresh();
        }

        private void BeginRefreshInformationPlot(InformationTrendPlotData plotData)
        {
            if (_disposed || !_Plot_Information.IsHandleCreated ||
                _Plot_Information.IsDisposed || _Plot_Information.Disposing)
                return;

            if (_Plot_Information.InvokeRequired)
            {
                try
                {
                    _Plot_Information.BeginInvoke(new Action(() =>
                    {
                        if (_disposed || _Plot_Information.IsDisposed || _Plot_Information.Disposing)
                            return;

                        RefreshInformationPlot(plotData);
                    }));
                }
                catch (InvalidOperationException) when (
                    _disposed || _Plot_Information.IsDisposed || _Plot_Information.Disposing)
                {
                    // 排程期間控制項已釋放，不再更新圖表。
                }
                return;
            }

            RefreshInformationPlot(plotData);
        }

        private int GetCurrentTickCount()
        {
            return Environment.TickCount;
        }

        private bool CheckTimeOvermSec(int tick, int time)
        {
            int time_count = unchecked(GetCurrentTickCount() - tick);
            bool res = time_count > time;

            return res;
        }
        #endregion

        #region public function
        public void AddData(int boxIndex, DateTime time, double temperature, double voltage, double current)
        {
            if (_disposed || boxIndex < 0)
                return;

            InformationTrendPlotData plotData = null;
            lock (_InformationDataSync)
            {
                AddInformationPoint(boxIndex, time, temperature, voltage, current);

                if (boxIndex != _SelectedBoxIndex || !CheckTimeOvermSec(UpdateInterval, 200))
                    return;

                // 在排入UI前更新時間，確保BeginInvoke最少間隔200ms。
                UpdateInterval = GetCurrentTickCount();

                plotData = CreateInformationTrendPlotData(boxIndex);
            }

            BeginRefreshInformationPlot(plotData);
        }

        public void UpdateDisplay(int selectedBoxIndex, InformationTrendOptions options)
        {
            if (options == null || selectedBoxIndex < 0)
                return;

            if (_disposed || _Plot_Information.IsDisposed || _Plot_Information.Disposing)
                return;

            if (_Plot_Information.InvokeRequired)
            {
                try
                {
                    _Plot_Information.BeginInvoke(new Action(() =>
                        UpdateDisplay(selectedBoxIndex, options)));
                }
                catch (InvalidOperationException) when (
                    _disposed || _Plot_Information.IsDisposed || _Plot_Information.Disposing)
                {
                    // 排程期間控制項已釋放，不再更新圖表設定。
                }
                return;
            }

            InformationTrendPlotData plotData;
            lock (_InformationDataSync)
            {
                _SelectedBoxIndex = selectedBoxIndex;
                _Options = options;
                UpdateInterval = GetCurrentTickCount();
                plotData = CreateInformationTrendPlotData(selectedBoxIndex);
            }

            // 使用者切換溫控箱或顯示項目時立即更新，不等待下一筆溫度資料。
            RefreshInformationPlot(plotData);
        }

        public void Dispose()
        {
            _disposed = true;
        }
        #endregion
    }
}

