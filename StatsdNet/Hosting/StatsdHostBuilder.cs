using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using StatsdNet.Backend;
using StatsdNet.Frontend;
using StatsdNet.Middleware;
using StatsdNet.Middleware.Service;

namespace StatsdNet.Hosting
{
    public class StatsdHostBuilder : IHostBuilder
    {
        private class TypeArgsPair
        {
            public Type Type { get; set; }
            public object[] Args { get; set; }
        }

        private readonly IList<TypeArgsPair> _frontends = new List<TypeArgsPair>();

        private readonly IList<TypeArgsPair> _preMiddleware = new List<TypeArgsPair>();
        private readonly IList<TypeArgsPair> _postMiddleware = new List<TypeArgsPair>();
        private readonly StatsdServiceMiddlewareBuilder _builder = new StatsdServiceMiddlewareBuilder();

        public IHostBuilder UseConfig(StatsdServiceMiddlewareConfig config)
        {
            _builder.UseConfig(config);
            return this;
        }

        public IHostBuilder UseFrontend(Type server, params object[] args)
        {
            // TODO: check it implements IFrontend
            _frontends.Add(new TypeArgsPair {Type = server, Args = args});
            return this;
        }

        public IHostBuilder UsePreMiddleware(Type middleware, params object[] args)
        {
            // TODO: check it implements the middle ware type
            _preMiddleware.Add(new TypeArgsPair { Type = middleware, Args = args });
            return this;
        }

        public IHostBuilder UsePostMiddleware(Type middleware, params object[] args)
        {
            // TODO: check it implements the middle ware type
            _postMiddleware.Add(new TypeArgsPair { Type = middleware, Args = args });
            return this;
        }

        public IHostBuilder UseBackend(Type backendType, params object[] args)
        {
            _builder.UseBackend(backendType, args);
            return this;
        }

        private static bool TestArgForParameter(Type parameterType, object arg) 
        { 
            return (arg == null && !parameterType.IsValueType) || 
                   parameterType.IsInstanceOfType(arg); 
        }


        private Delegate CreateObjectFactory(Type type, object[] args)
        {
            var constructors = type.GetConstructors();

            foreach (var ctor in constructors)
            {
                var parameters = ctor.GetParameters();
                var paramTypes = parameters.Select(i => i.ParameterType).ToArray();
                if (paramTypes.Length != args.Length + 1)
                {
                    continue;
                }

                // Make sure the args all match up
                if (!paramTypes.Skip(1).Zip(args, TestArgForParameter)
                    .All(i => i))
                {
                    continue;
                }

                var paramExpr = parameters.Select(i => Expression.Parameter(i.ParameterType, i.Name)).ToArray();
                var callCtor = Expression.New(ctor, paramExpr);
                var createDelegate = Expression.Lambda(callCtor, paramExpr).Compile();
                return createDelegate;
            }
    
            return null;
        }

        public IHost Build()
        {
            Middleware.MiddlewareBase middle = new TerminalMiddleware();

            foreach (var builder in _postMiddleware.Reverse())
            {
                var factory = CreateObjectFactory(builder.Type, builder.Args);
                var args = new[] {middle}.Concat(builder.Args).ToArray();
                middle = (Middleware.MiddlewareBase)factory.DynamicInvoke(args);
            }
            
            middle = _builder.Build(middle);

            foreach (var builder in _preMiddleware.Reverse())
            {
                var factory = CreateObjectFactory(builder.Type, builder.Args);
                var args = new[] { middle }.Concat(builder.Args).ToArray();
                middle = (Middleware.MiddlewareBase)factory.DynamicInvoke(args);
            }

            var frontends = new List<IFrontend>();

            foreach (var builder in _frontends)
            {
                var factory = CreateObjectFactory(builder.Type, builder.Args);
                var args = new[] { middle }.Concat(builder.Args).ToArray();
                var frontend = (IFrontend) factory.DynamicInvoke(args);
                frontends.Add(frontend);
            }

            var host = new SimpleHost(middle, frontends);

            return host;
        }
    }
}