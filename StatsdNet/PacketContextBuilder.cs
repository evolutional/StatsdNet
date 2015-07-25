using System;
using System.IO;
using System.Net;

namespace StatsdNet
{
    public class PacketContextBuilder : IPacketContextBuilder
    {
        private readonly Func<TextWriter> _traceFactory;

        public PacketContextBuilder(Func<TextWriter> traceFactory)
        {
            _traceFactory = traceFactory;
        }

        public IPacketContext Build(IPEndPoint sender, string packet)
        {
            return new PacketContext(new StatsdPacket(sender, packet), _traceFactory());
        }
    }
}