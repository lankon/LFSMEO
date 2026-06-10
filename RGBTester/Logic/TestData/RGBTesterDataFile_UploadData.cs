using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RGBTester.Base;
using ToolFunction;

namespace RGBTester.Logic
{
    public class UploadData
    {
        public UploadData(IFunction_DataUpload function_DataUpload)
        {
            DataUpdate = function_DataUpload;
        }

        #region parameter define
        private List<string> Calibration = new List<string>();
        private IFunction_DataUpload DataUpdate;
        #endregion

        #region public function
        public void InputMessage(List<string> calibration, string cmd)
        {
            Calibration = calibration;
        }
        public bool UpdateResult(string command)
        {
            bool res = true;
            if (Calibration.Count != 0)
            {
                res = DataUpdate.DataUpdate(Calibration, command);
                Calibration.Clear();
            }

            return res;
        }
        #endregion
    }
}
