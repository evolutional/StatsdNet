using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace StatsdNet.Middleware
{
    public class TraceLogMiddleware : MiddlewareBase
    {
        public TraceLogMiddleware(MiddlewareBase next)
            : base(next)
        {
        }
        
        public override Task Invoke(IPacketContext context)
        {
            var line = string.Format("[{0}: {1}] {2}", context.Packet.Timestamp, context.Packet.Sender, context.Packet.Data);
            context.TraceOutput.WriteLine(line);
            return Next.Invoke(context);
        }
    }
}
