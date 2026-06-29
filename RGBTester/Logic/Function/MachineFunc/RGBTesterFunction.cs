using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ToolFunction;

namespace RGBTester.Logic
{
    public partial class RGBTesterFunction
    {
        public RGBTesterFunction(IServiceProvider serviceProvider, IRGBTesterMachine machine)
        {
            ServiceProvider = serviceProvider;
            Machine = machine;

            GetModuleType();

            HardwareParam = new TestHardwareParam(this);
        }

        #region parameter define
        public string SerialNumber = "";
        public bool IsFunctionTestProcess = false;
        IServiceProvider ServiceProvider;
        IRGBTesterMachine Machine;
        IFunction_LightEngine LEA;
        public TestHardwareParam HardwareParam { get; private set; }
        public TestFailReasonFlag FailReasonFlag { get; set; } = new TestFailReasonFlag();
        public TestSlopeOffsetResult SlopeOffsetResult { get; set; } = new TestSlopeOffsetResult();
        #endregion

        

        public class TestFailReasonFlag
        {
            public bool IsSlopeErr = false;
            public bool IsClampingErr = false;
            public bool IsTemperatureErr = false;
            public bool IsSlopeCalculateCurrentErr = false;
            public bool IsVoltageErr_6V = false;
            public bool IsVoltageErr_1V2 = false;
            public bool IsRedLuminousErr = false;
            public bool IsGreenLuminousErr = false;
            public bool IsBlueLuminousErr = false;
            public bool IsBlue2LuminousErr = false;

            public enum ERROR_CODE
            {
                NONE,
                SLOPE,
                _0TEMP,
                _00V6V,
                _0V1V2,
                _0LUMR,
                _0LUMG,
                _0LUMB,
                LUMB2,
                CLAMP,
                SLPCC,
            }

            public void ResetAllFlag()
            {
                IsSlopeErr = false;
                IsClampingErr = false;
                IsTemperatureErr = false;
                IsSlopeCalculateCurrentErr = false;
                IsVoltageErr_6V = false;
                IsVoltageErr_1V2 = false;
                IsRedLuminousErr = false;
                IsGreenLuminousErr = false;
                IsBlueLuminousErr = false;
                IsBlue2LuminousErr = false;
            }

            public bool IsTestFail()
            {
                if (IsSlopeErr || IsClampingErr || IsTemperatureErr ||
                    IsSlopeCalculateCurrentErr ||
                    IsVoltageErr_6V || IsVoltageErr_1V2 || IsRedLuminousErr ||
                    IsGreenLuminousErr || IsBlueLuminousErr || IsBlue2LuminousErr)
                    return true;
                else
                    return false;
            }

            public string GetFailDescription()
            {
                List<string> error_message = new List<string>();

                if (IsSlopeErr)
                    error_message.Add("Slope Fail");
                if (IsClampingErr)
                    error_message.Add("Clamping Fail");
                if (IsTemperatureErr)
                    error_message.Add("Temperature Fail");
                if(IsSlopeCalculateCurrentErr)
                    error_message.Add("Slope Cal Current Fail");
                if (IsVoltageErr_6V)
                    error_message.Add("Voltage 6V Fail");
                if (IsVoltageErr_1V2)
                    error_message.Add("Voltage 1.2V Fail");
                if(IsRedLuminousErr)
                    error_message.Add("Red Luminous Fail");
                if (IsGreenLuminousErr)
                    error_message.Add("Green Luminous Fail");
                if (IsBlueLuminousErr)
                    error_message.Add("Blue Luminous Fail");
                if (IsBlue2LuminousErr)
                    error_message.Add("Blue2 Luminous Fail");

                if (IsSlopeErr == false && IsClampingErr == false && 
                    IsTemperatureErr == false && IsSlopeCalculateCurrentErr==false && 
                    IsVoltageErr_6V && IsVoltageErr_1V2 && IsRedLuminousErr && IsGreenLuminousErr &&
                    IsBlueLuminousErr && IsBlue2LuminousErr)
                    error_message.Add("None");

                string res = "";
                for(int i=0; i<error_message.Count; i++)
                {
                    if (i == error_message.Count - 1)
                        res += error_message[i];
                    else
                        res += error_message[i] + " && ";
                }

                return res;
            }

            public ERROR_CODE UploadErrorCode()
            {
                if(IsSlopeErr)
                    return ERROR_CODE.SLOPE;
                else if(IsTemperatureErr)
                    return ERROR_CODE._0TEMP;
                else if(IsVoltageErr_6V)
                    return ERROR_CODE._00V6V;
                else if(IsVoltageErr_1V2)
                    return ERROR_CODE._0V1V2;
                else if(IsRedLuminousErr)
                    return ERROR_CODE._0LUMR;
                else if(IsGreenLuminousErr)
                    return ERROR_CODE._0LUMG;
                else if(IsBlueLuminousErr)
                    return ERROR_CODE._0LUMB;
                else if(IsBlue2LuminousErr)
                    return ERROR_CODE.LUMB2;
                else if(IsSlopeCalculateCurrentErr)
                    return ERROR_CODE.CLAMP;
                else if(IsClampingErr)
                    return ERROR_CODE.SLPCC;
                else
                    return ERROR_CODE.NONE;
            }
        }

        public void Set_LED_Rigester()
        {
            LEA = ServiceProvider.GetRequiredService<IFunction_LightEngine>();

            byte[] intput = new byte[1];
            //intput[0] = 0x24;
            //Deps.LightEngine.Set_RegisterValue(0x02, 1, intput);
            intput[0] = 0xFF;
            LEA.Set_RegisterValue(0x018, 1, intput);
            intput[0] = 0xC0;
            LEA.Set_RegisterValue(0x1E, 1, intput);

            Tool.SaveLogToFile("Set LED Voltage Value", level: "DBG");
        }
        public void SetMaxCurrent(double LCM_I, double HCM_I)
        {
            MaxCurrent_LCM = LCM_I;
            MaxCurrent_HCM = HCM_I;
        }
        public void YieldStatistics(bool pass, string sn, string description = "None")
        {
            if (ApplicationSetting.Get_Int_Recipe<eF_YieldReport>((int)eF_YieldReport.Cmbx_YieldRecord) == 0)
                return;
            
            string product_type = "None";
            int i_pass = pass == true ? 1 : 0;

            if (sn.Substring(2,1) == "A")
                product_type = "STM1";
            else if(sn.Substring(2, 1) == "C")
                product_type = "STM2";

            DataBaseTestResult.ProductionLog log = new DataBaseTestResult.ProductionLog()
            {
                ProductType = product_type,
                SN = sn,
                TestTime = DateTime.Now,
                IsPass = i_pass,
                Exclude = 0,
                Description = description
            };

            DataBaseTestResult data_base = ServiceProvider.GetRequiredService<DataBaseTestResult>();
            data_base.Manager.InsertData(log);
        }
        public void SetFunctionTestProcess(bool flag)
        {
            IsFunctionTestProcess = flag;
        }
        public bool GetFunctionTestProcess() 
        {
            return IsFunctionTestProcess;
        }
        public eModuleType GetModuleType()
        {
            bool isElectrical = ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_ElectricalModule) == 1 ? true : false;
            bool isOptical = ApplicationSetting.Get_Int_Recipe<eF_Equipment_Setting>((int)eF_Equipment_Setting.Cmbx_OpticalModule) == 1 ? true : false;
            
            if(isElectrical && isOptical)
                return eModuleType.Function_Test;
            else
                return eModuleType.IV_Calibration;
        }
    }
}
