using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Client
{
    public class StatdsClient : IDisposable
    {
        private readonly ITransport _transport;
        private readonly StatdsClientConfiguration _configuration;
        private const string BasicFormat = "{0}:{1}|{2}";
        private const string ModifierFormat = "{0}:{1}{2}|{3}";
        private const string SampleFormat = "{0}:{1}|{2}|@{3}";
        private bool _disposed = false;
        private readonly BlockingCollection<string> _packets = new BlockingCollection<string>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        
        internal StatdsClient(ITransport transport, StatdsClientConfiguration configuration)
        {
            _transport = transport;
            _configuration = configuration;
            var dummy = Task.Run(() => FlushToTransport(_cts.Token));
        }

        public void Close()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
            _disposed = true;
        }

        private async Task FlushToTransport(CancellationToken cancellationToken)
        {
            while (!_packets.IsAddingCompleted && !cancellationToken.IsCancellationRequested)
            {
                var coalescedPackets = new StringBuilder();

                var packet = _packets.Take();

                coalescedPackets.AppendFormat("{0}\n", packet);

                while (_packets.TryTake(out packet))
                {
                    coalescedPackets.AppendFormat("{0}\n", packet);
                    if (coalescedPackets.Length >= _configuration.MaxCoalescedPacketSize)
                    {
                        break;
                    }
                }

                var packetData = Encoding.UTF8.GetBytes(coalescedPackets.ToString());
                await _transport.SendAsync(packetData);
            }
        }

        private void CheckCanSend()
        {
            if (_disposed)
            {
                throw new InvalidOperationException("Client was disposed");
            }
        }

        public void SetGauge(string bucket, long value)
        {
            CheckCanSend();
            EnqueuePacket(bucket, value, "g");
        }

        public void ModifyGauge(string bucket, long modifierValue)
        {
            CheckCanSend();
            EnqueueModifierPacket(bucket, modifierValue, "g");
        }

        public void AddCounter(string bucket, long value)
        {
            CheckCanSend();
            EnqueuePacket(bucket, value, "c");
        }

        public void AddCounter(string bucket, long value, float sampleInterval)
        {
            CheckCanSend();
            EnqueuePacket(bucket, value, sampleInterval, "c");
        }
        
        public void AddTimer(string bucket, long value)
        {
            CheckCanSend();
            EnqueuePacket(bucket, value, "ms");
        }

        public void AddTimer(string bucket, long value, float sampleInterval)
        {
            CheckCanSend();
            EnqueuePacket(bucket, value, sampleInterval, "ms");
        }

        private void EnqueueModifierPacket(string bucket, long value, string type)
        {
            _packets.Add(string.Format(ModifierFormat, bucket, value < 0 ? "-" : "+",  value, type));
        }

        private void EnqueuePacket(string bucket, long value, string type)
        {
            _packets.Add(string.Format(BasicFormat, bucket, value, type));
        }

        private void EnqueuePacket(string bucket, long value, float sampleInterval, string type)
        {
            _packets.Add(string.Format(SampleFormat, bucket, value, sampleInterval.ToString("f"), type));
        }

        public void Dispose()
        {
            Close();
        }
    }
}
