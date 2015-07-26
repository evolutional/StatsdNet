using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Backend
{
    /// <summary>
    /// Null-object backend
    /// </summary>
    public class NullBackend : IBackend
    {
        public Task Start(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task ReceiveSnapshotAsync(DateTimeOffset timestamp, IMetricsSnapshot snapshot)
        {
            return Task.FromResult(true);
        }

        public Task Stop()
        {
            return Task.FromResult(true);
        }
    }
}