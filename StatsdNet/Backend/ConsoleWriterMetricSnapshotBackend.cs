using System;
using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Backend
{
    public class ConsoleWriterMetricSnapshotBackend : IBackend
    {
        public Task Start(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task ReceiveSnapshotAsync(DateTimeOffset timestamp, IMetricsSnapshot snapshot)
        {
            Console.WriteLine("Start snapshot from: {0}", timestamp);
            
            Console.WriteLine("\nCounters\n--------");
            foreach (var counter in snapshot.Counters)
            {
                Console.WriteLine("{0}\t = {1}", counter.Key, counter.Value);
            }
            if (snapshot.Counters.Count == 0)
            {
                Console.WriteLine("None");
            }

            Console.WriteLine("\nGauges\n------");
            foreach (var gauge in snapshot.Gauges)
            {
                Console.WriteLine("{0}\t = {1}", gauge.Key, gauge.Value);
            }
            if (snapshot.Gauges.Count == 0)
            {
                Console.WriteLine("None");
            }

            Console.WriteLine("\nSets\n------");
            foreach (var set in snapshot.Sets)
            {
                Console.WriteLine("{0}\t = {1}", set.Key, string.Join(", ", set.Value));
            }
            if (snapshot.Sets.Count == 0)
            {
                Console.WriteLine("None");
            }

            Console.WriteLine("\nTimers\n------");
            foreach (var timer in snapshot.Timers)
            {
                Console.WriteLine("{0}\t = {1}ms", timer.Key, timer.Value);
            }
            if (snapshot.Timers.Count == 0)
            {
                Console.WriteLine("None");
            }
            Console.WriteLine("End snapshot from: {0}", timestamp);
            Console.WriteLine("\n");

            return Task.FromResult(true);
        }

        public Task Stop()
        {
            return Task.FromResult(true);
        }
    }
}