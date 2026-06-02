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
using System.Threading;

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
            public double[] CH1 { get; set; }
            public double[] CH2 { get; set; }
            public double[] CH3 { get; set; }
            public double[] CH4 { get; set; }
            public double[] CH5 { get; set; }
        }
        #endregion
        public void SetLedCondition(byte test_side, byte color, int value, string test_mode)
        {
            //[Debug]
            
            IFunction_LightEngine lea = ServiceProvider.GetRequiredService<IFunction_LightEngine>();
            IF_StatusBox status_box = ServiceProvider.GetRequiredService<IF_StatusBox>();
            RGBTesterFunction func = ServiceProvider.GetRequiredService<RGBTesterFunction>();

            if(func.GetModuleType() == eModuleType.Function_Test)
            {
                if (color == lea.LED_B2)
                    lea.SetLed_AllColorVoltage(test_side, 3.6, 3.6, 3.6, 3.6);
                else
                    lea.SetLed_AllColorVoltage(test_side, 5.5, 5.5, 5.5, 5.5);
            }

            if (!lea.SetLed_CurrentMode(test_mode))
            {
                status_box.ShowMessage("Set Light Engine Current Mode Fail");
                return;
            }

            if (!lea.SetLed_DAC(color, test_side, value))
            {
                status_box.ShowMessage("Set Light Engine DAC Fail");
                return;
            }

            Thread.Sleep(30);   //等待反應時間
        }

        public DAQDataResult Get_DAQ_Data(byte test_side, byte color, string test_mode)
        {
            ApplicationSetting.ReadAllRecipe<eF_DAQ_ChartSetting>();

            DAQDataResult result = new DAQDataResult();
            int test_count = ApplicationSetting.Get_Int_Recipe<eF_DAQ_ChartSetting>((int)eF_DAQ_ChartSetting.TxtBx_ReadCount);

            if (test_count == -1)
                return result;

            int channel = 0;
            for(int i=0; i<5; i++)
            {
                if (ApplicationSetting.Get_Int_Recipe<eF_DAQ_ChartSetting>((int)eF_DAQ_ChartSetting.Cmbx_UseCH1+i) == 1)
                    channel++;
            }

            double[] CH1 = new double[test_count];
            double[] CH2 = new double[test_count];
            double[] CH3 = new double[test_count];
            double[] CH4 = new double[test_count];
            double[] CH5 = new double[test_count];

            for (int i = 0; i < test_count; i++)
            {
                double[] data = new double[5];
                
                for(int j=0; j< channel; j++)
                {
                    string io_name = ApplicationSetting.Get_String_Recipe<eF_DAQ_ChartSetting>((int)eF_DAQ_ChartSetting.TxtBx_CH1+j);
                    EIOName daq = Tool.StringToEnum<EIOName>(io_name);

                    data[j] = RGBTesterMachine.DIOL.GetAInputStatus(daq);
                }

                CH1[i] = data[0];
                CH2[i] = data[1];
                CH3[i] = data[2];
                CH4[i] = data[3];
                CH5[i] = data[4];
            }

            result.CH1 = CH1;
            result.CH2 = CH2;
            result.CH3 = CH3;
            result.CH4 = CH4;
            result.CH5 = CH5;

            Save_DAQ_Data(result, $"{AppDomain.CurrentDomain.BaseDirectory}\\Result\\DAQData_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            return result;
        }

        public void Save_DAQ_Data(DAQDataResult data, string file_path)
        {
            List<string> lines = new List<string>();
            lines.Add("Index,CH1,CH2,CH3,CH4,CH5");
            for (int i = 0; i < data.CH1.Length; i++)
            {
                string line = $"{i},{data.CH1[i]},{data.CH2[i]},{data.CH3[i]},{data.CH4[i]},{data.CH5[i]}";
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

            double Vin_value = dataFilter.GetPreciseHighLevel(data.CH1.ToList(), 0.95, 0.002);
            double Iin_value = dataFilter.GetPreciseHighLevel(data.CH2.ToList(), 0.95, 0.002);
            double Vled_value = dataFilter.GetPreciseHighLevel(data.CH3.ToList(), 0.95, 0.002);
            double Vf_value = dataFilter.GetPreciseHighLevel(data.CH4.ToList(), 0.95, 0.002);
            double Iled_value = dataFilter.GetPreciseHighLevel(data.CH5.ToList(), 0.95, 0.002);

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
