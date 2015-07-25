using System.Collections.Generic;

namespace StatsdNet.Backend
{
    internal class StaticSnapshot : IMetricsSnapshot
    {
        public IDictionary<string, long> Counters { get; set; }
        public IDictionary<string, long> Gauges { get; set; }
        public IDictionary<string, List<long>> Timers { get; set; }
        public IDictionary<string, long> TimerCounters { get; set; }
        public IDictionary<string, SortedSet<long>> Sets { get; set; }
    }
}