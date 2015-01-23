using System;

namespace Blacklite.Framework.Features.Resolvers
{
    public abstract class FeatureResolver<TFeature> : IFeatureResolver
        where TFeature : IFeature
    {
        public Type GetFeatureType() => typeof(TFeature);

        public abstract int Priority { get; }

        public bool CanResolve(IFeatureResolutionContext context) => typeof(TFeature) == context.FeatureType;
    }
}
