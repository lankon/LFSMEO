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
using DeviceCore;
using UserPrivilege.Base;
using RGBTester.Base;
using RGBTester.Logic;


namespace RGBTester.UI
{
    public partial class F_OpticalSetting : Form
    {
        public F_OpticalSetting(IFunction_Spectrometer function_Spectrometer, IF_UserPrivilegeLogic f_UserPrivilegeLogic)
        {
            InitializeComponent();

            Spectrometer = function_Spectrometer;
            UserPrivilege = f_UserPrivilegeLogic;

            InitialForm();
        }

        #region parameter define
        IFunction_Spectrometer Spectrometer;
        IF_UserPrivilegeLogic UserPrivilege;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);
        }
        void ShowHint()
        {

        }
        private void ReadAllEnumSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_OpticalSetting>();
            //ApplicationSetting.ReadAllRecipe<eF_StartForm>();

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.ReadAllRecipe<eF_StartFormRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            ApplicationSetting.UpdataRecipeToForm<eF_OpticalSetting>(this);
            //ApplicationSetting.UpdataRecipeToForm<eF_StartFormRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_OpticalSetting>(this);

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.SaveRecipeFromForm<eF_StartFormRecipe>(this, recipe_name);
        }
        private void UpdatePage()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            if (!Tool.DataGrid_DataLoad(DGV_CalibrationData, "PowerCalibrationData.xml"))
                Tool.SaveLogToFile("PowerCalibrationData讀取失敗");

            bool oem = UserPrivilege.AtLeastOEM();
            bool eng = UserPrivilege.AtLeastEng();

            TxtBx_BackgroundOffset.Enabled = oem;
            TxtBx_BackgroundGain.Enabled = oem;
            TxtBx_OpticalKValue.Enabled = oem;
            TxtBx_Standard.Enabled = oem;
            TxtBx_PowerGain.Enabled = oem;
            TxtBx_PowerOffset.Enabled = oem;
            DGV_CalibrationData.Enabled = oem;
            TbLy_Button.Enabled = oem;
            TxtBx_RedPowerGain.Enabled = oem;
            TxtBx_BluePowerGain.Enabled = oem;
            TxtBx_GreenPowerGain.Enabled = oem;
            TxtBx_RedWavelengthGain.Enabled = oem;
            TxtBx_GreenWavelengthGain.Enabled = oem;
            TxtBx_BlueWavelengthGain.Enabled = oem;
        }
        private void LeavePage()
        {
            //再存一次,這樣使用者直接輸入數值時才可將數值設定至分光卡
            double std = Tool.StringToDouble(TxtBx_Standard.Text);
            double slope = Tool.StringToDouble(TxtBx_BackgroundGain.Text);
            double offset = Tool.StringToDouble(TxtBx_BackgroundOffset.Text);

            Spectrometer.SetBackgroundCoef(std, slope, offset);
        }

        private void CalculatePowerCalibration()
        {
            List<double> std = new List<double>();
            List<double> measure = new List<double>();

            foreach (DataGridViewRow row in DGV_CalibrationData.Rows)
            {
                if (row.IsNewRow) continue;

                std.Add(Tool.StringToDouble(row.Cells["Title_Std"]?.Value?.ToString()));
                measure.Add(Tool.StringToDouble(row.Cells["Title_Measure"]?.Value?.ToString()));
            }

            LinearCurveFitting fitting = new LinearCurveFitting(measure.ToArray(), std.ToArray());

            TxtBx_PowerGain.Text = fitting.Slope.ToString();
            TxtBx_PowerOffset.Text = fitting.Offset.ToString();
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {

        }
        #endregion
        private void F_Equipment_Setting_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                SaveAllEnumSetting();
                ReadAllEnumSetting();

                LeavePage();
                ////釋放記憶體資源
                Tool.ReleaseButtonImages(this);
                this.Close();
                this.Dispose();
            }
            else
            {
                UpdatePage();
            }
        }

        private void Btn_Calibration_Click(object sender, EventArgs e)
        {
            F_OpticalSettingLogic logic = new F_OpticalSettingLogic(Spectrometer);
            LinearCurveFitting res = logic.BackgroundCalibration(out double result);

            TxtBx_Standard.Text = result.ToString();
            TxtBx_BackgroundGain.Text = res.Slope.ToString();
            TxtBx_BackgroundOffset.Text = res.Offset.ToString();
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_DataSave(DGV_CalibrationData, "PowerCalibrationData.xml");
        }

        private void Btn_Load_Click(object sender, EventArgs e)
        {
            if (!Tool.DataGrid_DataLoad(DGV_CalibrationData, "PowerCalibrationData.xml"))
                Tool.SaveLogToFile("PowerCalibrationData讀取失敗");
        }

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            string[] context = new string[] { "0", "0", "0"};

            Tool.DataGrid_AddRow(DGV_CalibrationData, context);
        }

        private void Btn_Remove_Click(object sender, EventArgs e)
        {
            Tool.DataGrid_DeleteRow(DGV_CalibrationData);
        }

        private void Btn_PowerCalibration_Click(object sender, EventArgs e)
        {
            CalculatePowerCalibration();
        }
    }
}
