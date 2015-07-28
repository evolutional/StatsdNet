using System.Net;
using StatsdNet.Hosting;

namespace StatsdNet.Frontend
{
    public static class UdpFrontendExtensions
    {
        public static IHostBuilder UseUdpFrontend(this IHostBuilder builder, IPEndPoint serverEndPoint)
        {
            return builder.UseFrontend(typeof(UdpFrontend), serverEndPoint);
        }
    }
}