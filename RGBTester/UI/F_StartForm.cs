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
    public partial class F_StartForm : Form
    {
        public F_StartForm(F_StartFormLogic f_StartFormLogic)
        {
            InitializeComponent();

            StartFormLogic = f_StartFormLogic;
            //InitialForm();
        }

        #region parameter define
        F_StartFormLogic StartFormLogic;
        #endregion

        #region private function
        //void InitialForm()
        //{
        //    ReadAllEnumRecipe();
        //    ApplicationSetting.UpdataRecipeToForm<eOEMSetting>(this);

        //    ShowHint();

        //    if (ApplicationSetting.Get_Int_Recipe<eOEMSetting>((int)eOEMSetting.Cmbx_ShowFormName) == 1)
        //        Tool.ShowFormName(this);
        //}
        //private void ReadAllEnumRecipe()
        //{
        //    ApplicationSetting.ReadAllRecipe<eOEMSetting>();
        //    ApplicationSetting.ReadAllRecipe<eOEMSetting>();
        //}
        //void ShowHint()
        //{
        //}
        #endregion

        #region public function
        #endregion



        private void Btn_Start_Click(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void Btn_Load_Click(object sender, EventArgs e)
        {
        }

        private void Btn_Open_Click(object sender, EventArgs e)
        {
            StartFormLogic.OpenChillerComm();
        }

        private void Btn_GetChillerStatus_Click(object sender, EventArgs e)
        {
            StartFormLogic.GetChillerStatus();
        }
    }
}
