using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Backend
{
    /// <summary>
    /// Null-object backend
    /// </summary>
    public class NullBackend : BackendBase
    {
        public override Task ReceiveSnapshotAsync(DateTimeOffset timestamp, IMetricsSnapshot snapshot)
        {
            return Task.FromResult(true);
        }
    }
}