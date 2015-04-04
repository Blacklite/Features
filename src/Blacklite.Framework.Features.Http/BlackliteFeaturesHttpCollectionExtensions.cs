using Blacklite;
using Blacklite.Framework;
using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Microsoft.Framework.DependencyInjection
{
    public static class BlackliteFeaturesMvcCollectionExtensions
    {
        public static IServiceCollection AddFeaturesHttp(
            [NotNull] this IServiceCollection services,
            IConfiguration configuration = null)
        {
            services.AddFeatureEditorModel(configuration)
                    .AddDataProtection(configuration);
            services.TryAdd(BlackliteFeaturesHttpServices.GetFeaturesHttp(services, configuration));
            return services;
        }
    }
}
