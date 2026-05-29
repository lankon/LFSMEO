using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RGBTester.Base;
using ToolFunction;

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
        private int CallQuantaMain(string command)
        {
            string res = "";
            string processName = "Main";    //呼叫程式名稱

            try
            {
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

                ProcessStartInfo localStartInfo = new ProcessStartInfo
                {
                    FileName = processName + ".exe",
                    Arguments = command,
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

                    if (process.WaitForExit(30000))
                    {
                        res = process.StandardOutput.ReadToEnd();
                        res = process.StandardError.ReadToEnd();
                        return 0;
                    }
                    else
                    {
                        process.Kill(); // 強制中斷它
                        res = "ERROR: MES Server Timeout (30s)";
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
        #endregion

        public int CheckRoutingSMT(string info = "")
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

            string res = "";
            string processName = "Main";    //呼叫程式名稱

            try
            {
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

                ProcessStartInfo localStartInfo = new ProcessStartInfo
                {
                    FileName = processName + ".exe",
                    Arguments = $"-m CheckRoutingSMT -p {sn} {line} {op_id} {equip_id}",
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

                    if (process.WaitForExit(30000))
                    {
                        res = process.StandardOutput.ReadToEnd();
                        res = process.StandardError.ReadToEnd();
                        return 0;
                    }
                    else
                    {
                        process.Kill(); // 強制中斷它
                        res = "ERROR: MES Server Timeout (30s)";
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

        public int UpdateToSMTDB(List<string>data,string sn)
        {
            string CalibrationData = "";

            for(int i=0; i< data.Count; i++)
            {
                string title = data[i].Split(',')[1];

                CalibrationData = CalibrationData + $"##{title}={data[i]}";
            }
            sn = sn.Split(',')[4];
            string command = "";
            if (Scope.TestFail == true)
                command = $"-m UpdateToSMTDB -p {sn} BFT FAIL {CalibrationData} A12 OPID";
            else
                command = $"-m UpdateToSMTDB -p {sn} BFT PASS {CalibrationData} A12 OPID";

            DateTime DateNow = DateTime.Now;
            string Time = DateNow.ToString("yyyyMMddHHmmss");
            string file_name = $"\\Result\\{DateNow.ToString("yyyyMMdd")}\\Calibration_{sn}_{Time}";

            StreamWriter file =  Tool.CreateFile(file_name, ".csv", false);
            Tool.WriteFile(file, command);
            Tool.CloseFile(file);

            return 0;

        }
    }
}
