using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BurnInTester.Logic
{
    public class TemperatureThread
    {




    }


    public class ChamberRs485Manager
    {
        // 確保同一時間只有一個指令在 RS485 線路上
        private readonly SemaphoreSlim _lineLock = new SemaphoreSlim(1, 1);
        private SerialPort _serialPort;

        // 1. 輪詢溫度 (持續執行)
        public async Task StartPolling(CancellationToken ct)
        {
            int currentId = 1;
            while (!ct.IsCancellationRequested)
            {
                await ReadTemperatureAsync(currentId);

                currentId++;
                if (currentId > 40) currentId = 1;

                // 稍微喘息，給 CPU 處理 UI 的時間 (約 10-50ms)
                await Task.Delay(10);
            }
        }

        // 2. 讀取溫度的非同步方法
        public async Task<float> ReadTemperatureAsync(int stationId)
        {
            await _lineLock.WaitAsync(); // 取得線路控制權
            try
            {
                // 實作 Modbus RTU 讀取邏輯
                // SendRequest(stationId, 0x03, ...);
                // var data = await ReceiveResponseAsync();
                return 123;
            }
            finally
            {
                _lineLock.Release(); // 釋放線路
            }
        }

        // 3. 升降溫指令 (非同步執行，會與輪詢爭搶 Lock)
        public async Task<bool> SetTemperatureAsync(int stationId, float targetTemp)
        {
            await _lineLock.WaitAsync(); // 請求插隊
            try
            {
                // 實作 Modbus RTU 寫入邏輯 (例如 0x06 或 0x10 功能碼)
                // bool success = await SendWriteCommandAsync(stationId, targetTemp);
                return true;
            }
            finally
            {
                _lineLock.Release();
            }
        }
    }

    public class ComScheduler
    {
        // 存放待發送的任務。ConcurrentQueue為執行緒安全,Queue為非執行緒安全
        private ConcurrentQueue<ComTask> TaskQueue = new ConcurrentQueue<ComTask>();

        private class ComTask
        {
            public byte[] ByteCommand { get; set; }
            public string StrCommand { get; set; }
            public SerialPort Comport { get; set; }
            public TaskCompletionSource<byte[]> CompletionSource { get; set; }
            public TaskCompletionSource<string> CompletionSourceStr { get; set; }
        }

        public ComScheduler()
        {
            // 啟動背景處理迴圈
            Task.Run(() => ProcessLoop());
        }

        // 生產者：UI 或輪詢程式把指令丟進來
        public Task<byte[]> EnqueueCommand(byte[] byte_command = null, string str_command = "")
        {
            var tcs = new TaskCompletionSource<byte[]>();
            TaskQueue.Enqueue(new ComTask { ByteCommand = byte_command, StrCommand = str_command, CompletionSource = tcs });
            return tcs.Task; // 回傳一個 Task，讓呼叫者可以用 await 等待結果
        }

        public Task<string> EnqueueStrCommand(SerialPort port, string str_command = "")
        {
            var tcs = new TaskCompletionSource<string>();
            TaskQueue.Enqueue(new ComTask { Comport = port, ByteCommand = null, StrCommand = str_command, CompletionSourceStr = tcs });
            return tcs.Task; // 回傳一個 Task，讓呼叫者可以用 await 等待結果
        }

        // 消費者：唯一的通訊執行者
        private async Task ProcessLoop()
        {
            while (true)
            {
                if (TaskQueue.TryDequeue(out var task))
                {
                    try
                    {
                        // 這裡執行真正的 SerialPort 通訊
                        // SerialPort.Write(task.ByteCommand);
                        // byte[] result = await ReadResponse();

                        //task.CompletionSource.SetResult(result); // 通知呼叫者拿到資料了
                    }
                    catch (Exception ex)
                    {
                        task.CompletionSource.SetException(ex);
                    }
                }
                else
                {
                    await Task.Delay(1); // 隊列空的時候休息，避免 CPU 100%
                }
            }
        }

        
    }

}
