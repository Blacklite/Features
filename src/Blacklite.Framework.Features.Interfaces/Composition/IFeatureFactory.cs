using System;

namespace Blacklite.Framework.Features.Composition
{
    public interface IFeatureFactory
    {
        TFeature GetFeature<TFeature>()
            where TFeature : class, new();
    }
}
