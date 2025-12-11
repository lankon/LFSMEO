using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolFunction;

using RGBTester.Base;
using DeviceCore;
using Microsoft.Extensions.DependencyInjection;

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

        public void ReadAllSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();
            ApplicationSetting.ReadAllRecipe<eF_Recipe>();

            Tool.SaveLogToFile("Load Recipe File", level: "INF");
            var recipe = ServiceProvider.GetRequiredService<F_RecipeLogic>();
            string cur_recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
            recipe.ReadRecipe(cur_recipe_name);
        }

        public void Initial_IO_Function()
        {
            ServiceProvider.GetRequiredService<IF_IO_Card>();

            RGBTesterMachine.DIOL.Initial_All_IO();
        }

        public int DeleteExpireFileInFolder()
        {
            string app_path = AppDomain.CurrentDomain.BaseDirectory;

            string delete_foler = app_path + "\\History";

            Tool.DeleteExpireFiles(delete_foler, 90);

            return 0;
        }

        

        public void SetForm(IF_MainForm f_MainForm)
        {
            MainForm = f_MainForm;
        }
    }
}
