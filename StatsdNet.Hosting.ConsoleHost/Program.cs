using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StatsdNet.Azure.EventHub.Frontend;
using StatsdNet.Backend;
using StatsdNet.Frontend;
using StatsdNet.Middleware;

namespace StatsdNet.Hosting.ConsoleHost
{
    class Program
    {
        static async Task Run()
        {
            var host = new StatsdHostBuilder()
                .UseFrontend(typeof(UdpFrontend), new IPEndPoint(IPAddress.Loopback, 6699))
                .UseFrontend(typeof(TcpFrontend), new IPEndPoint(IPAddress.Loopback, 8125))
                .UsePreMiddleware(typeof(TraceLogMiddleware))
                .UseBackend(typeof(ConsoleWriterMetricSnapshotBackend))
                .Build();

            var cts = new CancellationTokenSource();
            Console.WriteLine("Starting Service");
            await host.Start(cts.Token);

            Console.WriteLine("\nPress a key to terminate...");
            Console.ReadKey();
            cts.Cancel();
            Console.WriteLine("Stopping service");
            await host.Stop();
        }

        static void Main(string[] args)
        {
            Run().Wait();
        }
    }
}
