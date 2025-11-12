using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using RGBTester.Base;
using RGBTester.UI;
using RGBTester.Logic;

namespace RGBTester
{
    public static class RGBTesterAssemble
    {
        public static IServiceCollection AddRGBTesterServices(this IServiceCollection services)
        {
            //[RGBTesterMachine]
            services.AddSingleton<IRGBTesterMachine, RGBTesterMachine>();

            //[Thread]
            services.AddSingleton<IBaseMainTask, MainTask>();
            services.AddSingleton<IBaseMainTaskMulti, MainTaskMulti>();
            services.AddSingleton<IBaseTaskDependence, BaseTaskDependence>();

            //[Form]
            services.AddSingleton<F_MainForm>();
            services.AddSingleton<F_StartForm>();
            services.AddSingleton<F_StartForm_ButtonGroup>();

            //[Form Logic]
            services.AddSingleton<F_MainFormLogic>();
            services.AddSingleton<F_StartFormLogic>();

            //[Form]
            //退出Form後即close掉,要用再new
            services.AddTransient<F_OEM_Setting>();
            services.AddTransient<F_Equipment_Setting>();               
            services.AddTransient<IF_StateControl, F_StateControl>();   //一個Thread會有獨立的一個StateControl

            return services;
        }
    }
}
