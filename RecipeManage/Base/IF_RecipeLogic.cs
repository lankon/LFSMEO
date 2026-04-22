using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManage.Base
{
    public interface IF_RecipeLogic
    {
        void AddEnumType(Type enumType);
        bool CheckRecipeExist(string recipe_name);
        bool SaveRecipe(string recipe_name);
        bool ReadRecipe(string recipe_name);
        string[] LoadRecipeFolderName();
        void DeleteRecipeFolder(string recipe_name);
        void CopyRecipeFolder(string sourceFolder, string targetFolder);
    }
}
