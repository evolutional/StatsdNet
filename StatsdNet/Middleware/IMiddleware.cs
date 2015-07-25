using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Middleware
{
    public interface IMiddleware
    {
        void Start(CancellationToken cancellationToken);
        Task Invoke(IPacketContext context);
    }
}