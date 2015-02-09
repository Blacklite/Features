using System;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.OptionModel
{

    public class FeatureOptionsManager<TOptions> : IFeatureOptions<TOptions>
        where TOptions : class, new()
    {
        private readonly IEnumerable<IFeatureConfigureOptions> _configurators;
        private object _lock = new object();

        public FeatureOptionsManager(IEnumerable<IFeatureConfigureOptions<TOptions>> configurators, IEnumerable<IFeatureConfigureOptions> globalConfigurators)
        {
            _configurators = configurators
                .Select(x => new ObjectConfigurator<TOptions>(x))
                .Union(globalConfigurators)
                .OrderByDescending(x => x.Priority);
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
            if (_configurators == null || !_configurators.Any())
                return new TOptions();

            return _configurators
                .Aggregate(new TOptions(), (options, setup) =>
            {
                setup.Configure(options);
                return options;
            });
        }
    }

    class ObjectConfigurator<TOptions> : IFeatureConfigureOptions
        where TOptions : class, new()
    {
        private readonly IFeatureConfigureOptions<TOptions> _configurator;
        public ObjectConfigurator(IFeatureConfigureOptions<TOptions> configurator)
        {
            _configurator = configurator;
        }

        public int Priority { get { return _configurator.Priority; } }

        public void Configure(object options) => _configurator.Configure((TOptions)options);
    }
}