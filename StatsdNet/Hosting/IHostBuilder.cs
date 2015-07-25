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
        IHostBuilder UseConfig(StatsdServiceMiddlewareConfig config);
        IHostBuilder UseServer(Type server, params object[] args);
        IHostBuilder UsePreMiddleware(Type middlewareType, params object[] args);
        IHostBuilder UsePostMiddleware(Type middlewareType, params object[] args);
        IHostBuilder UseBackend(Type backendType, params object[] args);

        IHost Build();
    }
}
