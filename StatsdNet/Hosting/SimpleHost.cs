using System.Collections.Generic;
using System.Threading;
using StatsdNet.Frontend;
using StatsdNet.Middleware;

namespace StatsdNet.Hosting
{
    public class SimpleHost : IHost
    {
        private readonly Middleware.Middleware _middleware;
        private readonly IList<IFrontend> _servers;

        public SimpleHost(Middleware.Middleware middleware, IList<IFrontend> servers)
        {
            _middleware = middleware;
            _servers = servers;
        }

        public void Start(CancellationToken cancellationToken)
        {
            _middleware.Start(cancellationToken);

            foreach (var server in _servers)
            {
                server.Start(cancellationToken);
            }
        }
    }
}