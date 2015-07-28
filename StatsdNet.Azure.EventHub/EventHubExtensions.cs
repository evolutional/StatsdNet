using StatsdNet.Azure.EventHub.Backend;
using StatsdNet.Azure.EventHub.Frontend;
using StatsdNet.Azure.EventHub.Middleware;
using StatsdNet.Hosting;
using StatsdNet.Middleware.Service;

namespace StatsdNet.Azure.EventHub
{
    public static class EventHubExtensions
    {
        public static IHostBuilder UseEventHubFrontend(this IHostBuilder builder, EventHubFrontendConfiguration config)
        {
            return builder.UseFrontend(typeof (EventHubFrontend), config);
        }

        public static IHostBuilder UseEventHubMiddleware(this IHostBuilder builder, EventHubMiddlewareConfiguration config)
        {
            return builder.UseFrontend(typeof(EventHubMiddleware), config);
        }

        public static IStatsdServiceBuilder UseEventHubBackend(this IStatsdServiceBuilder builder, EventHubBackendConfiguration config)
        {
            return builder.UseBackend(typeof(EventHubBackend), config);
        }
    }
}
