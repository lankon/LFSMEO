using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ToolFunction;

namespace LFSMEO
{
    public partial class F_SelectMachine : Form
    {

        #region private function
        void InitialForm()
        {
            ApplicationSetting.ReadAllRecipe<eMachineSetting>();
            ApplicationSetting.UpdataRecipeToForm<eMachineSetting>(this);
        }
        #endregion

        public F_SelectMachine()
        {
            InitializeComponent();

            InitialForm();
        }

        private void F_SelectMachine_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplicationSetting.SaveRecipeFromForm<eMachineSetting>(this);
        }
    }
}
