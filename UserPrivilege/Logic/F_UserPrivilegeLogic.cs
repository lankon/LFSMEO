using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using ToolFunction;
using UserPrivilege.Base;

namespace UserPrivilege.Logic
{
    public class F_UserPrivilegeLogic:IF_UserPrivilegeLogic
    {
        #region parameter define
        private eUserLevel CurLevel = eUserLevel.OP;
        private List<Dictionary<string, object>> AccountPasswordData;
        #endregion

        #region public function
        public eUserLevel GetCurLevel()
        {
            return CurLevel;
        }
        public void GetDataGridInfo(List<Dictionary<string, object>> data)
        {
            AccountPasswordData = data;
        }
        public void SaveAccountPassword()
        {
            StreamWriter File;
            string Msg;

            File = Tool.CreateFile("Setting\\Account&Password", ".dat", true);

            for(int i=0; i< AccountPasswordData.Count; i++)
            {
                AccountPasswordData[i].TryGetValue("Title_Account", out object account);
                AccountPasswordData[i].TryGetValue("Title_Password", out object password);
                AccountPasswordData[i].TryGetValue("Title_Level", out object level);

                Msg = account + "," + FixPassword((string)password) + "," + level;

                Tool.WriteFile(File, Msg);
            }

            Tool.CloseFile(File);
            Tool.SaveLogToFile("Account & Password Save Success");
        }
        #endregion






        protected string FixPassword(String Password)
        {
            try
            {
                string ToReturn = "";
                string publickey = "19960321";
                string secretkey = "85032143";
                byte[] secretkeyByte = { };
                secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(Password);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte),
                                            CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }
}
