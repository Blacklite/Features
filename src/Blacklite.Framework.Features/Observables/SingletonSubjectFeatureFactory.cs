using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using System;

namespace Blacklite.Framework.Features.Observables
{
    public class SingletonFeatureSubjectFactory : ISingletonFeatureSubjectFactory
    {
        private readonly IFeatureSubjectFactory _factory;
        public SingletonFeatureSubjectFactory(IFeatureFactory featureFactory,
            IRequiredFeaturesService requiredFeaturesService,
            IFeatureDescriberProvider featureDescriberProvider,
            IFeatureSubjectFactory subjectFactory)
        {
            _factory = new FeatureSubjectFactory(
                featureFactory,
                requiredFeaturesService,
                featureDescriberProvider,
                subjectFactory);
        }

        public IFeatureSubject GetSubject(Type featureType)
        {
            return _factory.GetSubject(featureType);
        }
    }
}
