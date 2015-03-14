using Blacklite.Framework.Features.Aspects;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.OptionModel
{

    public class AspectOptionsManager<TOptions> : IAspectOptions<TOptions>
        where TOptions : class, new()
    {
        private readonly Lazy<IEnumerable<IAspectConfigureOptions>> _configurators;
        private object _lock = new object();

        public AspectOptionsManager(Lazy<IEnumerable<IAspectConfigureOptions<TOptions>>> configurators,
            Lazy<IEnumerable<IAspectConfigureOptions>> globalConfigurators,
            Lazy<IServiceProvider> serviceProvider)
        {
            if (typeof(IAspect).GetTypeInfo().IsAssignableFrom(typeof(TOptions).GetTypeInfo()))
            {
                _options = serviceProvider.Value.GetService<Feature<TOptions>>().Value;
            }
            else
            {
                _configurators = new Lazy<IEnumerable<IAspectConfigureOptions>>(() =>
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
