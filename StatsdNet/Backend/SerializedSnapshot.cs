using System;

namespace StatsdNet.Backend
{
    public class SerializedSnapshot
    {
        public DateTimeOffset Timestamp { get; set; }
        public IMetricsSnapshot Data { get; set; }
    }
}