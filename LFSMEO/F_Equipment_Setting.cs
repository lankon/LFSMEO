using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LFSMEO.Base_LFSMEO;
using ToolFunction;

namespace LFSMEO.UI
{
    public partial class F_Equipment_Setting : Form
    {
        #region parameter define
        #endregion

        #region private function
        void InitialForm()
        {
            ApplicationSetting.ReadAllRecipe<eOEMSetting>();
            ApplicationSetting.UpdataRecipeToForm<eOEMSetting>(this);

            ShowHint();

            if (ApplicationSetting.Get_Int_Recipe<eOEMSetting>((int)eOEMSetting.Cmbx_ShowFormName) == 1)
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
                ApplicationSetting.SaveRecipeFromForm<eOEMSetting>(this);
                //重新讀取變數值
                ApplicationSetting.ReadAllRecipe<eOEMSetting>();

                //釋放記憶體資源
                Tool.ReleaseButtonImages(this);
                this.Close();
                this.Dispose();
            }
        }
    }
}
