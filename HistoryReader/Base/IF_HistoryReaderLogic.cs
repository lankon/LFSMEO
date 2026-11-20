using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HistoryReader.Base
{
    public interface IF_HistoryReaderLogic
    {
        long LiveVersion { get; }
        IReadOnlyList<LogEntry> GetLiveSnapshot();
        IReadOnlyList<LogEntry> Filter(IEnumerable<LogEntry> source, LogLevelFilter filter);
        Task<IReadOnlyList<string>> GetFilesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<IReadOnlyList<LogEntry>> LoadFileAsync(string fileName, CancellationToken cancellationToken = default(CancellationToken));
        Task<IReadOnlyList<LogEntry>> RefreshFileAsync(string fileName, CancellationToken cancellationToken = default(CancellationToken));
    }
}
