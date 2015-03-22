using Blacklite.Framework.Features.Describers;
using System;

namespace Blacklite.Framework.Features.Factory
{
    public class SingletonFeatureFactory : ISingletonFeatureFactory
    {
        private readonly IFeatureFactory _factory;
        public SingletonFeatureFactory(
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
