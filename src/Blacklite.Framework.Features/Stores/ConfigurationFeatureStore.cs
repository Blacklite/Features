using Blacklite.Framework.Features.Traits;
using Microsoft.Framework.ConfigurationModel;
using System;
using System.Linq;

namespace Blacklite.Framework.Features.Stores
{
    public class ConfigurationFeatureStore : IFeatureStore
    {
        private readonly IConfiguration _configuration;

        public ConfigurationFeatureStore(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int Priority { get; } = 100;

        public bool CanStore(IFeatureDescriber describer)
        {
            return describer.FeatureTypeInfo.CustomAttributes.OfType<ConfigurationStoreAttribute>().Any();
        }

        public void Store(ITrait feature, IFeatureDescriber describer)
        {
            _configuration.Set(describer.FeatureType.Name, feature.IsEnabled.ToString());
        }
    }
}
