using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Middleware
{
    public class TerminalMiddleware : MiddlewareBase
    {
        public TerminalMiddleware() : base(null)
        {
        }

        public override Task Start(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public override Task Invoke(IPacketContext context)
        {
            return Task.FromResult(true);
        }

        public override Task Stop()
        {
            return Task.FromResult(true);
        }
    }
}