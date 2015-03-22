using Blacklite.Framework.Features.Factory;
using System;
using Blacklite.Framework.Features.Describers;
using Microsoft.Framework.DependencyInjection;
using Blacklite.Framework.Multitenancy.Features.Describers;
using Blacklite.Framework.Features;

namespace Blacklite.Framework.Multitenancy.Features.Factory
{
    public class MultitenancyCompositeFeatureFactory : CompositeFeatureFactory
    {
        private readonly IFeatureDescriberProvider _describerProvider;
        private readonly Lazy<ITenantOnlyFeatureFactory> _tenantOnlyFeatureFactory;
        private readonly Lazy<IApplicationOnlyFeatureFactory> _applicationOnlyFeatureFactory;

        public MultitenancyCompositeFeatureFactory(
            ISingletonFeatureFactory singletonFeatureFactory,
            IScopedFeatureFactory scopedFeatureFactory,
            IServiceProvider serviceProvider,
            IFeatureDescriberProvider describerProvider)
            : base(singletonFeatureFactory, scopedFeatureFactory, describerProvider)
        {
            _describerProvider = describerProvider;

            _tenantOnlyFeatureFactory = new Lazy<ITenantOnlyFeatureFactory>(() =>
                serviceProvider.GetRequiredService<ITenantOnlyFeatureFactory>());

            _applicationOnlyFeatureFactory = new Lazy<IApplicationOnlyFeatureFactory>(() =>
                serviceProvider.GetRequiredService<IApplicationOnlyFeatureFactory>());
        }

        public override IFeature GetFeature(Type featureType)
        {
            var describer = (MultitenancyFeatureDescriber)_describerProvider.Describers[featureType];
            if (describer.IsTenantScoped)
            {
                return _tenantOnlyFeatureFactory.Value.GetFeature(featureType);
            }

            if (describer.IsApplicationScoped)
            {
                return _applicationOnlyFeatureFactory.Value.GetFeature(featureType);
            }

            return base.GetFeature(featureType);
        }
    }
}
