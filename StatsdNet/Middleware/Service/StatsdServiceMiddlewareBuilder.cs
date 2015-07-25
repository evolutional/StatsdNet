using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using StatsdNet.Backend;

namespace StatsdNet.Middleware.Service
{
    public class StatsdServiceMiddlewareBuilder : IStatsdServiceMiddlewareBuilder
    {
        private StatsdServiceMiddlewareConfig _config;
        private readonly List<Func<IBackend>> _backendFactories = new List<Func<IBackend>>();

        private static bool TestArgForParameter(Type parameterType, object arg)
        {
            return (arg == null && !parameterType.IsValueType) ||
                   parameterType.IsInstanceOfType(arg);
        }
        
        private Func<IBackend> CreateBackendFactory(Type type, object[] args)
        {
            var constructors = type.GetConstructors();

            foreach (var ctor in constructors)
            {
                var parameters = ctor.GetParameters();
                var paramTypes = parameters.Select(i => i.ParameterType).ToArray();
                if (paramTypes.Length != args.Length)
                {
                    continue;
                }

                // Make sure the args all match up
                if (!paramTypes.Zip(args, TestArgForParameter)
                    .All(i => i))
                {
                    continue;
                }

                var paramExpr = parameters.Select(i => Expression.Parameter(i.ParameterType, i.Name)).ToArray();
                var callCtor = Expression.New(ctor, paramExpr);
                var createDelegate = Expression.Lambda<Func<IBackend>>(callCtor, paramExpr).Compile();
                return createDelegate;
            }

            return null;
        }

        public IStatsdServiceMiddlewareBuilder UseBackend(Type type, params object[] args)
        {
            var factory = CreateBackendFactory(type, args);

            if (factory == null)
            {
                throw new ArgumentException("Type isn't constructable with given arguments");
            }
            _backendFactories.Add(factory);
            return this;
        }

        public IStatsdServiceMiddlewareBuilder UseConfig(StatsdServiceMiddlewareConfig config)
        {
            _config = config;
            return this;
        }

        public StatsdServiceMiddleware Build(IMiddleware next)
        {
            var backends = _backendFactories.Select(i => i()).ToList();

            return new StatsdServiceMiddleware(next, backends, _config);
        }

        public StatsdServiceMiddleware Build()
        {
            return Build(new TerminalMiddleware());
        }
    }
}