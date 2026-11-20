using System;

namespace HistoryReader.Base
{
    public enum LogLevel
    {
        Error,
        Warning,
        Info,
        Debug,
        Other
    }

    public enum LogLevelFilter
    {
        All,
        Error,
        Warning,
        Info,
        Debug
    }

    /// <summary>
    /// One log event. FullText also contains any following exception or continuation lines.
    /// </summary>
    public sealed class LogEntry
    {
        public LogEntry(long id, DateTimeOffset? timestamp, LogLevel level, string message, string fullText)
        {
            Id = id;
            Timestamp = timestamp;
            Level = level;
            Message = message ?? string.Empty;
            FullText = fullText ?? Message;
        }

        public long Id { get; }
        public DateTimeOffset? Timestamp { get; }
        public LogLevel Level { get; }
        public string Message { get; }
        public string FullText { get; }

        internal LogEntry WithFullText(string fullText)
        {
            return new LogEntry(Id, Timestamp, Level, Message, fullText);
        }
    }
}
