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

            CatchException();
            GetMachineType();
            LFSMEO_Assemble assemble = new LFSMEO_Assemble();
            Form main_form = assemble.BuildAndGetMainForm();

            Application.Run(main_form);
        }

        static void CatchException()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                Tool.SaveExceptionToFile(e.Exception, "UI Thread Unhandled Exception");
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Tool.SaveExceptionToFile(e.ExceptionObject as Exception, "Unhandled Exception");
            };
        }

        static EMachineType GetMachineType()
        {
            ApplicationSetting.ReadAllRecipe<eMachineSetting>();
            int option = ApplicationSetting.Get_Int_Recipe<eMachineSetting>((int)eMachineSetting.Cmbx_MachineType);

            //[新增機型]
            if (option == 0)
                Scope.MachineType = EMachineType.NONE;
            else if (option == 1)
                Scope.MachineType = EMachineType.ProbeTester;
            else if(option == 2)
                Scope.MachineType = EMachineType.RGBTester;
            else if(option == 3)
                Scope.MachineType = EMachineType.BurnInTester;
            else if(option == 4)
                Scope.MachineType = EMachineType.AAMachine;

            return Scope.MachineType;
        }
    }
}
