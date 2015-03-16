using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using Blacklite.Framework.Features.OptionsModel;
using Blacklite.Framework.Features.Repositories;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    public static class BlackliteFeaturesServices
    {
        internal static IEnumerable<IServiceDescriptor> GetFeatures(IServiceCollection services, IConfiguration configuration = null)
        {
            var describe = new ServiceDescriber(configuration);

            yield return describe.Singleton<IFeatureDescriberProvider, FeatureDescriberProvider>();
            yield return describe.Singleton<IFeatureDescriberFactory, FeatureDescriberFactory>();
            yield return describe.Singleton(typeof(IFeatureSubject<>), typeof(FeatureSubject<>));
            yield return describe.Singleton<IFeatureAssemblyProvider, FeatureAssemblyProvider>();
            yield return describe.Singleton<IFeatureTypeProvider, FeatureTypeProvider>();
            yield return describe.Singleton<IFeatureCompositionProvider, FeatureCompositionProvider>();
            yield return describe.Singleton<IFeatureManager, FeatureManager>();

            yield return describe.Scoped<IFeatureFactory, CompositeFeatureFactory>();
            yield return describe.Singleton<ISingletonFeatureFactory, SingletonFeatureFactory>();
            yield return describe.Scoped<IScopedFeatureFactory, ScopedFeatureFactory>();

            yield return describe.Singleton(typeof(Feature<>), typeof(FeatureImpl<>));
            yield return describe.Singleton(typeof(ObservableFeature<>), typeof(ObservableFeatureImpl<>));

            yield return describe.Transient<IRequiredFeaturesService, RequiredFeaturesService>();
            yield return describe.Singleton(typeof(IFeatureOptions<>), typeof(FeatureOptionsManager<>));

            yield return describe.Transient<DefaultFeatureDescriberEnumerable, DefaultFeatureDescriberEnumerable>();

            yield return describe.Transient<IFeatureComposition, OptionsFeatureComposer>();
            yield return describe.Transient<IFeatureComposition, RequiredFeatureComposer>();
        }

        internal static IEnumerable<IServiceDescriptor> GetFeaturesConfiguration(IServiceCollection services, IConfiguration configuration, Func<IFeatureDescriber, bool> predicate = null)
        {
            var describe = new ServiceDescriber(configuration);

            if (predicate == null)
            {
                yield return describe.Instance<IFeatureComposition>(new ConfigurationFeatureComposer(configuration));
                yield return describe.Instance<IFeatureRepository>(new ConfigurationFeatureRepository(configuration));
            }
            else
            {
                yield return describe.Instance<IFeatureComposition>(new ConfigurationFeatureComposer(configuration, predicate));
                yield return describe.Instance<IFeatureRepository>(new ConfigurationFeatureRepository(configuration, predicate));
            }
        }
    }
}
