using Blacklite;
using Blacklite.Framework;
using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Microsoft.Framework.DependencyInjection
{
    public static class BlackliteFeaturesMvcCollectionExtensions
    {
        public static IServiceCollection AddFeaturesHttp([NotNull] this IServiceCollection services)
        {
            services.AddFeatureEditorModel()
                    .AddDataProtection()
                    .AddAntiforgery();
            services.TryAdd(BlackliteFeaturesHttpServices.GetFeaturesHttp(services));
            return services;
        }
    }
}
