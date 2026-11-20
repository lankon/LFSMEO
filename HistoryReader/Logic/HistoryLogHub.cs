using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using HistoryReader.Base;
using Serilog.Core;
using Serilog.Events;

namespace HistoryReader.Logic
{
    public sealed class HistoryLogHub : ILogEventSink
    {
        // 接收Serilog訊息,存放Log訊息
        public HistoryLogHub(string historyFolder)
        {
            _historyFolder = Path.GetFullPath(historyFolder);
        }

        #region parameter
        public const int Capacity = 300;                // 最多顯示數量
        public string HistoryFolder { get { return _historyFolder; } }  // 歷史紀錄路徑
        public long Version { get { lock (_gate) return _version; } }     // 用來判斷log是否有更新(執行續安全)

        private int _nextSlot;
        private int _count;             // 目前緩衝區內有效的紀錄數量，最多為 Capacity
        private long _nextId;           // 下一筆 log 的識別碼來源，每新增一筆就遞增
        private long _version;          // 用來判斷log是否有更新
        private readonly object _gate = new object();   // 防止MultiThread同時讀取或修改
        private readonly string _historyFolder;         // 歷史紀錄路徑
        private readonly LogEntry[] _entries = new LogEntry[Capacity];  // 保存最近的 log 紀錄
        #endregion

        #region private function
        private LogLevel MapLevel(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Fatal:
                case LogEventLevel.Error:
                    return LogLevel.Error;
                case LogEventLevel.Warning:
                    return LogLevel.Warning;
                case LogEventLevel.Information:
                    return LogLevel.Info;
                case LogEventLevel.Debug:
                case LogEventLevel.Verbose:
                    return LogLevel.Debug;
                default:
                    return LogLevel.Other;
            }
        }

        #endregion

        #region public function
        public void Emit(LogEvent logEvent) // SaveLogToFile會觸發執行
        {
            if (logEvent == null)
                throw new ArgumentNullException(nameof(logEvent));

            string message = logEvent.RenderMessage(CultureInfo.CurrentCulture);
            string fullText = logEvent.Exception == null ? message : message + Environment.NewLine + logEvent.Exception;

            lock (_gate)
            {
                var entry = new LogEntry(++_nextId, logEvent.Timestamp, MapLevel(logEvent.Level), message, fullText);
                _entries[_nextSlot] = entry;
                _nextSlot = (_nextSlot + 1) % Capacity;

                if (_count < Capacity)
                    _count++;

                _version++;
            }
        }

        public IReadOnlyList<LogEntry> GetSnapshot()
        {
            lock (_gate)
            {
                var result = new LogEntry[_count];
                int first = (_nextSlot - _count + Capacity) % Capacity;
                for (int i = 0; i < _count; i++)
                    result[i] = _entries[(first + i) % Capacity];

                return result;
            }
        }
        #endregion
    }
}
