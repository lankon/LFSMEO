using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HistoryReader.Base;

namespace HistoryReader.Logic
{
    /// <summary>
    /// Supplies at most 300 events from either the live sink or one history file.
    /// File operations run on a worker thread and never access WinForms controls.
    /// </summary>
    public sealed class F_HistoryReaderLogic : IF_HistoryReaderLogic
    {
        public F_HistoryReaderLogic(HistoryLogHub hub)
        {
            _hub = hub ?? throw new ArgumentNullException(nameof(hub));
            _historyFolder = hub.HistoryFolder;
        }

        private readonly Regex LogHeader = new Regex(
            @"^\[(?<time>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}(?:\.\d+)?)\]\s+\[(?<level>[^\]]+)\]\s?(?<message>.*)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly string[] TimestampFormats =
        {
            "yyyy-MM-dd HH:mm:ss.fff",
            "yyyy-MM-dd HH:mm:ss"
        };

        private readonly HistoryLogHub _hub;
        private readonly string _historyFolder;
        private readonly SemaphoreSlim _fileGate = new SemaphoreSlim(1, 1);
        private FileState _currentFile;
        private long _nextFileId;

        

        public long LiveVersion { get { return _hub.Version; } }

        public IReadOnlyList<LogEntry> GetLiveSnapshot()
        {
            return _hub.GetSnapshot();
        }

        public IReadOnlyList<LogEntry> Filter(IEnumerable<LogEntry> source, LogLevelFilter filter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (filter == LogLevelFilter.All)
                return source.ToArray();

            LogLevel level;
            switch (filter)
            {
                case LogLevelFilter.Error: level = LogLevel.Error; break;
                case LogLevelFilter.Warning: level = LogLevel.Warning; break;
                case LogLevelFilter.Info: level = LogLevel.Info; break;
                case LogLevelFilter.Debug: level = LogLevel.Debug; break;
                default: throw new ArgumentOutOfRangeException(nameof(filter));
            }

            return source.Where(entry => entry.Level == level).ToArray();
        }

        public Task<IReadOnlyList<string>> GetFilesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run<IReadOnlyList<string>>(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                Directory.CreateDirectory(_historyFolder);
                var files = new DirectoryInfo(_historyFolder)
                    .EnumerateFiles("*.log", SearchOption.TopDirectoryOnly)
                    .OrderByDescending(file => file.LastWriteTimeUtc)
                    .ThenByDescending(file => file.Name, StringComparer.OrdinalIgnoreCase)
                    .Select(file => file.Name)
                    .ToArray();
                cancellationToken.ThrowIfCancellationRequested();
                return files;
            }, cancellationToken);
        }

        public Task<IReadOnlyList<LogEntry>> LoadFileAsync(
            string fileName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ReadFileAsync(fileName, true, cancellationToken);
        }

        public Task<IReadOnlyList<LogEntry>> RefreshFileAsync(
            string fileName, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ReadFileAsync(fileName, false, cancellationToken);
        }

