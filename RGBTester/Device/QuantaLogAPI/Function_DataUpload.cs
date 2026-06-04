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
        QuantaAPI Quanta = new QuantaAPI();
        UploadInfo UploadInfo;
        #endregion

        #region public function
        public bool SetInfromation(UploadInfo info)
        {
            UploadInfo = info;
            return true;
        }

        public bool CheckConnectStatus()
        {
            if (ApplicationSetting.Get_Int_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.Cmbx_UseUploadSystem) == 0)
                return true;    //違反程式架構寫法
            
            
            string command = UploadInfo.SerialNunber + "," + UploadInfo.Station + "," + UploadInfo.Line + "," + 
                                UploadInfo.OperatorID + "," + UploadInfo.FixtureID + "," + UploadInfo.ProgramVer  + "," + 
                                UploadInfo.Testplan + "," + UploadInfo.PCName;

            int res = Quanta.CheckRoutingSMT(command);

            return res == 0;
        }

        public bool DataUpdate(List<string> data,string sn)
        {
            if (ApplicationSetting.Get_Int_Recipe<eF_UploadDataSetting>((int)eF_UploadDataSetting.Cmbx_UseUploadSystem) == 0)
                return true;    //違反程式架構寫法

            sn = sn + "," + UploadInfo.Line + "," + UploadInfo.OperatorID;

            int res = Quanta.UpdateToSMTDB(data, sn);

            if (res != 0)
                Tool.SaveLogToFile($"資料上傳系統失敗,ErrorCode:{res}");

            return res == 0;
        }
        #endregion
    }
}
