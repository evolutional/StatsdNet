using Microsoft.ServiceBus.Messaging;
using StatsdNet.Middleware;

namespace StatsdNet.Azure.EventHub.Frontend
{
    public class StatsdFrontendEventProcessorFactory : IEventProcessorFactory
    {
        private readonly IMiddleware _hostedMiddleware;
        private readonly IPacketContextBuilder _contextBuilder;
        private readonly EventHubFrontendConfiguration _configuration;

        public StatsdFrontendEventProcessorFactory(IMiddleware hostedMiddleware, IPacketContextBuilder contextBuilder,
            EventHubFrontendConfiguration configuration)
        {
            _hostedMiddleware = hostedMiddleware;
            _contextBuilder = contextBuilder;
            _configuration = configuration;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new StatsdFrontendEventProcessor(_hostedMiddleware, _contextBuilder);
        }
    }
}