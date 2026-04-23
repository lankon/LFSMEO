using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ToolFunction;
using DeviceCore;

namespace BurnInTester.Logic
{
    public class TC_CommManage
    {
        public TC_CommManage(IFunction_TemperatureControl function_TemperatureControl, AgingInformation agingInformation)
        {
            Func_TC = function_TemperatureControl;

            _AgingInformation = agingInformation;

            // 啟動背景處理迴圈
            Task.Run(() => ProcessLoop());
        }

        #region parameter define
        IFunction_TemperatureControl Func_TC;
        private AgingInformation _AgingInformation;
        // 存放待發送的任務。ConcurrentQueue為執行緒安全,Queue為非執行緒安全
        private ConcurrentQueue<ExecuteCommandTask> TaskQueue = new ConcurrentQueue<ExecuteCommandTask>();
        private ConcurrentQueue<ExecuteCommandTask> MainTaskQueue = new ConcurrentQueue<ExecuteCommandTask>();
        private class ExecuteCommandTask
        {
            public Func<Task<string>> ExecutionFunc { get; set; }
            public TaskCompletionSource<string> CompletionSource { get; set; }
        }
        #endregion

        #region private function
        private async Task ProcessLoop()
        {
            while (true)
            {
                if (TaskQueue.TryDequeue(out var task))
                {
                    try
                    {
                        string result = await task.ExecutionFunc();     // 執行任務並等待結果
                        task.CompletionSource.SetResult(result);        // 通知呼叫者拿到資料了
                        await Task.Delay(30);                           // 每次執行完任務後休息一下，避免過度頻繁的通訊
                    }
                    catch (Exception ex)
                    {
                        task.CompletionSource.SetException(ex);
                    }
                }
                else
                {
                    await Task.Delay(30); // 隊列空的時候休息，避免 CPU 100%
                }
            }
        }
        private Task<string> EnqueueAction(Func<Task<string>> action)
        {
            var tcs = new TaskCompletionSource<string>();

            // 直接打包任務丟進隊列
            TaskQueue.Enqueue(new ExecuteCommandTask
            {
                ExecutionFunc = action,
                CompletionSource = tcs
            });

            return tcs.Task;
        }
        private Task<string> EnqueueMainAction(Func<Task<string>> action)
        {
            var tcs = new TaskCompletionSource<string>();

            // 直接打包任務丟進隊列
            MainTaskQueue.Enqueue(new ExecuteCommandTask
            {
                ExecutionFunc = action,
                CompletionSource = tcs
            });

            return tcs.Task;
        }
        #endregion

        #region public function
        public async Task UpdateTemperature(ETemperatureControlName name, string cmd = "", int index = 0)
        {
            try
            {
                double[] temperature = new double[] { 0, 0, 0, 0 };
                
                string result = await EnqueueAction(async() =>
                {
                    Func_TC.AskPV(name, cmd);

                    // GetAnswer內部會等候回傳並解析，直到拿到結果才會繼續往下走，所以這裡不需要額外的等待邏輯
                    string[] ans = Func_TC.GetAnswer(name, cmd);
                    
                    List<double> validValues = new List<double>();
                    foreach (string s in ans)
                    {
                        double res = Tool.StringToDouble(s);

                        if (Math.Abs(res + 999) < 0.001)
                        {
                            Tool.SaveLogToFile($"Task UpdateTemperature錯誤:{s}");
                        }

                        validValues.Add(res);
                    }
                    double[] d_ans = validValues.ToArray();
                    temperature = d_ans;

                    _AgingInformation.TemperatureInfos[index].UpdatePV(d_ans);

                    return "";
                });
            }
            catch (Exception ex)
            {
                Tool.SaveLogToFile($"通訊失敗: {ex.Message}");
            }
        }
        public async Task Start(ETemperatureControlName name, double sv, string cmd = "", int index = 0)
        {
            try
            {
                double[] temperature = new double[] { 0, 0, 0, 0 };
                //!!!!!要處理MainAction
                string result = await EnqueueMainAction(async () =>
                {
                    Func_TC.Start(name, sv, cmd);

                    // GetAnswer內部會等候回傳並解析，直到拿到結果才會繼續往下走，所以這裡不需要額外的等待邏輯
                    string[] ans = Func_TC.GetAnswer(name, cmd);

                    return "";
                });
            }
            catch (Exception ex)
            {
                Tool.SaveLogToFile($"通訊失敗: {ex.Message}");
            }
        }

        #endregion
    }

}
