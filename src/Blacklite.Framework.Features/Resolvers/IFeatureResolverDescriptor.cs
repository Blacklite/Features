using System;

namespace Blacklite.Framework.Features.Resolvers
{
    public interface IFeatureResolverDescriptor
    {
        bool IsGlobal { get; }
        Type FeatureType { get; }
        T Resolve<T>(IFeatureResolutionContext context) where T : IFeature;
        bool CanResolve<T>(IFeatureResolutionContext context) where T : IFeature;
    }
}
