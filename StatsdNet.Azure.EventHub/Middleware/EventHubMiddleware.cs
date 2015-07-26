using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using StatsdNet.Middleware;

namespace StatsdNet.Azure.EventHub.Middleware
{
    /// <summary>
    /// Middleware that writes the received packet to Azure EventHubs in Json format
    /// </summary>
    public class EventHubMiddleware : MiddlewareBase
    {
        private readonly EventHubMiddlewareConfiguration _config;
        private EventHubClient _eventHubClient;

        public EventHubMiddleware(IMiddleware next, EventHubMiddlewareConfiguration config)
            : base(next)
        {
            _config = config;
        }

        public override Task Start(CancellationToken cancellationToken)
        {
            _eventHubClient = EventHubClient.CreateFromConnectionString(_config.EventHubConnectionString);
            return Task.FromResult(false);
        }

        public override Task Invoke(IPacketContext context)
        {
            try
            {
                var json = JsonConvert.SerializeObject(context.Packet);
                var eventData = new EventData(Encoding.UTF8.GetBytes(json));
                return _eventHubClient.SendAsync(eventData);
            }
            catch (Exception)
            {
                // todo: log?
            }
            return Task.FromResult(false);
        }
    }
}
