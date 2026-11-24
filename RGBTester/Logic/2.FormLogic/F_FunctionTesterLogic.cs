using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

using ToolFunction;
using DeviceCore;
using RGBTester.Base;
using RGBTester.Base.UploadDataSetting;
using RGBTester.Base.F_OpticalTest;

namespace RGBTester.Logic
{
    public class F_FunctionTesterLogic
    {
        public F_FunctionTesterLogic(IRGBTesterMachine rGBTesterMachine, IServiceProvider serviceProvider, IF_StatusBox f_StatusBox,
                                     F_StartFormLogic f_StartFormLogic, RGBTesterFunction rGBTesterFunction)
        {
            RGBTesterMachine = rGBTesterMachine;
            ServiceProvider = serviceProvider;
            StartFormLogic = f_StartFormLogic;
            RGBfunc = rGBTesterFunction;
            StatusBox = f_StatusBox;
        }

        #region parameter define
        private IRGBTesterMachine RGBTesterMachine;
        private IServiceProvider ServiceProvider;
        private IF_StatusBox StatusBox;
        private RGBTesterFunction RGBfunc;
        private F_StartFormLogic StartFormLogic;
        #endregion

        #region private function
        private int UploadSetting()
        {
            IFunction_DataUpload data_upload = ServiceProvider.GetRequiredService<IFunction_DataUpload>();
            UploadInfo info = new UploadInfo
            {
                OperatorID = ApplicationSetting.Get_String_Recipe<eF_FunctionTester>((int)eF_FunctionTester.TxtBx_OperatorID),
                SerialNunber = RGBfunc.SerialNumber,

                FixtureID = ApplicationSetting.Get_String_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.TxtBx_FixtureID),
                PCName = ApplicationSetting.Get_String_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.TxtBx_PCName),
                ProgramVer = ApplicationSetting.Get_String_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.TxtBx_ProgramVer),
                Line = ApplicationSetting.Get_String_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.TxtBx_Line),
                Station = ApplicationSetting.Get_String_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.TxtBx_Station),
                Testplan = ApplicationSetting.Get_String_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.TxtBx_Testplan),
            };
            data_upload.SetInformation(info);

            if (ApplicationSetting.Get_Int_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.Cmbx_UseUploadSystem) == (int)eUseUploadSystem.USE)
                data_upload.IsUseUploadFunction(true);
            else
                data_upload.IsUseUploadFunction(false);

            if (data_upload.CheckConnectStatus() == false)
                return -1;

            return 0;
        }
        #endregion

        public void SetVirtual_IO_Rule()
        {
            RGBTesterMachine.DIOL.AddIORule(EIOName.SphereUp, true, (EIOName.SphereUpSensor, true));
            RGBTesterMachine.DIOL.AddIORule(EIOName.SphereUp, false, (EIOName.SphereUpSensor, false));

            RGBTesterMachine.DIOL.AddIORule(EIOName.SphereDown, true, (EIOName.SphereDownSensor, true));
            RGBTesterMachine.DIOL.AddIORule(EIOName.SphereDown, false, (EIOName.SphereDownSensor, false));

            RGBTesterMachine.DIOL.AddIORule(EIOName.Sphere_LR, true, (EIOName.SphereLeftSensor, true), (EIOName.SphereRightSensor, false));
            RGBTesterMachine.DIOL.AddIORule(EIOName.Sphere_LR, false, (EIOName.SphereLeftSensor, false), (EIOName.SphereRightSensor, true));

            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckUp, true, (EIOName.ChuckUpSensor, true));
            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckUp, false, (EIOName.ChuckUpSensor, false));

            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckDown, true, (EIOName.ChuckDownSensor, true));
            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckDown, false, (EIOName.ChuckDownSensor, false));

            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckLeft, true, (EIOName.ChuckLeftSensor, true));
            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckLeft, false, (EIOName.ChuckLeftSensor, false));

            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckRight, true, (EIOName.ChuckRightSensor, true));
            RGBTesterMachine.DIOL.AddIORule(EIOName.ChuckRight, false, (EIOName.ChuckRightSensor, false));
        }
        public int StartFunctionTest()
        {
            IFunction_LightEngine lea = ServiceProvider.GetRequiredService<IFunction_LightEngine>();
            
            lea.Open();

            ushort regAddr = 0x0438;
            byte regCount = 0x01;
            byte[] data = new byte[]{0xC0};

            lea.Set_RegisterValue(regAddr, regCount, data);

            RGBfunc.SerialNumber = (ApplicationSetting.Get_String_Recipe<eF_FunctionTester>((int)eF_FunctionTester.TxtBx_SerialNumber) ?? string.Empty).Trim();
            RGBfunc.SetFunctionTestProcess(true);

            RGBTesterMachine.DIOL.Clear_AI_VirtualData();
            StartFormLogic.ReadVirtual_AI_Data();

            if (UploadSetting() != 0)
                return -1;

            var MainTask = ServiceProvider.GetRequiredService<IBaseMainTask>();
            MainTask.SetTask<TaskFunctionTest>();
            MainTask.Run();

            return 0;
        }

        public bool CheckContact()
        {
            ushort regAddr = 0x0438;
            byte regCount = 0x01;
            byte[] data = new byte[] { 0xC0 };
            RGBTesterMachine.LightEngine.Set_RegisterValue(regAddr, regCount, data);
            Thread.Sleep(1000);

            RGBTesterMachine.DIOL.Clear_AI_VirtualData();
            StartFormLogic.ReadVirtual_AI_Data();

            double Voltage_6V = 0;
            double Voltage_1V2 = 0;
            double UL_6V = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_Voltage_6V_UL);
            double LL_6V = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_Voltage_6V_LL);
            double UL_1V2 = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_Voltage_1V2_UL);
            double LL_1V2 = ApplicationSetting.Get_Double_Recipe<eF_ParameterSettingRecipe>((int)eF_ParameterSettingRecipe.TxtBx_Voltage_1V2_LL);
            int method = ApplicationSetting.Get_Int_Recipe<eF_FunctionTester>((int)eF_FunctionTester.Cmbx_TestMode);
            if (method == (int)eTestMode.LEFT)
            {
                Voltage_6V = RGBTesterMachine.DIOL.GetAInputStatus(EIOName.Left_6V);
                Voltage_1V2 = RGBTesterMachine.DIOL.GetAInputStatus(EIOName.Left_1V2);
            }
            else if (method == (int)eTestMode.RIGHT)
            {
                Voltage_6V = RGBTesterMachine.DIOL.GetAInputStatus(EIOName.Right_6V);
                Voltage_1V2 = RGBTesterMachine.DIOL.GetAInputStatus(EIOName.Right_1V2);
            }

            if (Voltage_6V > UL_6V || Voltage_6V < LL_6V)
            {
                StatusBox.ShowMessage($"6V Contact Fail, Voltage={Voltage_6V}V");
                return false;
            }

            if(Voltage_1V2 > UL_1V2 || Voltage_1V2 < LL_1V2)
            {
                StatusBox.ShowMessage($"1.2V Contact Fail, Voltage={Voltage_1V2}V");
                return false;
            }

            return true;
        }

    }
}
