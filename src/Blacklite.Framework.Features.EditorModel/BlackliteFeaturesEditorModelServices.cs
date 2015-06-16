using Blacklite.Framework.Features.Editors.Factory;
using Blacklite.Framework.Features.Editors.JsonEditors;
using Blacklite.Framework.Features.Editors.JsonEditors.Resolvers;
using Blacklite.Framework.Features.OptionsModel;
using Blacklite.Json.Schema;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features.Editors
{
    public static class BlackliteFeaturesEditorModelServices
    {
        internal static IEnumerable<ServiceDescriptor> GetFeatureEditorModel(IServiceCollection services)
        {
            yield return ServiceDescriptor.Scoped<IFeatureJsonEditorDecorator, DefaultFeatureJsonEditorDecorator>();
            yield return ServiceDescriptor.Scoped(typeof(FeatureEditorFactory<>), typeof(FeatureEditorFactory<>));
            yield return ServiceDescriptor.Scoped(typeof(IFeatureEditor<>), typeof(FeatureEditor<>));
            yield return ServiceDescriptor.Scoped<EditorFeatureFactory, EditorFeatureFactory>();
        }

        internal static IEnumerable<ServiceDescriptor> GetFeatureEditorModelImplementations(IServiceCollection services)
        {
            yield return ServiceDescriptor.Transient<TabsJsonEditorResolver, TabsJsonEditorResolver>();
            yield return ServiceDescriptor.Transient<IConfigureOptions<JsonEditorOptions>, ConfigureJsonEditorOptions>();
        }
    }
}
