using Blacklite;
using Blacklite.Framework;
using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Describers;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Microsoft.Framework.DependencyInjection
{
    public static class BlackliteFeaturesCollectionExtensions
    {
        public static IServiceCollection AddFeatures(
            [NotNull] this IServiceCollection services,
            IConfiguration configuration = null)
        {
            services.TryAddImplementation(BlackliteFeaturesServices.GetFeatures(services, configuration));
            return services;
        }

        public static IServiceCollection AddFeaturesConfiguration(
            [NotNull] this IServiceCollection services,
            IConfiguration configuration,
            Func<IFeatureDescriber, bool> predicate = null)
        {
            services.TryAddImplementation(BlackliteFeaturesServices.GetFeaturesConfiguration(services, configuration, predicate));
            return services;
        }
    }
}
