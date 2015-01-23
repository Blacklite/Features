using System;

namespace Blacklite.Framework.Features.Resolvers
{
    public interface IFeatureResolverDescriptor
    {
        bool IsGlobal { get; }
        Type FeatureType { get; }
        IFeature Resolve(IFeatureResolutionContext context);
        bool CanResolve(IFeatureResolutionContext context);
    }
}
