using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StatsdNet.Middleware;

namespace StatsdNet.Frontend
{
    public class UdpServer : IServer
    {
        private readonly Middleware.Middleware _hostedMiddleware;
        private readonly IPacketContextBuilder _contextBuilder;
        private readonly UdpClient _udpClient;
        private bool _isStarted = false;

        public UdpServer(Middleware.Middleware hostedMiddleware, IPEndPoint serverEndpoint, IPacketContextBuilder contextBuilder)
        {
            _hostedMiddleware = hostedMiddleware;
            _contextBuilder = contextBuilder;
            _udpClient = new UdpClient(serverEndpoint);
        }

        public void Start(CancellationToken cancellationToken)
        {
            if (_isStarted)
            {
                throw new InvalidOperationException("Server is already started");
            }
            Task.Run(() => Listen(cancellationToken), cancellationToken);
            _isStarted = true;
        }
        
        private async Task Listen(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _udpClient.ReceiveAsync();
                var packetString = Encoding.UTF8.GetString(result.Buffer);
                var context = _contextBuilder.Build(result.RemoteEndPoint, packetString);
                try
                {
                    await _hostedMiddleware.Invoke(context);
                }
                catch (Exception)
                {
                    // todo: trace
                }
            }
        }
    }
}
