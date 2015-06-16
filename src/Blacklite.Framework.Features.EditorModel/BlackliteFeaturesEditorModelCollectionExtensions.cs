using Blacklite;
using Blacklite.Framework;
using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Editors;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Microsoft.Framework.DependencyInjection
{
    public static class BlackliteFeaturesEditorModelCollectionExtensions
    {
        public static IServiceCollection AddFeatureEditorModel([NotNull] this IServiceCollection services)
        {
            services.AddFeatures()
                    .AddJsonSchemaEditor();

            services.TryAdd(BlackliteFeaturesEditorModelServices.GetFeatureEditorModel(services));
            services.TryAddImplementation(BlackliteFeaturesEditorModelServices.GetFeatureEditorModelImplementations(services));
            services.TryAdd(OptionsServiceCollectionExtensions.AddOptions(services));
            return services;
        }
    }
}
