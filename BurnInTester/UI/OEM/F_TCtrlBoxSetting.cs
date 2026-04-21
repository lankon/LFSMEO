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
    public partial class F_TCtrlBoxSetting : Form, IF_TCtrlBoxSetting
    {
        public F_TCtrlBoxSetting(HW_ParamSetting hw_ParamSetting)
        {
            InitializeComponent();

            HW_ParamSetting = hw_ParamSetting;

            InitialForm();
        }

        #region parameter define
        private HW_ParamSetting HW_ParamSetting;
        private UC_TCtrlBoxSetting[] TCtrlBoxSettingArray;
        private F_TCtrlBoxSettingLogic TCtrlBoxSettingLogic;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            TCtrlBoxSettingLogic = new F_TCtrlBoxSettingLogic(HW_ParamSetting);
            TCtrlBoxSettingArray = new UC_TCtrlBoxSetting[HW_ParamSetting.TC_Box._CtrlBoxNum];

            ShowHint();

            CreateDynamicElement();
            TCtrlBoxSettingLogic.LoadTCtrlBoxSetting();
            UpdateSettingToForm();

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
            //ControlBox Left
            int start = 0;
            for (int i = start; i < 20; i++)
            {
                TCtrlBoxSettingArray[i] = new UC_TCtrlBoxSetting();

                //if (i / 4 == 0 && i % 4 != 3)
                //    LyPnl_CtrlBoxSetting.Controls.Add(TCtrlBoxSettingArray[i], i + 1, 0);
                //else if (i % 4 == 3)
                //    LyPnl_CtrlBoxSetting.Controls.Add(TCtrlBoxSettingArray[i], 0, i / 4 + 1);
                //else
                //    LyPnl_CtrlBoxSetting.Controls.Add(TCtrlBoxSettingArray[i], i % 4 + 1, i / 4);

                LyPnl_CtrlBoxSetting.Controls.Add(TCtrlBoxSettingArray[i], (i - start) % 4, (i - start) / 4);

                TCtrlBoxSettingArray[i].Dock = System.Windows.Forms.DockStyle.Fill;
                TCtrlBoxSettingArray[i].Location = new System.Drawing.Point(4, 4);
                TCtrlBoxSettingArray[i].Name = $"TCtrlBoxSetting{i + 1}";
                TCtrlBoxSettingArray[i].Size = new System.Drawing.Size(167, 87);
                TCtrlBoxSettingArray[i].TabIndex = 1;
                TCtrlBoxSettingArray[i].SetItemIndex($"TCtrlBoxSetting{i + 1}");
                //TCtrlBoxSettingArray[i].Click += new System.EventHandler(this.TCtrlBoxSetting1_Click);
            }

            //ControlBox Right
            start = 20;
            for (int i = start; i < 40; i++)
            {
                TCtrlBoxSettingArray[i] = new UC_TCtrlBoxSetting();
                LyPnl_CtrlBoxSetting1.Controls.Add(TCtrlBoxSettingArray[i], (i - start) % 4, (i - start) / 4);
                TCtrlBoxSettingArray[i].Dock = System.Windows.Forms.DockStyle.Fill;
                TCtrlBoxSettingArray[i].Location = new System.Drawing.Point(4, 4);
                TCtrlBoxSettingArray[i].Name = $"TCtrlBoxSetting{i + 1}";
                TCtrlBoxSettingArray[i].Size = new System.Drawing.Size(167, 87);
                TCtrlBoxSettingArray[i].TabIndex = 1;
                TCtrlBoxSettingArray[i].SetItemIndex($"TCtrlBoxSetting{i + 1}");
                //TCtrlBoxSettingArray[i].Click += new System.EventHandler(this.TCtrlBoxSetting1_Click);
            }
        }
        private void UpdateSettingToForm()
        {
            for(int i=0; i< HW_ParamSetting.TC_Box._CtrlBoxNum; i++)
            {
                TCtrlBoxSettingArray[i].UpdateSetting(HW_ParamSetting.TC_Box.BoxNum[i], 
                                                      HW_ParamSetting.TC_Box.ChNum[i], 
                                                      HW_ParamSetting.TC_Box.Use[i] ? "1" : "0");
            }
        }
        #endregion

        #region public function
        public void ShowFormName(bool show)
        {

        }
        public bool LoadTCtrlBoxSetting()
        {
            return TCtrlBoxSettingLogic.LoadTCtrlBoxSetting();
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
                //Tool.ReleaseButtonImages(this);
                //this.Close();
                //this.Dispose();
            }
            else
            {
                UpdatePage();
            }
        }

        private void F_TCtrlBoxSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = LyPnl_CtrlBoxSetting.Controls.Count - 1; i >= 0; i--)
            {
                Control ctrl = LyPnl_CtrlBoxSetting.Controls[i];

                // 從容器移除
                LyPnl_CtrlBoxSetting.Controls.Remove(ctrl);

                // 強制釋放資源（這會觸發你寫在 UserControl 裡的 Dispose override）
                if (ctrl != null && !ctrl.IsDisposed)
                {
                    ctrl.Dispose();
                }
            }
        }
        
        private void Btn_SaveSetting_Click(object sender, EventArgs e)
        {
            Dictionary<string, TCtrlBoxSetting> DicSetting = new Dictionary<string, TCtrlBoxSetting>();
            
            for (int i=0; i< TCtrlBoxSettingArray.Length; i++)
            {
                TCtrlBoxSetting setting = new TCtrlBoxSetting
                {
                    TCtrlBoxID = TCtrlBoxSettingArray[i].GetBoxIndex(),
                    Use = TCtrlBoxSettingArray[i].GetUse(),
                    BoxNum = TCtrlBoxSettingArray[i].GetBoxNum(),
                    ChNum = TCtrlBoxSettingArray[i].GetChNum()
                };

                string BoxName = $"Box_{TCtrlBoxSettingArray[i].GetBoxIndex()}";
                DicSetting.Add(BoxName, setting);
            }

            TCtrlBoxSettingLogic.SaveTCtrlBoxSetting(DicSetting);

            MessageBox.Show("Save Setting Success！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Btn_Load_Click(object sender, EventArgs e)
        {
            TCtrlBoxSettingLogic.LoadTCtrlBoxSetting();

            MessageBox.Show("Load Setting Success！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Btn_SetAllUse_Click(object sender, EventArgs e)
        {
            string message = "Confirm Setting？";
            string caption = "";
            
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Button btn = sender as Button;

                if (btn == null) return;

                int res =  Tool.StringToInt(btn.Tag.ToString());
                bool action = res == 1 ? true : false;
                
                for (int i = 0; i < TCtrlBoxSettingArray.Length; i++)
                {
                    TCtrlBoxSettingArray[i].SetTCtrlEnable(action);
                }
            }
            else
            {
            }
        }

    }
}
