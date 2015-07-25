using System.IO;

namespace StatsdNet
{
    public interface IPacketContext
    {
        IStatsdPacket Packet { get; }
        TextWriter TraceOutput { get; }
    }
}