using Blacklite.Framework.Features.Resolvers;
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
        public static IEnumerable<IServiceDescriptor> GetFeatures(IConfiguration configuration = null)
        {
            var describe = new ServiceDescriber(configuration);

            yield return describe.Transient(typeof(IFeature<>), typeof(Feature<>));
            yield return describe.Scoped<IFeatureProvider, FeatureProvider>();
            yield return describe.Singleton<IGlobalFeatureProvider, GlobalFeatureProvider>();
            yield return describe.Singleton<IFeatureDescriberProvider, FeatureDescriberProvider>();
        }

        public static IEnumerable<IServiceDescriptor> GetResolvers(IConfiguration configuration = null)
        {
            var describe = new ServiceDescriber(configuration);

            yield return describe.Transient<IFeatureResolver, CommonFeatureResolver>();
        }
    }
}
