﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StatsdNet.Backend;

namespace StatsdNet.Middleware.Service
{
    public class StatsdService : IStatsdService
    {
        private readonly IList<IBackend> _backends;
        private ActiveSnapshot _activeSnapshot;
        private DateTimeOffset _lastFlushTimestamp;
        private DateTimeOffset _serviceStartTimestamp;
        private readonly StatsdServiceConfig _config;
        private readonly Dictionary<string, Func<ParsedPacket,bool>> _handlers = new Dictionary<string, Func<ParsedPacket,bool>>();

        private readonly SemaphoreSlim _snapshotlock = new SemaphoreSlim(1);

        public StatsdService(IList<IBackend> backends, StatsdServiceConfig config = null)
        {
            _backends = backends;
            _handlers.Add(MetricTypeContants.Counter, HandleCounter);
            _handlers.Add(MetricTypeContants.Gauge, HandleGauge);
            _handlers.Add(MetricTypeContants.Set, HandleSet);
            _handlers.Add(MetricTypeContants.Timer, HandleTimer);
            _activeSnapshot = new ActiveSnapshot();
            _config = config ?? new StatsdServiceConfig();
        }

        public Task Start(CancellationToken cancellationToken)
        {
            var dummy = Task.Run(() => FlushSnapshots(_config.FlushInterval, cancellationToken), cancellationToken);
            _lastFlushTimestamp = _serviceStartTimestamp = DateTimeOffset.Now;

            return Task.WhenAll(_backends.Select(i => i.Start(cancellationToken)));
        }

        public Task Stop()
        {
            return Task.WhenAll(_backends.Select(i => i.Stop()));
        }

        private void IncCounter(string name)
        {
            AddCounter(name, 1, 1);
        }

        private void AddCounter(string name, long value, float sampleRate)
        {
           _activeSnapshot.AddCounter(name, value, sampleRate);
        }

        private void SetGauge(string name, long value)
        {
           _activeSnapshot.SetGauge(name, value);
        }

        private void AddGauge(string name, long value)
        {
            _activeSnapshot.AddGauge(name, value);
        }

        private void AddSet(string name, long value)
        {
            _activeSnapshot.AddSet(name, value);
        }

        private void AddTimer(string name, long value, float sampleRate)
        {
            _activeSnapshot.AddTimer(name, value, sampleRate);
        }
        
        private IMetricsSnapshot Snapshot(ref ActiveSnapshot activeSnapshot)
        {
            activeSnapshot.AddGauge(string.Format(StatsdServiceStatConstants.ServiceUptimeFormat, _config.ServiceStatsPrefix), (long)(DateTimeOffset.Now - _serviceStartTimestamp).TotalMilliseconds);

            var newSnapshot = new StaticSnapshot();
            
            newSnapshot.Counters = new Dictionary<string, long>(activeSnapshot.Counters);
            newSnapshot.Gauges = new Dictionary<string, long>(activeSnapshot.Gauges);
            newSnapshot.Sets = new Dictionary<string, SortedSet<long>>(activeSnapshot.Sets);
            newSnapshot.Timers = new Dictionary<string, List<long>>(activeSnapshot.Timers);

            if (_config.ClearKeys)
            {
                activeSnapshot.Counters.Clear();
                activeSnapshot.Sets.Clear();
                activeSnapshot.Timers.Clear();
                activeSnapshot.TimerCounters.Clear();
                // TODO: does DeleteGauges trump ClearKeys?
                activeSnapshot.Gauges.Clear();
            }
            else
            {
                foreach (var key in _activeSnapshot.Counters.Keys)
                {
                    activeSnapshot.Counters[key] = 0;
                }

                if (_config.DeleteGauges)
                {
                    foreach (var key in activeSnapshot.Gauges.Keys)
                    {
                        activeSnapshot.Gauges[key] = 0;
                    }
                }
                
                foreach (var key in activeSnapshot.Sets.Keys)
                {
                    activeSnapshot.Sets[key].Clear();
                }
                foreach (var key in activeSnapshot.Timers.Keys)
                {
                    activeSnapshot.Timers[key].Clear();
                    activeSnapshot.TimerCounters[key] = 0;
                }
            }

            return newSnapshot;
        }

        private async Task FlushSnapshots(TimeSpan flushInterval, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(flushInterval, cancellationToken);
                var snapshotTimestamp = DateTimeOffset.Now;

              
                await _snapshotlock.WaitAsync(cancellationToken);
                
                var snapshot = Snapshot(ref _activeSnapshot);

                _snapshotlock.Release();

                foreach (var backend in _backends)
                {
                    try
                    {
                        await backend.ReceiveSnapshotAsync(snapshotTimestamp, snapshot);
                    }
                    catch (Exception)
                    {
                        // todo: log
                    }
                }
                _lastFlushTimestamp = snapshotTimestamp;
            }
        }

        private bool HandleCounter(ParsedPacket packet)
        {
            AddCounter(packet.BucketName, packet.Value, packet.SampleRate);
            return true;
        }

        private bool HandleGauge(ParsedPacket packet)
        {
            if (packet.IsModifier)
            {
                // +/- adjusts the Gauge
                SetGauge(packet.BucketName, packet.Value);
            }
            else
            {
                // absolute set
                AddGauge(packet.BucketName, packet.Value);
            }
            return true;
        }

        private bool HandleSet(ParsedPacket packet)
        {
            AddSet(packet.BucketName, packet.Value);
            return true;
        }

        private bool HandleTimer(ParsedPacket packet)
        {
            AddTimer(packet.BucketName, packet.Value, packet.SampleRate);
            return true;
        }

        private void ProcessPacket(string packet, IPacketData context)
        {
            ParsedPacket parsedPacket;
            var packetResult = PacketParser.TryParse(packet, out parsedPacket);

            if (packetResult != PacketParseResult.Ok)
            {
                IncCounter(string.Format(StatsdServiceStatConstants.PacketBadFormat, _config.ServiceStatsPrefix));
                return;
            }
            
            Func<ParsedPacket, bool> handler;
            if (!_handlers.TryGetValue(parsedPacket.MetricType, out handler))
            {
                IncCounter(string.Format(StatsdServiceStatConstants.PacketBadFormat, _config.ServiceStatsPrefix));
                return;
            }

            if (!handler(parsedPacket))
            {
                IncCounter(string.Format(StatsdServiceStatConstants.PacketBadFormat, _config.ServiceStatsPrefix));
            }

            IncCounter(string.Format(StatsdServiceStatConstants.MetricsCountFormat, _config.ServiceStatsPrefix));
        }

        public Task Invoke(IPacketData context)
        {
            IncCounter(string.Format(StatsdServiceStatConstants.PacketCountFormat, _config.ServiceStatsPrefix));
            
            IEnumerable<string> packets;
            if (context.Data.Contains("\n"))
            {
                packets = context.Data.Split('\n');
            }
            else
            {
                packets = new[] { context.Data };
            }

            foreach (var packet in packets.Where(i => !string.IsNullOrWhiteSpace(i)).Select(i=>i.Replace("\r", "")))
            {
                ProcessPacket(packet, context);
            }

            // Done
            return Task.FromResult(true);
        }
    }
}
