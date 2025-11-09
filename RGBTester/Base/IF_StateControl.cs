using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public interface IF_StateControl
    {
        void SetMainTask(IBaseMainTask baseMainTask);
        void UpdateTask(string msg);
        void SetPauseAbortContinue(TASK_STATUS status);
        void ShowForm();
        void HideForm();
        void CloseForm();
        void GotoPause();
    }
}
