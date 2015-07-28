using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StatsdNet.Backend;
using StatsdNet.Middleware;
using StatsdNet.Middleware.Service;

namespace StatsdNet.Hosting
{
    public interface IHostBuilder
    {
        IHostBuilder UseFrontend(Type type, params object[] args);
        IHostBuilder UseMiddleware(Type type, params object[] args);
        IHostBuilder UseServiceBuilder(IStatsdServiceBuilder serviceBuilder);

        IHost Build();
    }
}
