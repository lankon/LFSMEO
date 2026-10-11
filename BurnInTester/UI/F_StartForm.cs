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
        public F_StartForm(IServiceProvider serviceProvider, F_StartFormLogic startFormLogic, 
            TC_Task tC_Task, AgingInformation agingInformation)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            StartFormLogic = startFormLogic;
            _TC_Task = tC_Task;
            _AgingInformation = agingInformation;
            InitialForm();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                InformationPlot?.Dispose();
                components?.Dispose();
            }

            base.Dispose(disposing);
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

            InformationPlot = new InformationTrendPlot(Plot_Information,
                Chk_InfoTemperature, Chk_InfoVoltage, Chk_InfoCurrent, Chk_InfoPower,
                Chk_InfoAutoFollow, Labl_InformationLatest);
        }
        private void OnUpdateTemperature(int box_num)
        {
            box_num = 0;

            if (_AgingInformation.CurSelectBoxNo != box_num)
                return;

            DateTime current_time = _AgingInformation.TemperatureInfos[box_num].GetRecordTime();
            double main_temperature = _AgingInformation.TemperatureInfos[box_num].GetPV()[0];
            InformationPlot.AddData(current_time, main_temperature, 5, 1);
        }
        private void SubscribeEvent()
        {
            _TC_Task.TemperatureUpdated += OnUpdateTemperature;
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

        private void F_StartForm_Load(object sender, EventArgs e)
        {
            SubscribeEvent();
        }
    }

    public class InformationTrendPlot : IDisposable
    {
        // 處理即時顯示圖表用
        public InformationTrendPlot(ScottPlot.FormsPlot plot,
            CheckBox temperature, CheckBox voltage, CheckBox current, CheckBox power,
            CheckBox autoFollow, Label latestLabel)
        {
            _Plot_Information = plot;
            _Chk_InfoTemperature = temperature;
            _Chk_InfoVoltage = voltage;
            _Chk_InfoCurrent = current;
            _Chk_InfoPower = power;
            _Chk_InfoAutoFollow = autoFollow;
            Labl_InformationLatest = latestLabel;

            CreateInformationTrendPanel();
            RefreshInformationPlot();
        }

        #region Parameter define
        private readonly ScottPlot.FormsPlot _Plot_Information;
        private readonly CheckBox _Chk_InfoTemperature;
        private readonly CheckBox _Chk_InfoVoltage;
        private readonly CheckBox _Chk_InfoCurrent;
        private readonly CheckBox _Chk_InfoPower;
        private readonly CheckBox _Chk_InfoAutoFollow;
        private readonly Label Labl_InformationLatest;
        private volatile bool _disposed;    // 物件是否被釋放
        private readonly List<InformationTrendPoint> InformationTrendPoints = new List<InformationTrendPoint>();    // 紀錄資料
        private readonly TimeSpan InformationKeepTime = TimeSpan.FromHours(5);  // 圖表顯示時間,最多N小時

        //private System.Windows.Forms.Timer InformationTestTimer;

        private class InformationTrendPoint
        {
            public DateTime Time { get; set; }
            public double Temperature { get; set; }
            public double Voltage { get; set; }
            public double Current { get; set; }
            public double Power { get; set; }
        }
        #endregion

        #region private function
        private void CreateInformationTrendPanel()
        {
            // 註冊事件
            _Chk_InfoTemperature.CheckedChanged += InformationCheckBox_CheckedChanged;
            _Chk_InfoVoltage.CheckedChanged += InformationCheckBox_CheckedChanged;
            _Chk_InfoCurrent.CheckedChanged += InformationCheckBox_CheckedChanged;
            _Chk_InfoPower.CheckedChanged += InformationCheckBox_CheckedChanged;
            _Chk_InfoAutoFollow.CheckedChanged += InformationCheckBox_CheckedChanged;
        }

        private void InformationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
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

        private void SetInformationPlotTimeRange()
        {
            DateTime now = DateTime.Now;
            DateTime startTime = now.AddSeconds(-60);
            DateTime endTime = now.AddSeconds(1);
            _Plot_Information.Plot.SetAxisLimitsX(startTime.ToOADate(), endTime.ToOADate());
            _Plot_Information.Plot.AxisAutoY();
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
                markerSize: 0,
                label: label);
        }

        private void RefreshInformationPlot()
        {
            bool autoFollow = _Chk_InfoAutoFollow.Checked;
            ScottPlot.AxisLimits axisLimitsBeforeRefresh = _Plot_Information.Plot.GetAxisLimits();

            TrimInformationPoints(DateTime.Now);

            _Plot_Information.Plot.Clear();
            _Plot_Information.Plot.Style(figureBackground: Color.FromArgb(217, 217, 217), dataBackground: Color.White);

            double[] timeValues = InformationTrendPoints.Select(point => point.Time.ToOADate()).ToArray();

            // 這邊要想一下之後怎麼因應測試項目新增或刪減
            if (_Chk_InfoTemperature.Checked)
                AddInformationScatter(timeValues, InformationTrendPoints.Select(point => point.Temperature).ToArray(), "Temperature (°C)", Color.FromArgb(210, 85, 35));

            if (_Chk_InfoVoltage.Checked)
                AddInformationScatter(timeValues, InformationTrendPoints.Select(point => point.Voltage).ToArray(), "Voltage (V)", Color.FromArgb(0, 92, 175));

            if (_Chk_InfoCurrent.Checked)
                AddInformationScatter(timeValues, InformationTrendPoints.Select(point => point.Current).ToArray(), "Current (A)", Color.FromArgb(0, 135, 75));

            if (_Chk_InfoPower.Checked)
                AddInformationScatter(timeValues, InformationTrendPoints.Select(point => point.Power).ToArray(), "Power (W)", Color.FromArgb(125, 75, 155));

            _Plot_Information.Plot.Title("Information Trend");
            _Plot_Information.Plot.XLabel("Time");
            _Plot_Information.Plot.YLabel("Value");
            _Plot_Information.Plot.XAxis.DateTimeFormat(true);

            if (autoFollow)
                SetInformationPlotTimeRange();
            else
                KeepInformationPlotZoom(axisLimitsBeforeRefresh);

            _Plot_Information.Plot.Legend();
            _Plot_Information.Refresh();

            UpdateInformationLatestLabel();
        }

        private void AddDataOnUiThread(DateTime time, double temperature, double voltage, double current)
        {
            if (_disposed || _Plot_Information.IsDisposed || _Plot_Information.Disposing)
                return;

            AddInformationPoint(time, temperature, voltage, current);
            RefreshInformationPlot();
        }


        private void UpdateInformationLatestLabel() // 應該不需要,需要的話還要檢查
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
        #endregion

        #region public function
        public void AddData(DateTime time, double temperature, double voltage, double current)
        {
            if (_disposed || _Plot_Information.IsDisposed || _Plot_Information.Disposing)
                return;

            // Handle 尚未建立時，InvokeRequired 可能回傳 false，不能據此更新 UI。
            if (!_Plot_Information.IsHandleCreated)
            {
                Tool.SaveLogToFile("請在圖表控制項的 Handle 建立後呼叫 AddData。");
                throw new InvalidOperationException("請在圖表控制項的 Handle 建立後呼叫 AddData。");
            }

            if (_Plot_Information.InvokeRequired)
            {
                try
                {
                    _Plot_Information.BeginInvoke(new Action(() =>
                        AddDataOnUiThread(time, temperature, voltage, current)));
                }
                catch (InvalidOperationException) when (
                    _disposed || _Plot_Information.IsDisposed || _Plot_Information.Disposing)
                {
                    // 排程期間控制項已釋放，不再接收資料。
                }
                return;
            }

            AddDataOnUiThread(time, temperature, voltage, current);
        }

        public void Dispose()
        {
            _disposed = true;
            _Chk_InfoTemperature.CheckedChanged -= InformationCheckBox_CheckedChanged;
            _Chk_InfoVoltage.CheckedChanged -= InformationCheckBox_CheckedChanged;
            _Chk_InfoCurrent.CheckedChanged -= InformationCheckBox_CheckedChanged;
            _Chk_InfoPower.CheckedChanged -= InformationCheckBox_CheckedChanged;
            _Chk_InfoAutoFollow.CheckedChanged -= InformationCheckBox_CheckedChanged;
        }
        #endregion
    }
}

