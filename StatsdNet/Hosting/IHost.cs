using System.Threading;

namespace StatsdNet.Hosting
{
    public interface IHost
    {
        void Start(CancellationToken cancellationToken);
    }
}