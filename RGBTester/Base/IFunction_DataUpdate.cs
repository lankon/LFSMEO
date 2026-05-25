using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public interface IFunction_DataUpdate
    {
        bool CheckConnectStatus(string command = "");
        bool DataUpdate();
    }
    
}
