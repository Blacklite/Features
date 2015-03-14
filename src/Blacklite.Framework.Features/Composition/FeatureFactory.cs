using Blacklite.Framework.Features.Describers;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Composition
{
    public class FeatureFactory : IFeatureFactory
    {
        private readonly IEnumerable<IFeatureComposition> _configurators;
        private readonly IServiceProvider _serviceProvider;
        private readonly IFeatureDescriberProvider _describerProvider;
        private readonly ConcurrentDictionary<Type, IEnumerable<IFeatureComposition>> _featureTypeSetups = new ConcurrentDictionary<Type, IEnumerable<IFeatureComposition>>();
        public FeatureFactory(IEnumerable<IFeatureComposition> globalConfigurators,
            IServiceProvider serviceProvider,
            IFeatureDescriberProvider describerProvider)
        {
            _configurators = globalConfigurators;
            _serviceProvider = serviceProvider;
            _describerProvider = describerProvider;
        }

        public TFeature GetFeature<TFeature>()
            where TFeature : class, new()
        {
            IFeatureDescriber describer;
            if (!_describerProvider.Describers.TryGetValue(typeof(TFeature), out describer))
            {
                throw new KeyNotFoundException($"Could not find feature ${typeof(TFeature).Name}.");
            }

            IEnumerable<IFeatureComposition> configurators;
            if (!_featureTypeSetups.TryGetValue(typeof(TFeature), out configurators))
            {

                configurators = _configurators.Union(
                        _serviceProvider.GetRequiredService<IEnumerable<IFeatureComposition<TFeature>>>()
                        .Select(x => new ObjectConfigurator<TFeature>(x))
                    )
                    .Where(x => x.IsApplicableTo(describer))
                    .OrderByDescending(x => x.Priority);
            }

            return configurators
                .Aggregate(new TFeature(), (feature, setup) => setup.Configure(feature, describer));
        }

        private class ObjectConfigurator<TFeature> : IFeatureComposition
            where TFeature : class, new()
        {
            private readonly IFeatureComposition<TFeature> _configurator;
            public ObjectConfigurator(IFeatureComposition<TFeature> configurator)
            {
                _configurator = configurator;
            }

            public int Priority { get { return _configurator.Priority; } }

            public T Configure<T>(T feature, IFeatureDescriber describer) => (T)_configurator.Configure(feature as TFeature, describer);

            public bool IsApplicableTo(IFeatureDescriber describer)
            {
                return typeof(TFeature).GetTypeInfo().IsAssignableFrom(describer.TypeInfo);
            }
        }
    }
}
