using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using DeviceCore;
using BurnInTester.Base;

namespace BurnInTester.Logic
{
    public class F_MainFormLogic
    {
        public F_MainFormLogic(IBurnInTesterMachine burnInTesterMachine , IServiceProvider serviceProvider)
        {
            BurnInTesterMachine = burnInTesterMachine;
            ServiceProvider = serviceProvider;
        }

        #region parameter define
        IBurnInTesterMachine BurnInTesterMachine;
        IServiceProvider ServiceProvider;
        #endregion

        #region private function
        private void Initial_IO_Function()
        {
            ServiceProvider.GetRequiredService<IF_IO_Card>();

            BurnInTesterMachine.DIOL.Initial_All_IO();
        }
        private void Initial_Motion_Function()
        {
            //Tool.SaveLogToFile("Initial Motion Card");
            //ProbeTesterMachine.DML.Initial_All_Motion();
            //Tool.SaveLogToFile("Load Motion Config");
            //ProbeTesterMachine.DML.LoadAxisConfig();
            //ProbeTesterMachine.DML.BindingAxis();
        }
        private void Initial_Light_Function()
        {
            //IF_LightControl f_light = ServiceProvider.GetRequiredService<IF_LightControl>();
            //f_light.Update_Light_List();
            //ProbeTesterMachine.Light.Initial_All_LightControl();
        }
        private void Initial_Camera_Function()
        {
            //Tool.SaveLogToFile("Initial Camera");
            //ProbeTesterMachine.CCD.Initial_All_Camera();
            //Tool.SaveLogToFile("Load Camera Config");
            //ProbeTesterMachine.CCD.LoadCameraConfig();
            //ProbeTesterMachine.CCD.BindingCamera();
            //IF_CameraSetting f_CameraSetting = ServiceProvider.GetRequiredService<IF_CameraSetting>();
            //f_CameraSetting.BindingDisplayEvent();
        }
        #endregion

        public void Initial_All_Device()
        {
            Initial_IO_Function();
            Initial_Motion_Function();
            Initial_Light_Function();
            Initial_Camera_Function();
        }

        public void ReadAllSetting()
        {
            //ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();
            //ApplicationSetting.ReadAllRecipe<eF_Recipe>();
            //ApplicationSetting.ReadAllRecipe<eF_ParameterSetting>();

            //Tool.SaveLogToFile("Load Recipe File");
            //var recipe = ServiceProvider.GetRequiredService<F_RecipeLogic>();
            //string cur_recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
            //recipe.ReadRecipe(cur_recipe_name);
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
    }
}
