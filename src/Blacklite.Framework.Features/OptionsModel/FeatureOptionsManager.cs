using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.OptionsModel
{
    public class FeatureOptionsManager<TOptions> : IFeatureOptions<TOptions>
        where TOptions : class, new()
    {
        private readonly Lazy<IEnumerable<IConfigureFeatureOptions>> _configurators;
        private object _lock = new object();

        public FeatureOptionsManager(Lazy<IEnumerable<IConfigureFeatureOptions<TOptions>>> configurators,
            Lazy<IEnumerable<IConfigureFeatureOptions>> globalConfigurators,
            Lazy<IServiceProvider> serviceProvider)
        {
            if (typeof(IFeature).GetTypeInfo().IsAssignableFrom(typeof(TOptions).GetTypeInfo()))
            {
                _options = serviceProvider.Value.GetService<Feature<TOptions>>().Value;
            }
            else
            {
                _configurators = new Lazy<IEnumerable<IConfigureFeatureOptions>>(() =>
                    configurators.Value
                        .Select(x => new ObjectConfigurator<TOptions>(x))
                        .Union(globalConfigurators.Value)
                        .OrderByDescending(x => x.Priority));
            }
        }

        private TOptions _options;
        public TOptions Options
        {
            get
            {
                if (_options == null)
                {
                    lock (_lock)
                    {
                        if (_options == null)
                        {
                            _options = Configure();
                        }
                    }
                }

                return _options;
            }
        }

        public virtual TOptions Configure()
        {
            if (_configurators == null || !_configurators.Value.Any())
                return new TOptions();

            return _configurators.Value
                .Aggregate(new TOptions(), (options, setup) =>
                {
                    setup.Configure(options);
                    return options;
                });
        }
    }

    class ObjectConfigurator<TOptions> : IConfigureFeatureOptions
        where TOptions : class, new()
    {
        private readonly IConfigureFeatureOptions<TOptions> _configurator;
        public ObjectConfigurator(IConfigureFeatureOptions<TOptions> configurator)
        {
            _configurator = configurator;
        }

        public int Priority { get { return _configurator.Priority; } }

        public void Configure(object options) => _configurator.Configure((TOptions)options);
    }
}
