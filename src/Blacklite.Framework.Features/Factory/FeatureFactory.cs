using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Factory
{
    public class FeatureFactory : IFeatureFactory
    {
        private readonly IFeatureCompositionProvider _featureCompositionProvider;
        private readonly ConcurrentDictionary<Type, object> _features = new ConcurrentDictionary<Type, object>();

        public FeatureFactory(
            IFeatureCompositionProvider featureCompositionProvider)
        {
            _featureCompositionProvider = featureCompositionProvider;
        }

        public TFeature GetFeature<TFeature>()
            where TFeature : class, new()
        {
            IFeatureDescriber describer;
            var composers = _featureCompositionProvider.GetComposers<TFeature>(out describer);

            return (TFeature)_features.GetOrAdd(typeof(TFeature), z => _featureCompositionProvider.GetComposers<TFeature>(out describer)
                .Aggregate(new TFeature(), (feature, setup) => setup.Configure(feature, describer)));
        }
    }

    public class CompositeFeatureFactory : IFeatureFactory
    {
        private readonly ISingletonFeatureFactory _singletonFeatureFactory;
        private readonly IScopedFeatureFactory _scopedFeatureFactory;
        private readonly IFeatureDescriberProvider _describerProvider;

        public CompositeFeatureFactory(
            ISingletonFeatureFactory singletonFeatureFactory,
            IScopedFeatureFactory scopedFeatureFactory,
            IFeatureDescriberProvider describerProvider)
        {
            _singletonFeatureFactory = singletonFeatureFactory;
            _scopedFeatureFactory = scopedFeatureFactory;
            _describerProvider = describerProvider;
        }

        public virtual TFeature GetFeature<TFeature>() where TFeature : class, new()
        {
            var describer = _describerProvider.Describers[typeof(TFeature)];

            if (describer.Lifecycle == LifecycleKind.Scoped)
                return _scopedFeatureFactory.GetFeature<TFeature>();

            return _singletonFeatureFactory.GetFeature<TFeature>();
        }
    }
}
