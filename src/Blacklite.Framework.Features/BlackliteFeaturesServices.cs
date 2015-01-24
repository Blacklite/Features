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
        internal static IEnumerable<IServiceDescriptor> GetFeatures(IConfiguration configuration = null)
        {
            var describe = new ServiceDescriber(configuration);

            yield return describe.Singleton<IFeatureDescriberProvider, FeatureDescriberProvider>();
            yield return describe.Singleton(typeof(ISubjectFeature<>), typeof(SubjectFeature<>));
            yield return describe.Singleton(typeof(IObservableFeature<>), typeof(IObservableFeature<>));
            yield return describe.Transient<IRequiredFeaturesService, RequiredFeaturesService>();
        }
    }
}
