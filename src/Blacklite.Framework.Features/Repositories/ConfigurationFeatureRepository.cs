using Blacklite.Framework.Features.Describers;
using Microsoft.Framework.ConfigurationModel;
using System;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Repositories
{
    public class ConfigurationFeatureRepository : IFeatureRepository
    {
        private readonly IConfiguration _configuration;
        private readonly Func<IFeatureDescriber, bool> _predicate;

        public ConfigurationFeatureRepository(IConfiguration configuration)
            : this(configuration, describer => describer.TypeInfo.GetCustomAttributes<ConfigurationFeatureAttribute>().Any())
        {
        }

        public ConfigurationFeatureRepository(IConfiguration configuration, Func<IFeatureDescriber, bool> predicate)
        {
            _configuration = configuration;
            _predicate = predicate;
        }

        public int Priority { get; } = 100;

        public bool IsApplicableTo(IFeatureDescriber describer) => _predicate(describer);

        public void Store(IFeature feature, IFeatureDescriber describer)
        {
            if (describer.HasEnabled)
            {
                _configuration.Set($"{describer.Type.Name}:IsEnabled", describer.GetIsEnabled<bool>(feature).ToString() ?? string.Empty);
            }

            if (describer.Properties.Any())
            {
                foreach (var property in describer.Properties)
                {
                    _configuration.Set($"{describer.Type.Name}:{property.Name}", property.GetProperty<object>(feature)?.ToString() ?? string.Empty);
                }
            }

            if (describer.HasOptions && !describer.Options.IsFeature)
            {
                var options = describer.GetOptions<object>(feature);
                foreach (var property in describer.Options.Type.GetRuntimeProperties())
                {
                    _configuration.Set($"{describer.Type.Name}:Options:{property.Name}", property.GetValue(options)?.ToString() ?? string.Empty);
                }
            }
        }
    }
}
