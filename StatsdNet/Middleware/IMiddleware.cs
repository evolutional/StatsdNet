using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Middleware
{
    public interface IMiddleware
    {
        Task Start(CancellationToken cancellationToken);
        Task Invoke(IPacketData context);
        Task Stop();
    }
}