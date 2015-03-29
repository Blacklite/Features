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
    public class FeatureCompositionProvider : IFeatureCompositionProvider
    {
        private readonly IEnumerable<IFeatureComposition> _composers;
        private readonly IFeatureDescriberProvider _describerProvider;
        private readonly ConcurrentDictionary<Type, IEnumerable<IFeatureComposition>> _featureComposers = new ConcurrentDictionary<Type, IEnumerable<IFeatureComposition>>();
        private readonly IFeatureComposition[] _requiredFeatureComposer;
        private readonly IFeatureComposition[] _optionsFeatureComposer;

        public FeatureCompositionProvider(
            IEnumerable<IFeatureComposition> globalComposers,
            IOptionsFeatureComposer optionsFeatureComposer,
            IRequiredFeatureComposer requiredFeatureComposer,
            IFeatureDescriberProvider describerProvider)
        {
            _composers = globalComposers;
            _optionsFeatureComposer = new IFeatureComposition[] { optionsFeatureComposer };
            _requiredFeatureComposer = new IFeatureComposition[] { requiredFeatureComposer };
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
                composers = _composers
                    .OrderByDescending(x => x.Priority);

                // Enforce options first (so it gets populated)
                // Enfroce required last, so it overrides anything else for IsEnabled
                composers = _optionsFeatureComposer
                    .Concat(_composers.OrderByDescending(x => x.Priority))
                    .Concat(_requiredFeatureComposer)
                    .Where(x => x.IsApplicableTo(describer))
                    .ToArray();
                _featureComposers.TryAdd(typeof(TFeature), composers);
            }

            return composers;
        }
    }
}
