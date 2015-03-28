using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using Blacklite.Framework.Features.Describers;
using Blacklite.Framework.Features.Repositories;
using Blacklite.Framework.Features.Observables;

namespace Blacklite.Framework.Features
{
    public class FeatureManager : IFeatureManager
    {
        private readonly IFeatureRepositoryProvider _repositoryProvider;
        private readonly IFeatureSubjectFactory _subjectFactory;
        private readonly IDictionary<Type, IFeatureDescriber> _observableOptionFeatures;

        public FeatureManager(IFeatureRepositoryProvider repositoryProvider, IFeatureDescriberProvider featureDescriberProvider, IFeatureSubjectFactory subjectFactory)
        {
            _observableOptionFeatures = featureDescriberProvider
                .Describers.Values
                .Where(z => z.HasOptions && z.Options.IsFeature)
                .Where(z => featureDescriberProvider.Describers[z.Options.Type].IsObservable)
                .ToDictionary(x => x.Type);

            _repositoryProvider = repositoryProvider;
            _subjectFactory = subjectFactory;
        }

        public bool TrySaveFeature(IFeatureDescriber describer, IFeature feature)
        {
            var result = false;
            var repository = _repositoryProvider.GetFeatureRepository(describer);
            if (repository != null)
            {
                repository.Store(feature, describer);
                result = true;
            }

            if (describer.IsObservable)
            {
                IFeatureDescriber observableDescriber;
                if (_observableOptionFeatures.TryGetValue(describer.Type, out observableDescriber))
                {
                    var optionsSubject = _subjectFactory.GetSubject(observableDescriber.Options.Type);
                    optionsSubject.Update();
                }

                var subject = _subjectFactory.GetSubject(describer.Type);
                subject.Update();
            }

            return result;
        }
    }
}
