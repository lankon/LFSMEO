using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using ToolFunction;

namespace LFSMEO
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            GetMachineType();
            LFSMEO_Assemble assemble = new LFSMEO_Assemble();
            Form main_form = assemble.BuildAndGetMainForm();

            Application.Run(main_form);
        }

        static EMachineType GetMachineType()
        {
            ApplicationSetting.ReadAllRecipe<eMachineSetting>();
            int option = ApplicationSetting.Get_Int_Recipe<eMachineSetting>((int)eMachineSetting.Cmbx_MachineType);

            if (option == 0)
                Scope.MachineType = EMachineType.NONE;
            else if (option == 1)
                Scope.MachineType = EMachineType.VPT_3IN1;

            return Scope.MachineType;
        }
    }
}
