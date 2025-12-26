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
    public partial class F_StartForm : Form, IF_StartForm
    {
        public F_StartForm(F_StartFormLogic f_StartFormLogic, IRGBTesterMachine rGBTesterMachine,
                            ILightEngineFunction lea, IF_UserPrivilegeLogic f_UserPrivilegeLogic)
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
        ILightEngineFunction LEA;   
        IF_UserPrivilegeLogic UserLevel;
        private static readonly Dictionary<(string Side, string Color, string Mode), TextBox> _slopeTextBoxes = new Dictionary<(string, string, string), TextBox>();
        #endregion

        #region private function
        void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);

            LEA.Set_LEA_Type();
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
        private void ShowHint()
        {
            toolTip1.SetToolTip(Btn_Start, "Auto");
            toolTip1.SetToolTip(Btn_SingleTest, "Test");
            toolTip1.SetToolTip(Btn_GetTemperature, "Get Temperature");
        }
        private void UpdateSlopeOffset(string side, string color, string mode, double value_slope, double value_offset)
        {
            //[Left/RGB/High]
            if(side == "Left" && color == "R" && mode == "HCM")
            {
                TxtBx_LeftRedOffset_H.Text = value_offset.ToString();
                TxtBx_LeftRedSlope_H.Text = value_slope.ToString();
            }
            else if (side == "Left" && color == "G" && mode == "HCM")
            {
                TxtBx_LeftGreenOffset_H.Text = value_offset.ToString();
                TxtBx_LeftGreenSlope_H.Text = value_slope.ToString();
            }
            else if (side == "Left" && color == "B" && mode == "HCM")
            {
                TxtBx_LeftBlueOffset_H.Text = value_offset.ToString();
                TxtBx_LeftBlueSlope_H.Text = value_slope.ToString();
            }
            //[Left/RGB/Low]
            else if (side == "Left" && color == "R" && mode == "LCM")
            {
                TxtBx_LeftRedOffset_L.Text = value_offset.ToString();
                TxtBx_LeftRedSlope_L.Text = value_slope.ToString();
            }
            else if (side == "Left" && color == "G" && mode == "LCM")
            {
                TxtBx_LeftGreenOffset_L.Text = value_offset.ToString();
                TxtBx_LeftGreenSlope_L.Text = value_slope.ToString();
            }
            else if (side == "Left" && color == "B" && mode == "LCM")
            {
                TxtBx_LeftBlueOffset_L.Text = value_offset.ToString();
                TxtBx_LeftBlueSlope_L.Text = value_slope.ToString();
            }
            //[Right/RGB/Gigh]
            else if (side == "Right" && color == "R" && mode == "HCM")
            {
                TxtBx_RightRedOffset_H.Text = value_offset.ToString();
                TxtBx_RightRedSlope_H.Text = value_slope.ToString();
            }
            else if (side == "Right" && color == "G" && mode == "HCM")
            {
                TxtBx_RightGreenOffset_H.Text = value_offset.ToString();
                TxtBx_RightGreenSlope_H.Text = value_slope.ToString();
            }
            else if (side == "Right" && color == "B" && mode == "HCM")
            {
                TxtBx_RightBlueOffset_H.Text = value_offset.ToString();
                TxtBx_RightBlueSlope_H.Text = value_slope.ToString();
            }
            //[Right/RGB/Low]
            else if (side == "Right" && color == "R" && mode == "LCM")
            {
                TxtBx_RightRedOffset_L.Text = value_offset.ToString();
                TxtBx_RightRedSlope_L.Text = value_slope.ToString();
            }
            else if (side == "Right" && color == "G" && mode == "LCM")
            {
                TxtBx_RightGreenOffset_L.Text = value_offset.ToString();
                TxtBx_RightGreenSlope_L.Text = value_slope.ToString();
            }
            else if (side == "Right" && color == "B" && mode == "LCM")
            {
                TxtBx_RightBlueOffset_L.Text = value_offset.ToString();
                TxtBx_RightBlueSlope_L.Text = value_slope.ToString();
            }
        }
        private void UpdatePage()
        {
            bool enable = UserLevel.AtLeastEng();
            bool enable_oem = UserLevel.AtLeastOEM();
            bool IsLeft = ApplicationSetting.Get_Int_Recipe<eF_StartForm>((int)eF_StartForm.Cmbx_TestMode) == 0;

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
            Pnl_OEM_Test.Visible = enable_oem;
            LyPnl_ProductType.Visible = enable_oem;
            TxtBx_Left_SN.Enabled = IsLeft || ApplicationSetting.Get_Int_Recipe<eF_StartForm>((int)eF_StartForm.Cmbx_TestMode) == 2;
            TxtBx_Right_SN.Enabled = !IsLeft || ApplicationSetting.Get_Int_Recipe<eF_StartForm>((int)eF_StartForm.Cmbx_TestMode) == 2;

            ReadAllEnumSetting();
            UpdateEnumSettingToForm();
        }
        #endregion

        #region public function
        public void ShowSlopeOffsetResult(string side, string color, string mode, double value, double value1)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateSlopeOffset(side,color,mode, value, value1)));
            }
            else
            {
                UpdateSlopeOffset(side, color, mode, value, value1);
            }
        }
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

        private void Btn_GetTemperature_Click(object sender, EventArgs e)
        {
            TxtBx_LeftTemperature.Text = StartFormLogic.GetTemperature();
        }

        private void Btn_Set_DAC_Click(object sender, EventArgs e)
        {
            Int32.TryParse(TxtBx_DAC_Red.Text, out int dac_r);
            Int32.TryParse(TxtBx_DAC_Green.Text, out int dac_g);
            Int32.TryParse(TxtBx_DAC_Blue.Text, out int dac_b);

            StartFormLogic.Set_DAC_Test(dac_r, dac_g, dac_b);
        }

        private void Btn_Get_DAC_Click(object sender, EventArgs e)
        {
            int[] res = StartFormLogic.Get_DAC_Value();

            TxtBx_DAC_Red.Text = res[0].ToString();
            TxtBx_DAC_Green.Text = res[1].ToString();
            TxtBx_DAC_Blue.Text = res[2].ToString();
        }

        private void Cmbx_TestMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool IsLeft = Cmbx_TestMode.SelectedIndex == 0;
            TxtBx_Left_SN.Enabled = IsLeft || Cmbx_TestMode.SelectedIndex == 2;
            TxtBx_Right_SN.Enabled = !IsLeft || Cmbx_TestMode.SelectedIndex == 2;
        }
    }
}
