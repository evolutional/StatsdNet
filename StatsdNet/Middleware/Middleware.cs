using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Middleware
{
    public abstract class Middleware : IMiddleware
    {
        public virtual void Start(CancellationToken cancellationToken)
        {
            Next.Start(cancellationToken);
        }

        public abstract Task Invoke(IPacketContext context);

        protected Middleware(IMiddleware next)
        {
            Next = next;
        }

        protected IMiddleware Next { get; private set; }
    }
}