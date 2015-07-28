using StatsdNet.Middleware.Service;

namespace StatsdNet.Backend
{
    public static class BackendExtensions
    {
        public static IStatsdServiceBuilder UseConsoleBackend(this IStatsdServiceBuilder builder)
        {
            return builder.UseBackend(typeof (ConsoleBackend));
        }

        public static IStatsdServiceBuilder UseNullBackend(this IStatsdServiceBuilder builder)
        {
            return builder.UseBackend(typeof(NullBackend));
        }
    }
}