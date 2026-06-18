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

namespace RGBTester.UI
{
    public partial class F_StartForm_ButtonGroup : Form
    {
        public F_StartForm_ButtonGroup(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            InitialForm();

            ServiceProvider = serviceProvider;
        }

        #region parameter define
        List<Panel> PnlPartList = new List<Panel>();
        private int curPnlPart = 0;
        private IServiceProvider ServiceProvider;
        #endregion

        #region private function
        private void SetHint()
        {
            toolTip1.SetToolTip(Btn_OEM_Setting, "OEM Setting");
            toolTip1.SetToolTip(Btn_ParameterSetting, "Parameter Setting");
            toolTip1.SetToolTip(Btn_LogIn, "LogIn");
            toolTip1.SetToolTip(Btn_Recipe, "Recipe");
            toolTip1.SetToolTip(Btn_YieldReport, "Yield Report");
            toolTip1.SetToolTip(Btn_HistoryLog, "History Log");
        }
        private void InitialForm()
        {
            SetPnlPartPos(Pnl_Part1);

            SetHint();

            if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                Tool.ShowFormName(this);

            PnlPartList.Add(Pnl_Part1);
            PnlPartList.Add(Pnl_Part2);
        }
        private void SetPnlPartPos(Panel pnl)   //應該可以移到ToolFunction
        {
            pnl.Location = new Point(0, 0);
            pnl.BringToFront();
        }
        private int NextPnlPart(List<Panel> list, int index)
        {
            index = index + 1;

            if (index >= list.Count)
                index = 0;

            SetPnlPartPos(list[index]);

            return index;
        }
        private int PreviousPnlPart(List<Panel> list, int index)
        {
            index = index - 1;

            if (index < 0)
                index = 0;

            SetPnlPartPos(list[index]);

            return index;
        }
        private void UpdatePage()
        {
            var UserPrivilege = ServiceProvider.GetRequiredService<IF_UserPrivilegeLogic>();

            bool oem = UserPrivilege.AtLeastOEM();
            bool eng = UserPrivilege.AtLeastEng();

            Btn_DAQ_Chart.Visible = eng;
            Btn_OEM_Setting.Enabled = oem;
            Btn_ParameterSetting.Enabled = eng;
        }
        #endregion

        private void Btn_PreviousPnlPart_Click(object sender, EventArgs e)
        {
            curPnlPart = PreviousPnlPart(PnlPartList, curPnlPart);
        }

        private void Btn_NextPnlPart_Click(object sender, EventArgs e)
        {
            curPnlPart = NextPnlPart(PnlPartList, curPnlPart);
        }

        private void Btn_OEM_Setting_Click(object sender, EventArgs e)
        {
            var oem_set = ServiceProvider.GetRequiredService<F_OEM_Setting>();

            Tool.HideElementOnPanel(Scope.MainPanel);
            Tool.SetForm(Scope.MainPanel, oem_set);
            oem_set.Show();
        }

        private void Btn_ParameterSetting_Click(object sender, EventArgs e)
        {
            var eng_set = ServiceProvider.GetRequiredService<F_EngineerSetting>();

            if(eng_set is Form form)
            {
                Tool.HideElementOnPanel(Scope.MainPanel);
                Tool.SetForm(Scope.MainPanel, form);
                form.Show();
            }
        }

        private void Btn_LogIn_Click(object sender, EventArgs e)
        {
            var set = ServiceProvider.GetRequiredService<IF_UserPrivilege>();

            if(set is Form form)
            {
                Tool.HideElementOnPanel(Scope.MainPanel);
                Tool.HideElementOnPanel(Scope.UpButtonPanel);
                Tool.SetForm(Scope.MainPanel, form);
                form.Show();

                if (ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ShowFormName) == 1)
                    set.ShowFormName(true);
            }
        }

        private void F_StartForm_ButtonGroup_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                ////儲存參數
                //ApplicationSetting.SaveRecipeFromForm<eF_Equipment_Setting>(this);
                //ApplicationSetting.SaveRecipeFromForm<eMachineSetting>(this);
                ////重新讀取變數值
                //ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();
                //ApplicationSetting.ReadAllRecipe<eMachineSetting>();

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

        private void Btn_Recipe_Click(object sender, EventArgs e)
        {
            var recipe = ServiceProvider.GetRequiredService<F_Recipe>();

            Tool.HideElementOnPanel(Scope.MainPanel);
            Tool.SetForm(Scope.MainPanel, recipe);
            recipe.Show();
        }

        private void Btn_DAQ_Chart_Click(object sender, EventArgs e)
        {
            var chart = ServiceProvider.GetRequiredService<F_DAQ_Chart>();

            Tool.HideElementOnPanel(Scope.MainPanel);
            Tool.SetForm(Scope.MainPanel, chart);
            chart.Show();
        }

        private void Btn_YieldReport_Click(object sender, EventArgs e)
        {
            var yield = ServiceProvider.GetRequiredService<F_YieldReport>();

            Tool.HideElementOnPanel(Scope.MainPanel);
            Tool.SetForm(Scope.MainPanel, yield);
            yield.Show();
        }

        private void Btn_HistoryLog_Click(object sender, EventArgs e)
        {
            var historyLog = ServiceProvider.GetRequiredService<F_HistoryLog>();

            Tool.HideElementOnPanel(Scope.MainPanel);
            Tool.SetForm(Scope.MainPanel, historyLog);
            historyLog.Show();
        }
    }
}
