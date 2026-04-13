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


using ToolFunction;
using static RGBTester.Logic.TestResultDataBase;

namespace RGBTester.UI
{
    public partial class F_YieldReport : Form
    {
        public F_YieldReport(IServiceProvider serviceProvider, F_YieldReportLogic f_YieldReportLogic)
        {
            InitializeComponent();
            
            ServiceProvider = serviceProvider;
            YieldReportLogic1 = f_YieldReportLogic;

            InitialForm();
        }

        #region parameter define
        private IServiceProvider ServiceProvider;
        private F_YieldReportLogic YieldReportLogic1;
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
            toolTip1.SetToolTip(Btn_OutputResult, $"{AppDomain.CurrentDomain.BaseDirectory}\\Result");
        }
        private void ReadAllEnumSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_YieldReport>();
            //ApplicationSetting.ReadAllRecipe<eF_StartForm>();

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.ReadAllRecipe<eF_StartFormRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            ApplicationSetting.UpdataRecipeToForm<eF_YieldReport>(this);
            //ApplicationSetting.UpdataRecipeToForm<eF_StartFormRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_YieldReport>(this);

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
                //釋放記憶體資源
                Tool.ReleaseButtonImages(this);
                this.Close();
                this.Dispose();
            }
            else
            {
                UpdatePage();
            }
        }

        private void Btn_Search_Click(object sender, EventArgs e)
        {
            Tool.SaveLogToFile("查詢測試資料庫並計算良率");

            //取得篩選條件
            TestResultDataBase.ProductionLog pickip_condition = new TestResultDataBase.ProductionLog()
            {
                ProductType = TxtBx_ProductType.Text,
                SN = TxtBx_SN.Text,
                IsPass = Cmbx_Pass.SelectedIndex,
                Exclude = Cmbx_Exclude.SelectedIndex,
                Description = TxtBx_Description.Text
            };
            DateTime start_time = Tool.StringToDateTime(TxtBx_StartTime.Text);
            DateTime end_time = Tool.StringToDateTime(TxtBx_EndTime.Text);

            // 從資料庫取得符合條件的測試結果
            TestResultDataBase data_base = ServiceProvider.GetRequiredService<TestResultDataBase>();
            var result = data_base.Manager.GetResult(pickip_condition, start_time, end_time);
            //DGV_ProductRawData.DataSource = null; // 先清空
            DGV_ProductRawData.DataSource = new BindingList<TestResultDataBase.ProductionLog>(result);

            // 顯示統計數據
            List<ProductionLog> pass_data = new List<ProductionLog>();
            List<ProductionLog> fail_data = new List<ProductionLog>();
            YieldResult yieldResult = new YieldResult();
            data_base.Manager.GetSummaryReport(ref pass_data, ref fail_data, ref yieldResult);

            TxtBx_Total.Text = yieldResult.TotalUnits.ToString();
            TxtBx_Pass.Text = yieldResult.PassUnits.ToString();
            TxtBx_Fail.Text = yieldResult.FailUnits.ToString();
            //TxtBx_Exclude.Text = report.ExcludeUnits.ToString();
            TxtBx_Yield.Text = yieldResult.Yield.ToString("P2");
        }

        private void TxtBx_StartTime_DoubleClick(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;

            // 1. 動態建立一個小視窗 (Form)
            Form form = new Form();
            form.Text = "選擇日期";
            form.StartPosition = FormStartPosition.Manual;
            form.FormBorderStyle = FormBorderStyle.FixedToolWindow; // 簡潔的視窗樣式
            form.Size = new Size(240, 210);

            // 將視窗顯示在滑鼠點擊位置附近
            form.Location = tb.PointToScreen(new Point(0, tb.Height));

            // 2. 建立日曆控制項 (MonthCalendar)
            MonthCalendar calendar = new MonthCalendar();
            calendar.MaxSelectionCount = 1;
            calendar.Dock = DockStyle.Fill;

            // 3. 綁定選中事件：點一下日期就存值並關閉
            calendar.DateSelected += (s, args) =>
            {
                tb.Text = args.Start.ToString("yyyy-MM-dd");
                form.Close();
            };

            form.Controls.Add(calendar);
            form.ShowDialog(); // 以對話框模式顯示
        }

        private void DGV_ProductRawData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 排除標題列與初始化的干擾
            if (e.RowIndex < 0) return;

            TestResultDataBase data_base = ServiceProvider.GetRequiredService<TestResultDataBase>();

            var dgv = sender as DataGridView;
            var row = dgv.Rows[e.RowIndex];

            // 取得這筆資料的唯一 ID (假設你的 ID 欄位名稱是 "ID")
            if (row.Cells["Title_ID"].Value == null) return;
            int id = Convert.ToInt32(row.Cells["Title_ID"].Value);

            // 取得被修改的欄位名稱與新數值
            string columnName = dgv.Columns[e.ColumnIndex].HeaderText;
            object newValue = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

            // 存回資料庫
            try
            {
                data_base.Manager.UpdateDatabase(id, columnName, newValue);
                // 可以選擇性加上視覺反饋，例如改變儲存格顏色
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新失敗：{ex.Message}");
            }
        }

        private void Btn_OutputResult_Click(object sender, EventArgs e)
        {
            YieldReportLogic1.OutPutYieldReport();
            MessageBox.Show("Save Yield Report Success!");
        }
    }
}
