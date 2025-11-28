using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using ToolFunction;
using UserPrivilege.Base;
using RGBTester.Base;

using RGBTester.Logic;


namespace RGBTester.UI
{
    public partial class F_StartForm : Form
    {
        public F_StartForm(F_StartFormLogic f_StartFormLogic, IRGBTesterMachine rGBTesterMachine,
                            ILightEngineCommand lea, IF_UserPrivilegeLogic f_UserPrivilegeLogic)
        {
            InitializeComponent();

            StartFormLogic = f_StartFormLogic;
            RGBTesterMachine = rGBTesterMachine;
            LEA = lea;
            UserLevel = f_UserPrivilegeLogic;
            InitialForm();
        }

        #region parameter define
        F_StartFormLogic StartFormLogic;
        IRGBTesterMachine RGBTesterMachine;
        ILightEngineCommand LEA;
        IF_UserPrivilegeLogic UserLevel;
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
        private void UpdatePage()
        {
            if (UserLevel.AtLeastEng())
            {
                Btn_SingleTest.Enabled = true;
                Pnl_HighLowMode.Enabled = true;
            }
            else
            {
                Btn_SingleTest.Enabled = false;
                Pnl_HighLowMode.Enabled = false;
            }
        }
        #endregion

        #region public function
        #endregion

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            ApplicationSetting.SaveRecipeFromForm<eF_StartForm>(this);
            ApplicationSetting.ReadAllRecipe<eF_StartForm>();

            string method = ApplicationSetting.Get_String_Recipe<eF_StartForm>((int)eF_StartForm.Cmbx_TestMode);

            if (method == "0")
                method = "Left";
            else if (method == "1")
                method = "Right";
            else
                method = "Both";

            StartFormLogic.StartTaskAction(method);
        }

        private void F_StartForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                ApplicationSetting.SaveRecipeFromForm<eF_StartForm>(this);
                ApplicationSetting.ReadAllRecipe<eF_StartForm>();
            }
            else
            {
                UpdatePage();
            }
        }

        private void F_StartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplicationSetting.SaveRecipeFromForm<eF_StartForm>(this);
            ApplicationSetting.ReadAllRecipe<eF_StartForm>();
        }

        private void Btn_Test_Click(object sender, EventArgs e)
        {
            int value = 00;

            Thread.Sleep(1);
            bool Red = LEA.SetLed_DAC(LEA.LED_R_LSB, LEA.LED_RightSide, value);
            Thread.Sleep(1);
            bool Green = LEA.SetLed_DAC(LEA.LED_G_LSB, LEA.LED_RightSide, value);
            Thread.Sleep(1);
            bool Blue = LEA.SetLed_DAC(LEA.LED_B_LSB, LEA.LED_RightSide, value);

            int test = 0;
        }
    }
}
