using Microsoft.ServiceBus.Messaging;
using StatsdNet.Middleware;

namespace StatsdNet.Azure.EventHub.Frontend
{
    public class StatsdFrontendEventProcessorFactory : IEventProcessorFactory
    {
        private readonly IMiddleware _hostedMiddleware;
        private readonly EventHubFrontendConfiguration _configuration;

        public StatsdFrontendEventProcessorFactory(IMiddleware hostedMiddleware, EventHubFrontendConfiguration configuration)
        {
            _hostedMiddleware = hostedMiddleware;
            _configuration = configuration;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new StatsdFrontendEventProcessor(_hostedMiddleware);
        }
    }
}