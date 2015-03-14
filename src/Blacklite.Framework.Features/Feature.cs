using Microsoft.Framework.DependencyInjection;
using Blacklite.Framework.Features.Aspects;
using Blacklite.Framework.Features.OptionModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    public interface IFeatureSetupFactory
    {
        TFeature GetFeature<TFeature>()
            where TFeature : class, new();
    }

    class FeatureSetupFactory : IFeatureSetupFactory
    {
        private readonly IEnumerable<IAspectSetup> _configurators;
        private readonly IServiceProvider _serviceProvider;
        private readonly IFeatureDescriberProvider _describerProvider;
        private readonly ConcurrentDictionary<Type, IEnumerable<IAspectSetup>> _featureTypeSetups = new ConcurrentDictionary<Type, IEnumerable<IAspectSetup>>();
        public FeatureSetupFactory(IEnumerable<IAspectSetup> globalConfigurators,
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
            IEnumerable<IAspectSetup> configurators;
            if (!_featureTypeSetups.TryGetValue(typeof(TFeature), out configurators))
            {
                configurators = _configurators.Union(
                    _serviceProvider.GetService<IEnumerable<IAspectSetup<TFeature>>>()
                    .Select(x => new ObjectConfigurator<TFeature>(x))
                )
                .OrderByDescending(x => x.Priority);
            }

            IFeatureDescriber describer;
            if (!_describerProvider.Describers.TryGetValue(typeof(TFeature), out describer))
            {
                throw new KeyNotFoundException($"Could not find feature ${typeof(TFeature).Name}.");
            }

            return configurators
                .Where(x => x.IsApplicableTo(describer))
                .Aggregate(new TFeature(), (feature, setup) => setup.Configure(feature));
        }
    }

    public class FeatureImpl<TAspect> : Feature<TAspect>
        where TAspect : class, new()
    {
        private readonly IFeatureSetupFactory _factory;
        private object _lock = new object();
        private Lazy<TAspect> _value;

        public FeatureImpl(IFeatureSetupFactory factory)
        {
            _factory = factory;
            _value = new Lazy<TAspect>(Configure);
        }
        public TAspect Value { get { return _value.Value; } }

        public virtual TAspect Configure()
        {
            return _factory.GetFeature<TAspect>();
        }
    }

    class ObjectConfigurator<TAspect> : IAspectSetup
        where TAspect : class, new()
    {
        private readonly IAspectSetup<TAspect> _configurator;
        public ObjectConfigurator(IAspectSetup<TAspect> configurator)
        {
            _configurator = configurator;
        }

        public int Priority { get { return _configurator.Priority; } }

        public T Configure<T>(T aspect) => (T)_configurator.Configure(aspect as TAspect);

        public bool IsApplicableTo(IFeatureDescriber describer)
        {
            return typeof(TAspect).GetTypeInfo().IsAssignableFrom(describer.FeatureTypeInfo);
        }
    }
}
