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
    public class F_DAQ_ChartLogic
    {
        public F_DAQ_ChartLogic(IRGBTesterMachine rGBTesterMachine, IServiceProvider serviceProvider)
        {
            RGBTesterMachine = rGBTesterMachine;
            ServiceProvider = serviceProvider;
        }

        #region parameter define
        IF_MainForm MainForm;
        IBaseMainTask MainTask;
        IRGBTesterMachine RGBTesterMachine;
        IServiceProvider ServiceProvider;

        public class DAQDataResult
        {
            public double[] Vin { get; set; }
            public double[] Iin { get; set; }
            public double[] Vled { get; set; }
            public double[] Vf { get; set; }
            public double[] Iled { get; set; }
        }
        #endregion

        public void ReadAllSetting()
        {
            ApplicationSetting.ReadAllRecipe<eF_Equipment_Setting>();
            ApplicationSetting.ReadAllRecipe<eF_Recipe>();

            Tool.SaveLogToFile("Load Recipe File", level: "INF");
            var recipe = ServiceProvider.GetRequiredService<F_RecipeLogic>();
            string cur_recipe_name = ApplicationSetting.Get_String_Recipe<eF_Recipe>((int)eF_Recipe.TxtBx_RecipeName);
            recipe.ReadRecipe(cur_recipe_name);
        }

        public void SetLedCondition(byte test_side, byte color, int value, string test_mode)
        {
            ILightEngineFunction lea = ServiceProvider.GetRequiredService<ILightEngineFunction>();
            IF_StatusBox status_box = ServiceProvider.GetRequiredService<IF_StatusBox>();

            if (!lea.SetLed_CurrentMode(test_mode))
            {
                status_box.ShowMessage("Set Light Engine Current Mode Fail");
                return;
            }

            if(!lea.SetLed_DAC(test_side, color, value))
            {
                status_box.ShowMessage("Set Light Engine DAC Fail");
                return;
            }
        }

        public DAQDataResult Get_DAQ_Data(byte test_side, byte color, string test_mode)
        {
            RGBTesterFunction func = ServiceProvider.GetRequiredService<RGBTesterFunction>();
            RGBTesterFunction.DAQ_IO_Point daq_io = func.Get_DAQ_IO_Point(test_side, color);
            RGBTesterFunction.TestHardwareParam hw_param = new RGBTesterFunction.TestHardwareParam();
            int test_count = hw_param.Period_DAQ_Count * 3;

            double[] Vin = new double[test_count];
            double[] Iin = new double[test_count];
            double[] Vled = new double[test_count];
            double[] Vf = new double[test_count];
            double[] Iled = new double[test_count];

            for (int i = 0; i < test_count; i++)
            {
                //一次取5個通道有增加或減少的話會影響Period_DAQ_Count
                Vin[i] = RGBTesterMachine.DIOL.GetAInputStatus(daq_io.DAQ_Vin);

                if(test_mode == "HCM")
                    Iin[i] = (RGBTesterMachine.DIOL.GetAInputStatus(daq_io.DAQ_Iin_HCM) - hw_param.CurrentMeasureBias);
                else if(test_mode == "LCM")
                    Iin[i] = (RGBTesterMachine.DIOL.GetAInputStatus(daq_io.DAQ_Iin_LCM) - hw_param.CurrentMeasureBias);
                
                Vled[i] = RGBTesterMachine.DIOL.GetAInputStatus(daq_io.DAQ_VLED);
                Vf[i] = RGBTesterMachine.DIOL.GetAInputStatus(daq_io.DAQ_Vf);
                Iled[i] = (RGBTesterMachine.DIOL.GetAInputStatus(daq_io.DAQ_ILED) - hw_param.CurrentMeasureBias);
            }

            return new DAQDataResult
            {
                Vin = Vin,
                Iin = Iin,
                Vled = Vled,
                Vf = Vf,
                Iled = Iled
            };
        }

        //public (double,double) CalculateOffset(DAQDataResult data)
        //{
        //    List<double> max_value = new List<double>();

        //    max_value.Add(data.Vin.Max());
        //    max_value.Add(data.Vin.Max());
        //    max_value.Add(data.Vled.Max());
        //    max_value.Add(data.Vf.Max());
        //    max_value.Add(data.Iled.Max());
        //    double maxAmp = max_value.Max();

        //    if (maxAmp < 0.1) maxAmp = 0.5;

        //    double offsetStep = maxAmp * 2.2;   //各通道高度offset

        //    return (offsetStep,maxAmp);
        //}
    }
}
