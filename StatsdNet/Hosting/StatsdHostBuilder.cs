using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        private readonly IList<TypeArgsPair> _middleware = new List<TypeArgsPair>();
        private IStatsdServiceBuilder _serviceBuilder;

        public IHostBuilder UseFrontend(Type type, params object[] args)
        {
            // TODO: check it implements IFrontend
            _frontends.Add(new TypeArgsPair {Type = type, Args = args});
            return this;
        }

        public IHostBuilder UseMiddleware(Type type, params object[] args)
        {
            // TODO: check it implements the middle ware type
            _middleware.Add(new TypeArgsPair { Type = type, Args = args });
            return this;
        }

        public IHostBuilder UseServiceBuilder(IStatsdServiceBuilder serviceBuilder)
        {
            _serviceBuilder = serviceBuilder;
            return this;
        }

        public IHostBuilder UseBackend(Type type, params object[] args)
        {
            _serviceBuilder.UseBackend(type, args);
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
            IMiddleware middle = new TerminalMiddleware();;

            if (_serviceBuilder != null)
            {
                middle = _serviceBuilder.Build();
            }

            foreach (var builder in _middleware.Reverse())
            {
                var factory = CreateObjectFactory(builder.Type, builder.Args);
                var args = new[] { middle }.Concat(builder.Args).ToArray();
                middle = (IMiddleware)factory.DynamicInvoke(args);
            }

            var frontends = new List<IFrontend>();

            foreach (var builder in _frontends)
            {
                var factory = CreateObjectFactory(builder.Type, builder.Args);
                var args = new[] { middle }.Concat(builder.Args).ToArray();
                var frontend = (IFrontend) factory.DynamicInvoke(args);
                frontends.Add(frontend);
            }
            return new SimpleHost(middle, frontends);
        }
    }
}