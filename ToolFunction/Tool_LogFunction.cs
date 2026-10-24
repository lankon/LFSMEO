using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serilog;

namespace ToolFunction
{
    /// <summary>
    /// Log Function
    /// </summary>
    public static partial class Tool
    {
        public static ILogger CreateLog(String Name = "History\\Log_", String Type = ".log")
        {
            return CreateLog(Name, Type, null);
        }

        public static ILogger CreateLog(String Name, String Type, Serilog.Core.ILogEventSink sink)
        {
            String path;
            path = System.IO.Directory.GetCurrentDirectory();
            path = path + "\\" + Name;
            path += Type;

            var configuration = new LoggerConfiguration().MinimumLevel.Debug();
            
            if (sink != null)
                configuration.WriteTo.Sink(sink);   // 將 HistoryLogHub 接到 logger，後續寫入日誌時由 Serilog 呼叫其 Emit()。
            var logger = configuration.WriteTo.File(path: path,
                              rollingInterval: RollingInterval.Day,
                              outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                              buffered: true,   //啟用緩衝
                              flushToDiskInterval: TimeSpan.FromSeconds(5)  //5s
                             )
                .CreateLogger();

            if (Name == "History\\Log_")
            {
                Log.Logger = logger;
            }

            return logger;
        }

        public static void SaveLogToFile(String Msg, ILogger log = null, string level = "DBG")
        {
            if (log == null)
                log = Log.Logger;

            switch (level)
            {
                case "DBG": log.Debug(Msg); break;
                case "INF": log.Information(Msg); break;
                case "WRN": log.Warning(Msg); break;
                case "ERR": log.Error(Msg); break;
                default: log.Debug(Msg); break;
            }
        }
        public static void SaveExceptionToFile(Exception ex, string msg = "")
        {
            string context =
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [ERR] {msg}\r\n" +
                $"{ex}\r\n";

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "History");
            Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, $"Exception_{DateTime.Now:yyyyMMdd}.log");
            File.AppendAllText(path, context, Encoding.UTF8);

            //SaveLogToFile($"{msg} {ex}", level: "ERR");
        }
        public static void CloseLog()
        {
            Log.CloseAndFlush();
        }
    }
}
