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
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);

            if (!LEA.Open())
                MessageBox.Show("LED Board Connect Fail！","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }
        private void ReadAllEnumSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();
            ApplicationSetting.ReadAllRecipe<eF_StartForm>();

            string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
            ApplicationSetting.ReadAllRecipe<eF_StartFormRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            ApplicationSetting.UpdataRecipeToForm<eF_StartForm>(this);
            ApplicationSetting.UpdataRecipeToForm<eF_StartFormRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_StartForm>(this);

            string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
            ApplicationSetting.SaveRecipeFromForm<eF_StartFormRecipe>(this, recipe_name);
        }
        void ShowHint()
        {
            toolTip1.SetToolTip(Btn_Start, "Auto");
            toolTip1.SetToolTip(Btn_SingleTest, "Test");
            toolTip1.SetToolTip(Btn_GetTemperature, "Get Temperature");
        }
        private void UpdatePage()
        {
            bool enable = UserLevel.AtLeastEng();

            Btn_SingleTest.Enabled = enable;
            Pnl_HighLowMode.Enabled = enable;
            Pnl_ShowTemperature.Enabled = enable;
            TxtBx_Left_DAC_Start.Enabled = enable;
            TxtBx_Left_DAC_End.Enabled = enable;
            TxtBx_Left_DAC_Step.Enabled = enable;
            TxtBx_Left_AvgCount.Enabled = enable;
            TxtBx_Right_DAC_Start.Enabled = enable;
            TxtBx_Right_DAC_End.Enabled = enable;
            TxtBx_Right_DAC_Step.Enabled = enable;
            TxtBx_Right_AvgCount.Enabled = enable;

            ReadAllEnumSetting();
            UpdateEnumSettingToForm();
        }
        #endregion

        #region public function
        #endregion

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            SaveAllEnumSetting();
            ReadAllEnumSetting();

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
                SaveAllEnumSetting();
                ReadAllEnumSetting();
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
            long RecordTime;

            bool Red = LEA.SetLed_DAC(LEA.LED_R_LSB, LEA.LED_RightSide, value);
            bool Green = LEA.SetLed_DAC(LEA.LED_G_LSB, LEA.LED_RightSide, value);
            bool Blue = LEA.SetLed_DAC(LEA.LED_B_LSB, LEA.LED_RightSide, value);

            if (!Red || !Green || !Blue)
                MessageBox.Show("錯誤");
        }

        private void Btn_SingleTest_Click(object sender, EventArgs e)
        {
            SaveAllEnumSetting();
            ReadAllEnumSetting();

            string method = ApplicationSetting.Get_String_Recipe<eF_StartForm>((int)eF_StartForm.Cmbx_TestMode);

            if (method == "0")
                method = "Left";
            else if (method == "1")
                method = "Right";
            else
                method = "Both";

            Scope.TaskRGBTest.IsSingleTest = true;
            StartFormLogic.StartTaskAction(method);
        }
    }
}
