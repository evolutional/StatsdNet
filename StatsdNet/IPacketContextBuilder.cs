using System.Net;

namespace StatsdNet
{
    public interface IPacketContextBuilder
    {
        IPacketContext Build(IPEndPoint sender, string packet);
    }
}