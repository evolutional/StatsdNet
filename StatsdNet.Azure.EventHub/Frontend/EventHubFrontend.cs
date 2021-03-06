﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using StatsdNet.Frontend;
using StatsdNet.Middleware;

namespace StatsdNet.Azure.EventHub.Frontend
{
    public class EventHubFrontend : IFrontend
    {
        private readonly EventProcessorHost _eventProcessorHost;
        private readonly StatsdFrontendEventProcessorFactory _eventProcessorFactory;

        public EventHubFrontend(IMiddleware hostedMiddleware, EventHubFrontendConfiguration configuration)
        {
            _eventProcessorFactory = new StatsdFrontendEventProcessorFactory(hostedMiddleware, configuration);
            _eventProcessorHost = new EventProcessorHost(configuration.EventProcessorName, configuration.EventHubName, EventHubConsumerGroup.DefaultGroupName, configuration.EventHubConnectionString, configuration.StorageConnectionString);
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            await _eventProcessorHost.RegisterEventProcessorFactoryAsync(_eventProcessorFactory);
        }

        public Task Stop()
        {
            return _eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}
