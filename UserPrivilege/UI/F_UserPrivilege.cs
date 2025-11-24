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
using UserPrivilege.Base;

namespace UserPrivilege.UI
{
    public partial class F_UserPrivilege : Form, IF_UserPrivilege
    {
        public F_UserPrivilege(IF_UserPrivilegeLogic f_UserPrivilegeLogic)
        {
            InitializeComponent();

            UserPrivilegeLogic = f_UserPrivilegeLogic;
            
            InitialForm();
        }

        #region parameter define
        IF_UserPrivilegeLogic UserPrivilegeLogic;
        #endregion

        #region private function
        void InitialForm()
        {
            //ApplicationSetting.ReadAllRecipe<eOEMSetting>();
            //ApplicationSetting.ReadAllRecipe<eMachineSetting>();
            //ApplicationSetting.UpdataRecipeToForm<eOEMSetting>(this);
            //ApplicationSetting.UpdataRecipeToForm<eMachineSetting>(this);

            ShowHint();

            //if (ApplicationSetting.Get_Int_Recipe<eOEMSetting>((int)eOEMSetting.Cmbx_ShowFormName) == 1)
            //    Tool.ShowFormName(this);
        }
        void ShowHint()
        {

        }
        private void UpdatePage()
        {
            if(UserPrivilegeLogic.AtLeastEng())
            {
                Pnl_CreateAccount.Visible = true;
            }
            else
            {
                Pnl_CreateAccount.Visible = false;
            }

            LoadAccountPasswordToDataGrid();
        }
        private void LoadAccountPasswordToDataGrid()
        {
            DGV_UserLevel.Rows.Clear();

            var list = UserPrivilegeLogic.LoadAccountPassword();

            foreach (var dict in list)
            {
                dict.TryGetValue("Title_Account", out object account);
                if (account.ToString() == "Maintenance")
                    continue;

                int rowIndex = DGV_UserLevel.Rows.Add();

                foreach (var kv in dict)
                {
                    if (kv.Value.ToString() == "OEM")
                        continue;
                    
                    if (DGV_UserLevel.Columns.Contains(kv.Key))
                        DGV_UserLevel.Rows[rowIndex].Cells[kv.Key].Value = kv.Value;
                }
            }
        }
        #endregion

        #region public function
        
        #endregion

        private void F_Equipment_Setting_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                ////儲存參數
                //ApplicationSetting.SaveRecipeFromForm<eOEMSetting>(this);
                //ApplicationSetting.SaveRecipeFromForm<eMachineSetting>(this);
                ////重新讀取變數值
                //ApplicationSetting.ReadAllRecipe<eOEMSetting>();
                //ApplicationSetting.ReadAllRecipe<eMachineSetting>();

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
            string[] context = new string[] { "None", "None", "ENG"};
            Tool.DataGrid_AddRow(DGV_UserLevel, context);
        }

        private void Btn_Remove_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_DeleteRow(DGV_UserLevel);
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            Dictionary<string, object> dict;


            foreach (DataGridViewRow row in DGV_UserLevel.Rows)
            {
                if (row.IsNewRow) continue;

                dict = new Dictionary<string, object>();

                foreach (DataGridViewCell cell in row.Cells)
                    dict[DGV_UserLevel.Columns[cell.ColumnIndex].Name] = cell.Value;

                list.Add(dict);
            }

            dict = new Dictionary<string, object>();
            dict["Title_Account"] = "Maintenance";
            dict["Title_Password"] = "70697332";
            dict["Title_Level"] = "OEM";
            list.Add(dict);

            UserPrivilegeLogic.GetDataGridInfo(list);
            UserPrivilegeLogic.SaveAccountPassword();
        }

        private void Btn_Login_Click(object sender, EventArgs e)
        {
            eUserLevel res = UserPrivilegeLogic.CheckUserPrivilege(TxtBx_Account.Text, TxtBx_Password.Text);
        
            if(res == eUserLevel.NONE)
            {
                Labl_LevelResult.Text = "FAIL";
                Labl_LevelResult.ForeColor = Color.Red;
            }
            if (res == eUserLevel.OP)
            {
                Labl_LevelResult.Text = "OP OK";
                Labl_LevelResult.ForeColor = Color.Blue;
            }
            else if(res == eUserLevel.ENG)
            {
                Labl_LevelResult.Text = "ENG OK";
                Labl_LevelResult.ForeColor = Color.Blue;
            }
            else if (res == eUserLevel.OEM)
            {
                Labl_LevelResult.Text = "OEM OK";
                Labl_LevelResult.ForeColor = Color.Blue;
            }

            this.Hide();
            this.Show();
        }

        private void Btn_Logout_Click(object sender, EventArgs e)
        {
            UserPrivilegeLogic.SetCurLevel(eUserLevel.OP);

            Labl_LevelResult.Text = "OP OK";
            Labl_LevelResult.ForeColor = Color.Blue;

            this.Hide();
            this.Show();
        }

        private void DGV_UserLevel_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (DGV_UserLevel.Columns[e.ColumnIndex].Name == "Title_Password")
            {
                if (e.Value != null)
                {
                    e.Value = new string('*', e.Value.ToString().Length);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            Dictionary<string, object> dict;


            foreach (DataGridViewRow row in DGV_UserLevel.Rows)
            {
                if (row.IsNewRow) continue;

                dict = new Dictionary<string, object>();

                foreach (DataGridViewCell cell in row.Cells)
                    dict[DGV_UserLevel.Columns[cell.ColumnIndex].Name] = cell.Value;

                list.Add(dict);
            }

            dict = new Dictionary<string, object>();
            dict["Title_Account"] = "Maintenance";
            dict["Title_Password"] = "70697332";
            dict["Title_Level"] = "OEM";
            list.Add(dict);

            UserPrivilegeLogic.GetDataGridInfo(list);
            UserPrivilegeLogic.SaveAccountPassword();
        }
    }
}
