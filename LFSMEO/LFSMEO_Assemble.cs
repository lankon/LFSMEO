using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows.Forms;

//[Device]
using DeviceCore;
using DeviceFunction;
using Device_MN200;

namespace LFSMEO
{
    class LFSMEO_Assemble
    {
        private IHost host;

        public Form BuildAndGetMainForm()
        {
            switch (Scope.MachineType)
            {
                case EMachineType.VPT_3IN1:
                    //return host.Services.GetRequiredService<F_VPT3IN1_Main>();
                    
                default:
                    return host.Services.GetRequiredService<F_SelectMachine>();
            }
        }

        public LFSMEO_Assemble()
        {
            host = CreateHostBuilder().Build();
            host.Start();
        }

        private IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureApplicationServices(services);
                });

        private void ConfigureApplicationServices(IServiceCollection services)
        {
            MN200 mn200 = new MN200();
            
            services.AddSingleton<IMotionCard>(mn200);

            // --- 步驟 3: 註冊「管理者」(DeviceFunction) ---
            // (這部分完全不變，DI 容器會自動注入步驟 2 註冊的所有卡片)
            services.AddSingleton<IFunction_MotionCard, Function_Motion_Card>();
            //services.AddSingleton<IIOManager, IOManager>();


            // --- 步驟 4: 註冊「機台邏輯」(Machine) ---
            // (這部分只依賴 "machineType" 變數)
            switch (Scope.MachineType)
            {
                case EMachineType.VPT_3IN1:
                    //services.AddTransient<, ProbeTesterLogic>();
                    //services.AddTransient<IMachineLogic, ProbeTesterLogic>();
                    break;
                default:
                    services.AddSingleton<F_SelectMachine>();
                    break;
            }
        }


    }
}
