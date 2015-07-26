using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using StatsdNet.Backend;

namespace StatsdNet.Azure.EventHub.Backend
{
    /// <summary>
    /// Backend that writes the received snapshot to Azure EventHubs in Json format
    /// </summary>
    public class EventHubBackend : IBackend
    {
        private readonly EventHubBackendConfiguration _config;
        private EventHubClient _eventHubClient;

        public EventHubBackend(EventHubBackendConfiguration config)
        {
            _config = config;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            _eventHubClient = EventHubClient.CreateFromConnectionString(_config.EventHubConnectionString);
            return Task.FromResult(false);
        }

        public Task ReceiveSnapshotAsync(DateTimeOffset timestamp, IMetricsSnapshot snapshot)
        {
            var json = JsonConvert.SerializeObject(new SerializedSnapshot { Timestamp = timestamp, Data = snapshot });
            var eventData = new EventData(Encoding.UTF8.GetBytes(json));
            return _eventHubClient.SendAsync(eventData);
        }

        public Task Stop()
        {
            if (_eventHubClient == null || _eventHubClient.IsClosed)
            {
                return Task.FromResult(false);
            }

            return _eventHubClient.CloseAsync();
        }
    }
}
