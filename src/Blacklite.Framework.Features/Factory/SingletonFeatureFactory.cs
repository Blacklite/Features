using System;

namespace Blacklite.Framework.Features.Factory
{
    public class SingletonFeatureFactory : ISingletonFeatureFactory
    {
        private readonly IFeatureFactory _factory;
        public SingletonFeatureFactory(
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
