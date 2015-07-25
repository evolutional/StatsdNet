using System.Collections.Concurrent;
using System.Collections.Generic;

namespace StatsdNet.Middleware.Service
{
    /// <summary>
    /// Represents the active snapshot
    /// </summary>
    internal class ActiveSnapshot : IMetricsSnapshot
    {
        private readonly ConcurrentDictionary<string, long> _counters = new ConcurrentDictionary<string, long>();
        private readonly ConcurrentDictionary<string, long> _gauges = new ConcurrentDictionary<string, long>();
        private readonly ConcurrentDictionary<string, SortedSet<long>> _sets = new ConcurrentDictionary<string, SortedSet<long>>();
        private readonly ConcurrentDictionary<string, List<long>> _timers = new ConcurrentDictionary<string, List<long>>();
        private readonly ConcurrentDictionary<string, long> _timerCounters = new ConcurrentDictionary<string, long>();

        public void IncCounter(string name)
        {
            AddCounter(name, 1, 1);
        }

        public void AddCounter(string name, long value, float sampleRate)
        {
            _counters.AddOrUpdate(name, value, (n, v) => v + (long)(value * sampleRate));
        }

        public void SetGauge(string name, long value)
        {
            _gauges.AddOrUpdate(name, value, (n, v) => value);
        }

        public void AddGauge(string name, long value)
        {
            _gauges.AddOrUpdate(name, value, (n, v) => v + value);
        }

        public void AddSet(string name, long value)
        {
            SortedSet<long> set;
          
            if (!_sets.TryGetValue(name, out set))
            {
                set = new SortedSet<long>();
                _sets.TryAdd(name, set);
            }

            set.Add(value);
        }

        public void AddTimer(string name, long value, float sampleRate)
        {
            List<long> timer;

            if (!_timers.TryGetValue(name, out timer))
            {
                timer = new List<long>();
                _timers.TryAdd(name, timer);
            }

            timer.Add(value);
            var adjValue = (long) (value/sampleRate);
            _timerCounters.AddOrUpdate(name, adjValue, (n, v) => v + adjValue);
        }

        public IDictionary<string, long> Counters
        {
            get { return _counters; }
        }

        public IDictionary<string, long> Gauges
        {
            get { return _gauges; }
        }

        public IDictionary<string, List<long>> Timers
        {
            get { return _timers; }
        }

        public IDictionary<string, long> TimerCounters
        {
            get { return _timerCounters; }
        }

        public IDictionary<string, SortedSet<long>> Sets
        {
            get { return _sets; }
        }
    }
}