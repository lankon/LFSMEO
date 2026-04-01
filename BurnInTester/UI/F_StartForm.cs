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


using ToolFunction;

namespace BurnInTester.UI
{
    public partial class F_StartForm : Form
    {
        public F_StartForm()
        {
            InitializeComponent();

            InitialForm();
        }

        #region parameter define
        #endregion

        #region private function
        private void InitialForm()
        {
            ReadAllEnumSetting();
            UpdateEnumSettingToForm();

            ShowHint();

            //if (ApplicationSetting.Get_Int_Recipe<eOEMSetting>((int)eOEMSetting.Cmbx_ShowFormName) == 1)
            //    Tool.ShowFormName(this);
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
            ResourceMonitor.GetStats();
        }
    }

    public static class ResourceMonitor
    {
        // 引入 Win32 API
        [DllImport("user32.dll")]
        private static extern uint GetGuiResources(IntPtr hProcess, uint uiFlags);

        public static void GetStats()
        {
            IntPtr hProcess = Process.GetCurrentProcess().Handle;

            // uiFlags: 0 = GDI Objects (畫筆、字體、圖片)
            // uiFlags: 1 = User Objects (視窗、控制項、選單)
            uint gdiObjects = GetGuiResources(hProcess, 0);
            uint userObjects = GetGuiResources(hProcess, 1);
            int totalHandles = Process.GetCurrentProcess().HandleCount;

            Console.WriteLine($"[資源監控]");
            Console.WriteLine($"- 使用者物件 (User): {userObjects} / 10000 (上限)");
            Console.WriteLine($"- GDI 物件: {gdiObjects} / 10000 (上限)");
            Console.WriteLine($"- 系統句柄 (Handles): {totalHandles}");
        }
    }
}
