using System.Threading;

namespace StatsdNet.Frontend
{
    public interface IServer
    {
        void Start(CancellationToken cancellationToken);
    }
}