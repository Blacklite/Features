using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using Blacklite.Framework.Features.Observables;
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
            yield return describe.Singleton<IFeatureAssemblyProvider, FeatureAssemblyProvider>();
            yield return describe.Singleton<IFeatureTypeProvider, FeatureTypeProvider>();
            yield return describe.Singleton<IFeatureCompositionProvider, FeatureCompositionProvider>();
            yield return describe.Singleton<IFeatureOptionsProvider, FeatureOptionsProvider>();
            yield return describe.Scoped<IFeatureManager, FeatureManager>();

            yield return describe.Scoped<IFeatureFactory, CompositeFeatureFactory>();
            yield return describe.Singleton<ISingletonFeatureFactory, SingletonFeatureFactory>();
            yield return describe.Scoped<IScopedFeatureFactory, ScopedFeatureFactory>();

            yield return describe.Scoped<IFeatureSubjectFactory, CompositeFeatureSubjectFactory>();
            yield return describe.Singleton<ISingletonFeatureSubjectFactory, SingletonFeatureSubjectFactory>();

            yield return describe.Scoped(typeof(Feature<>), typeof(FeatureImpl<>));
            yield return describe.Scoped(typeof(IFeatureSubject<>), typeof(FeatureSubject<>));
            yield return describe.Scoped(typeof(ObservableFeature<>), typeof(ObservableFeatureImpl<>));
            yield return describe.Singleton<IObservableFeatureFactory, ObservableFeatureFactory>();

            yield return describe.Singleton<IRequiredFeaturesService, RequiredFeaturesService>();
            yield return describe.Singleton(typeof(IFeatureOptions<>), typeof(FeatureOptionsManager<>));

            yield return describe.Transient<DefaultFeatureDescriberEnumerable, DefaultFeatureDescriberEnumerable>();

            yield return describe.Transient<IPreFeatureComposition, OptionsFeatureComposer>();
            yield return describe.Transient<IPostFeatureComposition, RequiredFeatureComposer>();
            yield return describe.Singleton<IFeatureRepositoryProvider, FeatureRepositoryProvider>();
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

        internal static IEnumerable<IServiceDescriptor> GetFeaturesOptions(IServiceCollection services, IConfiguration configuration = null, Func<IFeatureDescriber, bool> predicate = null)
        {
            var describe = new ServiceDescriber(configuration);
            yield return describe.Transient<IPreFeatureComposition, OptionsFeatureComposer>();
        }

        internal static IEnumerable<IServiceDescriptor> GetFeaturesRequired(IServiceCollection services, IConfiguration configuration = null, Func<IFeatureDescriber, bool> predicate = null)
        {
            var describe = new ServiceDescriber(configuration);
            yield return describe.Transient<IPostFeatureComposition, RequiredFeatureComposer>();
        }
    }
}
