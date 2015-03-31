using Blacklite.Framework.Features.Describers;
using System;
using System.Reactive.Subjects;
using Blacklite.Framework.Features.Factory;

namespace Blacklite.Framework.Features.Observables
{
    interface IFeatureSubject<T> : IObservable<T>, IObserver<T>
           where T : class, IObservableFeature, new()
    {
        T Value { get; }
    }

    class FeatureSubject<T> : IFeatureSubject<T>, IFeatureSubject
        where T : class, IObservableFeature, new()
    {
        private readonly BehaviorSubject<T> _feature;
        private readonly IFeatureFactory _featureFactory;

        public FeatureSubject(Feature<T> feature,
            IFeatureFactory featureFactory,
            IRequiredFeaturesService requiredFeaturesService,
            IFeatureDescriberProvider featureDescriberProvider,
            IFeatureSubjectFactory subjectFactory)
        {
            _feature = new BehaviorSubject<T>(feature.Value);
            _featureFactory = featureFactory;
            var describer = featureDescriberProvider.Describers[typeof(T)];

            var observable = requiredFeaturesService.GetObservableRequiredFeatures(typeof(T));
            if (observable != null)
            {
                observable.Subscribe(x => Update());
            }

            if (describer.HasOptions && describer.Options.IsFeature)
            {
                var optionsDesciber = featureDescriberProvider.Describers[describer.Options.Type];
                if (optionsDesciber.IsObservable)
                {
                    var relatedSubject = subjectFactory.GetSubject(optionsDesciber.Type);
                    _feature.Subscribe(x => relatedSubject.Update());
                }
            }
        }

        public void Update()
        {
            _feature.OnNext(_featureFactory.GetFeature<T>());
        }

        public void OnNext(T value)
        {
            _feature.OnNext(value);
        }

        void IObserver<T>.OnError(Exception error)
        {
            _feature.OnError(error);
        }

        void IObserver<T>.OnCompleted()
        {
            _feature.OnCompleted();
        }

        IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
        {
            return _feature.Subscribe(observer);
        }

        public T Value { get { return _feature.Value; } }
    }
}
