using Blacklite.Framework.Features.OptionModel;
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
            yield return describe.Singleton(typeof(ISubjectAspect<>), typeof(SubjectAspect<>));
            yield return describe.Singleton(typeof(IObservableAspect<>), typeof(ObservableAspect<>));
            yield return describe.Transient<IRequiredFeaturesService, RequiredFeaturesService>();
            yield return describe.Singleton(typeof(IAspectOptions<>), typeof(AspectOptionsManager<>));
            yield return describe.Instance(new FeatureServicesCollection(services));
            yield return describe.Transient<DefaultFeatureDescriberEnumerable, DefaultFeatureDescriberEnumerable>();
        }
    }
}
