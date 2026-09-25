using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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
            SetCulture setCulture = new SetCulture();

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

            if (option == 0)
                Scope.MachineType = EMachineType.NONE;
            else if (option == 1)
                Scope.MachineType = EMachineType.ProbeTester;
            else if(option == 2)
                Scope.MachineType = EMachineType.RGBTester;
            else if(option == 3)
                Scope.MachineType = EMachineType.BurnInTester;

            return Scope.MachineType;
        }
    }

    public class SetCulture
    {
        public SetCulture()
        {
            SetCultureInfo("");
        }

        public void SetCultureInfo(string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
