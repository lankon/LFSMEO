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

namespace DeviceUI.SourceMeter
{
    public partial class F_SourceMeter : Form, IF_SourceMeter
    {
        public F_SourceMeter(IFunction_SourceMeter function_SourceMeter)
        {
            InitializeComponent();

            Func_SMU = function_SourceMeter;

            InitialForm();
        }

        #region parameter define
        IFunction_SourceMeter Func_SMU;
        List<SourceMeterData> SourceMeterList = new List<SourceMeterData>();
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            if (!Tool.DataGrid_DataLoad(DGV_SMU, "SourceMeter.xml"))
                Tool.SaveLogToFile("Spectrum控制表讀取失敗");

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
        private void Update_SMU_List(DataGridView DGV, List<SourceMeterData> smu_list)
        {
            smu_list.Clear();

            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.IsNewRow) continue;

                var data = new SourceMeterData()
                {
                    Title_SMU_Type = row.Cells["Title_SMU_Type"]?.Value?.ToString(),
                    Title_Name = row.Cells["Title_Name"]?.Value?.ToString(),
                    Title_CardID = row.Cells["Title_CardID"]?.Value?.ToString(),
                    Title_Description = row.Cells["Title_Description"]?.Value?.ToString(),
                };

                smu_list.Add(data);
            }

            Func_SMU.LoadConfiguration(smu_list);
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {
            if (show)
                Tool.ShowFormName(this);
        }
        public void Update_SMU_List()
        {
            Update_SMU_List(DGV_SMU, SourceMeterList);
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

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_DataSave(DGV_SMU, "SourceMeter.xml");
        }

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            string[] context = new string[] { "None", "None", "None", "None" };

            Tool.DataGrid_AddRow(DGV_SMU, context);
        }

        private void Btn_Remove_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_DeleteRow(DGV_SMU);
        }

        private void Btn_RowUp_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_RowUp(DGV_SMU);
        }

        private void Btn_RowDown_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_RowDown(DGV_SMU);
        }

        private void Btn_Load_Click(object sender, EventArgs e)
        {
            if (!Tool.DataGrid_DataLoad(DGV_SMU, "SourceMeter.xml"))
                Tool.SaveLogToFile("SourceMeter表讀取失敗");
        }
    }
}
