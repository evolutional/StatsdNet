using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StatsdNet.Middleware;

namespace StatsdNet.Frontend
{
    public class UdpFrontend : IFrontend
    {
        private readonly IMiddleware _hostedMiddleware;
        private readonly UdpClient _udpClient;
        private bool _isStarted = false;

        public UdpFrontend(IMiddleware hostedMiddleware, IPEndPoint serverEndpoint)
        {
            _hostedMiddleware = hostedMiddleware;
            _udpClient = new UdpClient(serverEndpoint);
        }

        public Task Start(CancellationToken cancellationToken)
        {
            if (_isStarted)
            {
                throw new InvalidOperationException("Server is already started");
            }
            _isStarted = true;

            var dummy = Task.Run(() => Listen(cancellationToken), cancellationToken);
            return Task.FromResult(false);
        }

        public Task Stop()
        {
            _isStarted = false;
            return Task.FromResult(false);
        }

        private async Task Listen(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _isStarted)
            {
                var result = await _udpClient.ReceiveAsync();
                var packetString = Encoding.UTF8.GetString(result.Buffer);
                var context = new PacketData(result.RemoteEndPoint, packetString);
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
