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
using DeviceUI.Motion;
using DeviceUI.IO;
using Device_MN200;
using Device_PCIS_DASK;
using Device_Virtual;
using Device_Klzx;

//[Tool]
using UserPrivilege.Base;
using UserPrivilege.UI;
using UserPrivilege.Logic;

//[Machine]
using RGBTester;
using System.IO;
using System.Diagnostics;
using System.Drawing;

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
            Pcis_dask pcis_9111DG = new Pcis_dask("PCI_9111DG");
            Pcis_dask pcis_9111HR = new Pcis_dask("PCI_9111HR");
            Virtual_IO virtual_io = new Virtual_IO();

            ChillerControl_Klzx Klxz = new ChillerControl_Klzx();

            // 同一個實例，註冊成多個介面
            services.AddSingleton(Klxz);

            services.AddSingleton<IMotionCard>(mn200);
            services.AddSingleton<IIOCard>(mn200);
            services.AddSingleton<IIOCard>(pcis_9111DG);
            services.AddSingleton<IIOCard>(pcis_9111HR);
            services.AddSingleton<IIOCard>(virtual_io);
            services.AddSingleton<IChillerControl>(sp => sp.GetRequiredService<ChillerControl_Klzx>());

            services.AddSingleton<IF_MotionSetting, F_MotionSetting>();
            services.AddSingleton<IF_AxisSetting, F_AxisSetting>();
            services.AddSingleton<IF_AxisButton, F_AxisButton>();
            services.AddSingleton<F_MotionSettingLogic>();
            services.AddSingleton<IF_IO_Card, F_IO_Card>();

            // --- 步驟 3: 註冊「管理者」(DeviceFunction) ---
            // (這部分完全不變，DI 容器會自動注入步驟 2 註冊的所有卡片)
            services.AddSingleton<IFunction_MotionCard, Function_Motion_Card>();
            services.AddSingleton<IFunction_IO_Card, Function_IO_Card>();

            //[Tool]
            services.AddSingleton<IF_UserPrivilegeLogic, F_UserPrivilegeLogic>();
            services.AddSingleton<IF_UserPrivilege, F_UserPrivilege>();

            //[Machine]
            Icon AppIcon = null; string AppName = "";
            switch (Scope.MachineType)
            {
                case EMachineType.VPT_3IN1:
                    //services.AddTransient<, ProbeTesterLogic>();
                    //services.AddTransient<IMachineLogic, ProbeTesterLogic>();
                    break;
                case EMachineType.RGBTester:
                    services.AddRGBTesterServices();
                    AppIcon = Properties.Resources.RGBTester;
                    AppName = "RGBTester";
                    break;
                default:
                    services.AddSingleton<F_SelectMachine>();
                    break;
            }

            CreateShortcut(AppIcon, AppName);
        }

        //建立應用程式捷徑
        public void CreateShortcut(Icon AppIcon, string AppName)
        {
            if (AppIcon == null)
                return;
            
            // 取得目前程式的完整路徑
            string exePath = Process.GetCurrentProcess().MainModule.FileName;

            // 設定桌面捷徑的路徑
            string desktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{AppName}.lnk");

            // 從資源取出 Icon 物件
            Icon myIcon = AppIcon;

            // 設定存放路徑
            string tempIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_icon.ico");

            // 使用 FileStream 將 Icon 直接儲存
            using (FileStream fs = new FileStream(tempIconPath, FileMode.Create))
            {
                myIcon.Save(fs); // 這是 Icon 物件內建的方法，會直接轉成 ico 格式存檔
            }

            // 建立捷徑指向這個生成的臨時檔
            string psCommand = $"-Command \"$s = (New-Object -ComObject WScript.Shell).CreateShortcut('{desktopPath}'); " +
                               $"$s.WorkingDirectory = '{AppDomain.CurrentDomain.BaseDirectory}'; " +
                               $"$s.TargetPath = '{exePath}'; " +
                               $"$s.IconLocation = \\\"{tempIconPath},0\\\"; " +
                               "$s.Save()\"";

            // 執行指令 (隱藏視窗執行)
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = psCommand,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(startInfo);
        }
    }
}
