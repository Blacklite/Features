using System;

namespace Blacklite.Framework.Features.Resolvers
{
    public interface IFeatureResolutionContext : IServicesContext
    {
        Type FeatureType { get; }
    }
}
