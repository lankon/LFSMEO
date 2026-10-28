using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ToolFunction;
using RecipeManage.Base;
using BurnInTester.Base;
using BurnInTester.Logic;
using BurnInTester.Logic.TestSetting;

namespace BurnInTester.UI
{
    public partial class F_TestSetting : Form
    {
        public F_TestSetting(AgingInformation agingInformation, F_TestSettingLogic f_TestSettingLogic)
        {
            _AgingInfo = agingInformation;
            _TestSettingLogic = f_TestSettingLogic;

            InitializeComponent();

            InitialForm();
        }

        #region parameter define
        private string CurRecipeName = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
        private readonly AgingInformation _AgingInfo;
        private readonly F_TestSettingLogic _TestSettingLogic;
        private string _SelectedTestConditionId;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            TabCtrl_TestCondition.Appearance = TabAppearance.FlatButtons;
            TabCtrl_TestCondition.SizeMode = TabSizeMode.Fixed;
            TabCtrl_TestCondition.ItemSize = new Size(0, 1);

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);
        }

        private void ShowHint()
        {
        }

        private void ReadAllEnumSetting()
        {
            CurRecipeName = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
        }

        private void UpdateEnumSettingToForm()
        {
        }

        private void SaveAllEnumSetting()
        {
        }

        private void UpdatePage()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            if (!LoadTestSettings())
                Tool.SaveLogToFile("測試條件表讀取失敗");
        }

        private void LeavePage()
        {
            SaveTestSettings();
            _TestSettingLogic.LoadAllAgingProcessSettings();
        }

        private void ShowTestProcessPage(TestProcess testProcess)
        {
            switch (testProcess)
            {
                case TestProcess.Temperature:
                    TabCtrl_TestCondition.SelectedTab = TabPg_Temperature;
                    break;
                case TestProcess.LIV:
                    TabCtrl_TestCondition.SelectedTab = TabPg_LIV;
                    break;
            }
        }

        private void TestProcess_SelectionChangeCommitted(object sender, EventArgs e)
        {
            DataGridViewCell cell = DGV_TestCondition.CurrentCell;
            if (cell?.OwningColumn != Title_TestProcess)
                return;

            // Let the ComboBox finish updating its selection and closing before ending the edit.
            BeginInvoke(new Action(() =>
            {
                if (DGV_TestCondition.IsDisposed || DGV_TestCondition.CurrentCell != cell ||
                    DGV_TestCondition.EditingControl != sender)
                    return;

                try
                {
                    if (DGV_TestCondition.CommitEdit(DataGridViewDataErrorContexts.Commit))
                        DGV_TestCondition.EndEdit();

                    string id = GetCellString(cell.OwningRow, Title_ID.Name);
                    TestProcess process = ParseTestProcess(
                        GetCellString(cell.OwningRow, Title_TestProcess.Name, TestProcess.None.ToString()));
                    _TestSettingLogic.ChangeTestProcess(id, process);
                    SelectTestCondition(cell.OwningRow);
                }
                catch (Exception ex)
                {
                    ShowError("測試項目設定失敗", ex);
                }
            }));
        }

        private string GetTestSettingFilePath()
        {
            string fileName = $"TestSettingBox{_AgingInfo.CurSelectBoxNo + 1}.xml";
            return Path.Combine(Application.StartupPath, "Setting", "Package", CurRecipeName, fileName);
        }

        private bool SaveTestSettings()
        {
            string filePath = GetTestSettingFilePath();

            try
            {
                DGV_TestCondition.EndEdit();
                _TestSettingLogic.SynchronizeTestSteps(GetTestStepsFromGrid());
                _TestSettingLogic.Save(filePath);

                Tool.SaveLogToFile($"{filePath}儲存完成");
                return true;
            }
            catch (Exception ex)
            {
                Tool.SaveLogToFile($"{filePath}儲存失敗：{ex}");
                ShowError("測試條件儲存失敗", ex);
                return false;
            }
        }

        private bool LoadTestSettings()
        {
            string filePath = GetTestSettingFilePath();
            if (!File.Exists(filePath))
                return false;

            try
            {
                _TestSettingLogic.Load(filePath);
                UpdateGridFromData();

                _SelectedTestConditionId = null;
                ClearTemperatureCondition();

                if (DGV_TestCondition.Rows.Count > 0)
                {
                    DGV_TestCondition.CurrentCell = DGV_TestCondition.Rows[0].Cells[0];
                    SelectTestCondition(DGV_TestCondition.Rows[0]);
                }

                return true;
            }
            catch (Exception ex)
            {
                Tool.SaveLogToFile($"{filePath}讀取失敗：{ex}");
                ShowError("測試條件讀取失敗", ex);
                return false;
            }
        }

        private List<TestStep> GetTestStepsFromGrid()
        {
            // 取得表單上的設定值
            List<TestStep> testSteps = new List<TestStep>();

            foreach (DataGridViewRow row in DGV_TestCondition.Rows)
            {
                if (row.IsNewRow)
                    continue;

                testSteps.Add(new TestStep
                {
                    No = GetCellString(row, Title_No.Name),
                    Skip = GetCellString(row, Title_Skip.Name),
                    TestProcess = ParseTestProcess(
                        GetCellString(row, Title_TestProcess.Name, TestProcess.None.ToString())),
                    Content = GetCellString(row, Title_Content.Name),
                    Id = GetCellString(row, Title_ID.Name)
                });
            }

            return testSteps;
        }

        private void UpdateGridFromData()
        {
            DGV_TestCondition.Rows.Clear();

            foreach (TestStep step in _TestSettingLogic.Data.TestSteps)
            {
                DGV_TestCondition.Rows.Add(
                    step.No,
                    step.Skip,
                    step.TestProcess.ToString(),
                    step.Content,
                    step.Id);
            }
        }

        private void SelectTestCondition(DataGridViewRow row)
        {
            _SelectedTestConditionId = GetCellString(row, Title_ID.Name);
            TestProcess process = ParseTestProcess(
                GetCellString(row, Title_TestProcess.Name, TestProcess.None.ToString()));

            ShowTestProcessPage(process);
            
            if (process == TestProcess.Temperature && !string.IsNullOrWhiteSpace(_SelectedTestConditionId))
            {
                ClearTemperatureCondition();
                TemperatureCondition condition;
                if (_TestSettingLogic.TryGetTemperatureCondition(_SelectedTestConditionId, out condition))
                    ShowTemperatureCondition(condition);
            }

            if (process == TestProcess.LIV && !string.IsNullOrWhiteSpace(_SelectedTestConditionId))
            {
                ClearLivCondition();
                LivCondition condition;
                if (_TestSettingLogic.TryGetLIVCondition(_SelectedTestConditionId, out condition))
                    ShowLivCondition(condition);
            }
        }

        private void ClearTemperatureCondition()
        {
            TxtBx_TargetTemperature.Clear();
            TxtBx_BurnInDuration.Clear();
            TxtBx_StabiliztionWaitTime.Clear();
            TxtBx_TemperatureTolerance.Clear();
            TxtBx_StabilizationTimeout.Clear();
        }

        private void ClearLivCondition()
        {
            TxtBx_StartI.Clear();
            TxtBx_StepI.Clear();
            TxtBx_EndI.Clear();
            TxtBx_LIVDelayTime.Clear();
        }

        private void ShowTemperatureCondition(TemperatureCondition condition)
        {
            TxtBx_TargetTemperature.Text = condition.TargetTemperature ?? "";
            TxtBx_BurnInDuration.Text = condition.BurnInDuration ?? "";
            TxtBx_StabiliztionWaitTime.Text = condition.StabilizationWaitTime ?? "";
            TxtBx_TemperatureTolerance.Text = condition.TemperatureTolerance ?? "";
            TxtBx_StabilizationTimeout.Text = condition.StabilizationTimeout ?? "";
        }

        private void ShowLivCondition(LivCondition condition)
        {
            TxtBx_StartI.Text = condition.StartI ?? "";
            TxtBx_StepI.Text = condition.StepI ?? "";
            TxtBx_EndI.Text = condition.EndI ?? "";
            TxtBx_LIVDelayTime.Text = condition.DelayTime ?? "";
        }

        private string GetCellString(DataGridViewRow row, string columnName, string defaultValue = "")
        {
            object value = row.Cells[columnName].Value;
            return value == null ? defaultValue : value.ToString();
        }

        private TestProcess ParseTestProcess(string value)
        {
            TestProcess process;
            bool success = Enum.TryParse(value, true, out process);

            if (!success || !Enum.IsDefined(typeof(TestProcess), process))
                throw new InvalidOperationException($"無法辨識的測試項目：{value}");

            return process;
        }

        private void ShowError(string title, Exception ex)
        {
            MessageBox.Show(ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {
        }
        #endregion


        private void F_TestSetting_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                SaveAllEnumSetting();
                ReadAllEnumSetting();
                
                LeavePage();
                Tool.ReleaseButtonImages(this);
                Close();
                Dispose();
            }
            else
            {
                UpdatePage();
            }
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            SaveTestSettings();
        }

        private void Btn_Load_Click(object sender, EventArgs e)
        {
            if (!LoadTestSettings())
                Tool.SaveLogToFile("測試條件表讀取失敗");
        }

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            int insertIndex = DGV_TestCondition.CurrentCell?.RowIndex ?? DGV_TestCondition.Rows.Count;
            TestStep step = _TestSettingLogic.AddTestStep(insertIndex);

            DGV_TestCondition.Rows.Insert(
                insertIndex,
                step.No,
                step.Skip,
                step.TestProcess.ToString(),
                step.Content,
                step.Id);

            DGV_TestCondition.CurrentCell = DGV_TestCondition.Rows[insertIndex].Cells[0];
            SelectTestCondition(DGV_TestCondition.Rows[insertIndex]);
        }

        private void DGV_TestCondition_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (DGV_TestCondition.CurrentCell?.OwningColumn != Title_TestProcess)
                return;

            DataGridViewComboBoxEditingControl comboBox = e.Control as DataGridViewComboBoxEditingControl;
            if (comboBox != null)
            {
                // DataGridView reuses the editing control between cells.
                comboBox.SelectionChangeCommitted -= TestProcess_SelectionChangeCommitted;
                comboBox.SelectionChangeCommitted += TestProcess_SelectionChangeCommitted;
            }
        }

        private void DGV_TestCondition_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow gridRow = DGV_TestCondition.Rows[e.RowIndex];
            SelectTestCondition(gridRow);

            if (e.ColumnIndex != Title_TestProcess.Index)
                return;

            DataGridViewCell cell = gridRow.Cells[e.ColumnIndex];
            if (cell.ReadOnly)
                return;

            // Open after the click finishes so the native click handling cannot close it again.
            BeginInvoke(new Action(() =>
            {
                if (DGV_TestCondition.IsDisposed || DGV_TestCondition.CurrentCell != cell ||
                    !DGV_TestCondition.BeginEdit(true))
                    return;

                DataGridViewComboBoxEditingControl comboBox =
                    DGV_TestCondition.EditingControl as DataGridViewComboBoxEditingControl;
                if (comboBox != null)
                    comboBox.DroppedDown = true;
            }));
        }

        private void Btn_Remove_Click(object sender, EventArgs e)
        {
            string id = DGV_TestCondition.CurrentRow == null
                ? ""
                : GetCellString(DGV_TestCondition.CurrentRow, Title_ID.Name);

            if (!string.IsNullOrWhiteSpace(id))
                _TestSettingLogic.RemoveTestStep(id);

            Tool.DataGrid_DeleteRow(DGV_TestCondition);

            _SelectedTestConditionId = null;
            ClearTemperatureCondition();

            if (DGV_TestCondition.CurrentRow != null)
                SelectTestCondition(DGV_TestCondition.CurrentRow);
        }

        private void Btn_SetTemperature_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_SelectedTestConditionId))
            {
                MessageBox.Show(
                    "請先選擇一筆 Temperature 測試條件。",
                    "尚未選擇測試條件",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                TemperatureCondition condition = new TemperatureCondition
                {
                    TargetTemperature = TxtBx_TargetTemperature.Text.Trim(),
                    BurnInDuration = TxtBx_BurnInDuration.Text.Trim(),
                    StabilizationWaitTime = TxtBx_StabiliztionWaitTime.Text.Trim(),
                    TemperatureTolerance = TxtBx_TemperatureTolerance.Text.Trim(),
                    StabilizationTimeout = TxtBx_StabilizationTimeout.Text.Trim()
                };

                _TestSettingLogic.UpdateTemperatureCondition(
                    _SelectedTestConditionId,
                    condition);

                UpdateGridFromData();
            }
            catch (Exception ex)
            {
                ShowError("測試條件設定失敗", ex);
            }
        }

        private void Btn_SetLIV_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_SelectedTestConditionId))
            {
                MessageBox.Show(
                    "請先選擇一筆 LIV 測試條件。",
                    "尚未選擇測試條件",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                LivCondition condition = new LivCondition
                {
                    StartI = TxtBx_StartI.Text.Trim(),
                    StepI = TxtBx_StepI.Text.Trim(),
                    EndI = TxtBx_EndI.Text.Trim(),
                    DelayTime = TxtBx_LIVDelayTime.Text.Trim(),
                };

                _TestSettingLogic.UpdateLIVCondition(
                    _SelectedTestConditionId,
                    condition);

                UpdateGridFromData();
            }
            catch (Exception ex)
            {
                ShowError("測試條件設定失敗", ex);
            }
        }
    }
}
