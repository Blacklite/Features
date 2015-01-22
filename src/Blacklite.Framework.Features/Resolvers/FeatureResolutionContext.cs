using Microsoft.Framework.DependencyInjection;
using System;

namespace Blacklite.Framework.Features.Resolvers
{
    class FeatureResolutionContext : IFeatureResolutionContext
    {
        public FeatureResolutionContext(IServiceProvider serviceProvider, Type metadatumType)
        {
            ServiceProvider = serviceProvider;
            FeatureType = metadatumType;
        }

        public Type FeatureType { get; }

        public IServiceProvider ServiceProvider { get; }
    }
}
