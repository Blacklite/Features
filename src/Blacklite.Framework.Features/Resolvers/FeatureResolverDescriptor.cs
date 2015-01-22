using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Blacklite.Framework.Shared.Reflection;

namespace Blacklite.Framework.Features.Resolvers
{
    class FeatureResolverDescriptor: IFeatureResolverDescriptor
    {
        public IFeatureResolver Resolver { get; }

        private readonly IDictionary<Type, Func<IServiceProvider, IFeatureResolutionContext, object>> _resolversCache = new Dictionary<Type, Func<IServiceProvider, IFeatureResolutionContext, object>>();
        private readonly MethodInfo _resolveMethod;

        public bool IsGlobal { get; }

        public Type FeatureType { get; }

        public int Priority { get; }

        public FeatureResolverDescriptor(IFeatureResolver resolver)
        {
            Resolver = resolver;
            var typeInfo = Resolver.GetType().GetTypeInfo();

            _resolveMethod = typeInfo.DeclaredMethods.SingleOrDefault(x => x.Name == nameof(Resolve));

            FeatureType = resolver.GetFeatureType();
            IsGlobal = FeatureType == null;
            Priority = resolver.Priority;
        }

        public bool CanResolve<T>(IFeatureResolutionContext context) where T : IFeature
        {
            return Resolver.CanResolve<T>(context);
        }

        public T Resolve<T>(IFeatureResolutionContext context) where T : IFeature
        {
            Func<IServiceProvider, IFeatureResolutionContext, object> method;
            if (!_resolversCache.TryGetValue(typeof(T), out method))
            {
                var genericMethod = _resolveMethod.MakeGenericMethod(typeof(T));
                var contextTypeInfo = typeof(IFeatureResolutionContext).GetTypeInfo();

                method = genericMethod.CreateInjectableMethod()
                    .ConfigureParameter(x => contextTypeInfo.IsAssignableFrom(x.ParameterType.GetTypeInfo()))
                    .ReturnType(typeof(IFeature))
                    .CreateFunc<IFeatureResolutionContext, IFeature>(Resolver);

                _resolversCache.Add(typeof(T), method);
            }

            return (T)method(context.ServiceProvider, context);
        }
    }

}
