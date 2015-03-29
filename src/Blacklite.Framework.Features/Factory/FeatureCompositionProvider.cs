using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Factory
{
    public class FeatureCompositionProvider : IFeatureCompositionProvider
    {
        private readonly IFeatureDescriberProvider _describerProvider;
        private readonly ConcurrentDictionary<Type, IEnumerable<IFeatureComposition>> _featureComposers = new ConcurrentDictionary<Type, IEnumerable<IFeatureComposition>>();
        private readonly IEnumerable<IFeatureComposition> _composers;

        public FeatureCompositionProvider(
            IEnumerable<IFeatureComposition> globalComposers,
            IEnumerable<IPreFeatureComposition> preFeatureComposers,
            IEnumerable<IPostFeatureComposition> postFeatureComposers,
            IFeatureDescriberProvider describerProvider)
        {
            _composers = Enumerable.Concat(
                    preFeatureComposers.OrderByDescending(z => z.Priority),
                    globalComposers.OrderByDescending(z => z.Priority)
                ).Concat(postFeatureComposers.OrderByDescending(z => z.Priority))
                .ToArray();
            _describerProvider = describerProvider;
        }

        public IEnumerable<IFeatureComposition> GetComposers<TFeature>()
            where TFeature : class, new()
        {
            IFeatureDescriber describer = null;
            if (!_describerProvider.Describers.TryGetValue(typeof(TFeature), out describer))
            {
                throw new KeyNotFoundException($"Could not find feature ${typeof(TFeature).Name}.");
            }

            IEnumerable<IFeatureComposition> composers;
            if (!_featureComposers.TryGetValue(typeof(TFeature), out composers))
            {
                // Enforce options first (so it gets populated)
                // Enfroce required last, so it overrides anything else for IsEnabled
                composers = _composers
                    .Where(x => x.IsApplicableTo(describer))
                    .ToArray();
                _featureComposers.TryAdd(typeof(TFeature), composers);
            }

            return composers;
        }
    }
}
