using System.Threading;
using System.Threading.Tasks;

namespace StatsdNet.Frontend
{
    public interface IFrontend
    {
        Task Start(CancellationToken cancellationToken);
        Task Stop();
    }
}