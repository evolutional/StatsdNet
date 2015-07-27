using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace StatsdNet.Client
{
    internal class TcpTransport : ITransport
    {
        private readonly TcpClient _client;
        private readonly IPEndPoint _remoteEndpoint;

        public TcpTransport(IPEndPoint remoteEndpoint)
        {
            _client = new TcpClient();
            _remoteEndpoint = remoteEndpoint;
        }

        public Task ConnectAsync()
        {
            return _client.ConnectAsync(_remoteEndpoint.Address, _remoteEndpoint.Port);
        }

        public Task SendAsync(byte[] data)
        {
            return _client.GetStream().WriteAsync(data, 0, data.Length);
        }
    }
}