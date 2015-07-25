using System.IO;

namespace StatsdNet
{
    public class PacketContext : IPacketContext
    {
        public PacketContext(IStatsdPacket packet, TextWriter traceOutput)
        {
            Packet = packet;
            TraceOutput = traceOutput;
        }

        public IStatsdPacket Packet { get; private set; }
        public TextWriter TraceOutput { get; private set; }
    }
}