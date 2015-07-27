using System.Net;

namespace StatsdNet.Client
{
    public static class UdpClientFactory
    {
        public static StatdsClient CreateClient(IPEndPoint remoteEndpoint)
        {
            return CreateClient(remoteEndpoint, StatdsClientConfiguration.Default);
        }

        public static StatdsClient CreateClient(IPEndPoint remoteEndpoint, StatdsClientConfiguration configuration)
        {
            var transport = new UdpTransport(remoteEndpoint);
            return new StatdsClient(transport, configuration);
        }
    }
}