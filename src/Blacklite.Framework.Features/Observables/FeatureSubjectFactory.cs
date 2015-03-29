using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Factory;
using System;
using System.Collections.Concurrent;

namespace Blacklite.Framework.Features.Observables
{
    public class FeatureSubjectFactory : IFeatureSubjectFactory
    {
        private readonly IFeatureDescriberProvider _featureDescriberProvider;
        private readonly IFeatureFactory _featureFactory;
        private readonly IRequiredFeaturesService _requiredFeaturesService;
        private readonly IFeatureSubjectFactory _subjectFactory;
        private readonly ConcurrentDictionary<Type, IFeatureSubject> _subjects = new ConcurrentDictionary<Type, IFeatureSubject>();

        public FeatureSubjectFactory(IFeatureFactory featureFactory,
            IRequiredFeaturesService requiredFeaturesService,
            IFeatureDescriberProvider featureDescriberProvider,
            IFeatureSubjectFactory subjectFactory)
        {
            _featureFactory = featureFactory;
            _requiredFeaturesService = requiredFeaturesService;
            _featureDescriberProvider = featureDescriberProvider;
            _subjectFactory = subjectFactory;
        }

        public IFeatureSubject GetSubject(Type featureType)
        {
            return _subjects.GetOrAdd(featureType, CreateSubject);
        }

        private IFeatureSubject CreateSubject(Type featureType)
        {
            return FeatureSubject.Create(featureType,
                _featureFactory,
                _requiredFeaturesService,
                _featureDescriberProvider,
                _subjectFactory
                );
        }
    }
}
