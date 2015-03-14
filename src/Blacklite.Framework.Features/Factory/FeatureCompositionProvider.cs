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
    public interface IFeatureCompositionProvider
    {
        IEnumerable<IFeatureComposition> GetComposers<TFeature>(out IFeatureDescriber describer)
            where TFeature : class, new();
    }
    public class FeatureCompositionProvider : IFeatureCompositionProvider
    {
        private readonly IEnumerable<IFeatureComposition> _composers;
        private readonly IServiceProvider _serviceProvider;
        private readonly IFeatureDescriberProvider _describerProvider;
        private readonly ConcurrentDictionary<Type, IEnumerable<IFeatureComposition>> _featureComposers = new ConcurrentDictionary<Type, IEnumerable<IFeatureComposition>>();

        public FeatureCompositionProvider(
            IEnumerable<IFeatureComposition> globalComposers,
            IServiceProvider serviceProvider,
            IFeatureDescriberProvider describerProvider)
        {
            _composers = globalComposers;
            _serviceProvider = serviceProvider;
            _describerProvider = describerProvider;
        }

        public IEnumerable<IFeatureComposition> GetComposers<TFeature>(out IFeatureDescriber describer)
            where TFeature : class, new()
        {
            if (!_describerProvider.Describers.TryGetValue(typeof(TFeature), out describer))
            {
                throw new KeyNotFoundException($"Could not find feature ${typeof(TFeature).Name}.");
            }
            IEnumerable<IFeatureComposition> composers;
            var d = describer;
            if (!_featureComposers.TryGetValue(typeof(TFeature), out composers))
            {
                composers = _composers.Union(
                        _serviceProvider.GetRequiredService<IEnumerable<IFeatureComposition<TFeature>>>()
                        .Select(x => new ObjectComposer<TFeature>(x))
                    )
                    .Where(x => x.IsApplicableTo(d))
                    .OrderByDescending(x => x.Priority);
            }

            return composers;
        }

        public class ObjectComposer<TFeature> : IFeatureComposition
            where TFeature : class, new()
        {
            private readonly IFeatureComposition<TFeature> _configurator;
            public ObjectComposer(IFeatureComposition<TFeature> configurator)
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
