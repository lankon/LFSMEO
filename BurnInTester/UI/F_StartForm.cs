using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using BurnInTester.Base;
using BurnInTester.Logic;

namespace BurnInTester.UI
{
    public partial class F_StartForm : Form
    {
        public F_StartForm(IServiceProvider serviceProvider, F_StartFormLogic startFormLogic)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            StartFormLogic = startFormLogic;
            InitialForm();
        }

        #region parameter define
        private UC_CtrlBoxStatus[] CtrlBoxStatusArray = new UC_CtrlBoxStatus[39];
        private IServiceProvider ServiceProvider;
        private DieMap _DieMap = new DieMap();
        private F_StartFormLogic StartFormLogic;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            CreateDynamicElement();

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);

            TC_Test.UpdateTemperature += UpdateTemperature;
            TC_Test.UpdateErrorCount += UpdateErrorCount;
            //TC_Test.Open();
        }
        void ShowHint()
        {

        }
        private void ReadAllEnumSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_StartForm>();

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.ReadAllRecipe<eF_StartFormRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            ApplicationSetting.UpdataRecipeToForm<eF_StartForm>(this);
            //ApplicationSetting.UpdataRecipeToForm<eF_StartFormRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_StartForm>(this);

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.SaveRecipeFromForm<eF_StartFormRecipe>(this, recipe_name);
        }
        private void UpdatePage()
        {
            ReadAllEnumSetting();
            StartFormLogic.UpdateAgingParam();
            UpdateEnumSettingToForm();
        }
        private void LeavePage()
        {
        }
        private void CreateDynamicElement()
        {
            CtrlBoxStatus1.SetInformation("Finish");

            //ControlBox
            for (int i = 0; i < CtrlBoxStatusArray.Length; i++)
            {
                CtrlBoxStatusArray[i] = new UC_CtrlBoxStatus();

                if (i / 4 == 0 && i % 4 != 3)
                    LyPnl_CtrlBoxStatus.Controls.Add(CtrlBoxStatusArray[i], i + 1, 0);
                else if (i % 4 == 3)
                    LyPnl_CtrlBoxStatus.Controls.Add(CtrlBoxStatusArray[i], 0, i / 4 + 1);
                else
                    LyPnl_CtrlBoxStatus.Controls.Add(CtrlBoxStatusArray[i], i % 4 + 1, i / 4);

                CtrlBoxStatusArray[i].Dock = System.Windows.Forms.DockStyle.Fill;
                CtrlBoxStatusArray[i].Location = new System.Drawing.Point(4, 4);
                CtrlBoxStatusArray[i].Name = $"CtrlBoxStatus{i + 2}";
                CtrlBoxStatusArray[i].Size = new System.Drawing.Size(167, 87);
                CtrlBoxStatusArray[i].TabIndex = 1;
                CtrlBoxStatusArray[i].SetItemIndex($"CtrlBoxStatus{i + 2}");
                CtrlBoxStatusArray[i].Click += new System.EventHandler(this.CtrlBoxStatus1_Click);
            }

            //DieMap
            _DieMap.Dock = System.Windows.Forms.DockStyle.Fill;
            _DieMap.Location = new System.Drawing.Point(0, 0);
            _DieMap.Name = "DieMap";
            Pnl_Info.Controls.Add(_DieMap);
            
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
                //Tool.ReleaseButtonImages(this);
                //this.Close();
                //this.Dispose();
            }
            else
            {
                UpdatePage();
            }
        }

        private void Btn_GetStats_Click(object sender, EventArgs e)
        {
            Tool.F_Monitor f_Monitor = new Tool.F_Monitor();
            f_Monitor.Show();
        }

        private void Btn_Test_Click(object sender, EventArgs e)
        {
            //CreateDynamicElement();
        }

        private void Btn_Start_Click(object sender, EventArgs e)
        {

        }

        private void Btn_TestSetting_Click(object sender, EventArgs e)
        {
            var para_set = ServiceProvider.GetRequiredService<F_TestSetting>();

            if (para_set is Form form)
            {
                Tool.HideElementOnPanel(Scope.MainPanel);
                Tool.SetForm(Scope.MainPanel, form);
                form.Show();
            }
        }

        private void CtrlBoxStatus1_Click(object sender, EventArgs e)
        {
            SaveAllEnumSetting();
            ReadAllEnumSetting();
            
            Control ctrl = sender as Control;

            if (ctrl != null)
            {
                string name = ctrl.Name; // 取得元件名稱，例如 "CtrlBoxStatus1"
                int boxNum = int.Parse(name.Replace("CtrlBoxStatus", "")); // 從名稱中提取數字部分
                StartFormLogic.SaveAgingParam();
                StartFormLogic.SetCurBoxNum(boxNum);
                StartFormLogic.UpdateAgingParam();
                UpdateEnumSettingToForm();
            }
        }

        #region 龜山溫控測試
        Guishan_TC_Test TC_Test = new Guishan_TC_Test();

        private void UpdateErrorCount(string count)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateErrorCount), count);
            }
            else
            {
                TxtBx_ErrorCount.Text = count;
            }
        }
        private void UpdateTemperature(string temp)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateTemperature), temp);
            }
            else
            {
                Labl_PresentValue.Text = temp;
            }
        }
        private void Btn_Start_TC_Click(object sender, EventArgs e)
        {
            try
            {
                SaveAllEnumSetting();
                double sv = double.Parse(TxtBx_SetValue.Text);
                int resp_delay = int.Parse(TxtBx_RespDelay.Text);
                int send_delay = int.Parse(TxtBx_SendDelay.Text);

                TC_Test.Start(sv, resp_delay, send_delay);
            }
            catch (Exception ex)
            {
                MessageBox.Show("請輸入正確的數值格式");
                return;
            }
        }
        private void Btn_Stop_TC_Click(object sender, EventArgs e)
        {
            TC_Test.Stop();
        }
        #endregion


    }


}

