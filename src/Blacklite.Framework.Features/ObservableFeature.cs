using Blacklite.Framework.Features.Observables;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Reactive.Subjects;

namespace Blacklite.Framework.Features
{
    public abstract class ObservableFeature : Feature, IObservableFeature { }

    class ObservableFeatureImpl<T> : ObservableFeature<T>
        where T : class, IObservableFeature
    {
        private readonly ISubject<T> _feature;
        public ObservableFeatureImpl(IFeatureSubjectFactory factory)
        {
            _feature = (ISubject<T>)factory.GetSubject(typeof(T));
        }

        public IDisposable Subscribe(IObserver<T> observer) => _feature.Subscribe(observer);
    }

    interface IFeatureSubject<T> : ISubject<T>
        where T : class, IObservableFeature
    {

    }

    class FeatureSubject<T> : IFeatureSubject<T>, IFeatureSubject
        where T : class, IObservableFeature
    {
        private readonly ISubject<T> _feature;
        private readonly IServiceProvider _serviceProvider;
        public FeatureSubject(Feature<T> feature, IServiceProvider serviceProvider, IRequiredFeaturesService requiredFeaturesService)
        {
            _feature = new BehaviorSubject<T>(feature.Value);
            _serviceProvider = serviceProvider;

            var observable = requiredFeaturesService.GetObservableRequiredFeatures(typeof(T));
            if (observable != null)
            {
                observable.Subscribe(x => Update());
            }
        }

        public void Update()
        {
            _feature.OnNext(_serviceProvider.GetService<Feature<T>>().Value);
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
