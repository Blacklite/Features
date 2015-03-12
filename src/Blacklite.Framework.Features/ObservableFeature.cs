using Blacklite.Framework.Features.Aspects;
using Blacklite.Framework.Features.Traits;
using System;
using System.Reactive.Subjects;

namespace Blacklite.Framework.Features
{
    class ObservableFeature<T> : IObservableFeature<T>
        where T : IObservableAspect
    {
        private readonly ISubject<T> _feature;
        public ObservableFeature(IFeatureSubject<T> feature)
        {
            _feature = feature;
        }

        public IDisposable Subscribe(IObserver<T> observer) => _feature.Subscribe(observer);
    }

    interface IFeatureSubject<T> : ISubject<T>
        where T : IObservableAspect
    {

    }

    class FeatureSubject<T> : IFeatureSubject<T>
        where T : IObservableAspect
    {
        private readonly ISubject<T> _feature;
        public FeatureSubject(T feature)
        {
            _feature = new BehaviorSubject<T>(feature);
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
