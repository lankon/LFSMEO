using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGBTester.Device
{
    public class QuantaAPI
    {
        public QuantaAPI()
        {
            Initial();
        }

        #region private function
        private ProcessStartInfo startInfo;
        #endregion

        #region private function
        private void Initial()
        {
            string command = "";
            string exePath = AppDomain.CurrentDomain.BaseDirectory + "Main.exe";

            startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = command,
                UseShellExecute = false,        // 不使用系統殼層，以便讀取輸出
                RedirectStandardOutput = true,  // 捕捉程式輸出的文字
                CreateNoWindow = false,          // 不跳出黑視窗 (如果你希望在背景執行)
                WindowStyle = ProcessWindowStyle.Hidden
            };
        }
        #endregion

        public int CheckRoutingSMT(out string res, string info = "")
        {
            res = "";
            
            try
            {
                string[] info_array = info.Split(';');

                if(info_array.Length < 5)
                    return -1;

                string sn = info_array[0];
                string station = info_array[1];
                string line = info_array[2];
                string op_id = info_array[3];
                string equip_id = info_array[4];
                
                startInfo.Arguments = $"Main.exe -m CheckRoutingSMT -p {sn} {line} {op_id} {equip_id}";

                using (Process process = Process.Start(startInfo))
                {
                    res = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public void UpdateToSMTDB()
        {
            try
            {
                startInfo.Arguments = "Main.exe -m UpdateToSMTDB -p UIRIOB2601220023 BFT PASS ##Test1=PASS##Test2=PASS##TestTime=56##FixtureID=UIR-BFT##Program_ver=0.0.1##Testplan_ver=0.0.1 B12 10812122";

                using (Process process = Process.Start(startInfo))
                {
                    string result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    Console.WriteLine("執行結果： " + result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("執行失敗： " + ex.Message);
            }
        }


    }
}
