using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using StatsdNet.Middleware;

namespace StatsdNet.Azure.EventHub.Frontend
{
    public class StatsdFrontendEventProcessor : IEventProcessor
    {
        private readonly IMiddleware _hostedMiddeMiddleware;
        private readonly Stopwatch _checkpointStopwatch = new Stopwatch();
        private readonly IPEndPoint _dummyEndpoint = new IPEndPoint(IPAddress.None, 0);

        public StatsdFrontendEventProcessor(IMiddleware hostedMiddeMiddleware)
        {
            _hostedMiddeMiddleware = hostedMiddeMiddleware;
        }

        public Task OpenAsync(PartitionContext context)
        {
            return Task.FromResult(false);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var packetData = Encoding.UTF8.GetString(eventData.GetBytes());
                var packetContext = new PacketData(_dummyEndpoint, packetData);
                await _hostedMiddeMiddleware.Invoke(packetContext);
            }

            await context.CheckpointAsync();
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            if (reason == CloseReason.Shutdown)
            {
                return context.CheckpointAsync();
            }
            return Task.FromResult(false);
        }
    }
}