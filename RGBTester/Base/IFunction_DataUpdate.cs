using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Base
{
    public class UploadInfo
    {
        public string OperatorID = "";
        public string SerialNunber = "";
        public string ProgramVer = "";
        public string Testplan = "";
        public string Station = "";
        public string PCName = "";
        public string Line = "";
        public string FixtureID = "";
    }
    
    public interface IFunction_DataUpload
    {
        bool SetInformation(UploadInfo info);
        UploadInfo GetInformation();
        void IsUseUploadFunction(bool use);
        bool CheckConnectStatus();
        bool DataUpdate(List<string> data, string command);
    }
}
