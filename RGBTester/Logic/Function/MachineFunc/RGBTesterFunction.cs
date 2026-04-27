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
    public enum eModuleType
    {
        IV_Calibration,
        Function_Test,
    }
    
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

            public void ResetAllFlag()
            {
                IsSlopeErr = false;
                IsClampingErr = false;
                IsTemperatureErr = false;
                IsSlopeCalculateCurrentErr = false;
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

                if (IsSlopeErr == false && IsClampingErr == false && 
                    IsTemperatureErr == false && IsSlopeCalculateCurrentErr==false)
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
