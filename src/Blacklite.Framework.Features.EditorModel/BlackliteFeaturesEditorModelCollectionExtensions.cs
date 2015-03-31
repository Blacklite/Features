using Blacklite;
using Blacklite.Framework;
using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Editors;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Microsoft.Framework.DependencyInjection
{
    public static class BlackliteFeaturesEditorModelCollectionExtensions
    {
        public static IServiceCollection AddFeatureEditorModel(
            [NotNull] this IServiceCollection services,
            IConfiguration configuration = null)
        {
            services.AddJsonSchemaEditor(configuration);
            services.TryAdd(BlackliteFeaturesEditorModelServices.GetFeatureEditorModel(services, configuration));
            services.TryAddImplementation(BlackliteFeaturesEditorModelServices.GetFeatureEditorModelImplementations(services, configuration));
            return services;
        }
    }
}
