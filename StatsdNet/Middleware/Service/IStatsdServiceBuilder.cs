using System;
using StatsdNet.Backend;

namespace StatsdNet.Middleware.Service
{
    public interface IStatsdServiceBuilder
    {
        IStatsdServiceBuilder UseBackend(Type type, params object[] args);
        IStatsdService Build();
    }
}