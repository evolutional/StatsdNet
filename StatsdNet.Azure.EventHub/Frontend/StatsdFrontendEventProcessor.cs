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

        public StatsdFrontendEventProcessor(IMiddleware hostedMiddeMiddleware)
        {
            _hostedMiddeMiddleware = hostedMiddeMiddleware;
        }

        public Task OpenAsync(PartitionContext context)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Make a 
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        private Uri MakeUri(EventData eventData)
        {
            var uriString = string.Format("{0}/{1}/{2}",
                eventData.SystemProperties[EventDataSystemPropertyNames.PartitionKey],
                eventData.SystemProperties[EventDataSystemPropertyNames.Offset],
                eventData.SystemProperties[EventDataSystemPropertyNames.Publisher]);
            var builder = new UriBuilder("eventhub", uriString);
            return builder.Uri;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var packetData = Encoding.UTF8.GetString(eventData.GetBytes());
               
                var packetContext = new PacketData(MakeUri(eventData), packetData);
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