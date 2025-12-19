using DeviceCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using ToolFunction;

namespace DeviceFunction
{
    public class F_MotionSettingLogic
    {
        public F_MotionSettingLogic()
        {
        }

        #region parameter define
        public IF_MotionSetting MotionSetting;
        public IF_AxisButton AxisButton;
        public IF_AxisSetting AxisSetting;
        #endregion

        public void SetAxisButtonIF(IF_AxisButton f_AxisButton)
        {
            AxisButton = f_AxisButton;
        }

        public void UpdateParameter()
        {
            AxisSetting.UpdateParmeter();
        }

        public int GetCurrentBtnNum()
        {
            return AxisButton.GetCurrentBtnNum();
        }

        public void SaveAxis(string filePath, string axisName, Dictionary<string, string> parameters)
        {
            XDocument doc;

            if (File.Exists(filePath))
                doc = XDocument.Load(filePath);
            else
                doc = new XDocument(new XElement("MachineConfig"));

            XElement root = doc.Element("MachineConfig");

            // 找出 Axis 節點
            var existingAxis = root.Elements("Axis")
                                   .FirstOrDefault(x => (string)x.Attribute("name") == axisName);

            if (existingAxis != null)
            {
                // 清空舊的參數
                existingAxis.RemoveNodes();
            }
            else
            {
                existingAxis = new XElement("Axis", new XAttribute("name", axisName));
                root.Add(existingAxis);
            }

            // 新增參數
            foreach (var kvp in parameters)
            {
                existingAxis.Add(new XElement("Parameter",
                    new XAttribute("key", kvp.Key),
                    new XAttribute("value", kvp.Value)
                ));
            }

            doc.Save(filePath);
        }
    }
}
