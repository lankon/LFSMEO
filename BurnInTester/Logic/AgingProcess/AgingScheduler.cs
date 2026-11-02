using System;
using System.Collections.Generic;
using System.Threading;
using BurnInTester.Base;
using BurnInTester.Logic.TestSetting;
using ToolFunction;

namespace BurnInTester.Logic
{
    /// <summary>
    /// 使用一條背景執行緒依序掃描所有溫控箱狀態機。
    /// 所有 Box 共用一條掃描執行緒，不會為每個 Box 各建立一條 Thread。
    /// </summary>
    public sealed class AgingScheduler : IDisposable
    {
        public AgingScheduler(
            AgingInformation agingInformation,
            IBaseTaskDependence dependencies,
            IEnumerable<IAgingProcessTaskFactory> factories)
        {
            if (agingInformation == null)
                throw new ArgumentNullException(nameof(agingInformation));
            if (dependencies == null)
                throw new ArgumentNullException(nameof(dependencies));
            if (factories == null)
                throw new ArgumentNullException(nameof(factories));

            _AgingInformation = agingInformation;
            List<IAgingProcessTaskFactory> registeredFactories =
                new List<IAgingProcessTaskFactory>(factories);
            _BoxTasks = new AgingBoxTask[_AgingInformation.AgingProcessSetting.Length];

            for (int boxIndex = 0; boxIndex < _BoxTasks.Length; boxIndex++)
            {
                _BoxTasks[boxIndex] = new AgingBoxTask(
                    dependencies,
                    boxIndex,
                    _AgingInformation.AgingProcessStatuses[boxIndex],
                    registeredFactories);
            }

            _WakeSignal = new AutoResetEvent(false);
            _ScanThread = new Thread(ScanLoop)
            {
                IsBackground = true,
                Name = "BurnInTester Aging Scheduler"
            };
            _ScanThread.Start();
        }

        private const int ScanIntervalMilliseconds = 50; // 所有 Box 完成一輪掃描後的最長等待時間

        private readonly AgingInformation _AgingInformation; // 每個 Box 已載入記憶體的排程與即時狀態
        private readonly AgingBoxTask[] _BoxTasks;           // 每個溫控箱各自獨立的狀態機
        private readonly AutoResetEvent _WakeSignal;         // Start/Pause/Continue/Abort 時立即喚醒掃描
        private readonly Thread _ScanThread;                 // 共用老化排程掃描執行緒
        private volatile bool _StopRequested;                // Dispose 要求掃描迴圈結束
        private bool _Disposed;                              // 防止 Dispose 後繼續操作

        public void StartBox(int boxIndex)
        {
            ThrowIfDisposed();
            ValidateBoxIndex(boxIndex);

            TestSettingData setting = _AgingInformation.AgingProcessSetting[boxIndex];
            _BoxTasks[boxIndex].Start(setting);
            _WakeSignal.Set();
        }

        public void PauseBox(int boxIndex)
        {
            SendCommand(boxIndex, TASK_STATUS.PAUSE);
        }

        public void ResumeBox(int boxIndex)
        {
            SendCommand(boxIndex, TASK_STATUS.CONTINUE);
        }

        public void AbortBox(int boxIndex)
        {
            SendCommand(boxIndex, TASK_STATUS.ABORT);
        }

        public AgingProcessStatusSnapshot GetStatus(int boxIndex)
        {
            ThrowIfDisposed();
            ValidateBoxIndex(boxIndex);
            return _AgingInformation.AgingProcessStatuses[boxIndex].GetSnapshot();
        }

        private void SendCommand(int boxIndex, TASK_STATUS command)
        {
            ThrowIfDisposed();
            ValidateBoxIndex(boxIndex);
            _BoxTasks[boxIndex].RequestCommand(command);
            _WakeSignal.Set();
        }

        private void ScanLoop()
        {
            //Tool.SaveLogToFile("Aging scheduler scan thread started");

            while (!_StopRequested)
            {
                for (int boxIndex = 0; boxIndex < _BoxTasks.Length; boxIndex++)
                {
                    if (_StopRequested)
                        break;

                    AgingBoxTask boxTask = _BoxTasks[boxIndex];
                    if (!boxTask.IsActive)
                        continue;

                    try
                    {
                        boxTask.Run();
                    }
                    catch (Exception ex)
                    {
                        boxTask.FailFromScheduler(ex);
                    }
                }

                _WakeSignal.WaitOne(ScanIntervalMilliseconds);
            }

            Tool.SaveLogToFile("Aging scheduler scan thread stopped");
        }

        private void ValidateBoxIndex(int boxIndex)
        {
            if (boxIndex < 0 || boxIndex >= _BoxTasks.Length)
                throw new ArgumentOutOfRangeException(nameof(boxIndex));
        }

        private void ThrowIfDisposed()
        {
            if (_Disposed)
                throw new ObjectDisposedException(nameof(AgingScheduler));
        }

        public void Dispose()
        {
            if (_Disposed)
                return;

            _Disposed = true;
            _StopRequested = true;
            _WakeSignal.Set();

            if (Thread.CurrentThread != _ScanThread && _ScanThread.IsAlive)
                _ScanThread.Join(TimeSpan.FromSeconds(2));

            _WakeSignal.Dispose();
        }
    }
}
