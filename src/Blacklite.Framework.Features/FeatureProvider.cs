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

        public abstract T GetFeature<T>() where T : IFeature;

        protected T ResolveValue<T>()
            where T : IFeature
        {
            IEnumerable<IFeatureResolverDescriptor> values;
            if (_featureResolverProvider.Resolvers.TryGetValue(typeof(T), out values))
            {
                var context = new FeatureResolutionContext(_serviceProvider, typeof(T));
                var resolvedValue = values
                    .Where(z => z.CanResolve<T>(context))
                    .Select(x => x.Resolve<T>(context))
                    .FirstOrDefault(x => x != null);

                if (resolvedValue != null)
                {
                    return resolvedValue;
                }
            }

            throw new ArgumentOutOfRangeException("T", $"Feature type '{typeof(T).Name}' must have at least one resolver registered.");
        }
    }

    public class FeatureProvider : FeatureProviderBase
    {
        private readonly IGlobalFeatureProvider _globalFeatureProvider;
        public FeatureProvider(IServiceProvider serviceProvider, IFeatureResolverProvider featureResolverProvider, IGlobalFeatureProvider globalFeatureProvider)
            : base(serviceProvider, featureResolverProvider)
        {
            _globalFeatureProvider = globalFeatureProvider;
        }

        public override T GetFeature<T>()
        {
            var describer = _globalFeatureProvider.FeatureDescribers[typeof(T)];
            if (describer.IsScoped)
            {
                var resolvedValue = ResolveValue<T>();
                Features.TryAdd(typeof(T), resolvedValue);
                return resolvedValue;
            }

            IFeature value;
            if (!Features.TryGetValue(typeof(T), out value))
            {
                value = ResolveOther<T>();
                if (EqualityComparer<T>.Default.Equals((T)value, default(T)))
                    value = _globalFeatureProvider.GetFeature<T>();
                Features.TryAdd(typeof(T), value);
            }

            return (T)value;
        }

        public virtual T ResolveOther<T>() where T : IFeature
        {
            return default(T);
        }
    }

    class GlobalFeatureProvider : FeatureProviderBase, IGlobalFeatureProvider
    {
        public GlobalFeatureProvider(IServiceProvider serviceProvider, IFeatureDescriberProvider featureDescriberProvider, IFeatureResolverProvider featureResolverProvider)
            : base(serviceProvider, featureResolverProvider)
        {
            FeatureDescribers = featureDescriberProvider.Describers.ToDictionary(x => x.FeatureType.AsType());
        }

        public IDictionary<Type, IFeatureDescriber> FeatureDescribers { get; }

        public override T GetFeature<T>()
        {
            IFeature value;
            if (!Features.TryGetValue(typeof(T), out value))
            {
                value = ResolveValue<T>();
            }

            return (T)value;
        }
    }
}
