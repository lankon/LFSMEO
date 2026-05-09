using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RGBTester.Base;
using RGBTester.Base.F_OpticalTest;
using ToolFunction;

namespace RGBTester.Logic
{
    public class F_OpticalTestLogic
    {
        public F_OpticalTestLogic(IBaseMainTask baseMainTask)
        {
            MainTask = baseMainTask;
        }

        IBaseMainTask MainTask;

        #region public function
        public void StartOpticalTest()
        {
            int side = ApplicationSetting.Get_Int_Recipe<eF_OpticalTest>((int)eF_OpticalTest.Cmbx_TestMode);
            string test_side = "";

            if (side == (int)eTestMode.LEFT)
                test_side = "Left";
            else if (side == (int)eTestMode.RIGHT)
                test_side = "Right";
            else
                test_side = "Both";

            MainTask.SetTask<TaskOpticalTest>(test_side);
            MainTask.Run();
        }

        #endregion



    }
}
