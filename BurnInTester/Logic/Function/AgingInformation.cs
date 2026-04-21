using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using ToolFunction;
using DeviceCore;
using BurnInTester.Base;
using BurnInTester.Device;

namespace BurnInTester.Logic
{
    public class AgingInformation
    {
        public AgingInformation(HW_ParamSetting hW_ParamSetting)
        {
            _HW_ParamSetting = hW_ParamSetting;

            InitialParameter();
        }

        #region parameter define
        private int BoxCount = 0;       //老化箱數量
        private List<AGING_INFO> PARAM_INFO = new List<AGING_INFO>();
        public string AgingConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Setting", "AgingConfig.xml");
        public TemperatureInfo[] TemperatureInfos;
        private HW_ParamSetting _HW_ParamSetting;

        public struct  AGING_INFO
        {
            //新增老化參數時需添加

            // [Product Info]
            public string ProductType;          //產品型號
            public string SerialNumber;         //序號

            // [Status]
            public string RunnningTime;        //已運行時間
            public string RemainingTime;       //剩餘時間
        }
        public class TemperatureInfo
        {
            private double[] PV = new double[5];
            private double[] SV = new double[5];

            public void UpdatePV(double[] newPV)
            {
                int length = Math.Min(newPV.Length, PV.Length);

                for (int i = 0; i < length; i++)
                    PV[i] = newPV[i];
            }
            public void UpdateSV(double[] newSV)
            {
                int length = Math.Min(newSV.Length, SV.Length);
                for (int i = 0; i < length; i++)
                    SV[i] = newSV[i];
            }
            public double[] GetPV()
            {
                return PV;
            }
            public double[] GetSV()
            {
                return SV;
            }
        }
        #endregion

        #region private function
        private void InitialParameter()
        {
            BoxCount = _HW_ParamSetting.TC_Box._CtrlBoxNum;

            TemperatureInfos = new TemperatureInfo[BoxCount];
            for (int i = 0; i < BoxCount; i++)
                TemperatureInfos[i] = new TemperatureInfo();

            //初始化老化箱參數
            for (int i = 0; i < BoxCount; i++)
                PARAM_INFO.Add(new AGING_INFO());
        }
        private void LoadAgingBoxConfig(string path)
        {
            XDocument doc = XDocument.Load(path);

            // 直接讀出所有 Box
            foreach (var box in doc.Descendants("Box"))
            {
                string name = (string)box.Attribute("name");

                // 讀出每個 Parameter
                foreach (var param in box.Elements("Parameter"))
                {
                    string key = (string)param.Attribute("key");
                    string value = (string)param.Attribute("value");

                    Project2BoxInfo(int.Parse(name.Replace("Box", "")), key, value);
                }
            }
        }
        private void Project2BoxInfo(int box, string item, string value)
        {
            //新增老化參數時需添加

            var info = PARAM_INFO[box];

            // [Product Info]
            if (item == eF_StartForm.TxtBx_ProductType.ToString())
                info.ProductType = value;
            else if (item == eF_StartForm.TxtBx_SerialNumber.ToString())
                info.SerialNumber = value;

            // [Status]
            else if (item == eF_StartForm.TxtBx_RunnningTime.ToString())
                info.RunnningTime = value;
            else if (item == eF_StartForm.TxtBx_RemainingTime.ToString())
                info.RemainingTime = value;

            PARAM_INFO[box] = info;
        }
        #endregion

        //[Read&Save Box Information]
        public void SaveBoxConfig(string filePath, string boxName, Dictionary<string, string> parameters)
        {
            XDocument doc;

            if (File.Exists(filePath))
                doc = XDocument.Load(filePath);
            else
                doc = new XDocument(new XElement("AgingConfig"));

            XElement root = doc.Element("AgingConfig");
            if (root == null)
            {
                Tool.SaveLogToFile($"SaveBoxConfig:資料格式錯誤請刪除 Invalid XML structure in {filePath}. Root element 'AgingConfig' not found.", level: "ERR");
                return;
            }

            // 找出 Box 節點
            var existingBox = root.Elements("Box")
                                   .FirstOrDefault(x => (string)x.Attribute("name") == boxName);

            if (existingBox != null)
            {
                // 清空舊的參數
                existingBox.RemoveNodes();
            }
            else
            {
                existingBox = new XElement("Box", new XAttribute("name", boxName));
                root.Add(existingBox);
            }

            // 新增參數
            foreach (var kvp in parameters)
            {
                existingBox.Add(new XElement("Parameter",
                    new XAttribute("key", kvp.Key),
                    new XAttribute("value", kvp.Value)
                ));
            }

            doc.Save(filePath);
        }
        public bool LoadBoxConfig()
        {
            if (!File.Exists(AgingConfigFilePath))
                return false;

            LoadAgingBoxConfig(AgingConfigFilePath);

            return true;
        }
        public IReadOnlyList<AGING_INFO> GetBoxConfig()
        {
            return PARAM_INFO.AsReadOnly();
        }
    }
}
