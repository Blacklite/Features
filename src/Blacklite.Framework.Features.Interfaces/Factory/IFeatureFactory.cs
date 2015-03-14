using System;

namespace Blacklite.Framework.Features.Factory
{
    public interface IFeatureFactory
    {
        TFeature GetFeature<TFeature>()
            where TFeature : class, new();
    }

    public interface IScopedFeatureFactory : IFeatureFactory { }

    public interface ISingletonFeatureFactory : IFeatureFactory { }
}
