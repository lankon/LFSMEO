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
        private bool TestMode = false;
        //private ProcessStartInfo startInfo;
        #endregion

        #region private function
        private void Initial()
        {
            //string command = "";
            //string exePath = AppDomain.CurrentDomain.BaseDirectory + "Main.exe";

            //startInfo = new ProcessStartInfo
            //{
            //    FileName = exePath,
            //    Arguments = command,
            //    UseShellExecute = false,        // 不使用系統殼層，以便讀取輸出
            //    RedirectStandardOutput = true,  // 捕捉程式輸出的文字
            //    CreateNoWindow = false,          // 不跳出黑視窗 (如果你希望在背景執行)
            //    WindowStyle = ProcessWindowStyle.Hidden
            //};
        }
        private int SendCommand(string command)
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

                    if (process.WaitForExit(30000)) //系統超過30秒未回覆
                    {
                        res = process.StandardOutput.ReadToEnd();

                        if (res.Contains("iResult:PASS"))
                            return 0;
                        else
                            return -3;

                        //res = process.StandardError.ReadToEnd();
                        //return 0;
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

            string[] info_array = info.Split(',');
            if (info_array.Length < 4)
                return -1;

            string sn = info_array[0];
            string station = info_array[1];
            string line = info_array[2];
            string op_id = info_array[3];
            string fixture_id = info_array[4];
            string program_id = info_array[5];
            string test_plan = info_array[6];
            string pc_name = info_array[7];

            int res = 0;

            if(!TestMode)
                res = SendCommand($"-m CheckRoutingSMT -p {sn} {station} {line} {op_id} FixtureID={fixture_id}##Program_ver={program_id}##Testplan_ver={test_plan}##PC_Name={pc_name}");

            return res;
        }

        public int UpdateToSMTDB(List<string>data,string sn)
        {
            int res = 0;
            string CalibrationData = "";
            string command;

            for (int i=0; i< data.Count; i++)
            {
                string title = data[i].Split(',')[1];

                CalibrationData = CalibrationData + $"##{title}={data[i]}";
            }
            string temp = sn;
            sn = temp.Split(',')[4];
            string line = temp.Split(',')[5];
            string op_id = temp.Split(',')[6];

            if (Scope.TestFail == true) //!!!!!違反架構寫法
                command = $"-m UpdateToSMTDB -p {sn} BFT FAIL {CalibrationData} {line} {op_id}";
            else
                command = $"-m UpdateToSMTDB -p {sn} BFT PASS {CalibrationData} {line} {op_id}";

            DateTime DateNow = DateTime.Now;
            string Time = DateNow.ToString("yyyyMMddHHmmss");
            string file_name = $"\\Result\\{DateNow.ToString("yyyyMMdd")}\\Calibration_{sn}_{Time}";

            if(!TestMode)
                res = SendCommand(command); //上傳系統

            //儲存至本機端
            StreamWriter file = Tool.CreateFile(file_name, ".csv", false);
            Tool.WriteFile(file, command);
            Tool.CloseFile(file);
                
            return res;
        }
    }
}
