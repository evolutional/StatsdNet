using System.Collections.Generic;

namespace StatsdNet
{
    public interface IMetricsSnapshot
    {
        IDictionary<string, long> Counters { get; } 
        IDictionary<string, long> Gauges { get; }
        IDictionary<string, List<long>> Timers { get; }
        IDictionary<string, long> TimerCounters { get; } 
        IDictionary<string, SortedSet<long>> Sets { get; }  
    }
}