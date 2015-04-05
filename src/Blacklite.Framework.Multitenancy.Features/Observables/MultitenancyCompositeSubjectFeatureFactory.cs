using Blacklite.Framework.Features.Observables;
using System;
using Blacklite.Framework.Features.Describers;
using Microsoft.Framework.DependencyInjection;
using Blacklite.Framework.Multitenancy.Features.Describers;
using Blacklite.Framework.Features;

namespace Blacklite.Framework.Multitenancy.Features.Observables
{
    public class MultitenancyCompositeFeatureSubjectFactory : CompositeFeatureSubjectFactory
    {
        private readonly IFeatureDescriberProvider _describerProvider;
        private readonly Lazy<ITenantOnlyFeatureSubjectFactory> _tenantOnlyFeatureFactory;
        private readonly Lazy<IApplicationOnlyFeatureSubjectFactory> _applicationOnlyFeatureFactory;

        public MultitenancyCompositeFeatureSubjectFactory(
            ISingletonFeatureSubjectFactory singletonFeatureFactory,
            IServiceProvider serviceProvider,
            IFeatureDescriberProvider describerProvider)
            : base(singletonFeatureFactory, describerProvider)
        {
            _describerProvider = describerProvider;

            _tenantOnlyFeatureFactory = new Lazy<ITenantOnlyFeatureSubjectFactory>(() =>
                serviceProvider.GetRequiredService<ITenantOnlyFeatureSubjectFactory>());

            _applicationOnlyFeatureFactory = new Lazy<IApplicationOnlyFeatureSubjectFactory>(() =>
                serviceProvider.GetRequiredService<IApplicationOnlyFeatureSubjectFactory>());
        }

        public override IFeatureSubject GetSubject(Type featureType)
        {
            var describer = (MultitenancyFeatureDescriber)_describerProvider.Describers[featureType];
            if (describer.IsTenantScoped)
            {
                return _tenantOnlyFeatureFactory.Value.GetSubject(featureType);
            }

            if (describer.IsApplicationScoped)
            {
                return _applicationOnlyFeatureFactory.Value.GetSubject(featureType);
            }

            return base.GetSubject(featureType);
        }
    }
}
