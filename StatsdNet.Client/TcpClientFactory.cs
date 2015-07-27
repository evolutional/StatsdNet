using System.Net;
using System.Threading.Tasks;

namespace StatsdNet.Client
{
    public static class TcpClientFactory
    {
        public static Task<StatdsClient> CreateClient(IPEndPoint remoteEndpoint)
        {
            return CreateClient(remoteEndpoint, StatdsClientConfiguration.Default);
        }

        public static async Task<StatdsClient> CreateClient(IPEndPoint remoteEndpoint, StatdsClientConfiguration configuration)
        {
            var transport = new TcpTransport(remoteEndpoint);
            await transport.ConnectAsync();
            return new StatdsClient(transport, configuration);
        }
    }
}