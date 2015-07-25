using System;
using System.Threading.Tasks;

namespace StatsdNet.Backend
{
    /// <summary>
    /// Backend that Receives metrics snapshots from the service
    /// </summary>
    public interface IBackend
    {
        Task ReceiveSnapshotAsync(DateTimeOffset timestamp, IMetricsSnapshot snapshot);
    }

    /// <summary>
    /// Null-object backend
    /// </summary>
    public class NullBackend : IBackend
    {
        public Task ReceiveSnapshotAsync(DateTimeOffset timestamp, IMetricsSnapshot snapshot)
        {
            return Task.FromResult(true);
        }
    }
}