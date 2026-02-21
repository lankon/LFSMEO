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

namespace DeviceUI.LightControl
{
    public partial class F_LightControl : Form, IF_LightControl
    {
        public F_LightControl(IFunction_LightControl function_LightControl)
        {
            InitializeComponent();

            LightControl = function_LightControl;

            InitialForm();
        }

        #region parameter define
        List<LightData> LightList = new List<LightData>();
        IFunction_LightControl LightControl;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            if (!Tool.DataGrid_DataLoad(DGV_Light, "Light.xml"))
                Tool.SaveLogToFile("Light控制表讀取失敗");
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
        private void Update_Light_List(DataGridView DGV, List<LightData> light_list)
        {
            light_list.Clear();

            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.IsNewRow) continue;

                int station = Tool.StringToInt(row.Cells["Title_Station"]?.Value.ToString());
                int port = Tool.StringToInt(row.Cells["Title_OutPort"]?.Value.ToString());
                int value = Tool.StringToInt(row.Cells["Title_Value"]?.Value.ToString());

                var data = new LightData()
                {
                    Title_LightType = row.Cells["Title_LightType"]?.Value?.ToString(),
                    Title_Name = row.Cells["Title_Name"]?.Value?.ToString(),
                    Title_Description = row.Cells["Title_Description"]?.Value?.ToString(),
                    Title_Comport = row.Cells["Title_Comport"]?.Value?.ToString(),
                    Title_Station = station,
                    Title_OutPort = port,
                    Title_Value = value,
                };

                light_list.Add(data);
            }

            LightControl.LoadConfiguration(light_list);

            //如果欄位太多需改成用以下方式處理
            //light_list.Clear();
            //var properties = typeof(LightData).GetProperties(); // 取得所有屬性

            //foreach (DataGridViewRow row in DGV.Rows)
            //{
            //    if (row.IsNewRow) continue;

            //    var data = new LightData();

            //    foreach (var prop in properties)
            //    {
            //        // 假設 DGV 的 Column Name 跟 LightData 的 Property Name 是一致的
            //        if (DGV.Columns.Contains(prop.Name))
            //        {
            //            object cellValue = row.Cells[prop.Name].Value;
            //            if (cellValue != null && cellValue != DBNull.Value)
            //            {
            //                try
            //                {
            //                    // 自動轉型並賦值 (處理 int, string, bool 等)
            //                    var safeValue = Convert.ChangeType(cellValue, prop.PropertyType);
            //                    prop.SetValue(data, safeValue);
            //                }
            //                catch { /* 處理轉型失敗，例如輸入英文字母到 int 欄位 */ }
            //            }
            //        }
            //    }
            //    light_list.Add(data);
            //}

            //LightControl.LoadConfiguration(light_list);
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {
            if (show)
                Tool.ShowFormName(this);
        }
        public void Update_Light_List()
        {
            Update_Light_List(DGV_Light, LightList);
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

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            string[] context = new string[] { "None", "None", "None", "COM1", "-1", "-1", "-1", "Open" };

            Tool.DataGrid_AddRow(DGV_Light, context);
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_DataSave(DGV_Light, "Light.xml");
        }

        private void Btn_Load_Click(object sender, EventArgs e)
        {
            if (!Tool.DataGrid_DataLoad(DGV_Light, "Light.xml"))
                Tool.SaveLogToFile("Light控制表讀取失敗");
        }

        private void Btn_Remove_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_DeleteRow(DGV_Light);
        }

        private void Btn_RowUp_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_RowUp(DGV_Light);
        }

        private void Btn_RowDown_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_RowDown(DGV_Light);
        }

        private void Btn_FunctionTest_Click(object sender, EventArgs e)
        {
            LightControl.SetLightValue(ELightName.LIGHT_1, 50);
        }

        private void DGV_Light_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (LightList.Count == 0 || e.RowIndex < 0) return;

            Update_Light_List(DGV_Light, LightList);
        }

        private void DGV_Light_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (DGV_Light.Columns[e.ColumnIndex].Name == "Title_Open")
            {
                var row = DGV_Light.Rows[e.RowIndex];

                string Title_LightType = row.Cells["Title_LightType"]?.Value?.ToString();
                string Title_Comport = row.Cells["Title_Comport"]?.Value?.ToString();
                int station = Tool.StringToInt(row.Cells["Title_Station"]?.Value.ToString());
                int port = Tool.StringToInt(row.Cells["Title_OutPort"]?.Value.ToString());
                int value = Tool.StringToInt(row.Cells["Title_Value"]?.Value.ToString());

                Enum.TryParse(Title_LightType, out ELightControlType eLightType);

                LightControl.SetLightValue(eLightType, Title_Comport, station, port, value);
            }
        }
    }
}
