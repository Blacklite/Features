using Blacklite;
using Blacklite.Framework;
using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Mvc;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Microsoft.Framework.DependencyInjection
{
    public static class BlackliteFeaturesMvcCollectionExtensions
    {
        public static IServiceCollection AddFeaturesMvc(
            [NotNull] this IServiceCollection services,
            IConfiguration configuration = null)
        {
            services.AddFeatureEditorModel(configuration);
            services.TryAdd(BlackliteFeaturesMvcServices.GetFeaturesMvc(services, configuration));
            return services;
        }
    }
}
