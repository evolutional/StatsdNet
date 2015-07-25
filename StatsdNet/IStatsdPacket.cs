using System;
using System.Net;

namespace StatsdNet
{
    public interface IStatsdPacket
    {
        DateTimeOffset Timestamp { get; }
        string Data { get; }
        IPEndPoint Sender { get; }
    }
}