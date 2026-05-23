using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using RGBTester.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

            //設定扣除背景值係數
            double slope = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_BackgroundGain);
            double offset = ApplicationSetting.Get_Double_Recipe<eF_OpticalSetting>((int)eF_OpticalSetting.TxtBx_BackgroundFOffset);
            RGBTesterMachine.Spectrometer.SetBackgroundCoef(slope, offset);
        }
        private void ShowModuleForm<T>() where T : class
        {
            var startForm = ServiceProvider.GetRequiredService<T>();
            if (startForm is Form form)
            {
                Tool.SetForm(Scope.MainPanel, form);
                form.Show();
            }
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
            ApplicationSetting.ReadAllRecipe<eF_OpticalTest>();
            ApplicationSetting.ReadAllRecipe<eF_StartForm>();
            ApplicationSetting.ReadAllRecipe<eF_OpticalSetting>();


            Tool.SaveLogToFile("Load Recipe File");
            var recipe = ServiceProvider.GetRequiredService<F_RecipeLogic>();
            string cur_recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
            recipe.ReadRecipe(cur_recipe_name);

            //[Read Recipe]
            ApplicationSetting.ReadAllRecipe<eF_OpticalTestRecipe>(cur_recipe_name);

        }

        public void ShowStartForm()
        {
            Tool.HideElementOnPanel(Scope.MainPanel);

            RGBTesterFunction func = ServiceProvider.GetRequiredService<RGBTesterFunction>();

            if(func.GetModuleType() == eModuleType.IV_Calibration)
                ShowModuleForm<IF_StartForm>();
            else if(func.GetModuleType() == eModuleType.Function_Test)
                ShowModuleForm<F_FunctionTester>();

            var group = ServiceProvider.GetRequiredService<F_StartForm_ButtonGroup>();
            Tool.SetForm(Scope.UpButtonPanel, group);
            group.Show();
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
