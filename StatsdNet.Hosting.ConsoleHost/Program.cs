using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StatsdNet.Backend;
using StatsdNet.Frontend;
using StatsdNet.Middleware;

namespace StatsdNet.Hosting.ConsoleHost
{
    class Program
    {
        static async void Run()
        {
            var host = new StatsdHostBuilder()
                .UseServer(typeof(UdpFrontend), new IPEndPoint(IPAddress.Loopback, 6699), new PacketContextBuilder(() => Console.Out))
                .UsePreMiddleware(typeof(TraceLogMiddleware))
                .UseBackend(typeof(ConsoleWriterMetricSnapshotBackend))
                .Build();

            var cts = new CancellationTokenSource();
            Console.WriteLine("Starting Service");
            host.Start(cts.Token);

            Console.WriteLine("\nPress a key to terminate...");
            Console.ReadKey();
            cts.Cancel();
            Console.WriteLine("Stopping service");
        }

        static void Main(string[] args)
        {
            Run();
        }
    }
}
