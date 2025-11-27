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
        public F_StartForm(F_StartFormLogic f_StartFormLogic, IRGBTesterMachine rGBTesterMachine,
                            ILightEngineCommand lea)
        {
            InitializeComponent();

            StartFormLogic = f_StartFormLogic;
            RGBTesterMachine = rGBTesterMachine;
            LEA = lea;
            InitialForm();
        }

        #region parameter define
        F_StartFormLogic StartFormLogic;
        IRGBTesterMachine RGBTesterMachine;
        ILightEngineCommand LEA;
        #endregion

        #region private function
        void InitialForm()
        {
            ReadAllEnumRecipe();
            ApplicationSetting.UpdataRecipeToForm<eF_StartForm>(this);

            ShowHint();

            if (ApplicationSetting.Get_Int_Recipe<eOEMSetting>((int)eOEMSetting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);
        }
        private void ReadAllEnumRecipe()
        {
            ApplicationSetting.ReadAllRecipe<eOEMSetting>();
            ApplicationSetting.ReadAllRecipe<eF_StartForm>();
        }
        void ShowHint()
        {
        }
        #endregion

        #region public function
        #endregion

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            StartFormLogic.StartTaskAction();
        }

        private void F_StartForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                ApplicationSetting.SaveRecipeFromForm<eF_StartForm>(this);
                ApplicationSetting.ReadAllRecipe<eF_StartForm>();
            }
        }

        private void F_StartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplicationSetting.SaveRecipeFromForm<eF_StartForm>(this);
            ApplicationSetting.ReadAllRecipe<eF_StartForm>();
        }

        private void Btn_Test_Click(object sender, EventArgs e)
        {
            bool open = LEA.CheckConnect();
        }
    }
}
