using System;

namespace Blacklite.Framework.Features.Observables
{
    public class ObservableFeatureFactory : IObservableFeatureFactory
    {
        private readonly IFeatureSubjectFactory _featureSubjectFactory;
        public ObservableFeatureFactory(IFeatureSubjectFactory featureSubjectFactory)
        {
            _featureSubjectFactory = featureSubjectFactory;
        }

        public ObservableFeature<T> GetObservableFeature<T>()
            where T : class, IObservableFeature
        {
            return new ObservableFeatureImpl<T>(_featureSubjectFactory);
        }
    }
}
