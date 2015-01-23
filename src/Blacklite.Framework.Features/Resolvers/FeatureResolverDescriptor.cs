using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Blacklite.Framework.Shared.Reflection;

namespace Blacklite.Framework.Features.Resolvers
{
    class FeatureResolverDescriptor : IFeatureResolverDescriptor
    {
        private readonly Func<IServiceProvider, IFeatureResolutionContext, IFeature> _resolveFunc;

        public IFeatureResolver Resolver { get; }

        public bool IsGlobal { get; }

        public Type FeatureType { get; }

        public int Priority { get; }

        public FeatureResolverDescriptor(IFeatureResolver resolver)
        {
            Resolver = resolver;

            var resolveMethod = resolver.GetType().GetTypeInfo().DeclaredMethods.SingleOrDefault(x => x.Name == nameof(Resolve));

            FeatureType = resolver.GetFeatureType();
            IsGlobal = FeatureType == null;
            Priority = resolver.Priority;

            var contextTypeInfo = typeof(IFeatureResolutionContext).GetTypeInfo();

            _resolveFunc = resolveMethod.CreateInjectableMethod()
                .ConfigureParameter(x => contextTypeInfo.IsAssignableFrom(x.ParameterType.GetTypeInfo()))
                .ReturnType(typeof(IFeature))
                .CreateFunc<IFeatureResolutionContext, IFeature>(resolver);
        }

        public bool CanResolve(IFeatureResolutionContext context)
        {
            return Resolver.CanResolve(context);
        }

        public IFeature Resolve(IFeatureResolutionContext context)
        {
            return _resolveFunc(context.ServiceProvider, context);
        }
    }

}
