using DeviceCore;
using DeviceFunction;
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


namespace DeviceUI.TemperatureControl
{
    

    public partial class F_TemperatureControl : Form, IF_TemperatureControl
    {
        public F_TemperatureControl(IFunction_TemperatureControl function_TemperatureControl)
        {
            InitializeComponent();

            Function_TemperatureControl = function_TemperatureControl;

            InitialForm();
        }

        #region parameter define
        List<TemperatureControlData> TemperatureControlList = new List<TemperatureControlData>();
        IFunction_TemperatureControl Function_TemperatureControl;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            if (!Tool.DataGrid_DataLoad(DGV_Temperature, "TemperatureControlSetting.xml"))
                Tool.SaveLogToFile("溫度控制表讀取失敗");

            //if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
            //    Tool.ShowFormName(this);
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
        }
        private void LeavePage()
        {
        }
        private void Update_Light_List(DataGridView DGV, List<TemperatureControlData> tc_list)
        {
            tc_list.Clear();
            var properties = typeof(TemperatureControlData).GetProperties(); // 取得所有屬性

            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.IsNewRow) continue;

                var data = new TemperatureControlData();

                foreach (var prop in properties)
                {
                    // 假設 DGV 的 Column Name 跟 TemperatureControlData 的 Property Name 是一致的
                    if (DGV.Columns.Contains(prop.Name))
                    {
                        object cellValue = row.Cells[prop.Name].Value;
                        if (cellValue != null && cellValue != DBNull.Value)
                        {
                            try
                            {
                                // 自動轉型並賦值 (處理 int, string, bool 等)
                                var safeValue = Convert.ChangeType(cellValue, prop.PropertyType);
                                prop.SetValue(data, safeValue);
                            }
                            catch { /* 處理轉型失敗，例如輸入英文字母到 int 欄位 */ }
                        }
                    }
                }
                tc_list.Add(data);
            }

            Function_TemperatureControl.LoadConfiguration(tc_list);
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {

        }
        public void Update_TC_List()
        {
            Update_Light_List(DGV_Temperature, TemperatureControlList);
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

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            string[] context = new string[] { "None", "None", "None", "COM1", "19200", "8", "One", "None" };

            Tool.DataGrid_AddRow(DGV_Temperature, context);
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_DataSave(DGV_Temperature, "TemperatureControlSetting.xml");
        }

        private void Btn_Load_Click(object sender, EventArgs e)
        {
            if (!Tool.DataGrid_DataLoad(DGV_Temperature, "TemperatureControlSetting.xml"))
                Tool.SaveLogToFile("溫控表讀取失敗");
        }

        private void Btn_Remove_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_DeleteRow(DGV_Temperature);
        }

        private void Btn_RowUp_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_RowUp(DGV_Temperature);
        }

        private void Btn_RowDown_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_RowDown(DGV_Temperature);
        }

        private void DGV_Temperature_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (TemperatureControlList.Count == 0 || e.RowIndex < 0) return;

            Update_Light_List(DGV_Temperature, TemperatureControlList);
        }
    }
}
