using System.Threading.Tasks;

namespace StatsdNet.Client
{
    internal interface ITransport
    {
        Task SendAsync(byte[] data);
    }
}