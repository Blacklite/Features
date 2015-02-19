using Blacklite.Framework.Features.EditorModel.JsonEditors;
using Blacklite.Framework.Features.EditorModel.JsonEditors.Resolvers;
using Blacklite.Framework.Features.OptionModel;
using Blacklite.Json.Schema;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.EditorModel
{
    public static class BlackliteFeaturesEditorModelServices
    {
        internal static IEnumerable<IServiceDescriptor> GetFeatureEditorModel(IServiceCollection services, IConfiguration configuration = null)
        {
            var describe = new ServiceDescriber(configuration);

            yield return describe.Transient<IFeatureJsonEditorDecorator, DefaultFeatureJsonEditorDecorator>();
        }

        internal static IEnumerable<IServiceDescriptor> GetFeatureEditorModelImplementations(IServiceCollection services, IConfiguration configuration = null)
        {
            var describe = new ServiceDescriber(configuration);

            yield return describe.Transient<IJsonEditorResolver, TabsJsonEditorResolver>();
            yield return describe.Transient<IConfigureOptions<JsonEditorOptions>, ConfigureJsonEditorOptions>();
        }
    }
}
