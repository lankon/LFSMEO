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

        public int CheckRoutingSMT(string info = "")
        {
            string res = "";
            string processName = "Main";    //呼叫程式名稱

            try
            {
                if (string.IsNullOrWhiteSpace(info))
                    return -1;

                string[] info_array = info.Split(';');
                if (info_array.Length < 4)
                    return -1;

                string sn = info_array[0];
                string line = info_array[1];
                string op_id = info_array[2];
                string equip_id = info_array[3];

                Process[] runningProcesses = Process.GetProcessesByName(processName);
                foreach (Process p in runningProcesses)
                {
                    try
                    {
                        // 發現上次殘留的進程（可能卡在 MES 網路超時），果斷強制殺掉，避免互相干擾
                        p.Kill();
                        p.WaitForExit(1000); // 等待 1 秒確保它徹底釋放記憶體
                    }
                    catch { /* 忽略已經在關閉中的進程錯誤 */ }
                }


                //"Main.exe -m UpdateToSMTDB -p B1A2J4T004 BFT PASS ##Test1=PASS##Test2=PASS##FixtureID=UIR-BFT A12 OPID",//$"Main.exe -m CheckRoutingSMT -p {sn} {line} {op_id} {equip_id}",
                // @"C:\Users\Fittech\Desktop\API_Test\Main.exe",

                // 配置這一次的啟動參數 (改用區域變數，不共用全域 startInfo)
                ProcessStartInfo localStartInfo = new ProcessStartInfo
                {
                    FileName = processName + ".exe",
                    Arguments = $"Main.exe -m CheckRoutingSMT -p {sn} {line} {op_id} {equip_id}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,  // 要讀取回傳結果，必須開啟
                    RedirectStandardError = true,
                    CreateNoWindow = false,      // 隱藏黑畫面視窗
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                };

                // 啟動並執行
                using (Process process = new Process { StartInfo = localStartInfo })
                {
                    if (process == null) return -1;

                    process.Start();

                    res = process.StandardOutput.ReadToEnd();
                    res = process.StandardError.ReadToEnd();

                    if (process.WaitForExit(5000))
                    {
                        // 正常在 5 秒內執行完畢並關閉了，這時候才安全地讀取結果
                        res = process.StandardOutput.ReadToEnd();
                        res = process.StandardError.ReadToEnd();
                        return 0;
                    }
                    else
                    {
                        process.Kill(); // 強制中斷它
                        res = "ERROR: MES Server Timeout (5s)";
                        return -2;
                    }
                }
            }
            catch (Exception ex)
            {
                res = $"Exception: {ex.Message}";
                return -1;
            }
        }

        public int UpdateToSMTDB()
        {
            try
            {
                string processName = "Main";
                string res = "";
                Process[] runningProcesses = Process.GetProcessesByName(processName);
                foreach (Process p in runningProcesses)
                {
                    try
                    {
                        // 發現上次殘留的進程（可能卡在 MES 網路超時），果斷強制殺掉，避免互相干擾
                        p.Kill();
                        p.WaitForExit(1000); // 等待 1 秒確保它徹底釋放記憶體
                    }
                    catch { /* 忽略已經在關閉中的進程錯誤 */ }
                }

                // 配置這一次的啟動參數 (改用區域變數，不共用全域 startInfo)
                ProcessStartInfo localStartInfo = new ProcessStartInfo
                {
                    FileName = processName + ".exe",//@"C:\Users\Fittech\Desktop\API_Test\Main.exe",
                    Arguments = "Main.exe -m UpdateToSMTDB -p B1A2J4T004 BFT PASS ##Test1=PASS##Test2=PASS A12 OPID",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,  // 要讀取回傳結果，必須開啟
                    RedirectStandardError = true,
                    CreateNoWindow = false,      // 隱藏黑畫面視窗
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                };

                // 啟動並執行
                using (Process process = new Process { StartInfo = localStartInfo })
                {
                    if (process == null) return -1;

                    process.Start();

                    if (process.WaitForExit(5000))
                    {
                        // 正常在 5 秒內執行完畢並關閉了，這時候才安全地讀取結果
                        res = process.StandardOutput.ReadToEnd();
                        res = process.StandardError.ReadToEnd();
                        return 0;
                    }
                    else
                    {
                        process.Kill(); // 強制中斷它
                        res = "ERROR: MES Server Timeout (5s)";
                        return -2;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
                Console.WriteLine("執行失敗： " + ex.Message);
            }
        }


    }
}
