using System;
using System.Net;

namespace StatsdNet
{
    public interface IPacketData
    {
        DateTimeOffset Timestamp { get; }
        string Data { get; }
        IPEndPoint Sender { get; }
    }
}