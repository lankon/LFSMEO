using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using RGBTester.Device;
using RGBTester.Logic;
using RGBTester.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester
{
    public static class RGBTesterAssemble
    {
        public static IServiceCollection AddRGBTesterServices(this IServiceCollection services)
        {
            //[RGBTesterMachine]
            services.AddSingleton<IRGBTesterMachine, RGBTesterMachine>();

            //[Device]
            services.AddSingleton<ILightEngineCommand, Virtual_LEA_Command>();
            services.AddSingleton<ILightEngineCommand, Z23A_API_Command>();
            services.AddSingleton<ILightEngineFunction, LightEngineFunction>();

            //[Thread]
            services.AddSingleton<IBaseMainTask, MainTask>();
            services.AddSingleton<IBaseMainTaskMulti, MainTaskMulti>();
            services.AddSingleton<IBaseTaskDependence, BaseTaskDependence>();

            //[Form]
            services.AddSingleton<F_MainForm>();
            services.AddSingleton<IF_StartForm, F_StartForm>();
            services.AddSingleton<F_StartForm_ButtonGroup>();
            services.AddSingleton<F_Recipe>();
            services.AddSingleton<F_DAQ_SamplingTest>();
            services.AddSingleton<IF_StatusBox, F_StatusBox>();
            services.AddSingleton<IF_ProgressBar, F_ProgressBar>();
            services.AddSingleton<F_DAQ_Chart>();

            //[Form]
            //退出Form後即close掉,要用再new
            services.AddTransient<F_OEM_Setting>();
            services.AddTransient<F_Equipment_Setting>();
            services.AddSingleton<IF_ParameterSetting, F_ParameterSetting>();
            services.AddTransient<IF_StateControl, F_StateControl>();   //一個Thread會有獨立的一個StateControl

            //[Form Logic]
            services.AddSingleton<F_MainFormLogic>();
            services.AddSingleton<F_StartFormLogic>();
            services.AddSingleton<F_RecipeLogic>();
            services.AddSingleton<F_DAQ_ChartLogic>();
            services.AddSingleton<F_ParameterSettingLogic>();
            
            //[Logic]
            services.AddSingleton<IWriteFile, RGBTesterDataFile>();
            services.AddSingleton<RGBTesterFunction>();
            services.AddSingleton<TestResultDataBase>();

            return services;
        }
    }
}
