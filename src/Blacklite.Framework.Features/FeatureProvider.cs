using Blacklite.Framework.Features.Resolvers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features
{
    public interface IFeatureProvider
    {
        T GetFeature<T>() where T : IFeature;
        IFeature GetFeature(Type type);
    }

    public interface IGlobalFeatureProvider : IFeatureProvider
    {
        IDictionary<Type, IFeatureDescriber> FeatureDescribers { get; }
    }

    public abstract class FeatureProviderBase : IFeatureProvider
    {
        private readonly IFeatureResolverProvider _featureResolverProvider;
        private readonly IServiceProvider _serviceProvider;
        protected ConcurrentDictionary<Type, IFeature> Features { get; } = new ConcurrentDictionary<Type, IFeature>();

        protected FeatureProviderBase(IServiceProvider serviceProvider, IFeatureResolverProvider featureResolverProvider)
        {
            _featureResolverProvider = featureResolverProvider;
            _serviceProvider = serviceProvider;
        }

        public T GetFeature<T>() where T : IFeature => (T)GetFeature(typeof(T));

        protected IFeature ResolveValue(Type type)
        {
            IEnumerable<IFeatureResolverDescriptor> values;
            if (_featureResolverProvider.Resolvers.TryGetValue(type, out values))
            {
                var context = new FeatureResolutionContext(_serviceProvider, type);
                var resolvedValue = values
                    .Where(z => z.CanResolve(context))
                    .Select(x => x.Resolve(context))
                    .FirstOrDefault(x => x != null);

                if (resolvedValue != null)
                {
                    return resolvedValue;
                }
            }

            throw new ArgumentOutOfRangeException("type", $"Feature type '{type.Name}' must have at least one resolver registered.");
        }

        public abstract IFeature GetFeature(Type type);
    }

    public class FeatureProvider : FeatureProviderBase
    {
        private readonly IGlobalFeatureProvider _globalFeatureProvider;
        public FeatureProvider(IServiceProvider serviceProvider, IFeatureResolverProvider featureResolverProvider, IGlobalFeatureProvider globalFeatureProvider)
            : base(serviceProvider, featureResolverProvider)
        {
            _globalFeatureProvider = globalFeatureProvider;
        }

        public override IFeature GetFeature(Type type)
        {
            var describer = _globalFeatureProvider.FeatureDescribers[type];

            var dependsOnSet = describer.DependsOn.Select(x => GetFeature(x.Key.FeatureType));

            if (describer.IsScoped)
            {
                var resolvedValue = ResolveValue(type);
                Features.TryAdd(type, resolvedValue);
                return resolvedValue;
            }

            IFeature value;
            if (!Features.TryGetValue(type, out value))
            {
                value = ResolveOther(type);

                if (value == null)
                    value = _globalFeatureProvider.GetFeature(type);

                Features.TryAdd(type, value);
            }

            return value;
        }

        public virtual IFeature ResolveOther(Type type)
        {
            return default(IFeature);
        }
    }

    class GlobalFeatureProvider : FeatureProviderBase, IGlobalFeatureProvider
    {
        public GlobalFeatureProvider(IServiceProvider serviceProvider, IFeatureDescriberProvider featureDescriberProvider, IFeatureResolverProvider featureResolverProvider)
            : base(serviceProvider, featureResolverProvider)
        {
            FeatureDescribers = featureDescriberProvider.Describers.ToDictionary(x => x.FeatureType);
        }

        public IDictionary<Type, IFeatureDescriber> FeatureDescribers { get; }

        public override IFeature GetFeature(Type type)
        {
            IFeature value;
            if (!Features.TryGetValue(type, out value))
            {
                value = ResolveValue(type);
            }

            return value;
        }
    }
}
