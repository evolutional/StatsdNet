using StatsdNet.Hosting;

namespace StatsdNet.Middleware
{
    public static class TraceLogMiddlewareExtensions
    {
        public static IHostBuilder UseTraceLogMiddleware(this IHostBuilder builder)
        {
            return builder.UseMiddleware(typeof (TraceLogMiddleware));
        }
    }
}