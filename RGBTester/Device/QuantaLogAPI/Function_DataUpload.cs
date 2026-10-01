using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;
using RGBTester.Base;

namespace RGBTester.Device
{
    public class Function_DataUpload : IFunction_DataUpload
    {
        public Function_DataUpload()
        {

        }

        #region parameter define
        private bool IsUseUpload = false;
        private QuantaAPI Quanta = new QuantaAPI(); //違規_違反程式架構寫法,必須要有interface,並且可以選擇客戶別
        private UploadInfo UploadInfo;
        #endregion

        #region public function
        public bool SetInformation(UploadInfo info)
        {
            UploadInfo = info;
            return true;
        }

        public UploadInfo GetInformation()
        {
            return UploadInfo;
        }

        public void IsUseUploadFunction(bool use)
        {
            IsUseUpload = use;
        }

        public bool CheckConnectStatus()
        {
            if (IsUseUpload == false)
                return true;
            
            string command = UploadInfo.SerialNunber + "," + UploadInfo.Station + "," + UploadInfo.Line + "," + 
                                UploadInfo.OperatorID + "," + UploadInfo.FixtureID + "," + UploadInfo.ProgramVer  + "," + 
                                UploadInfo.Testplan + "," + UploadInfo.PCName;

            int res = Quanta.CheckRoutingSMT(command);

            return res == 0;
        }

        public bool DataUpdate(List<string> data,string command)
        {
            if (IsUseUpload == false)
                return true;

            string test_result = command;
            string info = test_result + "," +
                          UploadInfo.SerialNunber + "," + UploadInfo.Line + "," + 
                          UploadInfo.OperatorID + "," + UploadInfo.FixtureID + "," + 
                          UploadInfo.ProgramVer + "," + UploadInfo.Testplan + "," + 
                          UploadInfo.PCName;

            int res = Quanta.UpdateToSMTDB(data, info);

            if (res != 0)
                Tool.SaveLogToFile($"資料上傳系統失敗,ErrorCode:{res}");

            return res == 0;
        }
        #endregion
    }
}
