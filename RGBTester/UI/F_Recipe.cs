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

namespace RGBTester.UI
{
    public partial class F_Recipe : Form
    {
        public F_Recipe()
        {
            InitializeComponent();

            InitialForm();
        }

        #region parameter define
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

    }
}
