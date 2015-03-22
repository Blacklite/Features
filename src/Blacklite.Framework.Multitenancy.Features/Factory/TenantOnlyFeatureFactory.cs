using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using System;

namespace Blacklite.Framework.Multitenancy.Features.Factory
{
    public interface ITenantOnlyFeatureFactory : IFeatureFactory { }
    public class TenantOnlyFeatureFactory : ITenantOnlyFeatureFactory
    {
        private readonly IFeatureFactory _factory;

        public TenantOnlyFeatureFactory(
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
