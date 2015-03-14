using Microsoft.Framework.DependencyInjection;
using Blacklite.Framework.Features.OptionsModel;
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

    public abstract partial class Feature : IFeature { }

    class FeatureSetupFactory : IFeatureSetupFactory
    {
        private readonly IEnumerable<IFeatureSetup> _configurators;
        private readonly IServiceProvider _serviceProvider;
        private readonly IFeatureDescriberProvider _describerProvider;
        private readonly ConcurrentDictionary<Type, IEnumerable<IFeatureSetup>> _featureTypeSetups = new ConcurrentDictionary<Type, IEnumerable<IFeatureSetup>>();
        public FeatureSetupFactory(IEnumerable<IFeatureSetup> globalConfigurators,
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
            IEnumerable<IFeatureSetup> configurators;
            if (!_featureTypeSetups.TryGetValue(typeof(TFeature), out configurators))
            {
                configurators = _configurators.Union(
                    _serviceProvider.GetService<IEnumerable<IFeatureSetup<TFeature>>>()
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

    class ObjectConfigurator<TAspect> : IFeatureSetup
        where TAspect : class, new()
    {
        private readonly IFeatureSetup<TAspect> _configurator;
        public ObjectConfigurator(IFeatureSetup<TAspect> configurator)
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
