using Blacklite.Framework.Features.Describers;
using System;

namespace Blacklite.Framework.Features.Factory
{
    public class ScopedFeatureFactory : IScopedFeatureFactory
    {
        private readonly IFeatureFactory _factory;
        public ScopedFeatureFactory(
            IFeatureCompositionProvider featureCompositionProvider,
            IFeatureDescriberProvider featureDescriberProvider)
        {
            _factory = new FeatureFactory(featureCompositionProvider, featureDescriberProvider);
        }
        public IFeature GetFeature(Type featureType)
        {
            return _factory.GetFeature(featureType);
        }
    }
}
