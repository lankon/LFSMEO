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
        public bool AtLeastEng()
        {
            if (CurLevel == eUserLevel.ENG || CurLevel == eUserLevel.OEM)
                return true;
            else
                return false;
        }
        public bool AtLeastOEM()
        {
            if (CurLevel == eUserLevel.OEM)
                return true;
            else
                return false;
        }
        public void SetCurLevel(eUserLevel level)
        {
            CurLevel = level;
        }
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

            File = Tool.CreateFile("Setting\\Account&Password", ".dat", false);

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
        public List<Dictionary<string, object>> LoadAccountPassword()
        {
            AccountPasswordData = new List<Dictionary<string, object>>();

            string path = System.IO.Directory.GetCurrentDirectory() + "\\Setting\\Account&Password.dat";

            if (!File.Exists(path))
                return AccountPasswordData;

            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 3)
                    continue;

                var dict = new Dictionary<string, object>();

                dict["Title_Account"] = parts[0];
                dict["Title_Password"] = DES_Decrypt(parts[1]);
                dict["Title_Level"] = parts[2];

                AccountPasswordData.Add(dict);
            }

            return AccountPasswordData;
        }
        public eUserLevel CheckUserPrivilege(string input_account, string input_password)
        {
            if (AccountPasswordData == null)
                return eUserLevel.OP;
            
            for (int i = 0; i < AccountPasswordData.Count; i++)
            {
                AccountPasswordData[i].TryGetValue("Title_Account", out object account);
                AccountPasswordData[i].TryGetValue("Title_Password", out object password);
                AccountPasswordData[i].TryGetValue("Title_Level", out object level);

                if(input_account == (string)account && input_password == (string)password)
                {
                    if ((string)level == eUserLevel.ENG.ToString())
                    {
                        CurLevel = eUserLevel.ENG;
                        return eUserLevel.ENG;
                    }
                    else if((string)level == eUserLevel.OP.ToString())
                    {
                        CurLevel = eUserLevel.OP;
                        return eUserLevel.OP;
                    }
                    else if ((string)level == eUserLevel.OEM.ToString())
                    {
                        CurLevel = eUserLevel.OEM;
                        return eUserLevel.OEM;
                    }
                }
            }

            return eUserLevel.NONE;
        }
        #endregion

        private string FixPassword(String Password)
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
        private string DES_Decrypt(string cipherText)
        {
            try
            {
                string publickey = "19960321";
                string secretkey = "85032143";

                byte[] keyBytes = Encoding.UTF8.GetBytes(publickey);
                byte[] ivBytes = Encoding.UTF8.GetBytes(secretkey);

                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(keyBytes, ivBytes), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("解密失敗: " + ex.Message);
            }
        }
    }
}
