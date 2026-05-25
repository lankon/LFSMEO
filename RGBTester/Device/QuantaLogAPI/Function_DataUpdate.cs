using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RGBTester.Base;

namespace RGBTester.Device
{
    public class Function_DataUpdate : IFunction_DataUpdate
    {
        public Function_DataUpdate()
        {

        }

        #region parameter define
        QuantaAPI Quanta = new QuantaAPI();
        #endregion

        #region public function
        public bool CheckConnectStatus(string command = "")
        {
            int res = Quanta.CheckRoutingSMT(command);
            return res == 0;
        }

        public bool DataUpdate()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
