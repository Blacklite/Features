using Blacklite;
using Blacklite.Framework;
using Blacklite.Framework.Features;
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
            services.TryAdd(BlackliteFeaturesServices.GetFeatures(configuration));
            //services.Add(BlackliteFeaturesServices.GetResolvers(configuration));
            services.AddInstance(new FeatureServicesCollection(services));

            return services;
        }
    }
}
