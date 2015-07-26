using System.Threading;

namespace StatsdNet.Frontend
{
    public interface IFrontend
    {
        void Start(CancellationToken cancellationToken);
    }
}