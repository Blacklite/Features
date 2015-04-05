using Blacklite.Framework.Features;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Observables;
using System;

namespace Blacklite.Framework.Multitenancy.Features.Observables
{
    public interface ITenantOnlyFeatureSubjectFactory : IFeatureSubjectFactory { }
    public class TenantOnlyFeatureSubjectFactory : ITenantOnlyFeatureSubjectFactory
    {
        private readonly IFeatureSubjectFactory _factory;

        public TenantOnlyFeatureSubjectFactory(IServiceProvider serviceProvider)
        {
            _factory = new FeatureSubjectFactory(serviceProvider);
        }

        public IFeatureSubject GetSubject(Type featureType)
        {
            return _factory.GetSubject(featureType);
        }
    }
}