        private async Task<IReadOnlyList<LogEntry>> ReadFileAsync(
            string fileName, bool forceReload, CancellationToken cancellationToken)
        {
            string path = GetSafePath(fileName);
            await _fileGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await Task.Run<IReadOnlyList<LogEntry>>(
                    () => ReadFile(path, forceReload, cancellationToken), cancellationToken)
                    .ConfigureAwait(false);
            }
            finally
            {
                _fileGate.Release();
            }
        }

        private IReadOnlyList<LogEntry> ReadFile(
            string path, bool forceReload, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete, 65536, FileOptions.SequentialScan))
            {
                DateTime createdUtc = File.GetCreationTimeUtc(path);
                FileState working = forceReload || _currentFile == null ||
                    !string.Equals(_currentFile.Path, path, StringComparison.OrdinalIgnoreCase) ||
                    _currentFile.CreatedUtc != createdUtc || stream.Length < _currentFile.Offset
                    ? new FileState(path, createdUtc)
                    : _currentFile.Copy();

                stream.Seek(working.Offset, SeekOrigin.Begin);
                using (var pendingLine = new MemoryStream())
                {
                    pendingLine.Write(working.PendingBytes, 0, working.PendingBytes.Length);
                    var buffer = new byte[65536];
                    int count;
                    while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        ProcessBytes(buffer, count, pendingLine, working);
                    }

                    working.Offset = stream.Position;
                    working.PendingBytes = pendingLine.ToArray();
                }

                // Publish only a complete scan. A cancelled read leaves the prior state intact.
                cancellationToken.ThrowIfCancellationRequested();
                _currentFile = working;
                return working.Entries.ToArray();
            }
        }

        private void ProcessBytes(byte[] buffer, int count, MemoryStream pendingLine, FileState working)
        {
            int start = 0;
            for (int i = 0; i < count; i++)
            {
                if (buffer[i] != (byte)'\n')
                    continue;

                pendingLine.Write(buffer, start, i - start);
                byte[] lineBytes = pendingLine.ToArray();
                int length = lineBytes.Length;
                if (length > 0 && lineBytes[length - 1] == (byte)'\r')
                    length--;

                string line = Encoding.UTF8.GetString(lineBytes, 0, length);
                if (line.Length > 0 && line[0] == '\uFEFF')
                    line = line.Substring(1);
                AppendLine(working, line);

                pendingLine.SetLength(0);
                pendingLine.Position = 0;
                start = i + 1;
            }

            if (start < count)
                pendingLine.Write(buffer, start, count - start);
        }

        private void AppendLine(FileState working, string line)
        {
            Match match = LogHeader.Match(line);
            if (match.Success)
            {
                DateTimeOffset timestamp;
                DateTimeOffset? parsedTime = DateTimeOffset.TryParseExact(
                    match.Groups["time"].Value, TimestampFormats,
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out timestamp)
                    ? timestamp
                    : (DateTimeOffset?)null;

                AddEntry(working, new LogEntry(
                    Interlocked.Increment(ref _nextFileId),
                    parsedTime,
                    ParseLevel(match.Groups["level"].Value),
                    match.Groups["message"].Value,
                    line));
                working.LastEntryCanContinue = true;
            }
            else if (working.LastEntryCanContinue && working.Entries.Count > 0)
            {
                var previous = working.Entries.Last.Value;
                working.Entries.Last.Value = previous.WithFullText(
                    previous.FullText + Environment.NewLine + line);
            }
            else
            {
                AddEntry(working, new LogEntry(
                    Interlocked.Increment(ref _nextFileId), null,
                    LogLevel.Other, line, line));
                working.LastEntryCanContinue = false;
            }
        }

        private void AddEntry(FileState working, LogEntry entry)
        {
            if (working.Entries.Count == HistoryLogHub.Capacity)
                working.Entries.RemoveFirst();
            working.Entries.AddLast(entry);
        }

        private string GetSafePath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) ||
                !string.Equals(fileName, Path.GetFileName(fileName), StringComparison.Ordinal) ||
                !string.Equals(Path.GetExtension(fileName), ".log", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Select a .log file from the History folder.", nameof(fileName));

            return Path.Combine(_historyFolder, fileName);
        }

        private LogLevel ParseLevel(string level)
        {
            switch (level.Trim().ToUpperInvariant())
            {
                case "ERR":
                case "ERROR":
                case "FTL":
                case "FATAL":
                    return LogLevel.Error;
                case "WRN":
                case "WARN":
                case "WARNING":
                    return LogLevel.Warning;
                case "INF":
                case "INFO":
                case "INFORMATION":
                    return LogLevel.Info;
                case "DBG":
                case "DEBUG":
                case "VRB":
                case "VERBOSE":
                    return LogLevel.Debug;
                default:
                    return LogLevel.Other;
            }
        }

        private sealed class FileState
        {
            public FileState(string path, DateTime createdUtc)
            {
                Path = path;
                CreatedUtc = createdUtc;
                Entries = new LinkedList<LogEntry>();
                PendingBytes = new byte[0];
            }

            public string Path { get; }
            public DateTime CreatedUtc { get; }
            public long Offset { get; set; }
            public byte[] PendingBytes { get; set; }
            public LinkedList<LogEntry> Entries { get; }
            public bool LastEntryCanContinue { get; set; }

            public FileState Copy()
            {
                var copy = new FileState(Path, CreatedUtc)
                {
                    Offset = Offset,
                    PendingBytes = (byte[])PendingBytes.Clone(),
                    LastEntryCanContinue = LastEntryCanContinue
                };
                foreach (var entry in Entries)
                    copy.Entries.AddLast(entry);
                return copy;
            }
        }
    }
}
