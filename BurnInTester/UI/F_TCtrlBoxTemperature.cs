using BurnInTester.Base;
using BurnInTester.Device;
using BurnInTester.Logic;
using BurnInTester.Logic.nTCtrlBoxSetting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolFunction;
using static BurnInTester.Logic.AgingInformation;

namespace BurnInTester.UI
{
    public partial class F_TCtrlBoxTemperature : Form
    {
        public F_TCtrlBoxTemperature(HW_ParamSetting hw_ParamSetting, AgingInformation agingInformation)
        {
            InitializeComponent();

            HW_ParamSetting = hw_ParamSetting;
            AgingInfo = agingInformation;

            InitialForm();
        }

        #region parameter define
        private HW_ParamSetting HW_ParamSetting;
        private AgingInformation AgingInfo;
        private UC_ShowTemperatureValue[] ShowTemperatureValueArray;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowTemperatureValueArray = new UC_ShowTemperatureValue[HW_ParamSetting.TC_Box._CtrlBoxNum];
            ShowHint();

            CreateDynamicElement();

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);
        }
        void ShowHint()
        {

        }
        private void ReadAllEnumSetting()
        {
            //ApplicationSetting.ReadAllRecipe<eOEMSetting>();
            //ApplicationSetting.ReadAllRecipe<eF_StartForm>();

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.ReadAllRecipe<eF_StartFormRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            //ApplicationSetting.UpdataRecipeToForm<eF_StartForm>(this);
            //ApplicationSetting.UpdataRecipeToForm<eF_StartFormRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            //ApplicationSetting.SaveRecipeFromForm<eF_StartForm>(this);

            //string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_CurRecipeName);
            //ApplicationSetting.SaveRecipeFromForm<eF_StartFormRecipe>(this, recipe_name);
        }
        private void UpdatePage()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();
        }
        private void LeavePage()
        {
        }
        private void CreateDynamicElement()
        {
            //ControlBox
            int start = 0;
            for (int i = start; i < 40; i++)
            {
                ShowTemperatureValueArray[i] = new UC_ShowTemperatureValue();

                if(i< 20)
                    LyPnl_CtrlBoxSetting.Controls.Add(ShowTemperatureValueArray[i], (i - start) % 4, (i - start) / 4);
                else
                    LyPnl_CtrlBoxSetting1.Controls.Add(ShowTemperatureValueArray[i], (i - start - 20) % 4, (i - start - 20) / 4);

                ShowTemperatureValueArray[i].Dock = System.Windows.Forms.DockStyle.Fill;
                ShowTemperatureValueArray[i].Location = new System.Drawing.Point(4, 4);
                ShowTemperatureValueArray[i].Name = $"ShowTemperatureValue{i + 1}";
                ShowTemperatureValueArray[i].Size = new System.Drawing.Size(167, 87);
                ShowTemperatureValueArray[i].TabIndex = 1;
                ShowTemperatureValueArray[i].SetItemIndex($"ShowTemperatureValue{i + 1}");
            }
        }
        #endregion

        #region public function
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

        private void Tm_UpdatePV_Tick(object sender, EventArgs e)
        {
            for(int i=0; i< HW_ParamSetting.TC_Box._CtrlBoxNum; i++)
            {
                if (HW_ParamSetting.TC_Box.Use[i] == true)
                    ShowTemperatureValueArray[i].Set_PV(AgingInfo.TemperatureInfos[i].GetPV());
            }
        }

        //private void F_TCtrlBoxSetting_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    for (int i = LyPnl_CtrlBoxSetting.Controls.Count - 1; i >= 0; i--)
        //    {
        //        Control ctrl = LyPnl_CtrlBoxSetting.Controls[i];

        //        // 從容器移除
        //        LyPnl_CtrlBoxSetting.Controls.Remove(ctrl);

        //        // 強制釋放資源（這會觸發你寫在 UserControl 裡的 Dispose override）
        //        if (ctrl != null && !ctrl.IsDisposed)
        //        {
        //            ctrl.Dispose();
        //        }
        //    }
        //}



    }
}
