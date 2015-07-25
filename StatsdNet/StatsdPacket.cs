using System;
using System.Net;

namespace StatsdNet
{
    public class StatsdPacket : IStatsdPacket
    {
        public StatsdPacket(IPEndPoint sender, string data)
            : this(sender, data, DateTimeOffset.Now)
        {
        }

        public StatsdPacket(IPEndPoint sender, string data, DateTimeOffset timestamp)
        {
            Sender = sender;
            Data = data;
            Timestamp = timestamp;
        }

        public DateTimeOffset Timestamp { get; private set; }

        public string Data { get; private set; }

        public IPEndPoint Sender { get; private set; }
    }
}