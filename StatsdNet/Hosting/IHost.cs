using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Hosting
{
    public interface IHost
    {
        Task Start(CancellationToken cancellationToken);
        Task Stop();
    }
}