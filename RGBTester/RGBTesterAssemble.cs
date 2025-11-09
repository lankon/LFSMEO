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
        // 建立一個擴充 IServiceCollection 的 public static 方法
        public static IServiceCollection AddRGBTesterServices(this IServiceCollection services)
        {
            services.AddTransient<IBaseMainTask, MainTask>();
            services.AddTransient<IRGBTesterMachine, RGBTesterMachine>();


            services.AddSingleton<F_MainForm>();
            services.AddSingleton<F_MainFormLogic>();
            services.AddSingleton<F_StartForm>();
            services.AddSingleton<F_StartForm_ButtonGroup>();
            
            services.AddTransient<F_OEM_Setting>();
            services.AddTransient<F_Equipment_Setting>();
            services.AddTransient<IF_StateControl, F_StateControl>();

            // 必須回傳 services
            return services;
        }
    }
}
