using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DETester.Base
{
    public interface IBaseMainTask
    {
        void SetTask<T>(string method = "Default") where T : IF_BaseTask;
        void Run();
        void GoToPause();
        void GoToAbort();
        void GoToContinue();
    }
    
}
