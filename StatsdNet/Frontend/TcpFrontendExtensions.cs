using System.Net;
using StatsdNet.Hosting;

namespace StatsdNet.Frontend
{
    public static class TcpFrontendExtensions
    {
        public static IHostBuilder UseTcpFrontend(this IHostBuilder builder, IPEndPoint serverEndPoint)
        {
            return builder.UseFrontend(typeof (TcpFrontend), serverEndPoint);
        }
    }
}