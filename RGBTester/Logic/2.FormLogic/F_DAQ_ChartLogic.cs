using DeviceCore;
using Microsoft.Extensions.DependencyInjection;
using RGBTester.Base;
using System;
using System.Collections.Generic;
using System.IO;
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
        public void SetLedCondition(byte test_side, byte color, int value, string test_mode)
        {
            ILightEngineFunction lea = ServiceProvider.GetRequiredService<ILightEngineFunction>();
            IF_StatusBox status_box = ServiceProvider.GetRequiredService<IF_StatusBox>();
            RGBTesterFunction func = ServiceProvider.GetRequiredService<RGBTesterFunction>();

            func.Set_LED_Rigester();

            if (!lea.SetLed_CurrentMode(test_mode))
            {
                status_box.ShowMessage("Set Light Engine Current Mode Fail");
                return;
            }

            if(!lea.SetLed_DAC(color, test_side, value))
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
                    Iin[i] = (RGBTesterMachine.DIOL.GetAInputStatus(daq_io.DAQ_Iin_HCM));
                else if(test_mode == "LCM")
                    Iin[i] = (RGBTesterMachine.DIOL.GetAInputStatus(daq_io.DAQ_Iin_LCM));
                
                Vled[i] = RGBTesterMachine.DIOL.GetAInputStatus(daq_io.DAQ_VLED);
                Vf[i] = Vled[i] - RGBTesterMachine.DIOL.GetAInputStatus(daq_io.DAQ_Vf);
                Iled[i] = (RGBTesterMachine.DIOL.GetAInputStatus(daq_io.DAQ_ILED));
            }

            DAQDataResult result = new DAQDataResult
            {
                Vin = Vin,
                Iin = Iin,
                Vled = Vled,
                Vf = Vf,
                Iled = Iled
            };

            Save_DAQ_Data(result, $"{AppDomain.CurrentDomain.BaseDirectory}\\Result\\DAQData_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            return result;
        }

        public void Save_DAQ_Data(DAQDataResult data, string file_path)
        {
            List<string> lines = new List<string>();
            lines.Add("Index,Vin,Iin,Vled,Vf,Iled");
            for (int i = 0; i < data.Vin.Length; i++)
            {
                string line = $"{i},{data.Vin[i]},{data.Iin[i]},{data.Vled[i]},{data.Vf[i]},{data.Iled[i]}";
                lines.Add(line);
            }
            File.WriteAllLines(file_path, lines);
        }

        /// <summary>
        /// 測試篩選資料結果
        /// </summary>
        /// <returns></returns>
        public RGBTesterFunction.AvgData CalculateCaptureDataAvg(DAQDataResult data)
        {
            DataFilter dataFilter = new DataFilter();

            double Vin_value = dataFilter.GetPreciseHighLevel(data.Vin.ToList(), 0.95, 0.002);
            double Iin_value = dataFilter.GetPreciseHighLevel(data.Iin.ToList(), 0.95, 0.002);
            double Vled_value = dataFilter.GetPreciseHighLevel(data.Vled.ToList(), 0.95, 0.002);
            double Vf_value = dataFilter.GetPreciseHighLevel(data.Vf.ToList(), 0.95, 0.002);
            double Iled_value = dataFilter.GetPreciseHighLevel(data.Iled.ToList(), 0.95, 0.002);

            RGBTesterFunction.AvgData avgData = new RGBTesterFunction.AvgData
            {
                Avg_Vin = Vin_value,
                Avg_Iin = Iin_value,
                Avg_Vled = Vled_value,
                Avg_Vf = Vf_value,
                Avg_Iled = Iled_value
            };

            return avgData;
        }


        //public RGBTesterFunction.AvgData CalculateCaptureDataAvg()
        //{
        //    string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Setting\\TestData.csv";

        //    if (!File.Exists(filePath))
        //        return -1;

        //    DataFilter dataFilter = new DataFilter();
        //    List<double>[] data_list = new List<double>[5];
        //    for (int i = 0; i < data_list.Length; i++)
        //    {
        //        data_list[i] = new List<double>();
        //    }

        //    string[] lines = File.ReadAllLines(filePath);

        //    IEnumerable<string> dataLines = lines.Skip(1);

        //    foreach (string line in dataLines)
        //    {
        //        // 如果是空行或無效行，則跳過
        //        if (string.IsNullOrWhiteSpace(line)) continue;

        //        // 使用逗號分隔符號分割字串
        //        string[] values = line.Split(',');

        //        if (values.Length < 2) continue; // 確保至少有 DAC 索引和一個通道值

        //        data_list[0].Add(double.Parse(values[1]));
        //        data_list[1].Add(double.Parse(values[2]));
        //        data_list[2].Add(double.Parse(values[3]));
        //        data_list[3].Add(double.Parse(values[4]));
        //        data_list[4].Add(double.Parse(values[5]));
        //    }

        //    double Vin_value = dataFilter.GetPreciseHighLevel(data_list[0].ToList(), 0.95, 0.002);
        //    double Iin_value = dataFilter.GetPreciseHighLevel(data_list[1].ToList(), 0.95, 0.002);
        //    double Vled_value = dataFilter.GetPreciseHighLevel(data_list[2].ToList(), 0.95, 0.002);
        //    double Vf_value = dataFilter.GetPreciseHighLevel(data_list[3].ToList(), 0.95, 0.002);
        //    double Iled_value = dataFilter.GetPreciseHighLevel(data_list[4].ToList(), 0.95, 0.002);

        //    RGBTesterFunction.AvgData avgData = new RGBTesterFunction.AvgData
        //    {
        //        Avg_Vin = Vin_value,
        //        Avg_Iin = Iin_value,
        //        Avg_Vled = Vled_value,
        //        Avg_Vf = Vf_value,
        //        Avg_Iled = Iled_value
        //    };

        //    return avgData;
        //}
    }
}
