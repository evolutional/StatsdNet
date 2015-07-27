using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StatsdNet.Middleware
{
    public class TraceLogMiddleware : MiddlewareBase
    {
        public TraceLogMiddleware(IMiddleware next)
            : base(next)
        {
        }

        public override Task Invoke(IPacketData context)
        {
            var line = string.Format("[{0}: {1}] {2}", context.Timestamp, context.Sender, context.Data);
            Trace.WriteLine(line);
            return Next.Invoke(context);
        }
    }
}
