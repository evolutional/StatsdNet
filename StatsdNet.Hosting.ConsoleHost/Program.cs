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
using StatsdNet.Middleware.Service;

namespace StatsdNet.Hosting.ConsoleHost
{
    class Program
    {
        static async Task Run()
        {
            var serviceBuilder = new StatsdServiceBuilder()
                .UseConsoleBackend();

            var host = new StatsdHostBuilder()
                .UseUdpFrontend(new IPEndPoint(IPAddress.Loopback, 6699))
                .UseTcpFrontend(new IPEndPoint(IPAddress.Loopback, 8125))
                .UseTraceLogMiddleware()
                .UseServiceBuilder(serviceBuilder)
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
