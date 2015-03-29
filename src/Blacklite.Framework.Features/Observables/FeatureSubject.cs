using Blacklite.Framework.Features.Describers;
using System;
using System.Reactive.Subjects;
using Blacklite.Framework.Features.Factory;
using System.Reflection;

namespace Blacklite.Framework.Features.Observables
{
    interface IFeatureSubject<T> : ISubject<T>
           where T : class, IObservableFeature, new()
    {

    }

    static class FeatureSubject
    {
        public static IFeatureSubject Create(
            Type featureType,
            IFeatureFactory featureFactory,
            IRequiredFeaturesService requiredFeaturesService,
            IFeatureDescriberProvider featureDescriberProvider,
            IFeatureSubjectFactory subjectFactory)
        {
            return (IFeatureSubject)_createSubjectMethod.MakeGenericMethod(featureType).Invoke(null, new object[] {
                featureFactory,
                requiredFeaturesService,
                featureDescriberProvider,
                subjectFactory
            });
        }

        private static MethodInfo _createSubjectMethod = typeof(FeatureSubject).GetTypeInfo().GetDeclaredMethod(nameof(FeatureSubject.CreateSubject));
        private static FeatureSubject<T> CreateSubject<T>(
            IFeatureFactory featureFactory,
            IRequiredFeaturesService requiredFeaturesService,
            IFeatureDescriberProvider featureDescriberProvider,
            IFeatureSubjectFactory subjectFactory)
            where T : class, IObservableFeature, new()
        {
            return new FeatureSubject<T>(new FeatureImpl<T>(featureFactory), featureFactory, requiredFeaturesService, featureDescriberProvider, subjectFactory);
        }
    }

    class FeatureSubject<T> : IFeatureSubject<T>, IFeatureSubject
        where T : class, IObservableFeature, new()
    {
        private readonly ISubject<T> _feature;
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
    }
}
