using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Middleware
{
    public class TerminalMiddleware : Middleware
    {
        public TerminalMiddleware() : base(null)
        {
        }

        public override void Start(CancellationToken cancellationToken)
        {
            return;
        }

        public override Task Invoke(IPacketContext context)
        {
            return Task.FromResult(true);
        }
    }
}