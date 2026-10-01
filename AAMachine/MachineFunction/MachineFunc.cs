using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using AAMachine.Base;
using AAMachine.Base.Equipment_Setting;
using ToolFunction;

namespace AAMachine.MachineFunction
{
    public partial class MachineFunc
    {
        public MachineFunc(IBaseTaskDependence dependencies) 
        {
            Deps = dependencies;
            InitialObjects();
        }

        #region parameter define
        public AxisDefine Axis { get; private set; } = new AxisDefine();
        public IBaseTaskDependence Deps;
        #endregion

        #region private function
        private void InitialObjects()
        {
            Axis.Initialize(this);
        }
        #endregion

        #region public function
        public eModuleType GetModuleType()
        {
            eModuleType moduleType = (eModuleType)ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ModuleType);
        
            return moduleType;
        }
        #endregion
    }
}
