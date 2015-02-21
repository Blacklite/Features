using System;
using System.Collections.Generic;
using System.Linq;

namespace Blacklite.Framework.Features.OptionModel
{

    public class AspectOptionsManager<TOptions> : IAspectOptions<TOptions>
        where TOptions : class, new()
    {
        private readonly IEnumerable<IAspectConfigureOptions> _configurators;
        private object _lock = new object();

        public AspectOptionsManager(IEnumerable<IAspectConfigureOptions<TOptions>> configurators, IEnumerable<IAspectConfigureOptions> globalConfigurators)
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

    class ObjectConfigurator<TOptions> : IAspectConfigureOptions
        where TOptions : class, new()
    {
        private readonly IAspectConfigureOptions<TOptions> _configurator;
        public ObjectConfigurator(IAspectConfigureOptions<TOptions> configurator)
        {
            _configurator = configurator;
        }

        public int Priority { get { return _configurator.Priority; } }

        public void Configure(object options) => _configurator.Configure((TOptions)options);
    }
}