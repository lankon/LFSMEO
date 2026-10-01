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
        public F_OpticalTestLogic(IBaseMainTask baseMainTask, RGBTesterFunction rGBFunc)
        {
            MainTask = baseMainTask;
            RGBFunc = rGBFunc;
        }

        #region parameter define
        IBaseMainTask MainTask;
        RGBTesterFunction RGBFunc;
        #endregion

        #region public function
        public void StartOpticalTest()
        {
            int side = ApplicationSetting.Get_Int_Recipe<eF_OpticalTest>((int)eF_OpticalTest.Cmbx_TestMode);
            string test_side = "";

            if (side == (int)eTestMode.LEFT)
            {
                RGBFunc.SerialNumber = ApplicationSetting.Get_String_Recipe<eF_OpticalTest>((int)eF_OpticalTest.TxtBx_Left_SN);
                test_side = "Left";
            }
            else if (side == (int)eTestMode.RIGHT)
            {
                RGBFunc.SerialNumber = ApplicationSetting.Get_String_Recipe<eF_OpticalTest>((int)eF_OpticalTest.TxtBx_Right_SN);
                test_side = "Right";
            }

            RGBFunc.SetFunctionTestProcess(false);

            MainTask.SetTask<TaskOpticalTest>(test_side);
            MainTask.Run();
        }

        #endregion



    }
}
