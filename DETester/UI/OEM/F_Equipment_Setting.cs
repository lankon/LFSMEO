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

using DETester.Base;

namespace DETester.UI
{
    public partial class F_Equipment_Setting : Form
    {
        #region parameter define
        #endregion

        #region private function
        void InitialForm()
        {
            ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();
            ApplicationSetting.ReadAllRecipe<eMachineSetting>();
            ApplicationSetting.UpdataRecipeToForm<eF_Equipment_Setting>(this);
            ApplicationSetting.UpdataRecipeToForm<eMachineSetting>(this);

            ShowHint();

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);
        }
        void ShowHint()
        {

        }
        #endregion

        #region public function
        
        #endregion

        public F_Equipment_Setting()
        {
            InitializeComponent();

            InitialForm();
        }

        private void F_Equipment_Setting_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                //儲存參數
                ApplicationSetting.SaveRecipeFromForm<eF_Equipment_Setting>(this);
                ApplicationSetting.SaveRecipeFromForm<eMachineSetting>(this);
                //重新讀取變數值
                ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();
                ApplicationSetting.ReadAllRecipe<eMachineSetting>();

                //釋放記憶體資源
                Tool.ReleaseButtonImages(this);
                this.Close();
                this.Dispose();
            }
        }
    }
}
