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
using Device_PCIS_DASK;

//[Machine]
using RGBTester;

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
                case EMachineType.RGBTester:
                    return host.Services.GetRequiredService<RGBTester.UI.F_MainForm>();
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
            Pcis_dask pcis_dask = new Pcis_dask(Pcis_dask_param.PCI_9111DG);

            // 同一個實例，註冊成多個介面
            services.AddSingleton(mn200);
            services.AddSingleton(pcis_dask);

            services.AddSingleton<IMotionCard>(sp => sp.GetRequiredService<MN200>());
            services.AddSingleton<IIOCard>(sp => sp.GetRequiredService<MN200>());
            services.AddSingleton<IIOCard>(sp => sp.GetRequiredService<Pcis_dask>());


            // --- 步驟 3: 註冊「管理者」(DeviceFunction) ---
            // (這部分完全不變，DI 容器會自動注入步驟 2 註冊的所有卡片)
            services.AddSingleton<IFunction_MotionCard, Function_Motion_Card>();
            services.AddSingleton<IFunction_IO_Card, Function_IO_Card>();


            // --- 步驟 4: 註冊「機台邏輯」(Machine) ---
            // (這部分只依賴 "machineType" 變數)
            switch (Scope.MachineType)
            {
                case EMachineType.VPT_3IN1:
                    //services.AddTransient<, ProbeTesterLogic>();
                    //services.AddTransient<IMachineLogic, ProbeTesterLogic>();
                    break;
                case EMachineType.RGBTester:
                    services.AddRGBTesterServices();
                    break;
                default:
                    services.AddSingleton<F_SelectMachine>();
                    break;
            }
        }


    }
}
