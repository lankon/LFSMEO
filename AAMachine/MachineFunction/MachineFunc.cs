using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using AAMachine.Base;

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
            Axis.Initial(Deps);
        }
        #endregion
    }
}
