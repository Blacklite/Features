using Blacklite.Framework.Features.Describers;
using Microsoft.Framework.DependencyInjection;
using System;

namespace Blacklite.Framework.Features.Factory
{
    public class CompositeFeatureFactory : IFeatureFactory
    {
        private readonly ISingletonFeatureFactory _singletonFeatureFactory;
        private readonly IScopedFeatureFactory _scopedFeatureFactory;
        private readonly IFeatureDescriberProvider _describerProvider;

        public CompositeFeatureFactory(
            ISingletonFeatureFactory singletonFeatureFactory,
            IScopedFeatureFactory scopedFeatureFactory,
            IFeatureDescriberProvider describerProvider)
        {
            _singletonFeatureFactory = singletonFeatureFactory;
            _scopedFeatureFactory = scopedFeatureFactory;
            _describerProvider = describerProvider;
        }

        public virtual IFeature GetFeature(Type featureType)
        {
            var describer = _describerProvider.Describers[featureType];

            if (!describer.IsObservable)
                return _scopedFeatureFactory.GetFeature(featureType);

            return _singletonFeatureFactory.GetFeature(featureType);
        }
    }
}
