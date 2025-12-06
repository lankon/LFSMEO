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
using RGBTester.Base;
using RGBTester.Logic;

namespace RGBTester.UI
{
    public partial class F_Recipe : Form
    {
        public F_Recipe(F_RecipeLogic f_RecipeLogic)
        {
            InitializeComponent();

            RecipeLogic = f_RecipeLogic;
            InitialForm();
        }

        #region parameter define
        F_RecipeLogic RecipeLogic;
        #endregion

        #region private function
        void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            //if (ApplicationSetting.Get_Int_Recipe<eOEMSetting>((int)eOEMSetting.Cmbx_ShowFormName) == 1)
            //    Tool.ShowFormName(this);
        }
        void ShowHint()
        {
            toolTip1.SetToolTip(Btn_Save, "Save");
            toolTip1.SetToolTip(Btn_Delete, "Delete");
            toolTip1.SetToolTip(Btn_LoadRecipe, "Load");
        }
        private void ReadAllEnumSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_Recipe>();

            string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            ApplicationSetting.ReadAllRecipe<eF_RecipeRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            ApplicationSetting.UpdataRecipeToForm<eF_Recipe>(this);
            ApplicationSetting.UpdataRecipeToForm<eF_RecipeRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_Recipe>(this);

            string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            ApplicationSetting.SaveRecipeFromForm<eF_RecipeRecipe>(this, recipe_name);
        }
        private void UpdatePage()
        {
            string[] name = RecipeLogic.LoadRecipeFolderName();

            ListBx_RecipeList.Items.Clear();

            for(int i=0; i< name.Length; i++)
            {
                ListBx_RecipeList.Items.Add(name[i]);
            }

            string res = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);

            TxtBx_RecipeName.Text = res;

            ReadAllEnumSetting();
            UpdateEnumSettingToForm();
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

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            bool exist = RecipeLogic.CheckRecipeExist(TxtBx_RecipeName.Text);

            if(exist)
            {
                // 顯示確認對話框
                DialogResult dialogResult = MessageBox.Show("Overwrite File?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // 根據用戶的選擇返回布爾值
                if (dialogResult == DialogResult.No)
                    return;
            }
            else
            {
                RecipeLogic.CopyRecipeFolder(TxtBx_CurRecipeName.Text,TxtBx_RecipeName.Text);
                ApplicationSetting.SetRecipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName, TxtBx_RecipeName.Text);
            }

            SaveAllEnumSetting();
            RecipeLogic.SaveRecipe(TxtBx_RecipeName.Text);
            RecipeLogic.ReadRecipe(TxtBx_RecipeName.Text);

            UpdatePage();
        }

        private void Btn_LoadRecipe_Click(object sender, EventArgs e)
        {
            bool res = RecipeLogic.ReadRecipe(TxtBx_RecipeName.Text);

            if(res == false)
            {
                Tool.SaveLogToFile("Load Recipe Fail", level: "ERR");
                MessageBox.Show("Load Recipe Fail", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Tool.SaveLogToFile("Load Recipe Success", level: "INF");
                MessageBox.Show("Load Recipe Success", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TxtBx_CurRecipeName.Text = TxtBx_RecipeName.Text;
                UpdatePage();
            }
        }

        private void ListBx_RecipeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            TxtBx_RecipeName.Text = ListBx_RecipeList.SelectedItem?.ToString();
        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            string select_recipe = ListBx_RecipeList.SelectedItem?.ToString();

            if(select_recipe == TxtBx_CurRecipeName.Text)
            {
                MessageBox.Show("Cannot Delete Current Recipe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult dialogResult = MessageBox.Show($"Delete {select_recipe} Recipe?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // 根據用戶的選擇返回布爾值
            if (dialogResult == DialogResult.Yes)
            {
                RecipeLogic.DeleteRecipeFolder(select_recipe);

                UpdatePage();
            }
        }

        private void ListBx_RecipeList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                Btn_Delete_Click(Btn_Delete, EventArgs.Empty);
            }
        }
    }
}
