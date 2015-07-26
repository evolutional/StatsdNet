using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Backend
{
    /// <summary>
    /// Backend that Receives metrics snapshots from the service
    /// </summary>
    public interface IBackend
    {
        Task Start(CancellationToken cancellationToken);
        Task ReceiveSnapshotAsync(DateTimeOffset timestamp, IMetricsSnapshot snapshot);
        Task Stop();
    }
}