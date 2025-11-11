using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeviceCore;

namespace RGBTester.Logic
{
    public class F_StartFormLogic
    {
        public F_StartFormLogic(IChillerControl chillerControl)
        {
            Chiller = chillerControl;
        }

        #region parameter define
        IChillerControl Chiller;
        #endregion

        #region public function
        public void OpenChillerComm()
        {
            //Chiller.Open();
        }

        public void ChillerStart()
        {
            Chiller.StartChiller();
        }
        #endregion
    }
}
