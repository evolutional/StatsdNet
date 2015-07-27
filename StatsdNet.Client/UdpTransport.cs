using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace StatsdNet.Client
{
    internal class UdpTransport : ITransport
    {
        private readonly UdpClient _client;
        private readonly IPEndPoint _remoteEndpoint;

        public UdpTransport(IPEndPoint remoteEndpoint)
        {
            _client = new UdpClient();
            _remoteEndpoint = remoteEndpoint;
        }

        public Task SendAsync(byte[] data)
        {
            return _client.SendAsync(data, data.Length, _remoteEndpoint);
        }
    }
}