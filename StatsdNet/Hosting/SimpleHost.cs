using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StatsdNet.Frontend;
using StatsdNet.Middleware;

namespace StatsdNet.Hosting
{
    public class SimpleHost : IHost
    {
        private readonly IMiddleware _middleware;
        private readonly IList<IFrontend> _fontends;

        public SimpleHost(IMiddleware middleware, IList<IFrontend> fontends)
        {
            _middleware = middleware;
            _fontends = fontends;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            await _middleware.Start(cancellationToken);

            foreach (var frontend in _fontends)
            {
                await frontend.Start(cancellationToken);
            }
        }

        public async Task Stop()
        {
            foreach (var frontend in _fontends)
            {
                await frontend.Stop();
            }

            await _middleware.Stop();
        }
    }
}