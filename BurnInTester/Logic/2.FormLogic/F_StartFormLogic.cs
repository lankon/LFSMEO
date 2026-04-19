using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using DeviceCore;
using BurnInTester.Base;

namespace BurnInTester.Logic
{
    public class F_StartFormLogic
    {
        public F_StartFormLogic(IBurnInTesterMachine burnInTesterMachine , IServiceProvider serviceProvider,
                                AgingInformation agingInformation)
        {
            BurnInTesterMachine = burnInTesterMachine;
            ServiceProvider = serviceProvider;
            AgingInformation = agingInformation;
        }

        #region parameter define
        private int CurBoxNum = 1;
        private AgingInformation AgingInformation;
        private IBurnInTesterMachine BurnInTesterMachine;
        private IServiceProvider ServiceProvider;
        private eF_StartForm[] AgingParam = new eF_StartForm[]
        {
            //新增老化參數時需添加
            
            // [Product Info]
            eF_StartForm.TxtBx_ProductType,
            eF_StartForm.TxtBx_SerialNumber,

            // [Status]
            eF_StartForm.TxtBx_RunnningTime,
            eF_StartForm.TxtBx_RemainingTime,
        };
        #endregion

        #region private function
        #endregion


        public int GetCurBoxNum()
        {
            return CurBoxNum;
        }
        public void SetCurBoxNum(int box_num)
        {
            CurBoxNum = box_num;
        }
        public void SaveAgingParam()
        {
            int num = GetCurBoxNum();
            string set;

            Dictionary<string, string> param = new Dictionary<string, string>();
            for (int i = 0; i < AgingParam.Length; i++)
            {
                set = ApplicationSetting.Get_String_Recipe<eF_StartForm>((int)AgingParam[i]);
                param.Add(AgingParam[i].ToString(), set);
            }
            AgingInformation.SaveBoxConfig(AgingInformation.AgingConfigFilePath, $"Box{num}", param);
        }
        public void UpdateAgingParam()
        {
            AgingInformation.LoadBoxConfig();
            var param = AgingInformation.GetBoxConfig();
            int num = GetCurBoxNum();

            // [Product Info]
            ApplicationSetting.SetRecipe<eF_StartForm>((int)eF_StartForm.TxtBx_ProductType, param[num].ProductType);
            ApplicationSetting.SetRecipe<eF_StartForm>((int)eF_StartForm.TxtBx_SerialNumber, param[num].SerialNumber);

            // [Status]
            ApplicationSetting.SetRecipe<eF_StartForm>((int)eF_StartForm.TxtBx_RunnningTime, param[num].RunnningTime);
            ApplicationSetting.SetRecipe<eF_StartForm>((int)eF_StartForm.TxtBx_RemainingTime, param[num].RemainingTime);
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

    }
}
