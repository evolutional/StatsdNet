using System;
using StatsdNet.Backend;

namespace StatsdNet.Middleware.Service
{
    public interface IStatsdServiceMiddlewareBuilder
    {
        IStatsdServiceMiddlewareBuilder UseBackend(Type type, params object[] args);
        StatsdServiceMiddleware Build(IMiddleware next);
        StatsdServiceMiddleware Build();
    }
}