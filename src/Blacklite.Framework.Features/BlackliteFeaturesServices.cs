using Blacklite.Framework.Features.Composition;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using Blacklite.Framework.Features.Observables;
using Blacklite.Framework.Features.OptionsModel;
using Blacklite.Framework.Features.Repositories;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blacklite.Framework.Features
{
    public static class BlackliteFeaturesServices
    {
        internal static IEnumerable<ServiceDescriptor> GetFeatures(IServiceCollection services)
        {
            yield return ServiceDescriptor.Singleton<IFeatureDescriberProvider, FeatureDescriberProvider>();
            yield return ServiceDescriptor.Singleton<IFeatureDescriberFactory, FeatureDescriberFactory>();
            yield return ServiceDescriptor.Singleton<IFeatureAssemblyProvider, FeatureAssemblyProvider>();
            yield return ServiceDescriptor.Singleton<IFeatureTypeProvider, FeatureTypeProvider>();
            yield return ServiceDescriptor.Singleton<IFeatureCompositionProvider, FeatureCompositionProvider>();
            yield return ServiceDescriptor.Singleton<IFeatureOptionsProvider, FeatureOptionsProvider>();
            yield return ServiceDescriptor.Scoped<IFeatureManager, FeatureManager>();

            yield return ServiceDescriptor.Scoped<IFeatureFactory, FeatureFactory>();

            yield return ServiceDescriptor.Scoped<IFeatureSubjectFactory, CompositeFeatureSubjectFactory>();
            yield return ServiceDescriptor.Singleton<ISingletonFeatureSubjectFactory, SingletonFeatureSubjectFactory>();

            yield return ServiceDescriptor.Scoped(typeof(Feature<>), typeof(FeatureImpl<>));
            yield return ServiceDescriptor.Scoped(typeof(IFeatureSubject<>), typeof(FeatureSubject<>));
            yield return ServiceDescriptor.Scoped(typeof(ObservableFeature<>), typeof(ObservableFeatureImpl<>));
            yield return ServiceDescriptor.Singleton<IObservableFeatureFactory, ObservableFeatureFactory>();

            yield return ServiceDescriptor.Singleton<IRequiredFeaturesService, RequiredFeaturesService>();
            yield return ServiceDescriptor.Singleton(typeof(IFeatureOptions<>), typeof(FeatureOptionsManager<>));

            yield return ServiceDescriptor.Transient<FeatureDescriberCollection, FeatureDescriberCollection>();

            yield return ServiceDescriptor.Transient<IPreFeatureComposition, OptionsFeatureComposer>();
            yield return ServiceDescriptor.Transient<IPostFeatureComposition, RequiredFeatureComposer>();
            yield return ServiceDescriptor.Singleton<IFeatureRepositoryProvider, FeatureRepositoryProvider>();
        }

        internal static IEnumerable<ServiceDescriptor> GetFeaturesConfiguration(IServiceCollection services, IConfiguration configuration, Func<IFeatureDescriber, bool> predicate = null)
        {
            if (predicate == null)
            {
                yield return ServiceDescriptor.Instance<IFeatureComposition>(new ConfigurationFeatureComposer(configuration));
                yield return ServiceDescriptor.Instance<IFeatureRepository>(new ConfigurationFeatureRepository(configuration));
            }
            else
            {
                yield return ServiceDescriptor.Instance<IFeatureComposition>(new ConfigurationFeatureComposer(configuration, predicate));
                yield return ServiceDescriptor.Instance<IFeatureRepository>(new ConfigurationFeatureRepository(configuration, predicate));
            }
        }

        internal static IEnumerable<ServiceDescriptor> GetFeaturesOptions(IServiceCollection services, Func<IFeatureDescriber, bool> predicate = null)
        {
            yield return ServiceDescriptor.Transient<IPreFeatureComposition, OptionsFeatureComposer>();
        }

        internal static IEnumerable<ServiceDescriptor> GetFeaturesRequired(IServiceCollection services, Func<IFeatureDescriber, bool> predicate = null)
        {
            yield return ServiceDescriptor.Transient<IPostFeatureComposition, RequiredFeatureComposer>();
        }
    }
}
