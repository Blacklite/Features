using System;

namespace Blacklite.Framework.Features.Factory
{
    public static class FeatureFactoryExtensions
    {
        public static TFeature GetFeature<TFeature>(this IFeatureFactory factory)
            where TFeature : class, new()
        {
            return (TFeature)factory.GetFeature(typeof(TFeature));
        }
    }
}
