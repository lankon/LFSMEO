using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LFSMEO
{
    public  static partial class Scope
    {
        public static Panel MainPanel;          //F_MainForm:Pnl_Group
        public static Panel UpButtonPanel;      //F_MainForm:Pnl_Group1
        
        public static string LFSMEO_StartupPath = Application.StartupPath;

        //[Machine Parameter]
        public static EMachineType MachineType = EMachineType.NONE;
    }

    //***************** Static Form *************
    //public static partial class Scope
    //{
    //    public static F_StartForm f_StartForm = new F_StartForm();
    //}
}
