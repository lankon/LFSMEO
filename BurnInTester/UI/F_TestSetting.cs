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
using RecipeManage.Base;
using BurnInTester.Base;
using BurnInTester.Logic;

namespace BurnInTester.UI
{
    public partial class F_TestSetting : Form
    {
        public F_TestSetting(AgingInformation agingInformation)
        {
            InitializeComponent();

            InitialForm();

            _AgingInfo = agingInformation;
        }

        #region parameter define
        private string CurRecipeName = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
        private AgingInformation _AgingInfo;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new Size(0, 1);

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);
        }
        void ShowHint()
        {

        }
        private void ReadAllEnumSetting()
        {
            CurRecipeName = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);

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

            string file_name = $"TestSettingBox{_AgingInfo.CurSelectBoxNo + 1}.xml";
            if (!Tool.DataGrid_DataLoad(DGV_TestCondition, $"\\Package\\{CurRecipeName}\\{file_name}"))
                Tool.SaveLogToFile("測試條件表讀取失敗");
        }
        private void LeavePage()
        {
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

                if (DGV_TestCondition.CommitEdit(DataGridViewDataErrorContexts.Commit))
                    DGV_TestCondition.EndEdit();
            }));
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

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            string file_name = $"TestSettingBox{_AgingInfo.CurSelectBoxNo + 1}.xml";
            Tool.DataGrid_DataSave(DGV_TestCondition, $"\\Package\\{CurRecipeName}\\{file_name}");
        }

        private void Btn_Load_Click(object sender, EventArgs e)
        {
            string file_name = $"TestSettingBox{_AgingInfo.CurSelectBoxNo + 1}.xml";
            if (!Tool.DataGrid_DataLoad(DGV_TestCondition, $"\\Package\\{CurRecipeName}\\{file_name}"))
                Tool.SaveLogToFile("測試條件表讀取失敗");
        }

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            string[] context = new string[] { "1", "FALSE", "None", "None"};

            Tool.DataGrid_AddRow(DGV_TestCondition, context);
        }

        private void DGV_TestCondition_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (DGV_TestCondition.CurrentCell?.OwningColumn != Title_TestProcess)
                return;

            var comboBox = e.Control as DataGridViewComboBoxEditingControl;
            if (comboBox != null)
            {
                // DataGridView reuses the editing control between cells.
                comboBox.SelectionChangeCommitted -= TestProcess_SelectionChangeCommitted;
                comboBox.SelectionChangeCommitted += TestProcess_SelectionChangeCommitted;
            }
        }

        private void DGV_TestCondition_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != Title_TestProcess.Index)
                return;

            DataGridViewCell cell = DGV_TestCondition.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell.ReadOnly)
                return;

            // Open after the click finishes so the native click handling cannot close it again.
            BeginInvoke(new Action(() =>
            {
                if (DGV_TestCondition.IsDisposed || DGV_TestCondition.CurrentCell != cell ||
                    !DGV_TestCondition.BeginEdit(true))
                    return;

                var comboBox = DGV_TestCondition.EditingControl as DataGridViewComboBoxEditingControl;
                if (comboBox != null)
                    comboBox.DroppedDown = true;
            }));
        }
    }
}
