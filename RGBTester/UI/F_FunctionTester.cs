using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using UserPrivilege.Base;
using RGBTester.Base;
using RGBTester.Device;
using RGBTester.Logic;

namespace RGBTester.UI
{
    public partial class F_FunctionTester : Form
    {
        public F_FunctionTester(IServiceProvider serviceProvider, F_FunctionTesterLogic f_FunctionTesterLogic,
                                IFunction_LightEngine lea, IFunction_DataUpload function_DataUpdate,
                                IF_UserPrivilegeLogic Privilege)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            FunctionTesterLogic = f_FunctionTesterLogic;
            LEA = lea;
            DataUpdate = function_DataUpdate;
            UserPrivilege = Privilege;

            InitialForm();
        }

        #region parameter define
        IServiceProvider ServiceProvider;
        IF_UserPrivilegeLogic UserPrivilege;
        IFunction_LightEngine LEA;
        IFunction_DataUpload DataUpdate;
        F_FunctionTesterLogic FunctionTesterLogic;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            FunctionTesterLogic.SetVirtual_IO_Rule();

            ShowHint();

            LEA.Set_LEA_Type();
            LEA.Open();

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);
        }
        void ShowHint()
        {

        }
        private void ReadAllEnumSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_FunctionTester>();
            //ApplicationSetting.ReadAllRecipe<eF_StartForm>();

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.ReadAllRecipe<eF_StartFormRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            ApplicationSetting.UpdataRecipeToForm<eF_FunctionTester>(this);
            //ApplicationSetting.UpdataRecipeToForm<eF_StartFormRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_FunctionTester>(this);

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.SaveRecipeFromForm<eF_StartFormRecipe>(this, recipe_name);
        }
        private void UpdatePage()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            bool oem = UserPrivilege.AtLeastOEM();
            bool eng = UserPrivilege.AtLeastEng();

            Btn_MoveToElectrical.Visible = eng;
            Btn_MoveToOptical.Visible = oem;
            Btn_ElectricalFrom.Enabled = eng;
            Btn_OpticalForm.Enabled = eng;

        }
        private void LeavePage()
        {
        }
        private void ShowForm<T>() where T : class
        {
            var startForm = ServiceProvider.GetRequiredService<T>();
            if (startForm is Form form)
            {
                Tool.HideElementOnPanel(Scope.MainPanel);
                Tool.SetForm(Scope.MainPanel, form);
                form.Show();
            }
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
                //釋放記憶體資源
                Tool.ReleaseButtonImages(this);
                this.Close();
                this.Dispose();
            }
            else
            {
                UpdatePage();
            }
        }

        private void Btn_ElectricalFrom_Click(object sender, EventArgs e)
        {
            ShowForm<IF_StartForm>();
        }

        private void Btn_OpticalForm_Click(object sender, EventArgs e)
        {
            ShowForm<F_OpticalTest>();
        }

        private void Btn_UnLoad_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control || UserPrivilege.AtLeastOEM())
            {
                var MainTask = ServiceProvider.GetRequiredService<IBaseMainTask>();
                MainTask.SetTask<TaskUnLoad>();
                MainTask.Run();
            }
            else
            {
                MessageBox.Show("Please press Ctrl + Click to start the test.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Btn_StartTest_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control || UserPrivilege.AtLeastOEM())
            {
                SaveAllEnumSetting();
                ReadAllEnumSetting();

                int res = FunctionTesterLogic.StartFunctionTest();

                if(res == -1)
                    MessageBox.Show("Upload System No Connent", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Please press Ctrl + Click to start the test.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Btn_Test_Click(object sender, EventArgs e)
        {
            //DataUpdate.DataUpdate();
            //UploadInfo info = new UploadInfo
            //{
            //    OperatorID = ApplicationSetting.Get_String_Recipe<eF_FunctionTester>((int)eF_FunctionTester.TxtBx_OperatorID),
            //    SerialNunber = ApplicationSetting.Get_String_Recipe<eF_FunctionTester>((int)eF_FunctionTester.TxtBx_SerialNumber),

            //    FixtureID = ApplicationSetting.Get_String_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.TxtBx_FixtureID),
            //    PCName = ApplicationSetting.Get_String_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.TxtBx_PCName),
            //    ProgramVer = ApplicationSetting.Get_String_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.TxtBx_ProgramVer),
            //    Line = ApplicationSetting.Get_String_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.TxtBx_Line),
            //    Station = ApplicationSetting.Get_String_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.TxtBx_Station),
            //    Testplan = ApplicationSetting.Get_String_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.TxtBx_Testplan),

            //};

            //DataUpdate.SetInformation(info);    //測試用
            //DataUpdate.CheckConnectStatus();    //測試用
        }

        private void Btn_MoveToElectrical_Click(object sender, EventArgs e)
        {
            var MainTask = ServiceProvider.GetRequiredService<IBaseMainTask>();
            MainTask.SetTask<SubTaskMoveToElectrical>();
            MainTask.Run();
        }
    }
}
