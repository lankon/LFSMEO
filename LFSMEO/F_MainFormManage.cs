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
using LFSMEO.Base_LFSMEO;

namespace LFSMEO
{
    public class F_MainFormManage
    {
        IF_StartForm f_StartForm;
        IF_StartForm_ButtonGroup f_StartForm_ButtonGroup;

        public void InitialMachine()
        {
            if (ApplicationSetting.Get_Int_Recipe<eOEMSetting>((int)eOEMSetting.Cmbx_MachineType) == 1)
                Scope.MachineType = EMachineType.VPT_3IN1;
            else
                Scope.MachineType = EMachineType.NONE;
        }

        public void CreateStartForm()
        {
            if(Scope.MachineType == EMachineType.VPT_3IN1)
            {
                f_StartForm_ButtonGroup = new F_StartForm_ButtonGroup();
                f_StartForm = new F_StartForm();
            }
        }

        public void ShowStartForm()
        {
            Tool.HideElementOnPanel(Scope.MainPanel);

            if (Scope.MachineType == EMachineType.NONE)
            {
                F_Equipment_Setting F_StartForm_ButtonGroup = new F_Equipment_Setting();
                Tool.SetForm(Scope.MainPanel, F_StartForm_ButtonGroup);
                F_StartForm_ButtonGroup.Show();
                return;
            }

            if (f_StartForm is Form form)
            {
                Tool.SetForm(Scope.MainPanel, form);
                form.Show();
            }

            if (f_StartForm_ButtonGroup is Form form_btn)
            {
                Tool.SetForm(Scope.UpButtonPanel, form_btn);
                form_btn.Show();
            }
        }

        public void SaveRecipeWhenCloseApp()
        {
            Tool.HideElementOnPanel(Scope.MainPanel);

            ApplicationSetting.ReadAllRecipe<eOEMSetting>();
            ApplicationSetting.SaveAllRecipe<eOEMSetting>();
        }

    }
}
