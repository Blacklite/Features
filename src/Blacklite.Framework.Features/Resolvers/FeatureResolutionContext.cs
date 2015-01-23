using Microsoft.Framework.DependencyInjection;
using System;

namespace Blacklite.Framework.Features.Resolvers
{
    class FeatureResolutionContext : IFeatureResolutionContext
    {
        public FeatureResolutionContext(IServiceProvider serviceProvider, Type featureType)
        {
            ServiceProvider = serviceProvider;
            FeatureType = featureType;
        }

        public Type FeatureType { get; }

        public IServiceProvider ServiceProvider { get; }
    }
}
