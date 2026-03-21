using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolFunction;
using static RGBTester.Logic.TestResultDataBase;

namespace RGBTester.Logic
{
    public class F_YieldReportLogic
    {
        public F_YieldReportLogic(IRGBTesterMachine machine, IServiceProvider serviceProvider)
        {
            Machine = machine;
            ServiceProvider = serviceProvider;
        }

        #region parameter define
        IRGBTesterMachine Machine;
        IServiceProvider ServiceProvider;
        #endregion

        #region public function
        public void OutPutYieldReport()
        {
            TestResultDataBase data_base = ServiceProvider.GetRequiredService<TestResultDataBase>();

            List<ProductionLog> pass_data = new List<ProductionLog>();
            List<ProductionLog> fail_data = new List<ProductionLog>();
            YieldResult yieldResult = new YieldResult();
            data_base.Manager.GetSummaryReport(ref pass_data, ref fail_data, ref yieldResult);

            StreamWriter file = Tool.CreateFile($"Result\\YieldReport_{DateTime.Now:yyyyMMdd_HHmmss}", ".csv", false);


            Tool.WriteFile(file, $"Equipment : Fittech IV Curve Calibration\n" +
                                 //$"Time Interval : {}\n" +
                                 //$"Product Type : {}\n" +
                                 $"Yield : {yieldResult.Yield:P2}\n" +
                                 $"Total Units : {yieldResult.TotalUnits}\n" +
                                 $"Pass Units : {yieldResult.PassUnits}\n" +
                                 $"Fail Units : {yieldResult.FailUnits}\n"
                                 );
            Tool.WriteFile(file, "======================== FAIL ===========================");
            Tool.WriteFile(file, "Product Type,SN,Test Time,Pass/Fail,Description");
            for(int i=0; i < fail_data.Count; i++)
            {
                string pass_fail = fail_data[i].IsPass == 1?"PASS" : "FAIL";
                Tool.WriteFile(file, $"{fail_data[i].ProductType},{fail_data[i].SN},{fail_data[i].TestTime.ToString("yyyy/MM/dd HH:mm:ss")},{pass_fail},{fail_data[i].Description}");
            }
            Tool.WriteFile(file, "\n======================== PASS ===========================");
            Tool.WriteFile(file, "Product Type,SN,Test Time,Pass/Fail,Description");
            for (int i = 0; i < pass_data.Count; i++)
            {
                string pass_fail = pass_data[i].IsPass == 1 ? "PASS" : "FAIL";
                Tool.WriteFile(file, $"{pass_data[i].ProductType},{pass_data[i].SN},{pass_data[i].TestTime.ToString("yyyy/MM/dd HH:mm:ss")},{pass_fail},{pass_data[i].Description}");
            }

            Tool.CloseFile(file);
        }
        #endregion

        #region private function

        #endregion
    }
}
