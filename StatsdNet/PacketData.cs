using System;
using System.Net;

namespace StatsdNet
{
    public class PacketData : IPacketData
    {
        public PacketData(Uri sender, string data)
            : this(sender, data, DateTimeOffset.Now)
        {
        }

        public PacketData(Uri sender, string data, DateTimeOffset timestamp)
        {
            Sender = sender;
            Data = data;
            Timestamp = timestamp;
        }

        public DateTimeOffset Timestamp { get; private set; }

        public string Data { get; private set; }

        public Uri Sender { get; private set; }
    }
}