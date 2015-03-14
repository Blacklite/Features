using System;

namespace Blacklite.Framework.Features.Factory
{
    public class ScopedFeatureFactory : IScopedFeatureFactory
    {
        private readonly IFeatureFactory _factory;
        public ScopedFeatureFactory(
            IFeatureCompositionProvider featureCompositionProvider)
        {
            _factory = new FeatureFactory(featureCompositionProvider);
        }
        public TFeature GetFeature<TFeature>() where TFeature : class, new()
        {
            return _factory.GetFeature<TFeature>();
        }
    }
}
