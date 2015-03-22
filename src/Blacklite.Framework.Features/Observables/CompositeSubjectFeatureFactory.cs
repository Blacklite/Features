using Blacklite.Framework.Features.Describers;
using Microsoft.Framework.DependencyInjection;
using System;

namespace Blacklite.Framework.Features.Observables
{
    public class CompositeFeatureSubjectFactory : IFeatureSubjectFactory
    {
        private readonly ISingletonFeatureSubjectFactory _singletonFeatureFactory;
        private readonly IFeatureDescriberProvider _describerProvider;

        public CompositeFeatureSubjectFactory(
            ISingletonFeatureSubjectFactory singletonFeatureFactory,
            IFeatureDescriberProvider describerProvider)
        {
            _singletonFeatureFactory = singletonFeatureFactory;
            _describerProvider = describerProvider;
        }

        public virtual IFeatureSubject GetSubject(Type featureType)
        {
            var describer = _describerProvider.Describers[featureType];

            // this allows multi-tenancy to inject it's own provider for tenant based subjects
            return _singletonFeatureFactory.GetSubject(featureType);
        }
    }
}
