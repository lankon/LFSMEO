using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToolFunction;

namespace RGBTester.Logic
{
    public class F_MainFormLogic
    {
        public F_MainFormLogic(IRGBTesterMachine rGBTesterMachine , IServiceProvider serviceProvider)
        {
            RGBTesterMachine = rGBTesterMachine;
            ServiceProvider = serviceProvider;
        }

        #region parameter define
        IF_MainForm MainForm;
        IBaseMainTask MainTask;
        IRGBTesterMachine RGBTesterMachine;
        IServiceProvider ServiceProvider;
        #endregion

        #region private function
        private void Initial_IO_Function()
        {
            ServiceProvider.GetRequiredService<IF_IO_Card>();

            RGBTesterMachine.DIOL.Initial_All_IO();
        }
        private void Initial_Motion_Function()
        {
            Tool.SaveLogToFile("Initial Motion Card");
            RGBTesterMachine.DML.Initial_All_Motion();
            Tool.SaveLogToFile("Load Motion Config");
            RGBTesterMachine.DML.LoadAxisConfig();
            RGBTesterMachine.DML.BindingAxis();
        }
        private void Initial_Light_Function()
        {
            IF_LightControl f_light = ServiceProvider.GetRequiredService<IF_LightControl>();
            f_light.Update_Light_List();
            RGBTesterMachine.Light.Initial_All_LightControl();
        }
        private void Initial_Spectrometer_Function()
        {
            IF_Spectrometer f_spectrometer = ServiceProvider.GetRequiredService<IF_Spectrometer>();
            f_spectrometer.Update_Spectrum_List();
            RGBTesterMachine.Spectrometer.Initial_All_Spectrometer();
        }
        #endregion

        public void Initial_AllDevice_Function()
        {
            Initial_IO_Function();
            Initial_Motion_Function();
            Initial_Light_Function();
            Initial_Spectrometer_Function();
        }

        public void ReadAllSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();
            ApplicationSetting.ReadAllRecipe<eF_Recipe>();
            ApplicationSetting.ReadAllRecipe<eF_ParameterSetting>();

            Tool.SaveLogToFile("Load Recipe File");
            var recipe = ServiceProvider.GetRequiredService<F_RecipeLogic>();
            string cur_recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
            recipe.ReadRecipe(cur_recipe_name);
        }

        public int DeleteExpireFileInFolder()
        {
            string app_path = AppDomain.CurrentDomain.BaseDirectory;

            string delete_folder = app_path + "\\History";

            Tool.DeleteExpireFiles(delete_folder, 90);

            return 0;
        }

        public string GetVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            Tool.SaveLogToFile("軟體版本:" + fvi.FileVersion);

            return "Version:" + fvi.FileVersion;
        }

        public void SetForm(IF_MainForm f_MainForm)
        {
            MainForm = f_MainForm;
        }
    }
}
