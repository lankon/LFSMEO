using System;
using System.Collections;
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
    public partial class F_ElectricalSetting : Form, IF_ElectricalSetting
    {
        public F_ElectricalSetting(IServiceProvider serviceProvider, F_ParameterSettingLogic f_ParameterSettingLogic)
        {
            InitializeComponent();

            ServiceProvider = serviceProvider;
            ParameterSettingLogic1 = f_ParameterSettingLogic;

            InitialForm();
        }

        #region parameter define
        private const int count = 5;
        Label[] Labl_CheckSlopeResult_LCM_R = new Label[count];
        Label[] Labl_CheckSlopeResult_LCM_G = new Label[count];
        Label[] Labl_CheckSlopeResult_LCM_B = new Label[count];
        Label[] Labl_CheckSlopeResult_LCM_R_Dev = new Label[count];
        Label[] Labl_CheckSlopeResult_LCM_G_Dev = new Label[count];
        Label[] Labl_CheckSlopeResult_LCM_B_Dev = new Label[count];
        Label[] Labl_CheckSlopeResult_HCM_R = new Label[count];
        Label[] Labl_CheckSlopeResult_HCM_G = new Label[count];
        Label[] Labl_CheckSlopeResult_HCM_B = new Label[count];
        Label[] Labl_CheckSlopeResult_HCM_R_Dev = new Label[count];
        Label[] Labl_CheckSlopeResult_HCM_G_Dev = new Label[count];
        Label[] Labl_CheckSlopeResult_HCM_B_Dev = new Label[count];
        IServiceProvider ServiceProvider;
        F_ParameterSettingLogic ParameterSettingLogic1;
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);

            CreateDynamicElement();
        }
        void ShowHint()
        {
            
        }
        private void ReadAllEnumSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_ParameterSetting>();
            //ApplicationSetting.ReadAllRecipe<eF_ParameterSetting>();

            string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
            ApplicationSetting.ReadAllRecipe<eF_ParameterSettingRecipe>(recipe_name);
        }
        private void UpdateEnumSettingToForm()
        {
            ApplicationSetting.UpdataRecipeToForm<eF_ParameterSetting>(this);
            ApplicationSetting.UpdataRecipeToForm<eF_ParameterSettingRecipe>(this);
        }
        private void SaveAllEnumSetting()
        {
            ApplicationSetting.SaveRecipeFromForm<eF_ParameterSetting>(this);

            string recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
            ApplicationSetting.SaveRecipeFromForm<eF_ParameterSettingRecipe>(this, recipe_name);
        }
        private void UpdatePage()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();
        }

        private void CreateLabel(ref Label label, TableLayoutPanel tableLayoutPanel, int column, int row)
        {
            label = new Label();
            label.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            label.Location = new System.Drawing.Point(95, 37);
            label.Name = $"Labl_Check_Slope_{column}_{row}";
            label.Size = new System.Drawing.Size(84, 32);
            label.TabIndex = 21;
            label.Text = $"{column}_{row}";
            label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            tableLayoutPanel.Controls.Add(label, column, row);
        }

        private void CreateDynamicElement()
        {
            for (int i = 0; i < count; i++)
            {
                // [LCM] 
                CreateLabel(ref Labl_CheckSlopeResult_LCM_R[i], LyPnl_LCM_CheckSlope, 1, i + 1);
                CreateLabel(ref Labl_CheckSlopeResult_LCM_R_Dev[i], LyPnl_LCM_CheckSlope, 2, i + 1);

                CreateLabel(ref Labl_CheckSlopeResult_LCM_G[i], LyPnl_LCM_CheckSlope, 3, i + 1);
                CreateLabel(ref Labl_CheckSlopeResult_LCM_G_Dev[i], LyPnl_LCM_CheckSlope, 4, i + 1);

                CreateLabel(ref Labl_CheckSlopeResult_LCM_B[i], LyPnl_LCM_CheckSlope, 5, i + 1);
                CreateLabel(ref Labl_CheckSlopeResult_LCM_B_Dev[i], LyPnl_LCM_CheckSlope, 6, i + 1);
                // {HCM]
                CreateLabel(ref Labl_CheckSlopeResult_HCM_R[i], LyPnl_HCM_CheckSlope, 1, i + 1);
                CreateLabel(ref Labl_CheckSlopeResult_HCM_R_Dev[i], LyPnl_HCM_CheckSlope, 2, i + 1);

                CreateLabel(ref Labl_CheckSlopeResult_HCM_G[i], LyPnl_HCM_CheckSlope, 3, i + 1);
                CreateLabel(ref Labl_CheckSlopeResult_HCM_G_Dev[i], LyPnl_HCM_CheckSlope, 4, i + 1);
                
                CreateLabel(ref Labl_CheckSlopeResult_HCM_B[i], LyPnl_HCM_CheckSlope, 5, i + 1);
                CreateLabel(ref Labl_CheckSlopeResult_HCM_B_Dev[i], LyPnl_HCM_CheckSlope, 6, i + 1);
            }
        }
        private void ShowSlopeCheckData(double[] r_current_LCM, double[] r_dev_LCM, double[] g_current_LCM, double[] g_dev_LCM,
                                       double[] b_current_LCM, double[] b_dev_LCM, double[] r_current_HCM, double[] r_dev_HCM,
                                       double[] g_current_HCM, double[] g_dev_HCM, double[] b_current_HCM, double[] b_dev_HCM)
        {
            for (int i = 0; i < count; i++)
            {
                Labl_CheckSlopeResult_LCM_R[i].Text = r_current_LCM[i].ToString("F3");
                Labl_CheckSlopeResult_LCM_R_Dev[i].Text = r_dev_LCM[i].ToString("F3");
                Labl_CheckSlopeResult_LCM_G[i].Text = g_current_LCM[i].ToString("F3");
                Labl_CheckSlopeResult_LCM_G_Dev[i].Text = g_dev_LCM[i].ToString("F3");
                Labl_CheckSlopeResult_LCM_B[i].Text = b_current_LCM[i].ToString("F3");
                Labl_CheckSlopeResult_LCM_B_Dev[i].Text = b_dev_LCM[i].ToString("F3");
                Labl_CheckSlopeResult_HCM_R[i].Text = r_current_HCM[i].ToString("F3");
                Labl_CheckSlopeResult_HCM_R_Dev[i].Text = r_dev_HCM[i].ToString("F3");
                Labl_CheckSlopeResult_HCM_G[i].Text = g_current_HCM[i].ToString("F3");
                Labl_CheckSlopeResult_HCM_G_Dev[i].Text = g_dev_HCM[i].ToString("F3");
                Labl_CheckSlopeResult_HCM_B[i].Text = b_current_HCM[i].ToString("F3");
                Labl_CheckSlopeResult_HCM_B_Dev[i].Text = b_dev_HCM[i].ToString("F3");
            }
        }
        #endregion

        #region public function
        public void ShowSlopeCheckDataInvoke(double[] r_current_LCM, double[] r_dev_LCM, double[] g_current_LCM, double[] g_dev_LCM,
                                           double[] b_current_LCM, double[] b_dev_LCM, double[] r_current_HCM, double[] r_dev_HCM,
                                           double[] g_current_HCM, double[] g_dev_HCM, double[] b_current_HCM, double[] b_dev_HCM)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowSlopeCheckData(r_current_LCM, r_dev_LCM, g_current_LCM, g_dev_LCM,
                                                                b_current_LCM, b_dev_LCM, r_current_HCM, r_dev_HCM,
                                                                g_current_HCM, g_dev_HCM, b_current_HCM, b_dev_HCM)));
            }
            else
            {
                ShowSlopeCheckData(r_current_LCM, r_dev_LCM, g_current_LCM, g_dev_LCM,
                                    b_current_LCM, b_dev_LCM, r_current_HCM, r_dev_HCM,
                                    g_current_HCM, g_dev_HCM, b_current_HCM, b_dev_HCM);
            }
        }

        #endregion

        private void F_Equipment_Setting_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                SaveAllEnumSetting();
                ReadAllEnumSetting();

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
    }
}
