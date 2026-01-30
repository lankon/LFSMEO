using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using DeviceCore;
using RGBTester.Base;
using System.Threading;
using System.IO;

namespace RGBTester.Logic
{
    public class F_RecipeLogic
    {
        public F_RecipeLogic(IRGBTesterMachine machine, IServiceProvider serviceProvider)
        {
            Machine = machine;
            ServiceProvider = serviceProvider;
        }

        #region parameter define
        IRGBTesterMachine Machine;
        IServiceProvider ServiceProvider;
        List<Type> enumTypes = new List<Type>
        {
            typeof(eF_StartFormRecipe),
            typeof(eF_ParameterSettingRecipe),
        };
        #endregion

        #region public function
        public bool CheckRecipeExist(string recipe_name)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + $"Setting\\Package\\{recipe_name}";

            if (Directory.Exists(path))
                return true;
            else
                return false;
        }
        public bool SaveRecipe(string recipe_name)
        {
            foreach (var enumType in enumTypes)
            {
                var method = typeof(ApplicationSetting)
                    .GetMethod("SaveAllRecipe")
                    .MakeGenericMethod(enumType);

                method.Invoke(null, new object[] { recipe_name });
            }

            Tool.SaveLogToFile($"Save {recipe_name} Recipe", level: "INF");
            return true;
        }
        public bool ReadRecipe(string recipe_name)
        {
            //判斷Recipe是否存在
            string[] name = LoadRecipeFolderName();

            for(int i =0; i<name.Length; i++)
            {
                if (name[i] == recipe_name)
                    break;

                if (i == name.Length - 1)
                    return false;
            }

            //讀取Recipe
            foreach (var enumType in enumTypes)
            {
                var method = typeof(ApplicationSetting)
                    .GetMethod("ReadAllRecipe")
                    .MakeGenericMethod(enumType);

                object res = method.Invoke(null, new object[] { recipe_name, "Reread" });
            }

            ApplicationSetting.SetRecipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName, recipe_name);
            ApplicationSetting.SaveAllRecipe<eF_Recipe>();
            ApplicationSetting.ReadAllRecipe<eF_Recipe>();

            return true;
        }
        public string[] LoadRecipeFolderName()
        {
            string folderPath = AppDomain.CurrentDomain.BaseDirectory + @"Setting\Package";
            string[] subDirs = Directory.GetDirectories(folderPath);
            string[] folderName = new string[subDirs.Length];

            for(int i=0; i<folderName.Length; i++)
            {
                string name = Path.GetFileName(subDirs[i]);
                folderName[i] = name;
            }

            return folderName;
        }
        public void DeleteRecipeFolder(string recipe_name)
        {
            string folderPath = AppDomain.CurrentDomain.BaseDirectory + $@"Setting\Package\{recipe_name}";

            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
        }
        public void CopyRecipeFolder(string sourceFolder, string targetFolder)
        {
            targetFolder = AppDomain.CurrentDomain.BaseDirectory + $@"Setting\Package\{targetFolder}";
            sourceFolder = AppDomain.CurrentDomain.BaseDirectory + $@"Setting\Package\{sourceFolder}";

            // 如果目的地資料夾不存在就建立
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            // 複製所有檔案
            foreach (var file in Directory.GetFiles(sourceFolder))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetFolder, fileName);
                File.Copy(file, destFile, true);
            }

            // 複製所有子資料夾（遞迴）
            foreach (var folder in Directory.GetDirectories(sourceFolder))
            {
                string folderName = Path.GetFileName(folder);
                string destFolder = Path.Combine(targetFolder, folderName);
                CopyRecipeFolder(folder, destFolder);
            }
        }
        #endregion

        #region private function

        #endregion
    }
}
