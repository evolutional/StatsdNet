using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StatsdNet.Middleware;

namespace StatsdNet.Frontend
{
    public class TcpFrontend : IFrontend
    {
        private readonly IMiddleware _hostedMiddleware;
        private readonly IPacketContextBuilder _contextBuilder;
        private readonly TcpListener _listener;
        private bool _isStarted = false;

        public TcpFrontend(IMiddleware hostedMiddleware, IPEndPoint serverEndpoint, IPacketContextBuilder contextBuilder)
        {
            _hostedMiddleware = hostedMiddleware;
            _contextBuilder = contextBuilder;
            _listener = new TcpListener(serverEndpoint);
        }

        public Task Start(CancellationToken cancellationToken)
        {
            if (_isStarted)
            {
                throw new InvalidOperationException("Server is already started");
            }
            _isStarted = true;

            _listener.Start();
            var dummy = Task.Run(() => Listen(cancellationToken), cancellationToken);
            return Task.FromResult(false);
        }

        public Task Stop()
        {
            _listener.Stop();
            _isStarted = false;
            return Task.FromResult(false);
        }

        private async Task ReceiveClient(TcpClient client, CancellationToken cancellationToken)
        {
            var receiveBuffer = new byte[4096];
            var receiveIndex = 0;
            var hasError = false;
            
            while (!cancellationToken.IsCancellationRequested && _isStarted && !hasError)
            {
                try
                {
                    var stream = client.GetStream();
                    var bytesRead =
                        await stream.ReadAsync(receiveBuffer, receiveIndex, receiveBuffer.Length - receiveIndex, cancellationToken);

                    if (bytesRead <= 0)
                    {
                        break;
                    }

                    receiveIndex += bytesRead;

                    // look for the last newline char
                    for (var i = receiveIndex; i >= 0; --i)
                    {
                        if (receiveBuffer[i] == '\n')
                        {
                            var slice = new byte[i];
                            Buffer.BlockCopy(receiveBuffer, 0, slice, 0, i);
                            Buffer.BlockCopy(receiveBuffer, i+1, receiveBuffer, 0, receiveIndex - i);
                            receiveIndex = 0;

                            var packetString = Encoding.UTF8.GetString(slice);
                            var context = _contextBuilder.Build((IPEndPoint)client.Client.RemoteEndPoint, packetString);

                            await _hostedMiddleware.Invoke(context);
                            break;
                        }
                    }

                }
                catch (Exception)
                {
                    hasError = true;
                }
            }

            client.Close();
        }

        private async Task Listen(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _isStarted)
            {
                var client = await _listener.AcceptTcpClientAsync();
                var dummy = Task.Run(() => ReceiveClient(client, cancellationToken), cancellationToken);
            }
        }
    }
}
