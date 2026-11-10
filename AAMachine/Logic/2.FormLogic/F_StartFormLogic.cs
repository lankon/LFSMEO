using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.IO;
using Microsoft.Extensions.DependencyInjection;


using ToolFunction;
using DeviceCore;


namespace AAMachine.Logic
{
    public class F_StartFormLogic
    {
        public F_StartFormLogic(IAAMachineMachine machine, IServiceProvider serviceProvider)
        {
            Machine = machine;
            ServiceProvider = serviceProvider;
        }

        #region parameter define
        IAAMachineMachine Machine;
        IServiceProvider ServiceProvider;
        #endregion

        #region public function
        
        public void SetVirtual_IO_Rule()
        {
            //Machine.DIOL.AddIORule(EIOName.CCD_FiducialMaskWork, true, (EIOName.CCD_FiducialMaskWork_Sensor, true));
            //Machine.DIOL.AddIORule(EIOName.CCD_FiducialMaskWork, false, (EIOName.CCD_FiducialMaskWork_Sensor, false));
            //Machine.DIOL.AddIORule(EIOName.CCD_FiducialMaskIdle, true, (EIOName.CCD_FiducialMaskIdle_Sensor, true));
            //Machine.DIOL.AddIORule(EIOName.CCD_FiducialMaskIdle, false, (EIOName.CCD_FiducialMaskIdle_Sensor, false));
        }
        #endregion

        #region private function
        #endregion
    }
}
