using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Middleware
{
    public abstract class MiddlewareBase : IMiddleware
    {
        public virtual Task Start(CancellationToken cancellationToken)
        {
            return Next.Start(cancellationToken);
        }

        public abstract Task Invoke(IPacketContext context);

        public virtual Task Stop()
        {
            return Next.Stop();
        }

        protected MiddlewareBase(IMiddleware next)
        {
            Next = next;
        }

        protected IMiddleware Next { get; private set; }
    }
}