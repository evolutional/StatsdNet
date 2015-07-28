using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Backend
{
    public abstract class BackendBase : IBackend
    {
        public virtual Task Start(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public abstract Task ReceiveSnapshotAsync(DateTimeOffset timestamp, IMetricsSnapshot snapshot);

        public virtual Task Stop()
        {
            return Task.FromResult(true);
        }
    }
}